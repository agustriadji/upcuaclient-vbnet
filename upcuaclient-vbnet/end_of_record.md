### end of recording function : 
- melakukan penghentian pengujian yang akan anggap status telah Finished.
- melakukan update data status di table record_metadata dari Recording menjadi Finished dan sync_status dari Pending Menjadi Finished dan update end_date today system.
- melakukan update settings.selectedNodeSensor yang status ya Running menjadi Idle, itu berdasarkan key object PressureTire / PressureGauge dan NodeId = pressure_tire_id/pressure_gauge_id
- sehingga module BackgroungWorker tidak perlu lagi menyimpan data sensor.
- melakukan pengambilan semua data sensor_data, mungkin query ya select sd.* from record_metadata mt left join sensor_data sd on sd.node_id in(mt.pressure_tire_id , mt.pressure_gauge_id) where mt.batch_id = @batchId, untuk kita simpan ke DB.
- melakukan penyimpanan data ke DB master menggunakan SQL server
   -> data yang disimpan adalah data yang sama persis ada di SQLite sebetulnya : record_metadata dan sensor_data.
- data sensor_alert juga akan disimpan ke DB berdasarkan nodeid yang berkaitan.
setelah DB dipastikan tersimpan, baru kita clear sensor_data dengan node_id yang berkaitan akan di hard remove (tapi nanti saja hard remove ya biar mudah testing ya).
- stop timmer dan close form