# Catatan Struktur Project `upcuaclient-vbnet`

Dokumen ini mencatat gambaran cepat struktur project agar workflow Visual Studio + Cursor lebih rapi.

## Root Solution dan Project

- Solution: `upcuaclient-vbnet.sln`
- Project: `upcuaclient-vbnet/upcuaclient-vbnet.vbproj`
- Startup object: `Sub Main` (WinForms / `WinExe`)

## Entry Point dan Alur Startup

1. `Main.vb`
   - Menjalankan pengecekan single instance (mutex).
   - Inisialisasi background worker.
   - Menjalankan aplikasi via `Application.Run(New TrayAppContext())`.
2. `Modules/Core/TrayAppContext.vb`
   - Inisialisasi tray icon + context menu.
   - Membuat dan mengelola `MainFormNew`.
3. `Forms/MainFormNew.vb`
   - Menjadi UI utama operasional.
   - Memuat data sensor/recording, status koneksi, alert, dan aksi sync/export/import.

## Struktur Folder Utama

- `Forms/`
  - UI WinForms (`MainFormNew`, `MainForm`, `FormConfigManager`, `FormConfigSensor`, `DetailRecord`, dll).
  - Tiap form umumnya punya trio file: `.vb`, `.Designer.vb`, `.resx`.
- `Modules/Core/`
  - Logic inti aplikasi: OPC, manager data, settings/config, logger, tray context, background worker.
- `Modules/Database/`
  - Koneksi database (contoh: `SqlServerConnection.vb`).
- `Modules/Interfaces/`
  - Kontrak/interface data antar komponen.
- `Modules/Manager/`
  - Utilitas manajemen (contoh: `TimeManager.vb`).
- `My Project/`
  - `Application.Designer.vb`, `Settings`, `Resources` (konfigurasi project VB.NET).
- `Config/`
  - Berkas konfigurasi dan script SQL (`meta.json`, `sqlite.sql`, `sqlserver.sql`).
- `Assets/`, `Resources/`
  - Ikon/gambar pendukung UI.
- `bin/`, `obj/`
  - Output build dan artefak intermediate.

## File Konfigurasi Penting

- `App.config`
- `packages.config`
- `Config/meta.json`
- `Config/sqlite.sql`
- `Config/sqlserver.sql`

## Catatan Workflow

- Gunakan **Visual Studio** untuk:
  - Form Designer
  - Packaging/installer
  - Debugging runtime
- Gunakan **Cursor** untuk:
  - Implementasi/refactor code
  - Review logic
  - Dokumentasi perubahan

---

Last updated: 2026-05-28
