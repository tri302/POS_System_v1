namespace POS_System.ViewModels
{
    // Pastikan kelas ini ada atau buat yang baru
    public class PaginatedListVm<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        // List item yang akan ditampilkan di halaman saat ini
        public List<T> Items { get; private set; }
        public PaginatedListVm(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }
        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }
    }
}
