# OPC UA Client - User Manual

## System Requirements

- **Operating System**: Windows 10/11
- **Database**: SQL Server Express (required)
- **OPC UA Server**: Compatible OPC UA server running

## Installation

1. Download and install SQL Server Express from Microsoft
2. Run the OPC UA Client installer
3. Launch the application

## Getting Started

### 1. Config Manager Setup

**Purpose**: Configure OPC server connection and database settings

**Steps**:
1. Go to **File** → **Config Manager**
2. **OPC Settings**:
   - Enter **Host OPC**: `opc.tcp://localhost:4840` (your OPC server URL)
   - Click **Test Host OPC** to verify connection
   - If successful, click **Browse Objects** to discover available objects
   - Select objects from dropdown and click **Select Object OPC** EX: `PressureTire` And `PressureGauge`
3. **Database Settings**:
   - Enter **Host DB**: `Server=localhost\SQLEXPRESS;Database=OpcUaClient;Integrated Security=true;TrustServerCertificate=true;`
   - Click **Test Host DB** to verify database connection
4. Click **Save** to apply settings

**Troubleshooting**:
- Ensure OPC server is running before testing connection
- Verify SQL Server Express is installed and running
- Check firewall settings if connection fails

### 2. Add New Sensor Configuration

**Purpose**: Configure which sensors to monitor from discovered OPC objects

**Steps**:
1. Go to **Tools** → **Add New Sensor**
2. **Select Object**: Choose from available objects in the tree view for view list child node (sensor)
3. **Configure Sensors** Child Node PressureTire and Child Node PressureGauge:
   - Check **NodeActive** = `True` for sensors you want to monitor
   - **NodeStatus** will show current status (Idle/Running)
   - **NodeType** shows sensor data type
4. Click **Save** to apply sensor configuration

**Notes**:
- Only sensors marked as **Active** will be monitored
- Sensors must be in **Idle** status to be used for new recordings

### 3. Add New Recording

**Purpose**: Start a new data recording session

**Steps**:
1. Go to **Tools** → **Add New Recording**
2. **Fill Recording Details**:
   - **Batch Name**: Auto-generated or enter custom name
   - **Tire Sensor**: Select from available PressureTire sensors
   - **Gauge Sensor**: Select from available PressureGauge sensors
   - **Size**: Enter tire size
   - **Operator**: Enter operator name
   - **Auto End Record**: Set days (0 = manual end)
3. Click **Create** to start recording

**Requirements**:
- At least one PressureTire sensor must be Active and Idle
- At least one PressureGauge sensor must be Active and Idle
- Sensors will automatically change to "Running" status

### 4. Monitor Recording

**Purpose**: View active and historical recordings

**Navigation**:
- **Recording Tab**: Shows all recording sessions
- **Sensor State Tab**: Shows current sensor status

**Recording Information**:
- **Batch**: Recording session ID
- **Sensor ID**: Associated sensors
- **Running Days**: Days since recording started
- **Status**: Recording/Finished
- **Last Updated**: Latest data timestamp

**View Details**:
- Click on any recording row to open **Detail Record** window
- View real-time pressure data, graphs, and statistics

### 5. Show Alerts

**Purpose**: Monitor system alerts and leaking detection

**Alert Types**:
- **Critical**: Pressure leaking detected (PressureGauge > 0)
- **Warning**: System warnings
- **Info**: General information

**View Alerts**:
1. Click **Alert** button in the bottom panel
2. Alerts show:
   - Timestamp
   - Severity icon (🔴 Critical, 🟡 Warning, 🔵 Info)
   - Batch ID
   - Sensor ID
   - Alert message
   - Current value

**Alert Indicators**:
- **Orange Alert Button**: New unread alerts
- **Real-time Updates**: Alerts appear automatically every 30 seconds

### 6. Export CSV

**Purpose**: Export recording data to CSV file

**Steps**:
1. Open **Detail Record** for the desired recording
2. Click **Export** button
3. Choose save location and filename
4. CSV includes:
   - Batch ID
   - Start Pressure
   - Current Pressure
   - Leak Pressure
   - Timestamp

**File Format**:
```csv
BatchId,StartPressure,CurrentPressure,LeakPressure,Timestamp
BATCH-1234-20241201-143022,25.50,24.80,0.00,2024-12-01 14:30
```

### 7. End Recording

**Purpose**: Stop active recording session

**Steps**:
1. Open **Detail Record** for active recording
2. Click **End Recording** button
3. Confirm the action
4. System will:
   - Change status to "Finished"
   - Set sensors back to "Idle"
   - Export data to SQL Server
   - Stop real-time data collection

**Notes**:
- Only active recordings can be ended
- Finished recordings cannot be restarted
- Data is automatically backed up to SQL Server

## Connection Status

**Status Bar Indicators**:
- **Green Server Icon**: OPC-UA connected
- **Red Server Icon**: OPC-UA disconnected
- **Green Database Icon**: Database connected
- **Red Database Icon**: Database disconnected

## Data Storage

**SQLite (Primary)**:
- Real-time sensor data
- Active recordings
- System logs and alerts
- Location: `%AppData%\OpcUaClient\data\sensor.db`

**SQL Server (Backup)**:
- Finished recordings
- Historical data archive
- Long-term storage

## Troubleshooting

### Common Issues

**"Failed to save record metadata"**:
- Check database permissions
- Verify SQL Server Express is running
- Restart application as administrator

**"OPC not connected"**:
- Verify OPC server is running
- Check OPC server URL in Config Manager
- Test connection in Config Manager

**"No sensors available"**:
- Configure sensors in Add New Sensor
- Ensure sensors are marked as Active
- Check OPC object discovery in Config Manager

**Alerts not showing**:
- Verify PressureGauge sensors are running
- Check if leaking is actually occurring (value > 0)
- Review debug logs in Log panel

### Debug Information

**View Logs**:
1. Go to **View** → **Log**
2. Click **Debug** tab for system logs
3. Click **Alert** tab for alert history

**Log Levels**:
- **DEBUG**: Detailed system information
- **INFO**: General information
- **WARN**: Warnings
- **ERROR**: Error messages
- **OK**: Success messages

## Best Practices

1. **Regular Backups**: Finished recordings are automatically backed up to SQL Server
2. **Monitor Connections**: Check status bar indicators regularly
3. **End Recordings**: Properly end recordings to ensure data integrity
4. **Alert Monitoring**: Check alerts regularly for leaking detection
5. **Sensor Management**: Keep sensor configurations updated

## Support

For technical support or issues:
1. Check debug logs for error details
2. Verify system requirements
3. Review troubleshooting section
4. Contact system administrator

---

**Version**: 1.0  
**Last Updated**: December 2024


--- use log

' Contoh penggunaan Logger

' 1. Log informasi biasa
Logger.LogInfo("Application started successfully")
Logger.LogInfo("User logged in", "Authentication")

' 2. Log error dengan source
Logger.LogError("Database connection failed", "SQLServer")
Logger.LogError($"Invalid parameter: {paramValue}", "Validation")

' 3. Log warning
Logger.LogWarning("Low disk space detected")
Logger.LogWarning("Connection timeout, retrying...", "Network")

' 4. Log debug (untuk development)
Logger.LogDebug($"Processing batch: {batchId}")
Logger.LogDebug($"Query executed: {sqlQuery}", "Database")

' 5. Dalam try-catch
Try
    ' Some operation
    Logger.LogInfo("Operation completed successfully")
Catch ex As Exception
    Logger.LogError($"Operation failed: {ex.Message}", "OperationName")
End Try

' 6. Dengan variabel
Dim recordCount = 150
Logger.LogInfo($"Retrieved {recordCount} records from database", "DataAccess")

' 7. Tanpa source (optional)
Logger.LogInfo("System ready")
Logger.LogError("Critical error occurred")