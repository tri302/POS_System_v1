using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using POS_System.Models;
using POS_System.Repository;
using POS_System.Services;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class AlurKasController : Controller
    {
        private readonly IDashboardService _dashboard;
        private readonly IHubContext<CashFlowHub> _hubContext;
        private readonly IAlurKasRepo _alurKasRepo;
        public AlurKasController(IDashboardService dashboard, IHubContext<CashFlowHub> hubContext, IAlurKasRepo alurKasRepo)
        {
            _dashboard = dashboard;
            _hubContext = hubContext;
            _alurKasRepo = alurKasRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            var paginatedData = await _alurKasRepo.GetPaginatedAlurKasAsync(pageNumber, pageSize);
            var data = paginatedData.Select(a => new AlurKasVm
            {
                Id = a.Id,
                Amount = a.Amount,
                Date = a.Date,
                Notes = a.Notes,
                IdCashFlow = a.IdCashFlow,
                CashFlowName = a.CashFlow?.CashFlowName ?? "Data Tidak Ada",
                SumberAlurKas = a.CashFlow?.CashFlowName ?? "Data Tidak Ada",
                TypeName = a.CashFlow?.IdTipeCashFlow == 1 ? "Masuk" : "Keluar"
            }).ToList();
            var model = await _alurKasRepo.GetSummaryViewModelAsync();
            model.ListKas = data;
            model.TotalPages = paginatedData.TotalPages;
            model.PageIndex = paginatedData.PageIndex;
            model.HasPreviousPage = paginatedData.HasPreviousPage;
            model.HasNextPage = paginatedData.HasNextPage;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var paginatedData = await _alurKasRepo.GetPaginatedAlurKasAsync(pageNumber, pageSize);
            var items = paginatedData.Select(a => new AlurKasVm
            {
                Id = a.Id,
                Amount = a.Amount,
                Date = a.Date,
                Notes = a.Notes,
                IdCashFlow = a.IdCashFlow,
                CashFlowName = a.CashFlow?.CashFlowName ?? "Data Tidak Ada",
                SumberAlurKas = a.CashFlow?.CashFlowName ?? "Data Tidak Ada",
                TypeName = a.CashFlow?.IdTipeCashFlow == 1 ? "Masuk" : "Keluar"
            }).ToList();
            var viewModel = await _alurKasRepo.GetSummaryViewModelAsync();
            viewModel.PageIndex = pageNumber;
            viewModel.TotalPages = paginatedData.TotalPages;
            viewModel.ListKas = items;
            return View("Index", viewModel);
        }
        [Route("AlurKas/GetSources/{IdTipeCashFlow}")]
        [HttpGet]
        public async Task<IActionResult> GetSources(int IdTipeCashFlow)
        {
            var sources = await _alurKasRepo.GetSourcesByTipeAsync(IdTipeCashFlow);
            return Json(sources);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new AlurKasVm { Date = DateTime.Today };
            return PartialView("~/Views/AlurKas/_Create.cshtml", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlurKasVm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/AlurKas/_Create.cshtml", model);
            }
            var alurKasModel = new AlurKas
            {
                IdCashFlow = model.IdCashFlow,
                Amount = model.Amount,
                Date = model.Date,
                Notes = model.Notes
            };
            await _alurKasRepo.AddAsync(alurKasModel);
            await _alurKasRepo.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveDataUpdate");
            _dashboard.ClearDashboardCache(model.Date, model.Date);
            return Json(new { success = true, message = "Cash flow berhasil disimpan." });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var alurKas = await _alurKasRepo.GetByIdWithIncludesAsync(id);
            if (alurKas == null) return NotFound();
            var viewModel = new AlurKasVm
            {
                Id = alurKas.Id,
                TipeAlurKas = alurKas.CashFlow?.TipeCashFlow?.IdTipeCashFlow ?? 0,
                IdCashFlow = alurKas.IdCashFlow,
                SumberAlurKas = alurKas.SumberAlurKas,
                Amount = alurKas.Amount,
                Date = alurKas.Date,
                Notes = alurKas.Notes
            };
            return PartialView("~/Views/AlurKas/_Edit.cshtml", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AlurKasVm vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/AlurKas/_Edit.cshtml", vm);
            }
            var getById = await _alurKasRepo.FindAsync(vm.Id);
            if (getById == null) return NotFound();
            getById.IdCashFlow = vm.IdCashFlow;
            getById.SumberAlurKas = vm.SumberAlurKas;
            getById.Amount = vm.Amount;
            getById.Date = vm.Date;
            getById.Notes = vm.Notes;
            _alurKasRepo.Update(getById);
            await _alurKasRepo.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveDataUpdate");
            return Json(new { success = true, message = "Cash flow berhasil diupdate." });
        }
        [HttpGet]
        public async Task<JsonResult> GetSummaryData()
        {
            var model = await _alurKasRepo.GetSummaryViewModelAsync();
            return Json(model.Summary);
        }
        [HttpGet]
        public async Task<IActionResult> GetAlurKasTableData()
        {
            var rawList = await _alurKasRepo.GetAlurKasRawListAsync();
            var alurKasList = rawList.Select(a => new AlurKasVm
            {
                Id = a.Id,
                Amount = a.Amount,
                Date = a.Date,
                Notes = a.Notes,
                IdCashFlow = a.IdCashFlow,
                CashFlowName = a.CashFlow?.CashFlowName ?? "Data Tidak Ada",
                SumberAlurKas = a.CashFlow?.CashFlowName ?? "Data Tidak Ada",
                TypeName = a.CashFlow?.IdTipeCashFlow == 1 ? "Masuk" : "Keluar"
            }).ToList();
            var viewModel = new AlurKasIndexVm { ListKas = alurKasList };
            return PartialView("_PartialKasTableBody", viewModel);
        }
    }
}
