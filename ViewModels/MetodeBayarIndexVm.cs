namespace POS_System.ViewModels
{
    public class MetodeBayarIndexVm
    {
        public List<MetodeBayarVmCreate>? MetodeBayarList { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
