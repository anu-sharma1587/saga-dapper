-- Availability & Pricing Service Database Schema - PostgreSQL

-- Create database
CREATE DATABASE availability;
\c availability;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Room inventory table
CREATE TABLE room_inventory (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL,
    room_type_id uuid NOT NULL,
    date date NOT NULL,
    total_rooms int NOT NULL,
    available_rooms int NOT NULL,
    blocked_rooms int NOT NULL DEFAULT 0,
    maintenance_rooms int NOT NULL DEFAULT 0,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL,
    CONSTRAINT uk_room_inventory_date UNIQUE (hotel_id, room_type_id, date)
);

-- Pricing rules table
CREATE TABLE pricing_rules (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL,
    room_type_id uuid NOT NULL,
    name varchar(100) NOT NULL,
    description text,
    start_date date NOT NULL,
    end_date date NOT NULL,
    day_of_week smallint, -- NULL for all days, 0-6 for specific days
    min_length_of_stay int,
    max_length_of_stay int,
    price_multiplier decimal(3,2) NOT NULL,
    min_occupancy int,
    max_occupancy int,
    is_active boolean NOT NULL DEFAULT true,
    priority int NOT NULL DEFAULT 0,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Seasonal rates table
CREATE TABLE seasonal_rates (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL,
    room_type_id uuid NOT NULL,
    season_name varchar(100) NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    base_price_adjustment decimal(10,2) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Blackout dates table
CREATE TABLE blackout_dates (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL,
    room_type_id uuid, -- NULL means applies to all room types
    start_date date NOT NULL,
    end_date date NOT NULL,
    reason text,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Indexes
CREATE INDEX idx_room_inventory_date ON room_inventory(date);
CREATE INDEX idx_room_inventory_hotel ON room_inventory(hotel_id);
CREATE INDEX idx_pricing_rules_date ON pricing_rules(start_date, end_date);
CREATE INDEX idx_seasonal_rates_date ON seasonal_rates(start_date, end_date);
CREATE INDEX idx_blackout_dates_date ON blackout_dates(start_date, end_date);

-- Stored Procedures

-- Update room inventory
CREATE OR REPLACE PROCEDURE sp_update_room_inventory(
    p_hotel_id uuid,
    p_room_type_id uuid,
    p_date date,
    p_total_rooms int,
    p_available_rooms int,
    p_blocked_rooms int,
    p_maintenance_rooms int,
    p_updated_by varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO room_inventory (
        hotel_id, room_type_id, date, total_rooms, available_rooms,
        blocked_rooms, maintenance_rooms, created_by
    )
    VALUES (
        p_hotel_id, p_room_type_id, p_date, p_total_rooms, p_available_rooms,
        p_blocked_rooms, p_maintenance_rooms, p_updated_by
    )
    ON CONFLICT (hotel_id, room_type_id, date)
    DO UPDATE SET
        total_rooms = EXCLUDED.total_rooms,
        available_rooms = EXCLUDED.available_rooms,
        blocked_rooms = EXCLUDED.blocked_rooms,
        maintenance_rooms = EXCLUDED.maintenance_rooms,
        updated_at = CURRENT_TIMESTAMP,
        updated_by = EXCLUDED.created_by;
END;
$$;

-- Get availability for date range
CREATE OR REPLACE FUNCTION sp_get_availability(
    p_hotel_id uuid,
    p_room_type_id uuid,
    p_start_date date,
    p_end_date date
)
RETURNS TABLE (
    date date,
    total_rooms int,
    available_rooms int,
    blocked_rooms int,
    maintenance_rooms int,
    is_blackout boolean
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        ri.date,
        ri.total_rooms,
        ri.available_rooms,
        ri.blocked_rooms,
        ri.maintenance_rooms,
        CASE WHEN bd.id IS NOT NULL THEN true ELSE false END as is_blackout
    FROM room_inventory ri
    LEFT JOIN blackout_dates bd ON 
        bd.hotel_id = ri.hotel_id 
        AND (bd.room_type_id IS NULL OR bd.room_type_id = ri.room_type_id)
        AND ri.date BETWEEN bd.start_date AND bd.end_date
        AND bd.is_active = true
    WHERE ri.hotel_id = p_hotel_id
        AND ri.room_type_id = p_room_type_id
        AND ri.date BETWEEN p_start_date AND p_end_date
    ORDER BY ri.date;
END;
$$;

-- Calculate room price
CREATE OR REPLACE FUNCTION sp_calculate_room_price(
    p_hotel_id uuid,
    p_room_type_id uuid,
    p_date date,
    p_length_of_stay int,
    p_occupancy int
)
RETURNS decimal
LANGUAGE plpgsql
AS $$
DECLARE
    v_base_price decimal;
    v_final_price decimal;
    v_multiplier decimal := 1.0;
BEGIN
    -- Get base price from room types (assumed to be passed as parameter or fetched from room_types table)
    -- For this example, we'll use a subquery to the hotel_inventory service
    
    -- Apply seasonal adjustments
    SELECT COALESCE(SUM(base_price_adjustment), 0)
    INTO v_base_price
    FROM seasonal_rates
    WHERE hotel_id = p_hotel_id
        AND room_type_id = p_room_type_id
        AND p_date BETWEEN start_date AND end_date
        AND is_active = true;

    -- Apply pricing rules
    SELECT COALESCE(MAX(price_multiplier), 1.0)
    INTO v_multiplier
    FROM pricing_rules
    WHERE hotel_id = p_hotel_id
        AND room_type_id = p_room_type_id
        AND p_date BETWEEN start_date AND end_date
        AND is_active = true
        AND (day_of_week IS NULL OR day_of_week = EXTRACT(DOW FROM p_date))
        AND (min_length_of_stay IS NULL OR min_length_of_stay <= p_length_of_stay)
        AND (max_length_of_stay IS NULL OR max_length_of_stay >= p_length_of_stay)
        AND (min_occupancy IS NULL OR min_occupancy <= p_occupancy)
        AND (max_occupancy IS NULL OR max_occupancy >= p_occupancy);

    -- Calculate final price
    v_final_price = (v_base_price * v_multiplier);
    
    RETURN v_final_price;
END;
$$;

-- Create pricing rule
CREATE OR REPLACE PROCEDURE sp_create_pricing_rule(
    p_hotel_id uuid,
    p_room_type_id uuid,
    p_name varchar,
    p_description text,
    p_start_date date,
    p_end_date date,
    p_day_of_week smallint,
    p_min_length_of_stay int,
    p_max_length_of_stay int,
    p_price_multiplier decimal,
    p_min_occupancy int,
    p_max_occupancy int,
    p_priority int,
    p_created_by varchar,
    INOUT p_id uuid
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO pricing_rules (
        hotel_id, room_type_id, name, description, start_date, end_date,
        day_of_week, min_length_of_stay, max_length_of_stay, price_multiplier,
        min_occupancy, max_occupancy, priority, created_by
    )
    VALUES (
        p_hotel_id, p_room_type_id, p_name, p_description, p_start_date, p_end_date,
        p_day_of_week, p_min_length_of_stay, p_max_length_of_stay, p_price_multiplier,
        p_min_occupancy, p_max_occupancy, p_priority, p_created_by
    )
    RETURNING id INTO p_id;
END;
$$;
