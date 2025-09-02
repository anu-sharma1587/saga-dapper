-- Reservation Service Database Schema - PostgreSQL

-- Create database
CREATE DATABASE reservations;
\c reservations;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Reservations table
CREATE TABLE reservations (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    hotel_id uuid NOT NULL,
    guest_id uuid NOT NULL,
    room_type_id uuid NOT NULL,
    rate_plan_id uuid NOT NULL,
    check_in_date date NOT NULL,
    check_out_date date NOT NULL,
    adults int NOT NULL,
    children int NOT NULL DEFAULT 0,
    total_amount decimal(10,2) NOT NULL,
    currency varchar(3) NOT NULL,
    status varchar(20) NOT NULL,
    confirmation_number varchar(20) NOT NULL UNIQUE,
    notes text,
    cancellation_reason text,
    cancelled_at timestamp,
    cancelled_by varchar(255),
    payment_status varchar(20) NOT NULL,
    payment_intent_id varchar(255),
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Reservation room assignments table
CREATE TABLE reservation_rooms (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    reservation_id uuid NOT NULL REFERENCES reservations(id),
    room_number varchar(20) NOT NULL,
    is_upgraded boolean NOT NULL DEFAULT false,
    upgrade_reason text,
    check_in_time timestamp,
    check_out_time timestamp,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Additional guest information table
CREATE TABLE reservation_guests (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    reservation_id uuid NOT NULL REFERENCES reservations(id),
    first_name varchar(100) NOT NULL,
    last_name varchar(100) NOT NULL,
    email varchar(255),
    phone_number varchar(20),
    is_primary boolean NOT NULL DEFAULT false,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by varchar(255) NOT NULL,
    xmin bigint NOT NULL
);

-- Special requests/preferences table
CREATE TABLE reservation_requests (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    reservation_id uuid NOT NULL REFERENCES reservations(id),
    request_type varchar(50) NOT NULL,
    request_details text NOT NULL,
    status varchar(20) NOT NULL DEFAULT 'Pending',
    notes text,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL
);

-- Reservation state changes for audit/tracking
CREATE TABLE reservation_state_changes (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    reservation_id uuid NOT NULL REFERENCES reservations(id),
    previous_state varchar(20) NOT NULL,
    new_state varchar(20) NOT NULL,
    changed_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    changed_by varchar(255) NOT NULL,
    reason text,
    xmin bigint NOT NULL
);

-- Indexes
CREATE INDEX idx_reservations_dates ON reservations(check_in_date, check_out_date);
CREATE INDEX idx_reservations_guest ON reservations(guest_id);
CREATE INDEX idx_reservations_hotel ON reservations(hotel_id);
CREATE INDEX idx_reservations_status ON reservations(status);
CREATE INDEX idx_reservation_rooms_reservation ON reservation_rooms(reservation_id);

-- Stored Procedures

-- Create reservation
CREATE OR REPLACE PROCEDURE sp_create_reservation(
    p_hotel_id uuid,
    p_guest_id uuid,
    p_room_type_id uuid,
    p_rate_plan_id uuid,
    p_check_in_date date,
    p_check_out_date date,
    p_adults int,
    p_children int,
    p_total_amount decimal,
    p_currency varchar,
    p_payment_intent_id varchar,
    p_notes text,
    p_created_by varchar,
    INOUT p_id uuid,
    OUT p_confirmation_number varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Generate confirmation number (simple implementation)
    SELECT CONCAT('RES', TO_CHAR(CURRENT_TIMESTAMP, 'YYMMDD'), LPAD(FLOOR(RANDOM() * 10000)::text, 4, '0'))
    INTO p_confirmation_number;
    
    INSERT INTO reservations (
        id, hotel_id, guest_id, room_type_id, rate_plan_id,
        check_in_date, check_out_date, adults, children,
        total_amount, currency, status, confirmation_number,
        notes, payment_status, payment_intent_id, created_by
    )
    VALUES (
        uuid_generate_v4(), p_hotel_id, p_guest_id, p_room_type_id, p_rate_plan_id,
        p_check_in_date, p_check_out_date, p_adults, p_children,
        p_total_amount, p_currency, 'Pending', p_confirmation_number,
        p_notes, 'Pending', p_payment_intent_id, p_created_by
    )
    RETURNING id INTO p_id;

    -- Record state change
    INSERT INTO reservation_state_changes (
        reservation_id, previous_state, new_state, changed_by
    )
    VALUES (
        p_id, 'New', 'Pending', p_created_by
    );
END;
$$;

-- Get reservation by id
CREATE OR REPLACE FUNCTION sp_get_reservation_by_id(p_id uuid)
RETURNS TABLE (
    id uuid,
    hotel_id uuid,
    guest_id uuid,
    room_type_id uuid,
    rate_plan_id uuid,
    check_in_date date,
    check_out_date date,
    adults int,
    children int,
    total_amount decimal,
    currency varchar,
    status varchar,
    confirmation_number varchar,
    notes text,
    payment_status varchar,
    payment_intent_id varchar,
    created_at timestamp,
    updated_at timestamp,
    xmin bigint
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        r.id, r.hotel_id, r.guest_id, r.room_type_id, r.rate_plan_id,
        r.check_in_date, r.check_out_date, r.adults, r.children,
        r.total_amount, r.currency, r.status, r.confirmation_number,
        r.notes, r.payment_status, r.payment_intent_id,
        r.created_at, r.updated_at, r.xmin
    FROM reservations r
    WHERE r.id = p_id;
END;
$$;

-- Update reservation status
CREATE OR REPLACE PROCEDURE sp_update_reservation_status(
    p_id uuid,
    p_status varchar,
    p_updated_by varchar,
    p_reason text DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_previous_state varchar;
BEGIN
    -- Get current status
    SELECT status INTO v_previous_state
    FROM reservations
    WHERE id = p_id;

    -- Update reservation
    UPDATE reservations
    SET 
        status = p_status,
        updated_at = CURRENT_TIMESTAMP,
        updated_by = p_updated_by
    WHERE id = p_id;

    -- Record state change
    INSERT INTO reservation_state_changes (
        reservation_id, previous_state, new_state, changed_by, reason
    )
    VALUES (
        p_id, v_previous_state, p_status, p_updated_by, p_reason
    );
END;
$$;

-- Get reservations by date range
CREATE OR REPLACE FUNCTION sp_get_reservations_by_date_range(
    p_hotel_id uuid,
    p_start_date date,
    p_end_date date
)
RETURNS TABLE (
    id uuid,
    guest_id uuid,
    room_type_id uuid,
    check_in_date date,
    check_out_date date,
    status varchar,
    confirmation_number varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        r.id, r.guest_id, r.room_type_id,
        r.check_in_date, r.check_out_date,
        r.status, r.confirmation_number
    FROM reservations r
    WHERE r.hotel_id = p_hotel_id
        AND r.check_in_date <= p_end_date
        AND r.check_out_date >= p_start_date
        AND r.status NOT IN ('Cancelled', 'No-Show');
END;
$$;

-- Assign room to reservation
CREATE OR REPLACE PROCEDURE sp_assign_room(
    p_reservation_id uuid,
    p_room_number varchar,
    p_is_upgraded boolean,
    p_upgrade_reason text,
    p_created_by varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO reservation_rooms (
        reservation_id, room_number, is_upgraded,
        upgrade_reason, created_by
    )
    VALUES (
        p_reservation_id, p_room_number, p_is_upgraded,
        p_upgrade_reason, p_created_by
    );
END;
$$;
