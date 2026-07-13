Aplikasi Point of Sale (POS) Versi 1.0

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Framework](https://img.shields.io/badge/Framework-.NET%20Core%20MVC-green.svg)](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview)
[![Database](https://img.shields.io/badge/Database-MySQL-orange.svg)](https://www.mysql.com/)
[![UI](https://img.shields.io/badge/UI-Bootstrap%205-purple.svg)](https://getbootstrap.com/)

Selamat datang di **Aplikasi POS Versi 1.0**, sebuah solusi mutakhir sistem kasir dan manajemen ritel modern yang dirancang untuk menghadirkan kecepatan, akurasi, dan skalabilitas tinggi bagi bisnis Anda. Dibangun di atas ekosistem terbaru **.NET 9** dengan arsitektur **.NET Core MVC**, aplikasi ini memadukan performa backend yang tangguh dengan antarmuka pengguna (*User Interface*) yang elegan, responsif, dan intuitif.

Sistem ini didesain khusus untuk mengatasi tantangan operasional harian ritel berskala dinamis—mulai dari manajemen inventori multi-satuan yang kompleks, analisis keuangan real-time, hingga optimalisasi transaksi di meja kasir. Dengan integrasi teknologi mutakhir seperti perlindungan *Anti-Race Condition* untuk menjamin konsistensi data finansial serta *Database Indexing* untuk query secepat kilat, Aplikasi POS Versi 1.0 adalah investasi digital terbaik untuk memastikan operasional bisnis berjalan tanpa hambatan, aman, dan siap menghadapi pertumbuhan volume transaksi yang masif.

---

## 🌐 Demo & Hak Akses

Uji coba langsung keandalan sistem kami melalui tautan demo berikut:

* **Link Demo:** [http://demopos.runasp.net/](http://demopos.runasp.net/)

### Akun Kredensial Pengujian:
1. **Role: Super Admin / Owner**
   * **Username:** `admin@gmail.com`
   * **Password:** `PasswordAnda!123`
   
2. **Role: Kasir**
   * **Username:** `rani@gmail.com`
   * **Password:** `PasswordAnda!123`

---

## 🛠️ Tech Stack & Fungsionalitas

Aplikasi ini mengadopsi jajaran teknologi industri terkemuka untuk memastikan efisiensi performa tinggi dan kemudahan pemeliharaan (*maintainability*):

* **.NET 9 & .NET Core MVC:** Berjalan pada framework lintas platform tercepat dari Microsoft untuk menangani logika bisnis (*backend*) yang solid, terstruktur dengan pola *Model-View-Controller* (MVC) untuk pemisahan kode yang bersih.
* **Visual Studio 2022:** Lingkungan pengembangan terintegrasi (IDE) utama yang memastikan standardisasi penulisan kode tingkat enterprise.
* **Entity Framework (EF) Core:** ORM (*Object-Relational Mapper*) modern untuk melakukan manipulasi database MySQL secara aman, mencegah celah SQL Injection, dan mempercepat pemrosesan relasi data.
* **MySQL Database:** Sistem manajemen database relasional yang tangguh, andal, dan efisien untuk penyimpanan records transaksi bervolume besar.
* **SignalR:** Pustaka khusus yang memungkinkan komunikasi dua arah secara *real-time* antara server dan browser (misal: pembaruan stok otomatis tanpa perlu *refresh* halaman).
* **Ajax & jQuery:** Digunakan untuk eksekusi asinkronus, membuat interaksi perpindahan data, validasi input, dan transaksi kasir berjalan mulus (*seamless*) tanpa interupsi muat ulang halaman.
* **Bootstrap:** Framework CSS modern yang memastikan seluruh visual aplikasi tampil **Responsive dan Elegan**, adaptif diakses dari perangkat PC, Laptop, maupun Tablet kasir.

---
Download .NET SDK 9.0.315 dan ASP.NET Core 9.0 Runtime (v9.0.17) - Windows Hosting Bundle Installer.
kemudian install

## 🚀 Keunggulan Sistem & Arsitektur

Sistem ini tidak sekadar melakukan operasi CRUD standar, melainkan dirancang dengan standar arsitektur sistem enterprise:

### 1. Performa Database Tinggi (Database Indexing)
Struktur tabel database telah dikonfigurasi menggunakan teknik **Indexing** pada kolom-kolom kritikal (seperti ID produk, nomor transaksi, dan foreign keys). Keunggulannya:
* Pencarian data produk dan pelanggan memakan waktu sub-detik meskipun data mencapai puluhan ribu baris.
* Pembuatan laporan keuangan dan inventori yang agresif tidak akan membebani kinerja server server.

### 2. Keamanan Transaksi Maksimal (Anti-Race Condition & Concurrency Control)
Dilengkapi dengan mekanisme proteksi *Concurrency*, aplikasi ini menjamin keamanan manipulasi data di saat yang bersamaan. Keunggulannya:
* **Mencegah Over-selling:** Jika dua kasir menjual produk dengan sisa stok 1 secara bersamaan, sistem secara cerdas hanya akan meloloskan 1 transaksi tercepat dan menolak transaksi kedua dengan aman.
* **Konsistensi Keuangan:** Saldo kas, laporan arus kas, dan pengurangan stok dijamin 100% akurat tanpa adanya duplikasi data atau kehilangan data finansial (*data corruption*).

### 3. User Experience (UX) Kelas Dunia
Antarmuka yang dirancang sangat modern, minimalis, dan berfokus pada kecepatan kerja operator kasir guna meminimalisir waktu tunggu pelanggan di antrean.

---

## 📦 Detail Fitur per Modul

### 1. Modul Karyawan
* **CRUD Data Karyawan:** Manajemen komprehensif data akun staf, pemblokiran akses, serta pembaruan profil kerja secara terpusat.

### 2. Modul Pelanggan (*Customer*)
* **CRUD Data Pelanggan:** Pencatatan basis data pelanggan setia untuk mempermudah strategi pemasaran, tracking piutang, ataupun pemberian diskon khusus.

### 3. Transaksi Halaman Kasir (Halaman Utama Kasir)
* **Point of Sales Interface:** Antarmuka responsif yang mendukung input barcode, pencarian produk cepat, kalkulasi kembalian otomatis, serta pencetakan struk belanja instan dengan arsitektur asinkronus (Ajax).

### 4. Manajemen Produk
* **CRUD Category & Data Produk:** Pengelompokan barang secara sistematis berdasarkan kategori bisnis.
* **Multi-Satuan Produk:** Mendukung variasi satuan produk dalam satu item (misal: Dus, Pak, Pcs) dengan konversi harga dan stok yang sinkron.
* **Sistem Barcode Terintegrasi:** Fitur bawaan untuk melakukan *generate* barcode unik produk siap cetak untuk efisiensi *scanning* kasir.
* **Ekspor Laporan Produk Makro (Excel):** Mengunduh laporan katalog produk ke format Excel berbasis parameter cerdas: *All* (Semua), *Stok Banyak*, *Stok Sedikit*, dan *Stok Habis*.

### 5. Manajemen Inventori
* **CRUD Inventori & Penyesuaian:** Mengelola siklus keluar masuk barang secara real-time.
* **Ekspor Laporan Inventori Multi-Parameter:** Mengunduh berkas Excel untuk mutasi barang berdasarkan parameter aksi (*Masuk, Keluar, Penyesuaian/Adjustment*).
* **Filter Rentang Tanggal:** Fleksibilitas peninjauan laporan stok berdasarkan *date range* harian, mingguan, bulanan, atau tahunan.

### 6. Manajemen Keuangan & Arus Kas
* **CRUD Transaksi & Metode Pembayaran:** Kebebasan mengonfigurasi multi metode pembayaran (Tunai, Debit, QRIS, Transfer Bank).
* **CRUD Cash Flow:** Pencatatan arus kas masuk dan kas keluar operasional di luar penjualan (seperti biaya listrik, sewa, gaji, dll).
* **Laporan Keuangan Komprehensif:** Pembuatan ringkasan laporan keuangan otomatis yang dikategorikan berdasarkan tipe *Masuk, Keluar, dan Penjualan*.

### 7. Pengaturan Toko & Keamanan Akses
* **Role-Based Access Control (RBAC):** Keamanan sistem berlapis menggunakan manajemen hak akses berbasis peran yang ketat.
* **Multi-Role Support:** Sistem mengenali batasan akses pengguna secara dinamis.
  * **Super Admin / Owner:** Memiliki kontrol mutlak atas laporan keuangan, hapus data, pengaturan toko, dan manajemen karyawan.
  * **Kasir:** Hanya diizinkan mengakses halaman penjualan transaksi kasir dan melihat produk tanpa hak akses konfigurasi keuangan sensitif.
* **CRUD Pengaturan Toko:** Kustomisasi profil toko seperti nama usaha, alamat, nomor telepon, dan pesan kaki (*footer*) pada struk belanja.

---

## ⚙️ Panduan Instalasi di Server IIS Lokal (Komputer / PC)

Ikuti langkah-langkah teknis berikut untuk mendeploy aplikasi ini pada Internet Information Services (IIS) lokal Anda:

### Langkah 1: Persiapan File Publikasi
1. Lakukan proses *Publish* proyek **Aplikasi POS Versi 1.0** dari Visual Studio 2022 ke folder lokal.
2. Buka **IIS Manager** pada Windows Anda.
3. Salin (*Copy*) seluruh file hasil publish tersebut ke dalam direktori server bawaan: `C:\\inetpub\\wwwroot\\nama_folder_aplikasi_anda`.

### Langkah 2: Konfigurasi Application Pool IIS
1. Pada panel *Connections* di IIS Manager, klik kanan menu **Application Pools** lalu pilih **Add Application Pool**.
2. Berikan nama, contoh: `MyAppPool`.
3. Pada opsi *.NET CLR version*, ubah menjadi **No Managed Code** (karena .NET 9 berjalan sebagai aplikasi *self-contained / reverse proxy*).
4. Klik **OK**.
5. Klik kanan pada `MyAppPool` yang baru dibuat, lalu pilih **Advanced Settings**.
6. Cari bagian **Identity**, klik tombol titik tiga, lalu ubah nilainya menjadi **LocalSystem**. Klik **OK**.

### Langkah 3: Konfigurasi Hak Akses File (Folder Security)
1. Buka File Explorer, arahkan ke folder fisik aplikasi Anda di `C:\\inetpub\\wwwroot\\nama_folder_aplikasi_anda`.
2. Klik kanan pada folder tersebut, pilih **Properties**, lalu masuk ke tab **Security**.
3. Klik tombol **Edit...**
4. Pada daftar *Group or user names*, cari dan pilih **IIS_IUSRS**. (Jika tidak ada, klik *Add* dan ketik *IIS_IUSRS*).
5. Pada bagian *Permissions for IIS_IUSRS*, beri centang (*checklist*) pada opsi **Full Control** di kolom **Allow**.
6. Klik **Apply** lalu **OK**. 
   *(Catatan: Langkah ini wajib agar aplikasi memiliki hak penuh membaca/menulis file, gambar produk, dan ekspor dokumen excel).*

### Langkah 4: Mapping Domain Lokal di File Hosts
1. Cari aplikasi **Notepad** di Windows, klik kanan dan pilih **Run as Administrator**.
2. Di dalam Notepad, buka file *hosts* yang terletak di direktori: `C:\Windows\System32\drivers\etc\hosts` *(Ubah filter file di Notepad dari .txt menjadi All Files `*.*` agar file hosts terlihat)*.
3. Tambahkan baris konfigurasi domain lokal berikut di bagian paling bawah file:
```text
   127.0.0.1    [www.posapps.local.com](https://www.posapps.local.com)
   127.0.0.1    posappsv.local.com
