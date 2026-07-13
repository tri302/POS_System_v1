namespace POS_System.ViewModels
{
    public class AlurKasIndexVm
    {
        public List<AlurKasVm> ListKas { get; set; } = new List<AlurKasVm>();
        public AlurKasVm Summary { get; set; } = new AlurKasVm();
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
