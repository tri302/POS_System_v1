// Variable global untuk menyimpan status filter yang aktif
let currentFilter = "all"; // Default: tampilkan semua
let currentSearch = "";
// 1. Modifikasi Listener Tombol Filter
$(document).on("click", ".filter-btn", function () {
    var filter = $(this).data("filter");
    // 💾 Simpan status filter saat ini
    currentFilter = filter;
    // ➡️ Selalu mulai dari halaman 1 saat filter diubah
    loadPage(1, currentFilter);
});
// 2. Modifikasi Fungsi loadPage (untuk Paging & Filtering)
// Fungsi ini sekarang menerima parameter 'filter' opsional
// 1. Modifikasi Fungsi loadPage (untuk Paging, Filtering, & Searching)
function loadPage(pageNumber, filter = currentFilter, search = currentSearch) {
    if (typeof FilterProdukUrl === 'undefined') {
        console.error("FilterProdukUrl is not defined. Check Index.cshtml.");
        return;
    }
    // Update status global
    currentFilter = filter;
    currentSearch = search;
    $('#ContainerLoadData').html('<div class="text-center"><i class="fa fa-spinner fa-spin fa-3x"></i> Loading...</div>');
    $.ajax({
        url: FilterProdukUrl,
        type: 'GET',
        data: {
            pageNumber: pageNumber,
            filter: currentFilter,
            searchTerm: currentSearch // ⬅️ Kirim kata kunci pencarian ke Controller
        },
        success: function (html) {
            $('#ContainerLoadData').html(html);
            // Update visual tombol filter
            $(".filter-btn").removeClass("active-filter");
            $(`.filter-btn[data-filter="${currentFilter}"]`).addClass("active-filter");
            attachPagingListeners();
        },
        error: function (xhr) {
            alert("Error loading page: " + xhr.responseText);
            $('#ContainerLoadData').html('<div class="alert alert-danger text-center">Gagal memuat data.</div>');
        }
    });
}
// 2. Tambahkan Listener untuk Input Pencarian di dalam document ready atau global
$(document).on("keyup", "#searchProduk", function (e) {
    if (e.keyCode === 13) { // Jika menekan Enter
        let keyword = $(this).val();
        loadPage(1, currentFilter, keyword);
    }
});
$(document).on("click", "#btnSearch", function () {
    let keyword = $("#searchProduk").val();
    loadPage(1, currentFilter, keyword);
});
// 3. Modifikasi Listener Paging (agar memanggil loadPage dengan filter)
function attachPagingListeners() {
    $(document).off("click", ".page-btn").on("click", ".page-btn", function (e) {
        e.preventDefault();
        var targetPage = parseInt($(this).data("page"));
        if ($(this).parent().hasClass("disabled")) {
            return;
        }
        // Panggil loadPage, yang akan menggunakan currentFilter secara default
        loadPage(targetPage);
    });
}
// SweetAlert success message
$(document).ready(function () {
    var successMessage = $("#ContainerLoadData").data("success");
    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Berhasil',
            text: successMessage,
            showConfirmButton: true,
            timer: 5000,
            toast: true,
            position: "top-end",
        });
    }
});
// Delete Produk
$(document).on("click", ".btn-delete", function () {
    var id = $(this).data("id");
    var row = $(this).closest("tr");
    Swal.fire({
        title: 'Yakin hapus produk?',
        text: "Data tidak bisa dikembalikan!",
        icon: 'warning',
        position: "top-end",
        toast: true,
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Ya, hapus!',
        cancelButtonText: 'Batal'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Produk/Delete/' + id,
                type: 'POST',
                success: function (res) {
                    if (res.success) {                        
                        Swal.fire({
                            icon: 'success',
                            title: 'Terhapus',
                            text: res.message,
                            position: 'top-end',
                            toast: true,
                            timer: 1200, showConfirmButton: false
                        });
                        row.fadeOut(500, function () {
                            $(this).remove();
                        });
                    } else {                        
                        Swal.fire({
                            icon: 'error',
                            title: 'Gagal',
                            text: res.message,
                            position: 'top-end',
                            toast: true,
                            timer: 1200, showConfirmButton: false
                        });
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) { // Lebih informatif
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: jqXHR.responseText || 'Terjadi kesalahan pada server.',
                        position: 'top-end',
                        toast: true,
                        timer: 1200, showConfirmButton: false
                    });
                }
            });
        }
    });
});
$(document).on("click", ".toggle-unit-row", function (e) {
    e.preventDefault();
    // Temukan ID Produk dari baris master
    const masterRow = $(this).closest(".master-row");
    const idProduk = masterRow.data("id");
    // Cari baris detail unit yang sesuai
    const detailRow = $(".unit-detail-" + idProduk);
    // Toggle tampilan baris detail
    detailRow.toggle();
    // Ganti ikon plus/minus
    const icon = $(this).find(".toggle-icon");
    if (detailRow.is(":visible")) {
        icon.removeClass("fa-plus-square").addClass("fa-minus-square");
    } else {
        icon.removeClass("fa-minus-square").addClass("fa-plus-square");
    }
});
//Cetak Barcode
$(document).ready(function () {
    // Logika Check All
    $(document).on('change', '#checkAllProduk', function () {
        $('.produk-checkbox').prop('checked', $(this).prop('checked'));
    });
    // Cetak Produk Terpilih (Checkbox)
    $('#btn-cetak-terpilih').on('click', function () {
        var selectedIds = [];
        $('.produk-checkbox:checked').each(function () {
            selectedIds.push($(this).val());
        });
        if (selectedIds.length === 0) {
            Swal.fire({
                position: "top-end",
                toast: true,
                icon: "warning",
                title: "Silahkan pilih produk terlebih dahulu !!!",
                showConfirmButton: false,
                timer: 3000
            });
            return;
        }
        // Kirim lewat POST
        postToBarcode(selectedIds, "selected", "", "");
    });
    // Cetak Sesuai Filter
    $('#btn-cetak-filter').on('click', function () {
        const activeFilter = $(".filter-btn.active-filter").data("filter") || "all";
        const keyword = $("#searchProduk").val();
        // Diubah menjadi POST juga via helper agar sinkron ke Controller [HttpPost]
        postToBarcode([], "filter", activeFilter, keyword);
    });
});
function postToBarcode(ids, mode, filter, searchTerm) {
    var form = document.createElement("form");
    form.method = "POST"; // Pastikan di Controller menggunakan [HttpPost]
    form.action = CetakBarcodeUrl;
    form.target = "_blank"; // Buka di tab baru
    // Input Mode
    var inputMode = document.createElement("input");
    inputMode.type = "hidden";
    inputMode.name = "mode";
    inputMode.value = mode;
    form.appendChild(inputMode);
    // Input Filter
    var inputFilter = document.createElement("input");
    inputFilter.type = "hidden";
    inputFilter.name = "filter";
    inputFilter.value = filter || "";
    form.appendChild(inputFilter);
    // Input SearchTerm
    var inputSearch = document.createElement("input");
    inputSearch.type = "hidden";
    inputSearch.name = "searchTerm";
    inputSearch.value = searchTerm || "";
    form.appendChild(inputSearch);
    // ⭐ PERBAIKAN DI SINI: Gunakan "productIds" atau "productIds[]" 
    // Agar ASP.NET Core Model Binder mengenalnya sebagai List/Array
    ids.forEach((id, index) => {
        var inputId = document.createElement("input");
        inputId.type = "hidden";
        inputId.name = `productIds[${index}]`; // Format mengikat berbasis indeks index (productIds[0], productIds[1], dst)
        inputId.value = id;
        form.appendChild(inputId);
    });
    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
}
