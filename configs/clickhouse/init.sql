-- Create database for logs
CREATE DATABASE IF NOT EXISTS logs;
DROP TABLE IF EXISTS logs.logs;

-- Create table for application logs
CREATE TABLE IF NOT EXISTS logs.logs
(
    timestamp DateTime,
    observed_timestamp DateTime,
    severity_number UInt8,
    severity_text String,
    message String,
    attributes Nested(
    key String,
    value String
),
    dropped_attributes_count UInt32,
    flags UInt8,
    resources Nested(
                        key String,
                        value String
                    ),
    scope_name String,
    span_id String,
    trace_id String,
    source_type String
    )
    ENGINE = MergeTree()
    ORDER BY timestamp;