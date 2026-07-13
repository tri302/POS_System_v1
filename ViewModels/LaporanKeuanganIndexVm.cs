namespace POS_System.ViewModels
{
    public class LaporanKeuanganIndexVm
    {
        public List<LaporanKeuanganVm>? LaporanKeuanganList { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
