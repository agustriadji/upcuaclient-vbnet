Forms/DetailRecord

sekarang kita masuk ke detail record yang berada di Form DetailRecord.
tujuannya : 
- menampilkan batch yang sedang berjalan.
- monitoring sensor_data pressure tire dan gauge/leak
- menampilkan chart
- dapat melakukan export
- dapat melakukan end of recording

DetailRecord memiliki 3 bagian tampilan:
1. record_metadata dengan component : 
  - TextBoxBatchId (ada di record_metadata)
  - TextBoxSensorId (PressureId and PressureGauge denagn child nodeId yang digunakan)
  - TextBoxRunningDay (ambil dari record_metadata.start_date dan di kalkulasi dengan last data dari sensor_data.timestamp dengan sensor_type PressureTire)
  - TextBoxSize (ada di record_metadata)
  - TextBoxOperator (ada di record_metadata)
  - TextBoxStartDate (ada di record_metadata)
  - TextBoxStartPressure (last data dari sensor_data.value dengan sensor_type pressureTire)
  - TextBoxState (ada di record_metadata.status)

2. TabPageRecord DataGridView DGVWatch untuk menampilkan list data sensor_data secara realtime : 
  - Field StartPressure (this sensor_data with first data sensor_type PressureTire)
  - Field Pressure (this current pressure, with last sensor_type PressureTire)
  - Field LeakPressure (this current pressure with last data sensor type PressureGauge)
  - Field Timestamp (this last data timestamp data from sensor_data with type PressureTire)
  - data di sort ke DESC
  
  Saat ini sudah ada fungsi aggregate untuk group data berdasarkan combobox dropdown CMBGroupingGraph dengan list (Default,2m,10m,1h,1d).

3. TabPageGraph menampilkan Live chart dengan data yang sudah di aggregate, component menggunakan LiveCharts

info : 
config/sqlite.sql
Module/Core/SQLiteManager

saat ini ada variable rawData dan rawDataRaw itu hanya untuk pressure tire collection data.
kita perlu membuat untuk raw data dan rawDataRaw untuk pressure gauge.

penggunaan var itu adalah berfungsi untuk menjadi data tetap origin saat terjadi action di combobox, sehingga sebelum interval get data berjalan kita masih ada data origin untuk sensor_data_
