-- Hotel Inventory Service Database Schema - PostgreSQL

-- Create database
CREATE DATABASE hotelinventory;
\c hotelinventory;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Hotels table
CREATE TABLE hotels (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    name varchar(255) NOT NULL,
    description text,
    address_line1 varchar(255) NOT NULL,
    address_line2 varchar(255),
    city varchar(100) NOT NULL,
    state varchar(100),
    country varchar(100) NOT NULL,
    postal_code varchar(20) NOT NULL,
    phone_number varchar(20) NOT NULL,
    email varchar(255) NOT NULL,
    timezone varchar(50) NOT NULL,
    check_in_time time NOT NULL,
    check_out_time time NOT NULL,
    rating smallint,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Hotel amenities table
CREATE TABLE hotel_amenities (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL REFERENCES hotels(id),
    name varchar(100) NOT NULL,
    description text,
    icon_url varchar(255),
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by varchar(255) NOT NULL,
    xmin bigint NOT NULL
);

-- Room types table
CREATE TABLE room_types (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL REFERENCES hotels(id),
    name varchar(100) NOT NULL,
    description text,
    capacity int NOT NULL,
    bed_type varchar(50) NOT NULL,
    room_size int NOT NULL,
    base_price decimal(10,2) NOT NULL,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Room type amenities table
CREATE TABLE room_type_amenities (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    room_type_id uuid NOT NULL REFERENCES room_types(id),
    name varchar(100) NOT NULL,
    description text,
    icon_url varchar(255),
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by varchar(255) NOT NULL,
    xmin bigint NOT NULL
);

-- Room type images table
CREATE TABLE room_type_images (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    room_type_id uuid NOT NULL REFERENCES room_types(id),
    image_url varchar(255) NOT NULL,
    caption varchar(255),
    is_primary boolean NOT NULL DEFAULT false,
    display_order int NOT NULL DEFAULT 0,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by varchar(255) NOT NULL,
    xmin bigint NOT NULL
);

-- Rate plans table
CREATE TABLE rate_plans (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL REFERENCES hotels(id),
    room_type_id uuid NOT NULL REFERENCES room_types(id),
    name varchar(100) NOT NULL,
    description text,
    cancellation_policy text,
    price_multiplier decimal(3,2) NOT NULL DEFAULT 1.00,
    is_refundable boolean NOT NULL DEFAULT true,
    is_active boolean NOT NULL DEFAULT true,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Stored Procedures

-- Create hotel
CREATE OR REPLACE PROCEDURE sp_create_hotel(
    p_name varchar,
    p_description text,
    p_address_line1 varchar,
    p_address_line2 varchar,
    p_city varchar,
    p_state varchar,
    p_country varchar,
    p_postal_code varchar,
    p_phone_number varchar,
    p_email varchar,
    p_timezone varchar,
    p_check_in_time time,
    p_check_out_time time,
    p_created_by varchar,
    INOUT p_id uuid
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO hotels (
        name, description, address_line1, address_line2, city, state, country,
        postal_code, phone_number, email, timezone, check_in_time, check_out_time, created_by
    )
    VALUES (
        p_name, p_description, p_address_line1, p_address_line2, p_city, p_state, p_country,
        p_postal_code, p_phone_number, p_email, p_timezone, p_check_in_time, p_check_out_time, p_created_by
    )
    RETURNING id INTO p_id;
END;
$$;

-- Get hotel by id
CREATE OR REPLACE FUNCTION sp_get_hotel_by_id(p_id uuid)
RETURNS TABLE (
    id uuid,
    name varchar,
    description text,
    address_line1 varchar,
    address_line2 varchar,
    city varchar,
    state varchar,
    country varchar,
    postal_code varchar,
    phone_number varchar,
    email varchar,
    timezone varchar,
    check_in_time time,
    check_out_time time,
    rating smallint,
    is_active boolean,
    created_at timestamp,
    updated_at timestamp,
    xmin bigint
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT h.id, h.name, h.description, h.address_line1, h.address_line2,
           h.city, h.state, h.country, h.postal_code, h.phone_number,
           h.email, h.timezone, h.check_in_time, h.check_out_time,
           h.rating, h.is_active, h.created_at, h.updated_at, h.xmin
    FROM hotels h
    WHERE h.id = p_id;
END;
$$;

-- Create room type
CREATE OR REPLACE PROCEDURE sp_create_room_type(
    p_hotel_id uuid,
    p_name varchar,
    p_description text,
    p_capacity int,
    p_bed_type varchar,
    p_room_size int,
    p_base_price decimal,
    p_created_by varchar,
    INOUT p_id uuid
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO room_types (
        hotel_id, name, description, capacity, bed_type,
        room_size, base_price, created_by
    )
    VALUES (
        p_hotel_id, p_name, p_description, p_capacity, p_bed_type,
        p_room_size, p_base_price, p_created_by
    )
    RETURNING id INTO p_id;
END;
$$;

-- Get room types by hotel
CREATE OR REPLACE FUNCTION sp_get_room_types_by_hotel(p_hotel_id uuid)
RETURNS TABLE (
    id uuid,
    name varchar,
    description text,
    capacity int,
    bed_type varchar,
    room_size int,
    base_price decimal,
    is_active boolean,
    created_at timestamp,
    updated_at timestamp,
    xmin bigint
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT rt.id, rt.name, rt.description, rt.capacity, rt.bed_type,
           rt.room_size, rt.base_price, rt.is_active, rt.created_at,
           rt.updated_at, rt.xmin
    FROM room_types rt
    WHERE rt.hotel_id = p_hotel_id;
END;
$$;
