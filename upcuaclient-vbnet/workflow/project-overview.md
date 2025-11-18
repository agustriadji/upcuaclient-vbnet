# UPC UA Client VB.NET - Project Workflow

## Project Overview
OPC UA Client application built with VB.NET for monitoring and managing sensor data.

## Architecture Components

### Core Modules
- **OpcConnection.vb** - OPC UA server connection management
- **BackgroundWorker.vb** - Background data processing
- **ConfigManager.vb** - Configuration file management
- **AnalyticsManager.vb** - Data analytics, melakukan calculation data, mapper data untuk siap disimpan
- **SQLiteManager.vb** - akan upsert data realtime sensor, read data collection sensor , dll.
- **DataViewManager.vb** - akan menyediakan kebutuhan query data

### Data Flow
1. OPC UA Server Connection
2. Sensor Data Reading
3. Data Processing
4. Analytics Data on SQLite

## Configuration
- **Config/meta.json** - Main configuration file
- OPC UA endpoint: `opc.tcp://localhost:4840`
- Sensor monitoring interval: 120 seconds
- etc

## Tentang Aplikasi
aplikasi ini akan manage sensor yang data ya di sediakan oleh opc server.
app ini akan digunakan untuk pengujian tekanan ban.
1 ban akan terdapat 2 sensor : 1 tekanan udara didalam ban kita akan sebut dengan pressureTire dan 1 tekanan di manifold kita sebut pressureGauge
jumlah ban ada 15, 15 sensor pressureTire dan 15 pressureGauge.
OPC akan membuat 1 node/address ns=2;s=PLC, 2 subnode [PressureTire,PressureGauge]
dan tiap subnode akan memiliki sensor node/variable node berserta metadata ya.
EX:
```
ns=2;s=PLC
   -> PressureTire
      -> Sensor1 [Name:Sensor1, Value:200, DataType:Float, Status: Good]
      -> Sensor2 [Name:Sensor2, Value:200, DataType:Float, Status: Good]
      -> ...
      -> Sensor15 [Name:Sensor15, Value:200, DataType:Float, Status: Good]
  -> PressureGauge
      -> Sensor1 [Name:Sensor1, Value:200, DataType:Float, Status: Good]
      -> Sensor2 [Name:Sensor2, Value:200, DataType:Float, Status: Good]
      -> ...
      -> Sensor15 [Name:Sensor15, Value:200, DataType:Float, Status: Good]
```
apps saat install pertama kali ini benar-benar kosong.
apps dapat setup connection to opc server pada FormConfigManager dengan minimal data seperti host dan port untuk check koneksi.
setelah connect user baru dapat melakukan node yang tersedia pada combobox dropdown list misalnya "ns=2;s=PLC"

kemudian user baru bisa menambahkan sensor, pada form akan terdapat combobox dropdown list untuk add sensor pressureTire / pressureGauge.
setelah di add maka sensor siap untuk digunkan di form new record, sensor yang sedang melakukan pengujian tidak akan ditampilkan sampai sensor dinyatakan selesai recording.

## Flow BackgroundWorker Sebagai Core engine.
backgroundworker akan menjadi tempat utama pengelolaan data.
dari melakukan get data ke opc per sensor (OpcConnection), kemudian di olah sama AnalyticsManager untuk siap disimpan, kemudian diserahkan ke SQLiteManager untuk disimpan.
backgroundworker , tray icon berjalan di Main.vb yang berperan sebagai bootstrap.

   ### Aturan BackgroundWoker
     - hanya berjalan jika ada sensor yang sedang di recording (terjadi ya new record di FormNewRecord).


expected saya adalah sampai data di simpan di sqlite saja.