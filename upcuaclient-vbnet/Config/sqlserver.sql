-- SQL Server Schema for OPC UA Client
-- Tabel utama untuk data sensor harian
CREATE TABLE sensor_data (
    id INT IDENTITY(1,1) PRIMARY KEY,
    node_id NVARCHAR(100) NOT NULL,
    node_text NVARCHAR(100) NOT NULL,
    sensor_type NVARCHAR(50) NOT NULL,
    value FLOAT NOT NULL,
    data_type NVARCHAR(50) NOT NULL,
    status NVARCHAR(20) NOT NULL,
    sync_status NVARCHAR(20) NOT NULL,
    timestamp DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Tabel untuk alert per sensor
CREATE TABLE sensor_alerts (
    id INT IDENTITY(1,1) PRIMARY KEY,
    node_id NVARCHAR(100) NOT NULL,
    node_text NVARCHAR(100) NOT NULL,
    sensor_type NVARCHAR(50) NOT NULL,
    message NVARCHAR(500) NOT NULL,
    threshold FLOAT NOT NULL,
    current_value FLOAT NOT NULL,
    severity NVARCHAR(20) NOT NULL,
    timestamp DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Metadata sensor tekanan ban
CREATE TABLE sensor_metadata_tire (
    node_id NVARCHAR(100) PRIMARY KEY,
    data_type NVARCHAR(50) NOT NULL,
    unit NVARCHAR(10) NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,
    is_running BIT NOT NULL DEFAULT 0
);

-- Metadata sensor tekanan gauge
CREATE TABLE sensor_metadata_guage (
    node_id NVARCHAR(100) PRIMARY KEY,
    data_type NVARCHAR(50) NOT NULL,
    unit NVARCHAR(10) NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,
    is_running BIT NOT NULL DEFAULT 0
);

-- Log sistem
CREATE TABLE system_logs (
    id INT IDENTITY(1,1) PRIMARY KEY,
    event_type NVARCHAR(50) NOT NULL,
    source NVARCHAR(100) NOT NULL,
    details NVARCHAR(MAX) NOT NULL,
    timestamp DATETIME2 NOT NULL DEFAULT GETDATE()
);

-- Metadata record pengujian
CREATE TABLE record_metadata (
    batch_id NVARCHAR(50) PRIMARY KEY,
    pressure_tire_id NVARCHAR(100) NOT NULL,
    pressure_guage_id NVARCHAR(100) NOT NULL,
    size INT NOT NULL,
    created_by NVARCHAR(100) NOT NULL,
    status NVARCHAR(20) NOT NULL,
    sync_status NVARCHAR(20) NOT NULL,
    start_date DATETIME2 NOT NULL,
    end_date DATETIME2 NOT NULL
);

-- Indexes
CREATE INDEX IX_sensor_data_node_time ON sensor_data (node_id, timestamp);
CREATE INDEX IX_alerts_node_time ON sensor_alerts (node_id, timestamp);
CREATE INDEX IX_logs_time ON system_logs (timestamp);