-- Identity Service Database Schema - PostgreSQL

-- Create database
CREATE DATABASE identity;
\c identity;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Users table
CREATE TABLE users (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    email varchar(255) NOT NULL UNIQUE,
    password_hash varchar(512) NOT NULL,
    first_name varchar(100) NOT NULL,
    last_name varchar(100) NOT NULL,
    phone_number varchar(20),
    is_active boolean NOT NULL DEFAULT true,
    two_factor_enabled boolean NOT NULL DEFAULT false,
    two_factor_secret varchar(32),
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp,
    created_by varchar(255) NOT NULL,
    updated_by varchar(255),
    xmin bigint NOT NULL -- Optimistic concurrency token
);

-- Roles table
CREATE TABLE roles (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    name varchar(50) NOT NULL UNIQUE,
    description varchar(255),
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by varchar(255) NOT NULL,
    xmin bigint NOT NULL
);

-- User roles mapping
CREATE TABLE user_roles (
    user_id uuid NOT NULL REFERENCES users(id),
    role_id uuid NOT NULL REFERENCES roles(id),
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by varchar(255) NOT NULL,
    PRIMARY KEY (user_id, role_id)
);

-- Refresh tokens
CREATE TABLE refresh_tokens (
    id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id uuid NOT NULL REFERENCES users(id),
    token varchar(512) NOT NULL UNIQUE,
    expires_at timestamp NOT NULL,
    created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_by varchar(255) NOT NULL,
    is_revoked boolean NOT NULL DEFAULT false,
    revoked_at timestamp,
    revoked_by varchar(255)
);

-- Stored Procedures

-- Create user
CREATE OR REPLACE PROCEDURE sp_create_user(
    p_email varchar,
    p_password_hash varchar,
    p_first_name varchar,
    p_last_name varchar,
    p_phone_number varchar,
    p_created_by varchar,
    INOUT p_id uuid
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO users (email, password_hash, first_name, last_name, phone_number, created_by)
    VALUES (p_email, p_password_hash, p_first_name, p_last_name, p_phone_number, p_created_by)
    RETURNING id INTO p_id;
END;
$$;

-- Get user by email
CREATE OR REPLACE FUNCTION sp_get_user_by_email(p_email varchar)
RETURNS TABLE (
    id uuid,
    email varchar,
    password_hash varchar,
    first_name varchar,
    last_name varchar,
    phone_number varchar,
    is_active boolean,
    two_factor_enabled boolean,
    created_at timestamp,
    updated_at timestamp,
    xmin bigint
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT u.id, u.email, u.password_hash, u.first_name, u.last_name, 
           u.phone_number, u.is_active, u.two_factor_enabled,
           u.created_at, u.updated_at, u.xmin
    FROM users u
    WHERE u.email = p_email;
END;
$$;

-- Get user roles
CREATE OR REPLACE FUNCTION sp_get_user_roles(p_user_id uuid)
RETURNS TABLE (
    role_id uuid,
    role_name varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT r.id, r.name
    FROM roles r
    JOIN user_roles ur ON ur.role_id = r.id
    WHERE ur.user_id = p_user_id;
END;
$$;

-- Create refresh token
CREATE OR REPLACE PROCEDURE sp_create_refresh_token(
    p_user_id uuid,
    p_token varchar,
    p_expires_at timestamp,
    p_created_by varchar,
    INOUT p_id uuid
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO refresh_tokens (user_id, token, expires_at, created_by)
    VALUES (p_user_id, p_token, p_expires_at, p_created_by)
    RETURNING id INTO p_id;
END;
$$;

-- Validate refresh token
CREATE OR REPLACE FUNCTION sp_validate_refresh_token(p_token varchar)
RETURNS TABLE (
    id uuid,
    user_id uuid,
    expires_at timestamp,
    is_revoked boolean
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT rt.id, rt.user_id, rt.expires_at, rt.is_revoked
    FROM refresh_tokens rt
    WHERE rt.token = p_token;
END;
$$;

-- Revoke refresh token
CREATE OR REPLACE PROCEDURE sp_revoke_refresh_token(
    p_token varchar,
    p_revoked_by varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE refresh_tokens
    SET is_revoked = true,
        revoked_at = CURRENT_TIMESTAMP,
        revoked_by = p_revoked_by
    WHERE token = p_token;
END;
$$;

-- Seed Data
INSERT INTO roles (id, name, description, created_by) VALUES
    (uuid_generate_v4(), 'Admin', 'System administrator', 'system'),
    (uuid_generate_v4(), 'Manager', 'Hotel manager', 'system'),
    (uuid_generate_v4(), 'Staff', 'Hotel staff', 'system'),
    (uuid_generate_v4(), 'Guest', 'Hotel guest', 'system');
