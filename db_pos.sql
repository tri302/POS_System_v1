-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jul 13, 2026 at 01:17 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `db_pos`
--

-- --------------------------------------------------------

--
-- Table structure for table `alurkass`
--

CREATE TABLE `alurkass` (
  `Id` bigint(20) NOT NULL,
  `Amount` decimal(18,2) NOT NULL,
  `Date` datetime(6) NOT NULL,
  `Notes` longtext DEFAULT NULL,
  `IdCashFlow` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `aspnetroleclaims`
--

CREATE TABLE `aspnetroleclaims` (
  `Id` int(11) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `aspnetroles`
--

CREATE TABLE `aspnetroles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `aspnetroles`
--

INSERT INTO `aspnetroles` (`Id`, `Name`, `NormalizedName`, `ConcurrencyStamp`) VALUES
('843bbdc2-43f3-4464-890f-77343c33c80e', 'Owner', 'OWNER', NULL),
('e80c802b-c57f-41bc-a7f7-594999f0c218', 'Kasir', 'KASIR', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `aspnetuserclaims`
--

CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext DEFAULT NULL,
  `ClaimValue` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `aspnetuserlogins`
--

CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext DEFAULT NULL,
  `UserId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `aspnetuserroles`
--

CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `aspnetuserroles`
--

INSERT INTO `aspnetuserroles` (`UserId`, `RoleId`) VALUES
('38fd85a8-282b-4698-9f8a-53df9a6bfc7b', '843bbdc2-43f3-4464-890f-77343c33c80e'),
('6800a39e-9a37-42b9-86f9-f7f897a12526', '843bbdc2-43f3-4464-890f-77343c33c80e'),
('a306f7f3-6279-448e-8140-297949b17280', 'e80c802b-c57f-41bc-a7f7-594999f0c218');

-- --------------------------------------------------------

--
-- Table structure for table `aspnetusers`
--

CREATE TABLE `aspnetusers` (
  `Id` varchar(255) NOT NULL,
  `Theme` longtext NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext DEFAULT NULL,
  `SecurityStamp` longtext DEFAULT NULL,
  `ConcurrencyStamp` longtext DEFAULT NULL,
  `PhoneNumber` longtext DEFAULT NULL,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `aspnetusers`
--

INSERT INTO `aspnetusers` (`Id`, `Theme`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`) VALUES
('38fd85a8-282b-4698-9f8a-53df9a6bfc7b', 'default', 'owner@gmail.com', 'OWNER@GMAIL.COM', 'owner@gmail.com', 'OWNER@GMAIL.COM', 0, 'AQAAAAIAAYagAAAAEIVjKR+OQXvVBLcNa330t5fySnE3GzqmMFtZCACx9DgQ1K9mEB0xJeNj4ILMm+0BGg==', '4UDS447DSJFV67MIMKXNIVHKMYCRU3S4', '37b51d98-1634-41be-bcba-61e0b4c1a7b0', NULL, 0, 0, NULL, 1, 0),
('6800a39e-9a37-42b9-86f9-f7f897a12526', 'default', 'admin@gmail.com', 'ADMIN@GMAIL.COM', 'admin@gmail.com', 'ADMIN@GMAIL.COM', 1, 'AQAAAAIAAYagAAAAEM0sCcS+tLyDMpyMpDuw8OPPPfTRQpEokb/nXDvgOMqzCxM1ZNRZMhwzr9v43F/hCw==', 'MJMWME25A4XJLLXMTTI7GJKE3A4HMXOS', 'cc04c480-69aa-47b2-a33d-17b07ecde8f4', NULL, 0, 0, NULL, 1, 0),
('a306f7f3-6279-448e-8140-297949b17280', 'default', 'rani@gmail.com', 'RANI@GMAIL.COM', 'rani@gmail.com', 'RANI@GMAIL.COM', 0, 'AQAAAAIAAYagAAAAECse3pqZ+uOlzKj07v0k8b9CEa8upOHwr6FpF6KBum5lxizOrPZKEosWofGyhLiBFw==', '2WDNLLKCPGEFGM6PJNFQO44HTCFRW2LN', '9713646a-0e0a-435e-8e33-c3b8e61304b6', NULL, 0, 0, NULL, 1, 0);

-- --------------------------------------------------------

--
-- Table structure for table `aspnetusertokens`
--

CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `cashflows`
--

CREATE TABLE `cashflows` (
  `IdCashFlow` int(11) NOT NULL,
  `CashFlowName` longtext DEFAULT NULL,
  `IdTipeCashFlow` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `cashflows`
--

INSERT INTO `cashflows` (`IdCashFlow`, `CashFlowName`, `IdTipeCashFlow`) VALUES
(1, 'Penjualan', 1),
(2, 'Restore Transaksi', 1),
(3, 'Pembayaran Piutang', 1),
(4, 'Modal Awal', 1),
(5, 'Modal Tambahan', 1),
(6, 'Pemasukan Lain-lain', 1),
(7, 'Penambahan Stok Barang', 2),
(8, 'Gaji Karyawan', 2),
(9, 'Biaya Operasional', 2),
(10, 'Penarikan Oleh Owner', 2),
(11, 'Perawatan Alat', 2),
(12, 'Return Pelanggan', 2),
(13, 'Listrik', 2),
(14, 'Sewa', 2),
(15, 'Pengeluaran Lain-lain', 2);

-- --------------------------------------------------------

--
-- Table structure for table `inventoris`
--

CREATE TABLE `inventoris` (
  `IdInventori` bigint(20) NOT NULL,
  `NoReferensi` longtext DEFAULT NULL,
  `TipeStok` longtext DEFAULT NULL,
  `Sumber` longtext DEFAULT NULL,
  `TotalModal` decimal(18,2) NOT NULL,
  `Notes` longtext DEFAULT NULL,
  `HargaModal` decimal(18,2) NOT NULL,
  `Jumlah` int(11) NOT NULL,
  `IdProduk` bigint(20) NOT NULL,
  `ProductUnitId` bigint(20) NOT NULL,
  `Tanggal` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `jenispelanggans`
--

CREATE TABLE `jenispelanggans` (
  `Id` int(11) NOT NULL,
  `NamaJenis` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `jenispelanggans`
--

INSERT INTO `jenispelanggans` (`Id`, `NamaJenis`) VALUES
(1, 'UMUM'),
(2, 'PERUSAHAAN'),
(3, 'CV');

-- --------------------------------------------------------

--
-- Table structure for table `karyawans`
--

CREATE TABLE `karyawans` (
  `IdKaryawan` int(11) NOT NULL,
  `UserId` longtext NOT NULL,
  `NIK` longtext DEFAULT NULL,
  `Name` longtext DEFAULT NULL,
  `TempatLahir` longtext DEFAULT NULL,
  `TanggalLahir` datetime(6) NOT NULL,
  `AlamatLengkap` longtext DEFAULT NULL,
  `Email` longtext DEFAULT NULL,
  `NoHp` longtext DEFAULT NULL,
  `StatusNikah` longtext DEFAULT NULL,
  `StatusKerja` longtext DEFAULT NULL,
  `DateJoin` datetime(6) NOT NULL,
  `Catatan` longtext DEFAULT NULL,
  `FotoPath` longtext DEFAULT NULL,
  `StatusKaryawan` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `karyawans`
--

INSERT INTO `karyawans` (`IdKaryawan`, `UserId`, `NIK`, `Name`, `TempatLahir`, `TanggalLahir`, `AlamatLengkap`, `Email`, `NoHp`, `StatusNikah`, `StatusKerja`, `DateJoin`, `Catatan`, `FotoPath`, `StatusKaryawan`) VALUES
(1, '38fd85a8-282b-4698-9f8a-53df9a6bfc7b', '0001', 'Owner', 'Riau', '1990-07-12 00:00:00.000000', '-', 'owner@gmail.com', '54564745', 'Single', 'Karyawan Tetap', '2026-07-12 00:00:00.000000', NULL, '/uploads/2da627bf-7cab-4ef3-b644-ccd6ba43a1b0_20260713_124752_avatar4.png', 'Aktif'),
(2, '6800a39e-9a37-42b9-86f9-f7f897a12526', '0003', 'admin', 'Palembang', '1975-02-12 00:00:00.000000', 'Palembang', 'admin@gmail.com', '54564745', 'Menikah', 'Karyawan Tetap', '2026-06-17 00:00:00.000000', NULL, '/uploads/b1fd7abd-3167-4e54-a298-e8e6c8222f91_20260713_124830_avatar5.png', 'Aktif'),
(3, 'a306f7f3-6279-448e-8140-297949b17280', '0002', 'Rani', 'Palembang', '2002-07-12 00:00:00.000000', '-', 'rani@gmail.com', '54564745', 'Single', 'Karyawan Kontrak', '2026-07-12 00:00:00.000000', NULL, '/uploads/44a3bcb4-bf16-4ea5-976a-6e33e531c8c1_20260713_124811_avatar2.png', 'Aktif');

-- --------------------------------------------------------

--
-- Table structure for table `kategoris`
--

CREATE TABLE `kategoris` (
  `IdKategori` bigint(20) NOT NULL,
  `NamaKategori` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `kategoris`
--

INSERT INTO `kategoris` (`IdKategori`, `NamaKategori`) VALUES
(19, 'Aksesoris Komputer'),
(9, 'Alat Kebersihan'),
(17, 'Alat Kesehatan'),
(27, 'Alat Olahraga'),
(22, 'Alat Tulis'),
(21, 'Baterai & Lampu'),
(10, 'Dekorasi & Furnitur'),
(20, 'Elektronik Rumah Tangga'),
(18, 'Gadget'),
(23, 'Kertas'),
(13, 'Kosmetik'),
(28, 'Lain-lain'),
(26, 'Mainan'),
(5, 'Makanan Beku (Frozen Food)'),
(4, 'Makanan Instan'),
(1, 'Makanan Pokok'),
(2, 'Minuman'),
(16, 'Obat-obatan'),
(7, 'Pembersih Rumah'),
(25, 'Perawatan Kendaraan'),
(11, 'Perawatan Mandi'),
(14, 'Perawatan Pria'),
(8, 'Perlengkapan Dapur'),
(24, 'Perlengkapan Kantor'),
(15, 'Produk Bayi'),
(6, 'Produk Segar'),
(12, 'Skincare'),
(3, 'Snack & Camilan');

-- --------------------------------------------------------

--
-- Table structure for table `laporankeuangans`
--

CREATE TABLE `laporankeuangans` (
  `KodeLaporanKeuangan` varchar(255) NOT NULL,
  `TipeLaporan` longtext DEFAULT NULL,
  `TanggalMulai` datetime(6) DEFAULT NULL,
  `TanggalSelesai` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `laporankeuangans`
--

INSERT INTO `laporankeuangans` (`KodeLaporanKeuangan`, `TipeLaporan`, `TanggalMulai`, `TanggalSelesai`) VALUES
('LAPORAN-20260713-01', 'Masuk', '2026-07-13 00:00:00.000000', '2026-07-13 00:00:00.000000'),
('LAPORAN-20260713-02', 'Keluar', '2026-07-13 00:00:00.000000', '2026-07-13 00:00:00.000000'),
('LAPORAN-20260713-03', 'Penjualan', '2026-07-13 00:00:00.000000', '2026-07-13 00:00:00.000000');

-- --------------------------------------------------------

--
-- Table structure for table `logs`
--

CREATE TABLE `logs` (
  `Id` int(11) NOT NULL,
  `Message` longtext DEFAULT NULL,
  `MessageTemplate` longtext DEFAULT NULL,
  `Level` varchar(50) DEFAULT NULL,
  `TimeStamp` datetime(6) NOT NULL,
  `Exception` longtext DEFAULT NULL,
  `Properties` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `metodebayars`
--

CREATE TABLE `metodebayars` (
  `IdMetodeBayar` int(11) NOT NULL,
  `NamaMetodeBayar` longtext DEFAULT NULL,
  `UrlGambar` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `metodebayars`
--

INSERT INTO `metodebayars` (`IdMetodeBayar`, `NamaMetodeBayar`, `UrlGambar`) VALUES
(1, 'Cash', '/images/metodebayar/962ac1af-5421-4345-b9dc-7b75c026adc8.png'),
(2, 'Transfer Bank', '/images/metodebayar/56e65e30-2d9b-49d1-a2b0-d28b043e8388.png'),
(3, 'Qris', '/images/metodebayar/a338a062-07bb-4666-b81b-3fd37b0d5ad7.png');

-- --------------------------------------------------------

--
-- Table structure for table `pelanggans`
--

CREATE TABLE `pelanggans` (
  `Id` int(11) NOT NULL,
  `Nama` longtext DEFAULT NULL,
  `Alamat` longtext DEFAULT NULL,
  `Email` longtext DEFAULT NULL,
  `Phone` longtext DEFAULT NULL,
  `IsPinned` tinyint(1) NOT NULL,
  `JenisPelangganId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `pelanggans`
--

INSERT INTO `pelanggans` (`Id`, `Nama`, `Alamat`, `Email`, `Phone`, `IsPinned`, `JenisPelangganId`) VALUES
(2, 'PT. SINAR JAYA', 'TJ. ENIM', 'sj@yahoo.com', '0888777345823', 0, 2),
(3, 'CV. Maju Jaya', 'Palembang', 'majujaya@gmail.com', '88433883', 0, 3),
(5, 'UMUM', '-', '-', '-', 1, 1);

-- --------------------------------------------------------

--
-- Table structure for table `pengaturantokos`
--

CREATE TABLE `pengaturantokos` (
  `Id` int(11) NOT NULL,
  `LogoToko` longtext DEFAULT NULL,
  `NamaToko` varchar(150) NOT NULL,
  `AlamatToko` varchar(300) NOT NULL,
  `NomorTelepon` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `pengaturantokos`
--

INSERT INTO `pengaturantokos` (`Id`, `LogoToko`, `NamaToko`, `AlamatToko`, `NomorTelepon`) VALUES
(1, '/uploads/logo/55b72d7b-0e4f-4b59-bc8b-ce08245b6aaa.png', 'Toko Maju Mandiri', 'Jln Lintas Sumatera Palembang-Jambi KM 85,5', '098767643242');

-- --------------------------------------------------------

--
-- Table structure for table `productunits`
--

CREATE TABLE `productunits` (
  `ProductUnitId` bigint(20) NOT NULL,
  `UnitName` longtext DEFAULT NULL,
  `HargaJual` decimal(18,2) NOT NULL,
  `HargaModal` decimal(18,2) NOT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  `IdProduk` bigint(20) NOT NULL,
  `ConversionValue` int(11) NOT NULL,
  `Is_Default` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `productunits`
--

INSERT INTO `productunits` (`ProductUnitId`, `UnitName`, `HargaJual`, `HargaModal`, `IsDeleted`, `IdProduk`, `ConversionValue`, `Is_Default`) VALUES
(1, 'Pcs', 7241.00, 6544.00, 0, 1, 1, 1),
(2, 'Pcs', 8640.00, 7930.00, 0, 2, 1, 1),
(3, 'Pcs', 5217.00, 6634.00, 0, 3, 1, 1),
(4, 'Pcs', 7117.00, 6766.00, 0, 4, 1, 1),
(5, 'Pcs', 8258.00, 6207.00, 0, 5, 1, 1),
(6, 'Pcs', 6813.00, 7435.00, 0, 6, 1, 1),
(7, 'Pcs', 5080.00, 7957.00, 0, 7, 1, 1),
(8, 'Pcs', 9865.00, 7101.00, 0, 8, 1, 1),
(9, 'Pcs', 8738.00, 7918.00, 0, 9, 1, 1),
(10, 'Pcs', 8741.00, 5302.00, 0, 10, 1, 1),
(11, 'Pcs', 8580.00, 7598.00, 0, 11, 1, 1),
(12, 'Pcs', 6400.00, 7029.00, 0, 12, 1, 1),
(13, 'Pcs', 5904.00, 7225.00, 0, 13, 1, 1),
(14, 'Pcs', 9871.00, 5123.00, 0, 14, 1, 1),
(15, 'Pcs', 5616.00, 6841.00, 0, 15, 1, 1),
(16, 'Pcs', 5129.00, 6722.00, 0, 16, 1, 1),
(17, 'Pcs', 6348.00, 6868.00, 0, 17, 1, 1),
(18, 'Pcs', 6873.00, 4211.00, 0, 18, 1, 1),
(19, 'Pcs', 5172.00, 6950.00, 0, 19, 1, 1),
(20, 'Pcs', 7178.00, 4640.00, 0, 20, 1, 1),
(21, 'Pcs', 6977.00, 6766.00, 0, 21, 1, 1),
(22, 'Pcs', 8875.00, 6238.00, 0, 22, 1, 1),
(23, 'Pcs', 5392.00, 5402.00, 0, 23, 1, 1),
(24, 'Pcs', 9940.00, 5224.00, 0, 24, 1, 1),
(25, 'Pcs', 8135.00, 5682.00, 0, 25, 1, 1),
(26, 'Pcs', 9253.00, 6390.00, 0, 26, 1, 1),
(27, 'Pcs', 5654.00, 5793.00, 0, 27, 1, 1),
(28, 'Pcs', 6553.00, 7206.00, 0, 28, 1, 1),
(29, 'Pcs', 5791.00, 6908.00, 0, 29, 1, 1),
(30, 'Pcs', 6575.00, 7773.00, 0, 30, 1, 1),
(31, 'Pcs', 9206.00, 7365.00, 0, 31, 1, 1),
(32, 'Pcs', 6455.00, 5109.00, 0, 32, 1, 1),
(33, 'Pcs', 6433.00, 5949.00, 0, 33, 1, 1),
(34, 'Pcs', 6819.00, 4685.00, 0, 34, 1, 1),
(35, 'Pcs', 6845.00, 6708.00, 0, 35, 1, 1),
(36, 'Pcs', 5542.00, 4719.00, 0, 36, 1, 1),
(37, 'Pcs', 7643.00, 5104.00, 0, 37, 1, 1),
(38, 'Pcs', 9581.00, 5163.00, 0, 38, 1, 1),
(39, 'Pcs', 7826.00, 4917.00, 0, 39, 1, 1),
(40, 'Pcs', 6816.00, 6967.00, 0, 40, 1, 1),
(41, 'Pcs', 8304.00, 7065.00, 0, 41, 1, 1),
(42, 'Pcs', 9453.00, 4776.00, 0, 42, 1, 1),
(43, 'Pcs', 9784.00, 4802.00, 0, 43, 1, 1),
(44, 'Pcs', 8668.00, 5400.00, 0, 44, 1, 1),
(45, 'Pcs', 5710.00, 4743.00, 0, 45, 1, 1),
(46, 'Pcs', 5293.00, 4981.00, 0, 46, 1, 1),
(47, 'Pcs', 6901.00, 5368.00, 0, 47, 1, 1),
(48, 'Pcs', 5918.00, 7553.00, 0, 48, 1, 1),
(49, 'Pcs', 7337.00, 7813.00, 0, 49, 1, 1),
(50, 'Pcs', 6714.00, 6917.00, 0, 50, 1, 1),
(51, 'Pcs', 8674.00, 4082.00, 0, 51, 1, 1),
(52, 'Pcs', 5278.00, 6583.00, 0, 52, 1, 1),
(53, 'Pcs', 9755.00, 7320.00, 0, 53, 1, 1),
(54, 'Pcs', 8593.00, 5887.00, 0, 54, 1, 1),
(55, 'Pcs', 6338.00, 7916.00, 0, 55, 1, 1),
(56, 'Pcs', 6297.00, 6833.00, 0, 56, 1, 1),
(57, 'Pcs', 8351.00, 5251.00, 0, 57, 1, 1),
(58, 'Pcs', 7067.00, 7426.00, 0, 58, 1, 1),
(59, 'Pcs', 8853.00, 5425.00, 0, 59, 1, 1),
(60, 'Pcs', 9744.00, 4171.00, 0, 60, 1, 1),
(61, 'Pcs', 6355.00, 6732.00, 0, 61, 1, 1),
(62, 'Pcs', 6602.00, 7815.00, 0, 62, 1, 1),
(63, 'Pcs', 5760.00, 6453.00, 0, 63, 1, 1),
(64, 'Pcs', 8794.00, 5751.00, 0, 64, 1, 1),
(65, 'Pcs', 7302.00, 7194.00, 0, 65, 1, 1),
(66, 'Pcs', 5800.00, 5757.00, 0, 66, 1, 1),
(67, 'Pcs', 5388.00, 5453.00, 0, 67, 1, 1),
(68, 'Pcs', 9866.00, 4452.00, 0, 68, 1, 1),
(69, 'Pcs', 7051.00, 6878.00, 0, 69, 1, 1),
(70, 'Pcs', 5265.00, 4581.00, 0, 70, 1, 1),
(71, 'Pcs', 7461.00, 5725.00, 0, 71, 1, 1),
(72, 'Pcs', 7362.00, 5971.00, 0, 72, 1, 1),
(73, 'Pcs', 7364.00, 5909.00, 0, 73, 1, 1),
(74, 'Pcs', 8094.00, 4763.00, 0, 74, 1, 1),
(75, 'Pcs', 5724.00, 7346.00, 0, 75, 1, 1),
(76, 'Pcs', 5085.00, 7038.00, 0, 76, 1, 1),
(77, 'Pcs', 6015.00, 4277.00, 0, 77, 1, 1),
(78, 'Pcs', 7154.00, 4702.00, 0, 78, 1, 1),
(79, 'Pcs', 5216.00, 6843.00, 0, 79, 1, 1),
(80, 'Pcs', 8195.00, 5403.00, 0, 80, 1, 1),
(81, 'Pcs', 8489.00, 6689.00, 0, 81, 1, 1),
(82, 'Pcs', 6901.00, 5919.00, 0, 82, 1, 1),
(83, 'Pcs', 8332.00, 4578.00, 0, 83, 1, 1),
(84, 'Pcs', 6208.00, 5645.00, 0, 84, 1, 1),
(85, 'Pcs', 8521.00, 7111.00, 0, 85, 1, 1),
(86, 'Pcs', 6858.00, 7243.00, 0, 86, 1, 1),
(87, 'Pcs', 6361.00, 4971.00, 0, 87, 1, 1),
(88, 'Pcs', 7757.00, 6072.00, 0, 88, 1, 1),
(89, 'Pcs', 6676.00, 7791.00, 0, 89, 1, 1),
(90, 'Pcs', 8405.00, 6687.00, 0, 90, 1, 1),
(91, 'Pcs', 5187.00, 7721.00, 0, 91, 1, 1),
(92, 'Pcs', 9919.00, 5729.00, 0, 92, 1, 1),
(93, 'Pcs', 6356.00, 7792.00, 0, 93, 1, 1),
(94, 'Pcs', 6113.00, 7987.00, 0, 94, 1, 1),
(95, 'Pcs', 5840.00, 6710.00, 0, 95, 1, 1),
(96, 'Pcs', 6455.00, 7915.00, 0, 96, 1, 1),
(97, 'Pcs', 7587.00, 6533.00, 0, 97, 1, 1),
(98, 'Pcs', 9134.00, 7823.00, 0, 98, 1, 1),
(99, 'Pcs', 9945.00, 4261.00, 0, 99, 1, 1),
(100, 'Pcs', 8309.00, 5994.00, 1, 100, 1, 1),
(102, 'Dus', 205000.00, 200000.00, 1, 100, 40, 0),
(103, 'Dus', 205000.00, 200000.00, 0, 100, 40, 0),
(104, 'Pcs', 1600.00, 1500.00, 0, 100, 1, 1);

-- --------------------------------------------------------

--
-- Table structure for table `produkgambars`
--

CREATE TABLE `produkgambars` (
  `IdProdukGambar` bigint(20) NOT NULL,
  `PathGambar` longtext DEFAULT NULL,
  `IdProduk` bigint(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `produks`
--

CREATE TABLE `produks` (
  `IdProduk` bigint(20) NOT NULL,
  `NamaProduk` longtext DEFAULT NULL,
  `KodeBarcode` longtext DEFAULT NULL,
  `SKU` longtext DEFAULT NULL,
  `StatusProduk` longtext DEFAULT NULL,
  `Deskripsi` longtext DEFAULT NULL,
  `IdKategori` bigint(20) NOT NULL,
  `Stok` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `produks`
--

INSERT INTO `produks` (`IdProduk`, `NamaProduk`, `KodeBarcode`, `SKU`, `StatusProduk`, `Deskripsi`, `IdKategori`, `Stok`) VALUES
(1, 'INDOMIE KALDU AYAM', '089686010107', 'INDOM-10107', 'Aktif', '', 28, 500),
(2, 'INDOMIE SOTO MIE', '089686010343', 'INDOM-10343', 'Aktif', '', 28, 500),
(3, 'INDOMIE SOTO BANJAR', '089686010435', 'INDOM-10435', 'Aktif', '', 28, 500),
(4, 'INDOMIE GORENG', '089686010824', 'INDOM-10824', 'Aktif', '', 28, 500),
(5, 'INDOMIE GRG PEDAS', '089686011036', 'INDOM-11036', 'Aktif', '', 28, 500),
(6, 'SARIMI ISI2 KOYA JERUK', '089686017755', 'SARIM-17755', 'Aktif', '', 28, 500),
(7, 'SARIMI ISI2 BASO SAPI', '089686018011', 'SARIM-18011', 'Aktif', '', 28, 500),
(8, 'SARIMI ISI2 TONGSENG AYAM', '089686018080', 'SARIM-18080', 'Aktif', '', 28, 500),
(9, 'INDOMIE COTO MAKASSAR', '089686043051', 'INDOM-43051', 'Aktif', '', 28, 500),
(10, 'INDOMIE SOTO LAMONGAN', '089686043419', 'INDOM-43419', 'Aktif', '', 28, 500),
(11, 'SUPERMI SOP BUNTUT', '089686044072', 'SUPER-44072', 'Aktif', '', 28, 500),
(12, 'SARIMI ISI2 AYAM KECAP', '089686048704', 'SARIM-48704', 'Aktif', '', 28, 500),
(13, 'BUMBU RACIK NASI GORENG', '089686386011', 'BUMBU-86011', 'Aktif', '', 28, 500),
(14, 'BUMBU RACIK AYAM GORENG 26G', '089686386066', 'BUMBU-86066', 'Aktif', '', 28, 500),
(15, 'BUMBU RACIK SAYUR ASEM', '089686386110', 'BUMBU-86110', 'Aktif', '', 28, 500),
(16, 'BUMBU RACIK TUMIS', '089686386219', 'BUMBU-86219', 'Aktif', '', 28, 500),
(17, 'BUMBU RACIK SAYUR SOP 20G', '089686386318', 'BUMBU-86318', 'Aktif', '', 28, 500),
(18, 'BUMBU RACIK IKAN GORENG', '089686386462', 'BUMBU-86462', 'Aktif', '', 28, 500),
(19, 'BUMBU INDOFOOD KARE', '089686440133', 'BUMBU-40133', 'Aktif', '', 28, 500),
(20, 'BUMBU INDOFOOD RAWON', '089686440386', 'BUMBU-40386', 'Aktif', '', 28, 500),
(21, 'BUMBU INDOFOOD SOTO AYAM', '089686440584', 'BUMBU-40584', 'Aktif', '', 28, 500),
(22, 'FREISS SIRUP MADU COCOPANDAN 500ML', '089686834109', 'FREIS-34109', 'Aktif', '', 28, 500),
(23, 'FREISS SIRUP MADU FRAMBOZ 500ML', '089686834154', 'FREIS-34154', 'Aktif', '', 28, 500),
(24, 'SARUNG TENUN CAP KUDA', '09098876567', 'SARUN-76567', 'Aktif', '', 28, 500),
(25, 'SARUNG TENUN CAP KADAL', '09098876568', 'SARUN-76568', 'Aktif', '', 28, 500),
(26, 'SARUNG TENUN CAP KAMPRET', '09098876569', 'SARUN-76569', 'Aktif', '', 28, 500),
(27, 'P/CASE BATIK JENIA', '1000000163018', 'P/CAS-63018', 'Aktif', '', 28, 500),
(28, 'KEMBANG API OLYMPIC TORCH ISI 6', '100000321034', 'KEMBA-21034', 'Aktif', '', 28, 500),
(29, 'KAOS KAKI BALMORAL TIPIS', '100125268887', 'KAOS -68887', 'Aktif', '', 28, 500),
(30, 'STARWAY KAOS KAKI 159', '1003020211219', 'STARW-11219', 'Aktif', '', 28, 500),
(31, 'SATTO SLIM ESSENCE 55G', '10064002', 'SATTO-64002', 'Aktif', '', 28, 500),
(32, 'SARIMI AYAM', '1008968601770', 'SARIM-01770', 'Aktif', '', 28, 500),
(33, 'SARIMI AYAM GORENG', '1008968604870', 'SARIM-04870', 'Aktif', '', 28, 500),
(34, 'POP MIE MINI RS AYAM BAWANG', '1008968606102', 'POP M-06102', 'Aktif', '', 28, 500),
(35, 'SARIM SOTO KOYA DUS', '10089686917267', 'SARIM-17267', 'Aktif', '', 28, 500),
(36, 'STAR SLICED JALAPENOS 1QT', '101110700238', 'STAR -00238', 'Aktif', '', 28, 500),
(37, 'SPIDOL STAMP MICKEY', '10113000013', 'SPIDO-00013', 'Aktif', '', 28, 500),
(38, 'MAP FILE CAMPUS JOYKO A5', '10139000002', 'MAP F-00002', 'Aktif', '', 28, 500),
(39, 'SANDAL ANDO', '1014100001656', 'SANDA-01656', 'Aktif', '', 28, 500),
(40, 'VOS L LEAF A5 50', '102000116006', 'VOS L-16006', 'Aktif', '', 28, 500),
(41, 'VOS L LEAF B5 100', '102000116007', 'VOS L-16007', 'Aktif', '', 28, 500),
(42, 'E-PRINT RBON CARTG 8750', '10343600096', 'E-PRI-00096', 'Aktif', '', 28, 500),
(43, 'EPSON RF 8758 ORG-17', '10343600102', 'EPSON-00102', 'Aktif', '', 28, 500),
(44, 'EPSON CARTRI 7754 KW2', '10343600447', 'EPSON-00447', 'Aktif', '', 28, 500),
(45, 'EPSON CARTID SO15140', '10343601802', 'EPSON-01802', 'Aktif', '', 28, 500),
(46, 'BUKU TEBAL NP', '1050003200005', 'BUKU -00005', 'Aktif', '', 28, 500),
(47, 'NOTES KOTAK NP', '1050003800007', 'NOTES-00007', 'Aktif', '', 28, 500),
(48, 'BUKU MEWARNAI DL 0337', '1057011902273', 'BUKU -02273', 'Aktif', '', 28, 500),
(49, 'BUKU MEWARNAI 2B 0407', '1057011904079', 'BUKU -04079', 'Aktif', '', 28, 500),
(50, 'CM GELAS SET 7PC AROMA C-1898', '1070902154759', 'CM GE-54759', 'Aktif', '', 28, 500),
(51, 'CM GELAS SET 6PC ROME 75007 RM', '1070906118177', 'CM GE-18177', 'Aktif', '', 28, 500),
(52, 'CM GELAS SET 7PC DRINKING C-1910', '1070906121702', 'CM GE-21702', 'Aktif', '', 28, 500),
(53, 'CM GELAS SET 7PC DREAMLIFE C-1981', '1070906130964', 'CM GE-30964', 'Aktif', '', 28, 500),
(54, 'HUGO BOS INTENSE WOMAN', '1073705604', 'HUGO -05604', 'Aktif', '', 28, 500),
(55, 'REYNOLDS WRAP ALUMINIUM FOIL 8M.', '10900300100', 'REYNO-00100', 'Aktif', '', 28, 500),
(56, 'REYNOLDS WRAP ALUMINIUM FOIL 16M.', '10900300117', 'REYNO-00117', 'Aktif', '', 28, 500),
(57, 'REYNOLDS SAMPLER 5M', '10900300131', 'REYNO-00131', 'Aktif', '', 28, 500),
(58, 'DAIMOND SAMPLER 5M', '10900300810', 'DAIMO-00810', 'Aktif', '', 28, 500),
(59, 'REYNOLDS ZIPPER STORAGE MED (30)', '10900302616', 'REYNO-02616', 'Aktif', '', 28, 500),
(60, 'RAYNOLDS ZIPER FREZ 18GAL', '10900302647', 'RAYNO-02647', 'Aktif', '', 28, 500),
(61, 'REYNOLDS CUT-RITE WAX PAPER 23M.', '10900303309', 'REYNO-03309', 'Aktif', '', 28, 500),
(62, 'RAYNOLDS CUT-RITE WAX PAPER 4.64M2', '10900303316', 'RAYNO-03316', 'Aktif', '', 28, 500),
(63, 'VB REFILL 7M', '10900303477', 'VB RE-03477', 'Aktif', '', 28, 500),
(64, 'PULPEN AE 7', '1100120010473', 'PULPE-10473', 'Aktif', '', 28, 500),
(65, 'PULPEN AE 7 MERAH', '1100120030471', 'PULPE-30471', 'Aktif', '', 28, 500),
(66, 'MARTINI ROSSI 1LTR', '11034410048', 'MARTI-10048', 'Aktif', '', 28, 500),
(67, 'LEM CASTOL KECIL', '11100416', 'LEM C-00416', 'Aktif', '', 28, 500),
(68, 'PASEO TISSUE SMART PARFUMED 6X12SCT', '11111111111', 'PASEO-11111', 'Aktif', '', 28, 500),
(69, 'ALPO DOG BEEF & RICE 374G', '11132000240', 'ALPO -00240', 'Aktif', '', 28, 500),
(70, 'ALPO CHICKEN CLASSIC CHUNKY 623G', '11132000332', 'ALPO -00332', 'Aktif', '', 28, 500),
(71, 'ALPO BEEF CLASSIC CHUNKY 623G', '11132000356', 'ALPO -00356', 'Aktif', '', 28, 500),
(72, 'ALPO TRIO CLASSIC CHUNKY 623G', '11132000363', 'ALPO -00363', 'Aktif', '', 28, 500),
(73, 'ALPO DOG BEEF & LIVER 374G', '11132003616', 'ALPO -03616', 'Aktif', '', 28, 500),
(74, 'ALPO DOG CHICKEN 374G', '11132003623', 'ALPO -03623', 'Aktif', '', 28, 500),
(75, 'ALPO TOP SIRLOIN 374G', '11132108359', 'ALPO -08359', 'Aktif', '', 28, 500),
(76, 'ALPO FILEDMIGNON 374G', '11132108403', 'ALPO -08403', 'Aktif', '', 28, 500),
(77, 'ALPO ROASTED CHICKEN 374G', '11132108601', 'ALPO -08601', 'Aktif', '', 28, 500),
(78, 'ALPO CHOP HOUSE RIBEYE 374G', '11132117559', 'ALPO -17559', 'Aktif', '', 28, 500),
(79, 'ALPO PRIME CUTS 623G BEEF', '11132125189', 'ALPO -25189', 'Aktif', '', 28, 500),
(80, 'ALPO PRIME CUTS 623G LAMB&RICE', '11132125479', 'ALPO -25479', 'Aktif', '', 28, 500),
(81, 'MOREHOUSE MUSTARD 567G', '11209002207', 'MOREH-02207', 'Aktif', '', 28, 500),
(82, 'TABASCO RED PEPPER SAUCE 60ML.', '11210000018', 'TABAS-00018', 'Aktif', '', 28, 500),
(83, 'TABASCO RED PEPPER SAUCE 150ML.', '11210000155', 'TABAS-00155', 'Aktif', '', 28, 500),
(84, 'TABASCO RED PEPPER SAUCE 350ML', '1121000032', 'TABAS-00032', 'Aktif', '', 28, 500),
(85, 'TABASCO 60ML HARBANERO HOT', '11210006508', 'TABAS-06508', 'Aktif', '', 28, 500),
(86, 'TABASCO 60ML GARLICPEPPER', '11210007253', 'TABAS-07253', 'Aktif', '', 28, 500),
(87, 'TABASCO 60ML GREENPEPPER MILD', '11210009387', 'TABAS-09387', 'Aktif', '', 28, 500),
(88, 'TYL ASSORTED CANDIES 320G', '1123456123456', 'TYL A-23456', 'Aktif', '', 28, 500),
(89, 'PONDS COMP BC 68G', '1162828537133', 'PONDS-37133', 'Aktif', '', 28, 500),
(90, 'FINNA 300G RASEDA KRUPUK UDANG', '11747168038', 'FINNA-68038', 'Aktif', '', 28, 500),
(91, 'DUA KELINCI100G', '11747233026', 'DUA K-33026', 'Aktif', '', 28, 500),
(92, 'DUA KELINCI KACANG GARINGPCK 250G', '11747233033', 'DUA K-33033', 'Aktif', '', 28, 500),
(93, 'DUA KELINCI KC KULIT 500 GR', '11747233040', 'DUA K-33040', 'Aktif', '', 28, 499),
(94, 'KACANG DUA KELINCI 1000 GR', '11747233057', 'KACAN-33057', 'Aktif', '', 28, 499),
(95, 'REGE KCG ATOM 65G', '11747233125', 'REGE -33125', 'Aktif', '', 28, 499),
(96, 'DOUBLE RABBIT KACANG KULIT 500G', '11747233897', 'DOUBL-33897', 'Aktif', '', 28, 499),
(97, 'SARI GURIH 100G', '11747233934', 'SARI -33934', 'Aktif', '', 28, 496),
(98, 'SARI GURIH 250G', '11747233941', 'SARI -33941', 'Aktif', '', 28, 496),
(99, 'DK KACANG GARING 36GR', '11747233965', 'DK KA-33965', 'Aktif', '', 28, 2),
(100, 'SUKRO KACANG ATOM 43G', '11747233972', 'SUKRO-33972', 'Aktif', NULL, 28, 0);

-- --------------------------------------------------------

--
-- Table structure for table `shifts`
--

CREATE TABLE `shifts` (
  `IdShift` int(11) NOT NULL,
  `IdKaryawan` int(11) NOT NULL,
  `WaktuBuka` datetime(6) NOT NULL,
  `WaktuTutup` datetime(6) DEFAULT NULL,
  `SaldoAwal` decimal(18,2) NOT NULL,
  `SaldoAkhirSistem` decimal(18,2) NOT NULL,
  `SaldoAkhirFisik` decimal(18,2) NOT NULL,
  `Selisih` decimal(18,2) NOT NULL,
  `SaldoAkhirFisikRevisi` decimal(18,2) NOT NULL,
  `SelisihRevisi` decimal(18,2) NOT NULL,
  `KeteranganRevisi` longtext DEFAULT NULL,
  `TanggalRevisi` datetime(6) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `sumberstoks`
--

CREATE TABLE `sumberstoks` (
  `Id` int(11) NOT NULL,
  `Nama` longtext NOT NULL,
  `TipeStokId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sumberstoks`
--

INSERT INTO `sumberstoks` (`Id`, `Nama`, `TipeStokId`) VALUES
(1, 'Penambahan Stock', 1),
(2, 'Barang Rusak', 2),
(3, 'Barang Kadaluarsa', 2),
(4, 'Stock Opname', 3);

-- --------------------------------------------------------

--
-- Table structure for table `tipecashflows`
--

CREATE TABLE `tipecashflows` (
  `IdTipeCashFlow` int(11) NOT NULL,
  `TipeCashFlowName` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tipecashflows`
--

INSERT INTO `tipecashflows` (`IdTipeCashFlow`, `TipeCashFlowName`) VALUES
(1, 'Masuk'),
(2, 'Keluar');

-- --------------------------------------------------------

--
-- Table structure for table `tipestoks`
--

CREATE TABLE `tipestoks` (
  `Id` int(11) NOT NULL,
  `Nama` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `tipestoks`
--

INSERT INTO `tipestoks` (`Id`, `Nama`) VALUES
(1, 'Masuk'),
(2, 'Keluar'),
(3, 'Penyesuaian');

-- --------------------------------------------------------

--
-- Table structure for table `transaksidetails`
--

CREATE TABLE `transaksidetails` (
  `Id` bigint(20) NOT NULL,
  `Qty` int(11) NOT NULL,
  `HargaJual` decimal(18,2) NOT NULL,
  `IdTransaksi` bigint(20) NOT NULL,
  `IdProduk` bigint(20) NOT NULL,
  `ProductUnitId` bigint(20) NOT NULL,
  `Profit` decimal(18,2) NOT NULL,
  `Conversion` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transaksis`
--

CREATE TABLE `transaksis` (
  `IdTransaksi` bigint(20) NOT NULL,
  `TotalBelanja` decimal(18,2) NOT NULL,
  `Notes` longtext DEFAULT NULL,
  `NominalBayar` decimal(18,2) NOT NULL,
  `Kembalian` decimal(18,2) NOT NULL,
  `NoTransaksi` longtext DEFAULT NULL,
  `Status` longtext NOT NULL,
  `TanggalTransaksi` datetime(6) NOT NULL,
  `IdPelanggan` int(11) NOT NULL,
  `IdKaryawan` int(11) NOT NULL,
  `IdMetodeBayar` int(11) NOT NULL,
  `IdShift` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20260712084426_InitialCreate', '9.0.10');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `alurkass`
--
ALTER TABLE `alurkass`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AlurKass_IdCashFlow` (`IdCashFlow`);

--
-- Indexes for table `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`);

--
-- Indexes for table `aspnetroles`
--
ALTER TABLE `aspnetroles`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `RoleNameIndex` (`NormalizedName`);

--
-- Indexes for table `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_AspNetUserClaims_UserId` (`UserId`);

--
-- Indexes for table `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  ADD KEY `IX_AspNetUserLogins_UserId` (`UserId`);

--
-- Indexes for table `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD PRIMARY KEY (`UserId`,`RoleId`),
  ADD KEY `IX_AspNetUserRoles_RoleId` (`RoleId`);

--
-- Indexes for table `aspnetusers`
--
ALTER TABLE `aspnetusers`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  ADD KEY `EmailIndex` (`NormalizedEmail`);

--
-- Indexes for table `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD PRIMARY KEY (`UserId`,`LoginProvider`,`Name`);

--
-- Indexes for table `cashflows`
--
ALTER TABLE `cashflows`
  ADD PRIMARY KEY (`IdCashFlow`),
  ADD KEY `IX_CashFlows_IdTipeCashFlow` (`IdTipeCashFlow`);

--
-- Indexes for table `inventoris`
--
ALTER TABLE `inventoris`
  ADD PRIMARY KEY (`IdInventori`),
  ADD KEY `IX_Inventoris_IdProduk` (`IdProduk`),
  ADD KEY `IX_Inventoris_ProductUnitId` (`ProductUnitId`);

--
-- Indexes for table `jenispelanggans`
--
ALTER TABLE `jenispelanggans`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `karyawans`
--
ALTER TABLE `karyawans`
  ADD PRIMARY KEY (`IdKaryawan`);

--
-- Indexes for table `kategoris`
--
ALTER TABLE `kategoris`
  ADD PRIMARY KEY (`IdKategori`),
  ADD UNIQUE KEY `UX_Kategori_NamaKategori` (`NamaKategori`);

--
-- Indexes for table `laporankeuangans`
--
ALTER TABLE `laporankeuangans`
  ADD PRIMARY KEY (`KodeLaporanKeuangan`);

--
-- Indexes for table `logs`
--
ALTER TABLE `logs`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Logs_Level` (`Level`),
  ADD KEY `IX_Logs_Level_TimeStamp` (`Level`,`TimeStamp`),
  ADD KEY `IX_Logs_TimeStamp` (`TimeStamp`);

--
-- Indexes for table `metodebayars`
--
ALTER TABLE `metodebayars`
  ADD PRIMARY KEY (`IdMetodeBayar`);

--
-- Indexes for table `pelanggans`
--
ALTER TABLE `pelanggans`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Pelanggans_JenisPelangganId` (`JenisPelangganId`);

--
-- Indexes for table `pengaturantokos`
--
ALTER TABLE `pengaturantokos`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `productunits`
--
ALTER TABLE `productunits`
  ADD PRIMARY KEY (`ProductUnitId`),
  ADD KEY `IX_ProductUnits_IdProduk` (`IdProduk`);

--
-- Indexes for table `produkgambars`
--
ALTER TABLE `produkgambars`
  ADD PRIMARY KEY (`IdProdukGambar`),
  ADD KEY `IX_ProdukGambars_IdProduk` (`IdProduk`);

--
-- Indexes for table `produks`
--
ALTER TABLE `produks`
  ADD PRIMARY KEY (`IdProduk`),
  ADD KEY `IX_Produks_IdKategori` (`IdKategori`);

--
-- Indexes for table `shifts`
--
ALTER TABLE `shifts`
  ADD PRIMARY KEY (`IdShift`),
  ADD KEY `IX_shifts_IdKaryawan` (`IdKaryawan`);

--
-- Indexes for table `sumberstoks`
--
ALTER TABLE `sumberstoks`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_SumberStoks_TipeStokId` (`TipeStokId`);

--
-- Indexes for table `tipecashflows`
--
ALTER TABLE `tipecashflows`
  ADD PRIMARY KEY (`IdTipeCashFlow`);

--
-- Indexes for table `tipestoks`
--
ALTER TABLE `tipestoks`
  ADD PRIMARY KEY (`Id`);

--
-- Indexes for table `transaksidetails`
--
ALTER TABLE `transaksidetails`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_TransaksiDetails_IdProduk` (`IdProduk`),
  ADD KEY `IX_TransaksiDetails_IdTransaksi` (`IdTransaksi`),
  ADD KEY `IX_TransaksiDetails_ProductUnitId` (`ProductUnitId`);

--
-- Indexes for table `transaksis`
--
ALTER TABLE `transaksis`
  ADD PRIMARY KEY (`IdTransaksi`),
  ADD KEY `IX_Transaksis_IdKaryawan` (`IdKaryawan`),
  ADD KEY `IX_Transaksis_IdMetodeBayar` (`IdMetodeBayar`),
  ADD KEY `IX_Transaksis_IdPelanggan` (`IdPelanggan`),
  ADD KEY `IX_Transaksis_IdShift` (`IdShift`);

--
-- Indexes for table `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `alurkass`
--
ALTER TABLE `alurkass`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `cashflows`
--
ALTER TABLE `cashflows`
  MODIFY `IdCashFlow` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT for table `inventoris`
--
ALTER TABLE `inventoris`
  MODIFY `IdInventori` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `jenispelanggans`
--
ALTER TABLE `jenispelanggans`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `karyawans`
--
ALTER TABLE `karyawans`
  MODIFY `IdKaryawan` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `kategoris`
--
ALTER TABLE `kategoris`
  MODIFY `IdKategori` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

--
-- AUTO_INCREMENT for table `logs`
--
ALTER TABLE `logs`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `metodebayars`
--
ALTER TABLE `metodebayars`
  MODIFY `IdMetodeBayar` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `pelanggans`
--
ALTER TABLE `pelanggans`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `pengaturantokos`
--
ALTER TABLE `pengaturantokos`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `productunits`
--
ALTER TABLE `productunits`
  MODIFY `ProductUnitId` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=105;

--
-- AUTO_INCREMENT for table `produkgambars`
--
ALTER TABLE `produkgambars`
  MODIFY `IdProdukGambar` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `produks`
--
ALTER TABLE `produks`
  MODIFY `IdProduk` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=101;

--
-- AUTO_INCREMENT for table `shifts`
--
ALTER TABLE `shifts`
  MODIFY `IdShift` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `sumberstoks`
--
ALTER TABLE `sumberstoks`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `tipecashflows`
--
ALTER TABLE `tipecashflows`
  MODIFY `IdTipeCashFlow` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `tipestoks`
--
ALTER TABLE `tipestoks`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `transaksidetails`
--
ALTER TABLE `transaksidetails`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `transaksis`
--
ALTER TABLE `transaksis`
  MODIFY `IdTransaksi` bigint(20) NOT NULL AUTO_INCREMENT;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `alurkass`
--
ALTER TABLE `alurkass`
  ADD CONSTRAINT `FK_AlurKass_CashFlows_IdCashFlow` FOREIGN KEY (`IdCashFlow`) REFERENCES `cashflows` (`IdCashFlow`);

--
-- Constraints for table `aspnetroleclaims`
--
ALTER TABLE `aspnetroleclaims`
  ADD CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetuserclaims`
--
ALTER TABLE `aspnetuserclaims`
  ADD CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetuserlogins`
--
ALTER TABLE `aspnetuserlogins`
  ADD CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetuserroles`
--
ALTER TABLE `aspnetuserroles`
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `aspnetusertokens`
--
ALTER TABLE `aspnetusertokens`
  ADD CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `cashflows`
--
ALTER TABLE `cashflows`
  ADD CONSTRAINT `FK_CashFlows_TipeCashFlows_IdTipeCashFlow` FOREIGN KEY (`IdTipeCashFlow`) REFERENCES `tipecashflows` (`IdTipeCashFlow`) ON DELETE CASCADE;

--
-- Constraints for table `inventoris`
--
ALTER TABLE `inventoris`
  ADD CONSTRAINT `FK_Inventoris_ProductUnits_ProductUnitId` FOREIGN KEY (`ProductUnitId`) REFERENCES `productunits` (`ProductUnitId`),
  ADD CONSTRAINT `FK_Inventoris_Produks_IdProduk` FOREIGN KEY (`IdProduk`) REFERENCES `produks` (`IdProduk`);

--
-- Constraints for table `pelanggans`
--
ALTER TABLE `pelanggans`
  ADD CONSTRAINT `FK_Pelanggans_JenisPelanggans_JenisPelangganId` FOREIGN KEY (`JenisPelangganId`) REFERENCES `jenispelanggans` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `productunits`
--
ALTER TABLE `productunits`
  ADD CONSTRAINT `FK_ProductUnits_Produks_IdProduk` FOREIGN KEY (`IdProduk`) REFERENCES `produks` (`IdProduk`) ON DELETE CASCADE;

--
-- Constraints for table `produkgambars`
--
ALTER TABLE `produkgambars`
  ADD CONSTRAINT `FK_ProdukGambars_Produks_IdProduk` FOREIGN KEY (`IdProduk`) REFERENCES `produks` (`IdProduk`) ON DELETE CASCADE;

--
-- Constraints for table `produks`
--
ALTER TABLE `produks`
  ADD CONSTRAINT `FK_Produks_Kategoris_IdKategori` FOREIGN KEY (`IdKategori`) REFERENCES `kategoris` (`IdKategori`) ON DELETE CASCADE;

--
-- Constraints for table `shifts`
--
ALTER TABLE `shifts`
  ADD CONSTRAINT `FK_shifts_Karyawans_IdKaryawan` FOREIGN KEY (`IdKaryawan`) REFERENCES `karyawans` (`IdKaryawan`) ON DELETE CASCADE;

--
-- Constraints for table `sumberstoks`
--
ALTER TABLE `sumberstoks`
  ADD CONSTRAINT `FK_SumberStoks_TipeStoks_TipeStokId` FOREIGN KEY (`TipeStokId`) REFERENCES `tipestoks` (`Id`) ON DELETE CASCADE;

--
-- Constraints for table `transaksidetails`
--
ALTER TABLE `transaksidetails`
  ADD CONSTRAINT `FK_TransaksiDetails_ProductUnits_ProductUnitId` FOREIGN KEY (`ProductUnitId`) REFERENCES `productunits` (`ProductUnitId`),
  ADD CONSTRAINT `FK_TransaksiDetails_Produks_IdProduk` FOREIGN KEY (`IdProduk`) REFERENCES `produks` (`IdProduk`),
  ADD CONSTRAINT `FK_TransaksiDetails_Transaksis_IdTransaksi` FOREIGN KEY (`IdTransaksi`) REFERENCES `transaksis` (`IdTransaksi`) ON DELETE CASCADE;

--
-- Constraints for table `transaksis`
--
ALTER TABLE `transaksis`
  ADD CONSTRAINT `FK_Transaksis_Karyawans_IdKaryawan` FOREIGN KEY (`IdKaryawan`) REFERENCES `karyawans` (`IdKaryawan`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Transaksis_MetodeBayars_IdMetodeBayar` FOREIGN KEY (`IdMetodeBayar`) REFERENCES `metodebayars` (`IdMetodeBayar`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Transaksis_Pelanggans_IdPelanggan` FOREIGN KEY (`IdPelanggan`) REFERENCES `pelanggans` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_Transaksis_shifts_IdShift` FOREIGN KEY (`IdShift`) REFERENCES `shifts` (`IdShift`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
