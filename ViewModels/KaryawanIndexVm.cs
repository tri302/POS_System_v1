namespace POS_System.ViewModels
{
    public class KaryawanIndexVm
    {
        // 1. Properti untuk menampung DAFTAR DATA Karyawan yang telah dipaginasi
        //    Ini adalah data yang akan di-looping di dalam tabel HTML
        public List<KaryawanVm>? KaryawanList { get; set; }
        // 2. Properti untuk menampung INFORMASI PAGINASI (diambil dari PaginatedList)
        //    Ini digunakan untuk membuat tombol Previous/Next dan nomor halaman
        /// <summary>
        /// Nomor halaman saat ini (Current Page Index).
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Total semua halaman yang tersedia (Total number of pages).
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// Status: Apakah ada halaman sebelumnya? (Digunakan untuk tombol "Previous").
        /// </summary>
        public bool HasPreviousPage { get; set; }
        /// <summary>
        /// Status: Apakah ada halaman selanjutnya? (Digunakan untuk tombol "Next").
        /// </summary>
        public bool HasNextPage { get; set; }
        // (Opsional, tapi sering berguna)
        /// <summary>
        /// Total seluruh record data (Total Count of all records).
        /// </summary>
        public int TotalCount { get; set; }
    }
}