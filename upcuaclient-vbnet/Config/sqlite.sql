-- Tabel utama untuk data sensor harian
CREATE TABLE IF NOT EXISTS sensor_data (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    node_id TEXT NOT NULL, -- node_id pressureTire | pressureGauge
    node_text TEXT NOT NULL, -- node_text pressureTire | pressureGauge
    sensor_type TEXT NOT NULL, -- pressureTire | pressureGauge
    value REAL NOT NULL,
    data_type TEXT NOT NULL,
    status TEXT NOT NULL, -- Good, Warning, Critical, Etc
    sync_status TEXT NOT NULL,
    timestamp TEXT NOT NULL
);

-- Tabel untuk alert per sensor
CREATE TABLE IF NOT EXISTS sensor_alerts (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    node_id TEXT NOT NULL, -- node_id pressureTire | pressureGauge
    node_text TEXT NOT NULL, -- node_text pressureTire | pressureGauge
    sensor_type TEXT NOT NULL, -- pressureTire | pressureGauge
    message TEXT NOT NULL,
    threshold REAL NOT NULL,
    current_value REAL NOT NULL,
    severity TEXT NOT NULL,
    timestamp TEXT NOT NULL
);

-- Log sistem: koneksi, error, event
CREATE TABLE IF NOT EXISTS system_logs (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_type TEXT NOT NULL,
    source TEXT NOT NULL,
    details TEXT NOT NULL,
    timestamp TEXT NOT NULL
);

-- Metadata new record pengujian
CREATE TABLE IF NOT EXISTS record_metadata (
    batch_id TEXT PRIMARY KEY,
    pressure_tire_id TEXT NOT NULL,
    pressure_gauge_id TEXT NOT NULL,
    size INTEGER NOT NULL,
    created_by TEXT NOT NULL,
    status TEXT NOT NULL, -- Recording | Idle | Offline
    sync_status TEXT NOT NULL, -- Sync state setelah simpan ke db mysql server, sehingga akan dianggap sebagai history
    start_date TEXT NOT NULL,
    end_date TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_sensor_data_node_time ON sensor_data (node_id, timestamp);
CREATE INDEX IF NOT EXISTS idx_alerts_node_time ON sensor_alerts (node_id, timestamp);
CREATE INDEX IF NOT EXISTS idx_logs_time ON system_logs (timestamp);