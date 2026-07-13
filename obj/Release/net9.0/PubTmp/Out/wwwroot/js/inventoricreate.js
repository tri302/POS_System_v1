// ======================================================
// MAIN MAPPER: Fungsi tunggal untuk mengisi data ke Card
// ======================================================
function mapProductToCard($card, product) {
    if (!product) return;
    // 1. Set ID Produk ke hidden input
    const productId = product.idProduk || product.id;
    $card.find(".produk-id-hidden").val(productId);
    // 2. Set Nama Produk ke Dropdown Select2 (agar teksnya berubah)
    const namaProduk = product.namaProduk || product.nama;
    const $produkDropdown = $card.find(".produk-dropdown");
    if ($produkDropdown.find(`option[value="${productId}"]`).length === 0) {
        const newOption = new Option(namaProduk, productId, true, true);
        $produkDropdown.append(newOption).trigger('change');
    } else {
        $produkDropdown.val(productId).trigger('change');
    }
    // 3. Sinkronisasi kolom Barcode jika ada data barcodenya
    if (product.kodeBarcode) {
        $card.find('input[name="ScanBarcode"]').val(product.kodeBarcode);
    }
    // 4. Set Stok global produk
    $card.find(".stok").val(product.stok || 0);
    // 5. Handle Satuan & Harga (Antisipasi format array ataupun objek tunggal dari backend)
    let selectedUnit = null;
    if (product.productUnit) {
        // Jika backend mengembalikan objek tunggal (Berdasarkan log network JSON)
        selectedUnit = product.productUnit;
    } else if (product.productUnits && product.productUnits.length > 0) {
        // Jika backend mengembalikan array list
        selectedUnit = product.productUnits[0];
    } else if (product.availableUnits && product.availableUnits.length > 0) {
        // Jika backend menggunakan nama availableUnits
        selectedUnit = product.availableUnits[0];
    }
    // Terapkan nilai satuan ke elemen HTML Card
    if (selectedUnit) {
        $card.find(".unitName").val(selectedUnit.unitName);
        let harga = parseFloat(selectedUnit.hargaModal) || 0;
        $card.find(".harga")
            .val(harga.toLocaleString("id-ID"))
            .data("raw", harga);
        // Pasangkan ID Satuan ke hidden input khusus unit
        $card.find(".product-unit-id-hidden").val(selectedUnit.productUnitId);
    } else {
        $card.find(".unitName").val("-");
        $card.find(".harga").val(0).data("raw", 0);
        $card.find(".product-unit-id-hidden").val("");
    }
    updateTotalModal();
}
// ======================================================
// API FETCHERS
// ======================================================
function GetProdukDetail(id, $card) {
    if (!id) return;
    $.ajax({
        url: '/Inventori/GetProdukDetail/' + id,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if (data) {
                mapProductToCard($card, data);
            } else {
                Swal.fire({
                    icon: "warning",
                    title: "Detail produk tidak ditemukan",
                    position: 'top-end',
                    toast: true
                });
            }
        },
        error: function (xhr) {
            let errorMessage = "Terjadi kesalahan pada server.";
            //Jika server mengembalikan respons JSON
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            //Jika respons berupa text biasa (bukan HTML)
            else if (xhr.responseText && !xhr.responseText.includes("<!DOCTYPE html>")) {
                errorMessage = xhr.responseText;
            }
            //Jika status code tertentu
            else if (xhr.status === 404) {
                errorMessage = "Data atau halaman tidak ditemukan (404).";
            } else if (xhr.status === 500) {
                errorMessage = "Server sedang bermasalah (500). Silakan hubungi admin.";
            }
            //Tampilkan SweetAlert
            Swal.fire({
                icon: "error",
                title: "Gagal",
                position: 'top-end',
                toast: true,
                text: errorMessage
            });
        }
    });
}
function handleBarcodeScan(barcode, containerElement) {
    if (!barcode) return;
    const $card = $(containerElement).closest('.inventory-item');
    $.ajax({
        url: "/Inventori/GetProductByBarcode",
        type: "GET",
        data: { barcode: String(barcode).trim() },
        success: function (product) {
            if (product) {
                mapProductToCard($card, product);
            } else {
                Swal.fire({
                    icon: "warning",
                    title: "Produk dengan barcode tersebut tidak ditemukan",
                    position: 'top-end',
                    toast: true
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: "error",
                title: "Gagal memuat data dari barcode!",
                position: 'top-end',
                toast: true
            });
        }
    });
}
// ======================================================
// SELECT2 INITIALIZATION & CARD TEMPLATE
// ======================================================
const cardTemplate = `
<div class="card mb-3 inventory-item">
    <div class="card-header bg-light">
        <div class="row align-items-center">
            <div class="col text-start">
                <strong><i class="bi bi-box-seam me-2"></i>Inventory Item</strong>
            </div>
            <div class="col text-end">
                <i class="fa fa-trash text-danger BtnTrash" style="cursor:pointer;" title="Hapus Item"></i>
            </div>
        </div>
    </div>
    <div class="card-body">
        <div class="row mb-2">
            <div class="col-md-4">
                <label class="form-label fw-bold">Barcode/Scan Produk</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="bi bi-upc-scan"></i></span>
                    <input type="text" autocomplete="off" name="ScanBarcode" class="form-control" placeholder="Scan barcode..." />
                    <button type="button" class="btn btn-success btn-scan-barcode" title="Scan">
                        <i class="bi bi-search"></i>
                    </button>
                </div>
                <small class="text-muted d-block mt-1">
                  Untuk pencarian Produk gunakan <b>barcode</b> untuk produk pabrikan atau <b>nama produk</b> untuk produk non-pabrik.
                </small>
            </div>
        </div>
        <div class="row g-3 align-items-end">                       
            <div class="col-md-3">
                <label class="form-label fw-bold">Nama Produk</label>
                <div class="input-group">
                    <select class="form-select form-control produk-dropdown" style="width: 100%;">
                        <option value="">Pilih Produk</option>
                    </select>
                </div>
                <input type="hidden" name="IdProduk" class="produk-id-hidden"/>
                <input type="hidden" name="ProductUnitId" class="product-unit-id-hidden"/>
            </div>                    
            <div class="col-md-2">
                <label class="form-label fw-bold">Satuan Produk</label>
                <input name="UnitName" class="form-control unitName" readonly />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-bold">Harga Modal</label>
                <div class="input-group">
                    <span class="input-group-text">Rp</span>
                    <input name="HargaModal" class="form-control harga" readonly />
                </div>
            </div>
            <div class="col-md-1"> 
                <label class="form-label fw-bold">Stok</label>
                <input name="Stok" class="form-control text-center stok" readonly />
            </div>
            <div class="col">
                <div class="qty-wrapper qty-normal-wrapper">
                    <label class="form-label fw-bold">Qty</label>
                    <div class="input-group">
                        <button class="btn btn-outline-danger btn-minus" type="button">-</button>
                        <input type="number" name="Jumlah" class="form-control text-center jumlah-normal" min="1" />
                        <button class="btn btn-outline-success btn-plus" type="button">+</button>
                    </div>
                </div>
                <div class="qty-wrapper qty-opname-wrapper d-none"> <label class="form-label fw-bold">Penyesuaian Qty Selisih</label>
                    <div class="input-group">
                        <select class="form-select adjustment-type" style="max-width: 65px; background-color: #f8f9fa;">
                            <option value="plus" selected>+</option>
                            <option value="minus">-</option>
                        </select>
                        <input type="number" class="form-control text-center jumlah-opname" name="Jumlah" min="0" placeholder="0" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>`;
function initProductSelect2($element) {
    $element.select2({
        placeholder: "Pilih Produk",
        minimumInputLength: 2,
        ajax: {
            url: "/Inventori/SearchProduk",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { term: params.term };
            },
            processResults: function (data) {
                return {
                    results: $.map(data, function (item) {
                        return { id: item.id, text: item.nama };
                    })
                };
            }
        }
    }).on("select2:select", function (e) {
        const produkId = e.params.data.id;
        const $card = $(this).closest(".inventory-item");
        GetProdukDetail(produkId, $card);
    });
    $element.next(".select2-container").addClass("form-control p-0");
}
// ======================================================
// DOM EVENTS INITIALIZATION
// ======================================================
$(document).ready(function () {    
    // Inisialisasi awal dropdown tipe stok & invoice
    updateDropdown(1);
    function toggleInvoiceRequired() {
        var tipe = $("input[name='TipeStok']:checked").val();
        $("#NoReferensi").attr(tipe === "1" ? "required" : "required", tipe === "1" ? "required" : false);
        if (tipe !== "1") $("#NoReferensi").removeAttr("required");
    }
    $("input[name='TipeStok']").change(toggleInvoiceRequired);
    toggleInvoiceRequired();
    // Jalankan select2 pada card default pertama yang sudah ada di layar (jika ada)
    initProductSelect2($(".produk-dropdown"));
});
// Tombol Tambah Card
$('#BtnAddInventoriItems').off('click').on('click', function () {
    const $newCard = $(cardTemplate);
    $('#InventoryItemsContainer').append($newCard);
    initProductSelect2($newCard.find(".produk-dropdown"));
    //alankan fungsi toggle agar input Qty card baru sinkron dengan TipeStok yang sedang aktif
    toggleQtyInput();
    updateTotalModal();
});
// Tipe Stok radio change
$(".tipe-stok").on("change", function () {
    updateDropdown($(this).val());
    updateTotalModal();
});
// Barcode keyboard enter press
$(document).on('keypress', 'input[name="ScanBarcode"]', function (e) {
    if (e.which === 13) {
        e.preventDefault();
        handleBarcodeScan($(this).val(), this);
    }
});
// Barcode search button click
$(document).on('click', '.btn-scan-barcode', function () {
    const $input = $(this).closest('.input-group').find('input[name="ScanBarcode"]');
    handleBarcodeScan($input.val(), this);
});
// ======================================================
// CALCULATION & MATHEMATICS
// ======================================================
function updateTotalModal() {
    let totalModal = 0;
    let tipeStok = $('input[name="TipeStok"]:checked').val();

    $(".inventory-item").each(function () {
        let harga = $(this).find(".harga").data("raw")
            || parseFloat($(this).find(".harga").val().replace(/\./g, ""))
            || 0;

        let jumlah = 0;
        if (tipeStok === "3") {
            let rawQty = parseFloat($(this).find('.jumlah-opname').val()) || 0;
            let adjType = $(this).find('.adjustment-type').val();
            // Jika minus, modalnya menjadi pengurang (minus)
            jumlah = (adjType === 'minus') ? -Math.abs(rawQty) : Math.abs(rawQty);
        } else {
            jumlah = parseFloat($(this).find('.jumlah-normal').val()) || 0;
        }

        totalModal += harga * jumlah;
    });

    // Menampilkan total modal (bisa bernilai minus jika opname berkurang banyak)
    if (totalModal < 0) {
        $("#TotalModal").val("-" + Math.abs(totalModal).toLocaleString("id-ID"));
    } else {
        $("#TotalModal").val(totalModal.toLocaleString("id-ID"));
    }
}
// Tombol Plus (+)
$(document).off("click", ".btn-plus").on("click", ".btn-plus", function () {
    // Cari input jumlah yang sedang tampil (apakah yang normal atau yang opname)
    let input = $(this).closest(".inventory-item").find(".jumlah-normal, .jumlah-opname").filter(":visible");
    let val = parseInt(input.val()) || 0;
    input.val(val + 1);
    updateTotalModal();
});

// Tombol Minus (-)
$(document).off("click", ".btn-minus").on("click", ".btn-minus", function () {
    let input = $(this).closest(".inventory-item").find(".jumlah-normal, .jumlah-opname").filter(":visible");
    let val = parseInt(input.val()) || 0;

    // Jika input normal, minimal 1. Jika opname (selisih), minimal 0
    let minVal = input.hasClass('jumlah-normal') ? 1 : 0;

    if (val > minVal) {
        input.val(val - 1);
    }
    updateTotalModal();
});

// Hitung ulang jika user mengetik manual di input
$(document).on("input", ".jumlah-normal, .jumlah-opname", function () {
    updateTotalModal();
});

// Hitung ulang jika tipe selisih (+ / -) pada opname diubah
$(document).on("change", ".adjustment-type", function () {
    updateTotalModal();
});
$('#InventoryItemsContainer').on('click', '.BtnTrash', function () {
    $(this).closest('.card').remove();
    updateTotalModal();
});
// ======================================================
// SERVER FORM SUBMISSION (POST)
// ======================================================
$(document).on("submit", "form", function (e) {
    e.preventDefault();

    // Bersihkan format rupiah untuk total modal
    let rawValue = $("#TotalModal").val().replace(/[^0-9-]/g, ""); // Tambah penanda minus jika ada
    $("#TotalModalHidden").val(rawValue);

    let tipeStokSelected = $('input[name="TipeStok"]:checked').val();

    let formData = {
        TipeStok: tipeStokSelected, // FIX: Menyimpan angka "1", "2", atau "3"
        Sumber: $("#sumberDropdown").val(),
        Notes: $("textarea[name='Notes']").val(),
        TotalModal: parseFloat(rawValue) || 0,
        NoReferensi: $("#NoReferensi").val(),
        Items: []
    };

    $("#InventoryItemsContainer .inventory-item").each(function () {
        let finalJumlahItem = 0;

        if (tipeStokSelected === "3") {
            let rawQty = parseFloat($(this).find('.jumlah-opname').val()) || 0;
            let adjType = $(this).find('.adjustment-type').val();
            // Hasil konversi final ke angka positif/negatif
            finalJumlahItem = (adjType === 'minus') ? -Math.abs(rawQty) : Math.abs(rawQty);
        } else {
            finalJumlahItem = parseFloat($(this).find('.jumlah-normal').val()) || 0;
        }

        let item = {
            IdProduk: $(this).find(".produk-id-hidden").val(),
            ProductUnitId: $(this).find(".product-unit-id-hidden").val(),
            HargaModal: $(this).find(".harga").data("raw") || $(this).find(".harga").val().replace(/\./g, ""),
            Stok: $(this).find(".stok").val() || 0,
            Jumlah: finalJumlahItem // FIX: Mengirimkan angka penyesuaian yang benar ke C#
        };
        formData.Items.push(item);
    });
    //console.log("TipeStok :", tipeStokSelected);
    //console.log("Sumber :", $("#sumberDropdown").val());
    //console.log(formData);
    $.ajax({
        url: "/Inventori/Create",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (res) {
            Swal.fire({
                icon: "success",
                title: "Sukses",
                text: "Data berhasil disimpan",
                position: 'top-end',
                toast: true
            }).then(() => {
                window.location.href = "/Inventori/Index";
            });
        },
        error: function (xhr) {
            let errorMessage = "Terjadi kesalahan pada server.";
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            Swal.fire({
                icon: "error",
                title: "Gagal",
                text: errorMessage,
                position: 'top-end',
                toast: true
            });
        }
    });
});
// Fungsi pembantu dropdown tipe stock
function updateDropdown(tipeId) {
    var $sumberSelect = $("#sumberDropdown");
    if (!$sumberSelect.length) return;
    $sumberSelect.empty().append('<option value="" disabled selected>Pilih</option>');
    if (tipeId) {
        $.ajax({
            url: '/Inventori/GetSumberByTipe/' + tipeId,
            type: 'GET',
            success: function (data) {
                $.each(data, function (i, item) {
                    $sumberSelect.append('<option value="' + item.id + '">' + item.nama + '</option>');
                });
            },
            error: function (xhr) {
                let errorMessage = "Terjadi kesalahan pada server.";
                //Jika server mengembalikan respons JSON
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    errorMessage = xhr.responseJSON.message;
                }
                //Jika respons berupa text biasa (bukan HTML)
                else if (xhr.responseText && !xhr.responseText.includes("<!DOCTYPE html>")) {
                    errorMessage = xhr.responseText;
                }
                //Jika status code tertentu
                else if (xhr.status === 404) {
                    errorMessage = "Data atau halaman tidak ditemukan (404).";
                } else if (xhr.status === 500) {
                    errorMessage = "Server sedang bermasalah (500). Silakan hubungi admin.";
                }
                // Tampilkan SweetAlert
                Swal.fire({
                    icon: "error",
                    title: "Gagal",
                    position: 'top-end',
                    toast: true,
                    text: errorMessage
                });
            }
        });
    }
}
// Fungsi untuk mengatur tampilan Qty secara dinamis
function toggleQtyInput() {
    let tipeStok = $('input[name="TipeStok"]:checked').val();
    if (tipeStok === "3") {
        // Jika Penyesuaian: Sembunyikan yang normal, tampilkan yang opname
        $('.inventory-item').find('.qty-normal-wrapper').addClass('d-none');
        $('.inventory-item').find('.qty-opname-wrapper').removeClass('d-none');
    } else {
        // Jika Masuk atau Keluar: Tampilkan yang normal, sembunyikan yang opname
        $('.inventory-item').find('.qty-normal-wrapper').removeClass('d-none');
        $('.inventory-item').find('.qty-opname-wrapper').addClass('d-none');
    }
}
// Jalankan fungsi saat halaman pertama kali dimuat (berguna untuk halaman EDIT)
toggleQtyInput();
updateTotalModal();
// Jalankan fungsi setiap kali user mengubah pilihan Radio Button TipeStok
$(document).on('change', '.tipe-stok', function () {
    toggleQtyInput();
    updateTotalModal();
});