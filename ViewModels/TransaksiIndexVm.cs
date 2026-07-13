namespace POS_System.ViewModels
{
    public class TransaksiIndexVm
    {
        public List<TransaksiVm>? TransaksiList { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public string NamaMetodeBayar { get; set; }
    }
}
