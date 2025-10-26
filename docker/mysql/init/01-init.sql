-- File: docker/mysql/init/01-init.sql
-- This script is run automatically by the MySQL container on its first startup.

-- Create the database for the UserService
CREATE DATABASE IF NOT EXISTS user_db;

-- Create the database for the CatalogService
CREATE DATABASE IF NOT EXISTS catalog_db;

-- Create the database for the LoanService
CREATE DATABASE IF NOT EXISTS loan_db;