-- -- Create database for logs
-- CREATE DATABASE IF NOT EXISTS logs;
-- 
-- -- Create table for application logs
-- CREATE TABLE IF NOT EXISTS logs.logs
-- (
--     timestamp DateTime64(9) DEFAULT now(),
--     trace_id String,
--     span_id String,
--     severity_text String,
--     severity_number UInt8,
--     service_name String,
--     service_version String,
--     body String,
--     resource_attributes Map(String, String),
--     attributes Map(String, String)
--     )
--     ENGINE = MergeTree()
--     ORDER BY (timestamp, service_name)
--     TTL timestamp + INTERVAL 30 DAY;
-- 
-- -- Create materialized view for quick access to error logs
-- CREATE MATERIALIZED VIEW IF NOT EXISTS logs.error_logs
-- ENGINE = MergeTree()
-- ORDER BY (timestamp, service_name)
-- TTL timestamp + INTERVAL 30 DAY
-- AS SELECT *
--    FROM logs.logs
--    WHERE severity_text IN ('ERROR', 'FATAL');

-- Create database for logs
CREATE DATABASE IF NOT EXISTS logs;

-- Create table for application logs
CREATE TABLE IF NOT EXISTS logs.logs
(
    timestamp DateTime64(9) DEFAULT now(),
    trace_id String,
    span_id String,
    severity_text String,
    severity_number UInt8,
    service_name String,
    service_version String,
    body String,
    resource_attributes Map(String, String),
    attributes Map(String, String)
    )
    ENGINE = MergeTree()
    ORDER BY (timestamp, service_name)
    TTL toDateTime(timestamp) + INTERVAL 30 DAY;