using POS_System.Pageing;
using POS_System.ViewModels;
using POS_System.Models;
namespace POS_System.Repository
{
    public interface IAlurKasRepo
    {
        Task<PaginatedList<AlurKas>> GetPaginatedAlurKasAsync(int pageNumber, int pageSize);
        Task<List<AlurKas>> GetAlurKasRawListAsync();
        Task<List<object>> GetSourcesByTipeAsync(int idTipeCashFlow);
        Task<AlurKas> GetByIdWithIncludesAsync(long id);
        Task<AlurKas> FindAsync(long id);
        Task AddAsync(AlurKas alurKas);
        void Update(AlurKas alurKas);
        Task SaveChangesAsync();
        Task<AlurKasIndexVm> GetSummaryViewModelAsync();
    }
}
