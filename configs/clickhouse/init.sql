CREATE DATABASE IF NOT EXISTS default;
CREATE DATABASE IF NOT EXISTS otel;
CREATE DATABASE IF NOT EXISTS logs;
-- USE otel;
DROP TABLE IF EXISTS otel.otel_logs;
DROP TABLE IF EXISTS otel.otel_traces;
-- Create logs table
CREATE TABLE otel.otel_logs
(
    timestamp          DateTime CODEC (Delta, ZSTD),
    traceId            String CODEC (ZSTD),
    spanId             String CODEC (ZSTD),
    traceFlags         UInt32 CODEC (ZSTD),
    severityText       LowCardinality(String) CODEC (ZSTD),
    severityNumber     Int32 CODEC (ZSTD),
    body               String CODEC (ZSTD),
    resourceAttributes Map(String, String) CODEC (ZSTD),
    logAttributes      Map(String, String) CODEC (ZSTD),
    INDEX idx_traceId traceId TYPE bloom_filter GRANULARITY 1
) ENGINE = MergeTree()
      ORDER BY (timestamp, traceId, spanId)
      TTL timestamp + INTERVAL 30 DAY;

-- Create traces table
CREATE TABLE IF NOT EXISTS otel.otel_traces
(
    Timestamp          DateTime64(9) CODEC (Delta, ZSTD(1)),
    TraceId            String CODEC (ZSTD(1)),
    SpanId             String CODEC (ZSTD(1)),
    ParentSpanId       String CODEC (ZSTD(1)),
    TraceState         String CODEC (ZSTD(1)),
    SpanName           LowCardinality(String) CODEC (ZSTD(1)),
    SpanKind           LowCardinality(String) CODEC (ZSTD(1)),
    ServiceName        LowCardinality(String) CODEC (ZSTD(1)),
    ResourceAttributes Map(LowCardinality(String), String) CODEC (ZSTD(1)),
    ScopeName          String CODEC (ZSTD(1)),
    ScopeVersion       String CODEC (ZSTD(1)),
    SpanAttributes     Map(LowCardinality(String), String) CODEC (ZSTD(1)),
    Duration           UInt64 CODEC (ZSTD(1)),
    StatusCode         LowCardinality(String) CODEC (ZSTD(1)),
    StatusMessage      String CODEC (ZSTD(1)),
    Events             Nested(Timestamp DateTime64(9),
                           Name LowCardinality(String),
                           Attributes Map(LowCardinality(String), String)) CODEC (ZSTD(1)),
    Links              Nested(TraceId String,
                           SpanId String,
                           TraceState String,
                           Attributes Map(LowCardinality(String), String)) CODEC (ZSTD(1)),
    INDEX idx_trace_id TraceId TYPE bloom_filter(0.001) GRANULARITY 1,
    INDEX idx_res_attr_key mapKeys(ResourceAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,
    INDEX idx_res_attr_value mapValues(ResourceAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,
    INDEX idx_span_attr_key mapKeys(SpanAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,
    INDEX idx_span_attr_value mapValues(SpanAttributes) TYPE bloom_filter(0.01) GRANULARITY 1,
    INDEX idx_duration Duration TYPE minmax GRANULARITY 1
) ENGINE = MergeTree()
      PARTITION BY toDate(Timestamp)
      ORDER BY (ServiceName, SpanName, toDateTime(Timestamp))
      TTL toDate(Timestamp) + toIntervalDay(180)
      SETTINGS index_granularity = 8192, ttl_only_drop_parts = 1;


-- CREATE TABLE IF NOT EXISTS otel_traces_trace_id_ts
-- (
--     TraceId String CODEC (ZSTD(1)),
--     Start   DateTime CODEC (Delta, ZSTD(1)),
--     End     DateTime CODEC (Delta, ZSTD(1)),
--     INDEX idx_trace_id TraceId TYPE bloom_filter(0.01) GRANULARITY 1
-- ) ENGINE = MergeTree()
--       PARTITION BY toDate(Start)
--       ORDER BY (TraceId, Start)
--       TTL toDate(Start) + toIntervalDay(180)
--       SETTINGS index_granularity = 8192, ttl_only_drop_parts = 1;
-- 
-- 
-- CREATE MATERIALIZED VIEW IF NOT EXISTS otel_traces_trace_id_ts_mv
--     TO otel_traces_trace_id_ts
-- AS
-- SELECT TraceId,
--        min(Timestamp) as Start,
--        max(Timestamp) as End
-- FROM otel_traces
-- WHERE TraceId != ''
-- GROUP BY TraceId;