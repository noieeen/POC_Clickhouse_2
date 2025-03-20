-- Create database for logs
CREATE
DATABASE IF NOT EXISTS default;

CREATE
DATABASE IF NOT EXISTS logs;

DROP TABLE IF EXISTS logs.logs;
DROP TABLE IF EXISTS logs.dummy;
DROP TABLE IF EXISTS logs.otel_traces;

-- -- LOGS      
DROP TABLE IF EXISTS default.otel_logs;
-- -- TRACES
DROP TABLE IF EXISTS default.traces;

CREATE TABLE IF NOT EXISTS default.otel_logs
(
    Timestamp         DateTime64(9) DEFAULT now(),
    ObservedTimestamp TIMESTAMP DEFAULT NULL,
    Body              String,
    SeverityText      String,
    SeverityNumber    UInt8,
    LogResources      Map(String, String),
    LogAttributes     Map(String, String),
    TraceId           String,
    SpanId            String,
    ScopeName         String,
    ServiceName       String,
    SourceType        String,
    Log               String
) ENGINE = MergeTree()
PARTITION BY toYYYYMM(Timestamp)
ORDER BY (Timestamp, ServiceName, SeverityNumber);

CREATE TABLE IF NOT EXISTS default.traces
(
    Timestamp         DateTime64(9) DEFAULT now(),
    ObservedTimestamp TIMESTAMP DEFAULT NULL,
    Body              String,
    LogResources      Map(String, String),
    LogAttributes     Map(String, String)
    ) ENGINE = MergeTree()
    PARTITION BY toYYYYMM(Timestamp)
    ORDER BY (Timestamp);


-- CREATE TABLE IF NOT EXISTS default.otel_traces
-- (
--     Timestamp         DateTime64(9) DEFAULT now(),
--     TraceId                String,
--     SpanId                 String,
--     ParentSpanId           String,
--     Name                   String,
--     Kind                   UInt8,
--     StartTimeUnixNano      DateTime64(9),
--     EndTimeUnixNano        DateTime64(9),
--     IngestTimestamp        DateTime64(9),
--     StatusCode             UInt8,
--     StatusMessage          String,
--     TraceState             String,
--     DroppedAttributesCount UInt32,
--     DroppedEventsCount     UInt32,
--     DroppedLinksCount      UInt32,
--     LogAttributes          Map(String, String),
--     LogResources           Map(String, String)
-- ) ENGINE = MergeTree()
-- PARTITION BY toYYYYMM(Timestamp)
-- ORDER BY (Timestamp, TraceId, SpanId);

CREATE TABLE logs.dummy
(
    Timestamp         DateTime64(9) DEFAULT now(),
    TraceId               String,
    Body                 String
) ENGINE = MergeTree()
PARTITION BY toYYYYMM(Timestamp)
ORDER BY (Timestamp);