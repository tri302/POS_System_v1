namespace POS_System.ViewModels
{
    public class ProdukIndexVm
    {
        public List<ProdukVm> ProdukList { get; set; } = new();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int TotalCount { get; set; }
   }
}