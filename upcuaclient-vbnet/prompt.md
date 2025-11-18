## file json analitics
saya memiliki workflow pada file json sensor yang disimpan ke folder analytics.
tujuan file json di folder analytics adalah untuk simpan data pressure secara berkala dari sensor.
setiap 1 sensor akan memiliki 1 folder sendiri di dalam folder analytics.
waktu pengujian pressure dari sensor adalah 180 hari.
jika saya interval dalam satuan menit, misalnya 2 menit, maka kemungkinan size file menjadi besar sekali dan dapat mempengaruhi performa sistem.
sehingga saya ingin melakukan split file json berdasarkan date_time/hari.
 - ex: Analytics/sensor1/sensor1-date_time.json
 - dengan pakai date_time sebagai penanda kapan file tersebut dibuat.

struktur foldernya seperti ini:
 Analytics/
   ├── sensor1/
   │   ├── sensor1-2024-01-01.json
   |   |── sensor1-2024-01-02.json	
   ├── sensor2/
   │   ├── sensor2-2024-01-01.json
   |   |── sensor2-2024-01-02.json

 berikut schema json ya : 
 ```json
  {
   "sensor_id": "sensor1",
   "high_pressure": float, // di isi dengan nilai max pressure sebelum membuat file baru
   "low_pressure": float, // di isi dengan nilai min pressure sebelum membuat file baru
   "collections": [
	 {
	   "timestamp": "2024-01-01T00:00:00Z",
	   "pressure": 101.5
	 },
	 {
	   "timestamp": "2024-01-01T00:02:00Z",
	   "pressure": 101.7
	 }
	 // ... lebih banyak data pressure
   ]
 }
 ```

 setiap ada data pressure dari sensor, maka akan menambahkan data ke dalam collections pada file json sesuai dengan tanggal saat data diterima.
 high_pressure dan low_pressure akan di update setiap saat akan pergantian hari / saat membuat file json baru.
 saat terjadi action end record, maka data sensor akan di sync ke database, dan update data sync_attempt di file state.json pada folder Temp.
 saat membuat new record, maka akan di check apakah sensor sudah memiliki file json di folder analytics : 
	- jika sudah ada, maka akan akan hapus file json lama dan membuat file json baru.
	- jika belum ada, maka akan langsung membuat file json baru.

 ---

 ## file json logs
 saya juga memiliki workflow pada file json logs yang disimpan ke folder Temp.
 tujuan file json di folder Temp adalah untuk simpan data logs dari sistem secara berkala.
 berikut contoh struktur folder ya:

 Temp/
   ├── sensor1/
   │   ├──state.json
   |   |--alert.logs.2024-01-01.json	
   │   ├──alert.logs.2024-01-02.json
   |   
 
 schema untuk file state.json :
 ```json
 {
   "sensor_id": "sensor1",
   "last_updated": "2025-11-04T01:45:00",
   "sync_attempt": 5,
 }
 ```
 schema alert.*.json untul :
 ```json
 {
   "alert": [
	 {
	   "timestamp": "2024-01-01T00:00:00Z",
	   "level": "INFO",
	   "message": "Sensor initialized."
	 },
	 {
	   "timestamp": "2024-01-01T01:00:00Z",
	   "level": "ERROR",
	   "message": "Pressure reading failed."
	 }
	 // ... lebih banyak data logs
   ]
 }
 ```

 kapan alert dibuat ? saat ada kondisi tertentu pada sensor, misalnya pressure di atas threshold tertentu.
 kapan file state.json di update ? setiap kali melaukan sync data ke db / saat action end record.


upcuaclient-vbnet\prompt.md