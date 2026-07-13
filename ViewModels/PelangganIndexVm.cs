namespace POS_System.ViewModels
{
    public class PelangganIndexVm
    {
        public List<PelangganVm>? PelangganList { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
