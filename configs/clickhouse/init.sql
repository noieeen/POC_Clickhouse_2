-- Create database for logs
CREATE DATABASE IF NOT EXISTS default;
DROP TABLE IF EXISTS default.otel_logs;
DROP TABLE IF EXISTS logs.logs;
-- 
-- -- Create table for application logs
-- CREATE TABLE IF NOT EXISTS logs.logs
-- (
--     timestamp DateTime,
--     observed_timestamp DateTime,
--     severity_number UInt8,
--     severity_text String,
--     message String,
--     attributes Nested(
--     key String,
--     value String
-- ),
--     dropped_attributes_count UInt32,
--     flags UInt8,
--     resources Nested(
--                         key String,
--                         value String
--                     ),
--     scope_name String,
--     span_id String,
--     trace_id String,
--     source_type String
--     )
--     ENGINE = MergeTree()
--     ORDER BY timestamp;


CREATE TABLE default.otel_logs
(
    Timestamp DateTime64(9) DEFAULT now(),
    ObservedTimestamp  TIMESTAMP DEFAULT NULL,
    Body String,
    SeverityText String,
    SeverityNumber UInt8,
    LogResources Map(String, String),
    LogAttributes Map(String, String),
    TraceId String,
    SpanId String,
    ScopeName String,
    ServiceName String,
    SourceType String,
    Log String
) ENGINE = MergeTree()
PARTITION BY toYYYYMM(Timestamp)
ORDER BY (Timestamp, ServiceName, SeverityNumber);