//Fungsi Hitung Profit per item & total keseluruhan
function HitungProfit() {
    $(".transaksi-item").each(function () {
        const $card = $(this);
        // Ambil nilai raw dari data-raw (Jika dimuat dari DB)
        let hargaJualRaw = $card.find(".hargaJual").data("raw") || 0;
        let hargaModalRaw = $card.find(".hargaModal").data("raw") || 0;
        // Fallback jika data-raw tidak ada, ambil dari val dan bersihkan format rupiah
        const hargaJual = parseFloat(hargaJualRaw) || parseFloat(($card.find(".hargaJual").val() || "0").replace(/\./g, "")) || 0;
        const hargaModal = parseFloat(hargaModalRaw) || parseFloat(($card.find(".hargaModal").val() || "0").replace(/\./g, "")) || 0;
        const qty = parseInt($card.find(".jumlah").val()) || 0;
        const profit = (hargaJual - hargaModal) * qty;
        $card.find(".profit")
            .val(profit.toLocaleString("id-ID"))
            .data("raw", profit);
    });
}
//Update total keseluruhan    
function updateTotalBelanja() {
    let total = 0;
    // Loop semua card transaksi yang ada
    $("#ContainerAdditemsProduk .transaksi-item").each(function () {
        const $card = $(this);
        // Ambil nilai mentah (raw) Harga Jual dari data-raw
        let hargaJualRaw = $card.find(".hargaJual").data("raw");
        // Konversi ke float, jika data-raw tidak ada (misalnya pada item baru yang belum dipilih produk)
        // Gunakan nilai dari input (dibersihkan dari format rupiah) sebagai fallback.
        const harga = parseFloat(hargaJualRaw) ||
            parseFloat(($card.find(".hargaJual").val() || "0").replace(/\./g, "")) || 0;
        const qty = parseInt($card.find(".jumlah").val()) || 0;
        total += harga * qty;
    });
    // Update tampilan total belanja
    $("#TotalBelanja")
        .val(total.toLocaleString("id-ID"))
        .data("raw", total);    
    HitungTotalBayar();
}
//Fungsi Autocomplete cari by nama produk
function initProductSelect2($element) {
    $element.select2({
        placeholder: "Pilih Produk",
        minimumInputLength: 2,
        allowClear: true,
        width: '100%',
        ajax: {
            url: "/Transaksi/SearchProdukAsync",
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return { term: params.term };
            },
            processResults: function (data) {
                return {
                    results: data.results
                };                
            },
            cache: true
        }
    }).on("select2:select", function (e) {
        const produkId = e.params.data.id;
        const $card = $(this).closest(".transaksi-item");
        GetProdukDetail(produkId, $card);  
        HitungProfit();
    }).on("select2:clear", function () {
        const $card = $(this).closest(".transaksi-item");
        $card.find(".produk-id-hidden").val("");
        $card.find(".SatuanProduk-dropdown").empty().append(`<option value="">-- pilih --</option>`);
        $card.find(".hargaModal").val("").data("raw", 0);
        $card.find(".hargaJual").val("").data("raw", 0);
        $card.find("input[name='Stok']").val("");
        $card.find(".profit").val("").data("raw", 0);
        HitungProfit();
        updateTotalBelanja();
    });
}
function cleanAndFormatMoneyInitialValue(elementId, forceZeroIfEmpty = false) {
    const $elem = $(`#${elementId}`);
    let rawValue = $elem.val() || "";
    let cleanedValue = 0;
    if (rawValue) {
        // Hapus 'Rp', hapus pemisah ribuan (titik)
        let tempValue = rawValue.replace(/Rp\s*/g, '').replace(/\./g, '');
        // Jika ada koma, asumsikan itu adalah pemisah desimal, 
        // dan ambil bagian BULAT (sebelum koma)
        const parts = tempValue.split(',');
        // Ambil nilai integer dari bagian BULAT
        cleanedValue = parseInt(parts[0]) || 0;
    }
    if (forceZeroIfEmpty && !rawValue) {
        cleanedValue = 0;
    }
    //  Format: Terapkan format "titik" ribuan
    let formattedValue = formatRupiah(cleanedValue);
    // Update input field
    $elem.val(formattedValue);
    // Simpan nilai mentah di data-raw 
    $elem.data("raw", cleanedValue);
}
// =========================================================================
// Load harga jual, modal, stok berdasarkan satuan produk yang dipilih
// =========================================================================
$(document).on("change", ".SatuanProduk-dropdown", function () {
    const $dropdown = $(this);
    const productUnitId = $dropdown.val(); // ID Satuan yang dipilih
    const $card = $dropdown.closest(".transaksi-item"); // Menggunakan konteks elemen induk item
    //Ambil nilai konversi dari option yang sedang aktif/dipilih
    var konversi = parseInt($dropdown.find(':selected').data('konversi'));
    //Jika tidak ditemukan di option, fallback ke data-konversi milik induk <select>
    if (isNaN(konversi)) {
        konversi = parseInt($dropdown.data('konversi')) || 1;
    }
    $card.find('.dbg-konversi').text('Konversi Aktif: ' + konversi + ' Pcs');
    // Pastikan attribute data pembungkus select ikut diperbarui agar sinkron saat submit form
    $dropdown.data('konversi', konversi);
    $dropdown.attr('data-konversi', konversi);
    if (!productUnitId) return;
    $.ajax({
        url: '/Transaksi/GetUnitDetail',
        type: 'GET',
        data: { productUnitId: productUnitId },
        success: function (data) {
            $card.find(".SatuanProduk-id-hidden").val(productUnitId);
            // Simpan nilai konversi baru dari server jika ada perubahan pilihan satuan
            const currentConversion = data.conversion || data.Conversion || konversi;
            $card.find(".SatuanProduk-dropdown").data("konversi", currentConversion).attr("data-konversi", currentConversion);
            $card.find('.dbg-konversi').text('Konversi Aktif: ' + currentConversion + ' Pcs');
            $card.find(".hargaModal").val(data.hargaModal.toLocaleString("id-ID")).data("raw", data.hargaModal);
            $card.find(".hargaJual").val(data.hargaJual.toLocaleString("id-ID")).data("raw", data.hargaJual);
            $card.find("input[name='Stok']").val(data.stok);
            HitungProfit();
            updateTotalBelanja();
        },
        error: function (xhr, status, error) {
            let errorMessage = "Gagal mengambil detail harga dari server.";
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            } else if (xhr.responseText) {
                errorMessage = xhr.responseText;
            } else if (error) {
                errorMessage = error;
            }
            Swal.fire({
                icon: "error",
                title: "Koneksi Gagal / Error",
                text: errorMessage,
                footer: `<span>Status: ${xhr.status} - ${status}</span>`,
                confirmButtonColor: "#3085d6",
                position: 'top-end',
                toast: true,
                confirmButtonText: "Oke"
            });
        }
    });
});
//document ready
$(document).ready(function () {           
    // Inisialisasi autocomplete pelanggan
    initautocomplete();
    LoadDetailsTransaksi();    
    // =======================================================
    // KODE EVENT HANDLER UNTUK NOMINAL BAYAR
    // =======================================================
    $(document).on("input", "#NominalBayar", function () {
        let $input = $(this);
        // 1. Ambil nilai & hilangkan semua selain angka
        let value = $input.val().replace(/[^0-9]/g, "");
        // 2. Format Rupiah dan update input & data-raw
        $input.val(formatRupiah(value));
        $input.data("raw", parseInt(value) || 0);
        // 3. Implementasi Debouncing
        clearTimeout(nominalBayarTimeout);
        nominalBayarTimeout = setTimeout(function () {
            HitungTotalBayar(); // Fungsi ini yang berisi SweetAlert
        }, 500); // Delay 500ms
    });
    // Template card item transaksi
    const templateCard = `
<div class="card mb-3 transaksi-item">
		<div class="card-header">
			<div class="row w-100">
				<div class="col text-start">
					<strong>Item Produk</strong>
				</div>
				<div class="col text-end">
					<i class="fa fa-trash text-danger BtnTrash" style="cursor:pointer;"></i>
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
			<div class="row d-flex align-items-end">
				<div class="col-md-4">
					<label class="form-label">Nama Produk</label>
					<div class="form-group">
						<select class="form-control NamaProdukSelect">							
						</select>
						<input type="hidden" name="IdProduk" class="produk-id-hidden" />
					</div>
				</div>
				<div class="col-md-3">
					<label class="form-label">Satuan Produk</label>
					<div class="form-group">
						<select class="form-control SatuanProduk-dropdown">
						</select>
						<input type="hidden" name="ProductUnitId" class="SatuanProduk-id-hidden" />
					</div>
				</div>
				<div class="col-md-5">
					<div class="form-group">
						<label>Harga Modal</label>
						<div class="input-group">
							<div class="input-group-text">Rp</div>
							<input name="HargaModal" class="form-control hargaModal" readonly />
						</div>
						<span class="text-danger"></span>
					</div>
				</div>
			</div>
			<div class="row d-flex align-items-end mt-3">
				<div class="col-md-3">
					<div class="form-group">
						<label>Harga Jual</label>
						<div class="input-group">
							<div class="input-group-text">Rp</div>
							<input name="HargaJual" class="form-control hargaJual" readonly />
						</div>
						<span class="text-danger"></span>
					</div>
				</div>
				<div class="col-md-2">
					<div class="form-group">
						<label>Stok</label>
						<input name="Stok" class="form-control text-center" readonly />
						<span class="text-danger"></span>
					</div>
				</div>
				<div class="col-md-3">
					<div class="form-group">
						<label>Profit</label>
						<div class="input-group">
							<div class="input-group-text">Rp</div>
							<input name="Profit" class="form-control profit" readonly />
						</div>
						<span class="text-danger"></span>
					</div>
				</div>
				<div class="col-md-4">
					<div class="form-group">
						<label>Quantity</label>
						<div class="input-group">
							<button class="btn btn-outline-danger btn-minus" type="button">-</button>
							<input type="number" name="Jumlah" class="form-control text-center jumlah" value="1" min="1" id="inputJumlah" style="max-width: 80px;" />
							<button class="btn btn-outline-success btn-plus" type="button">+</button>
						</div>
						<span class="text-danger"></span>
					</div>
				</div>
			</div>
		</div>
	</div>`;
    //Tambahkan item transaksi
    $('#BtnAddItemTransaksi').off('click').on('click', function () {
        const $newCard = $(templateCard);
        $('#ContainerAdditemsProduk').append($newCard);
        initProductSelect2($newCard.find(".NamaProdukSelect"));
    });
    // Hapus item transaksi
    $('#ContainerAdditemsProduk').on('click', '.BtnTrash', function () {
        $(this).closest('.card').remove();        
    });
    // Tombol plus
    $(document).on("click", ".btn-plus", function () {
        const $input = $(this).closest(".transaksi-item").find(".jumlah");
        let val = parseInt($input.val()) || 0; // Pastikan mengambil nilai sebagai integer, default 0
        $input.val(val + 1);
        HitungProfit();
        updateTotalBelanja();
    });
    // Tombol minus
    $(document).on("click", ".btn-minus", function () {
        const $input = $(this).closest(".transaksi-item").find(".jumlah");
        let val = parseInt($input.val()) || 0;
        if (val > 0) { // Ubah dari val > 1 menjadi val > 0 agar bisa diset minimal 0
            $input.val(val - 1);
            HitungProfit();
            updateTotalBelanja();
        }
    });
    // Jika qty diketik manual
    $(document).on("input", ".jumlah", function () {
        // Ambil card terdekat
        const $card = $(this).closest(".transaksi-item");
        // Ganti titik dengan kosong untuk memastikan nilai terbaca sebagai angka
        let qty = $(this).val().replace(/\./g, "") || 0;
        // Pastikan Qty tidak negatif
        if (parseInt(qty) < 0) {
            $(this).val(0);
        }
        HitungProfit();
        updateTotalBelanja();
    });
    // FUNGSI INI WAJIB DIPANGGIL SAAT LOAD AWAL DARI DB!
    cleanAndFormatMoneyInitialValue("NominalBayar", true);
    cleanAndFormatMoneyInitialValue("Kembalian", true);  
});
//end document ready
function HitungTotalBayar() {
    // Mengambil nilai TotalBelanja yang sudah dihitung (dari data-raw)
    const totalBelanja = parseFloat($("#TotalBelanja").data("raw")) || parseRupiah($("#TotalBelanja").val());
    // Mengambil NominalBayar (bersihkan dari format rupiah)
    const bayar = parseRupiah($("#NominalBayar").val());
    let kembalian = bayar - totalBelanja;
    // Tentukan apakah tombol harus diaktifkan (uang cukup/lebih)
    const isUangCukup = kembalian >= 0;
    // tampilkan nilai kembalian
    if (isUangCukup) {
        $("#Kembalian").val(formatRupiah(kembalian));
        $("#Kembalian").data("raw", kembalian);
        $("#NominalBayar").removeClass("is-invalid");        
        $("#BtnSubmit").attr("disabled", false);
    } else {
        // UANG KURANG (kembalian < 0)
        // 1. Update tampilan kembalian (tampilkan nilai negatif)
        $("#Kembalian").val("-" + formatRupiah(Math.abs(kembalian)));
        $("#Kembalian").data("raw", kembalian);        
        // Nonaktifkan tombol submit karena uang kurang
        $("#BtnSubmit").attr("disabled", true);
        // 2. Tampilkan SweetAlert & tandai input Nominal Bayar
        if (bayar > 0 && bayar < totalBelanja) {
            // Nominal bayar ada isinya tapi kurang
            $("#NominalBayar").addClass("is-invalid");
            // Tampilkan SweetAlert Warning
            Swal.fire({
                icon: 'warning',
                title: 'Uang Kurang',
                text: 'Nominal Bayar (' + formatRupiah(bayar) + ') kurang dari Total Belanja (' + formatRupiah(totalBelanja) + ').',
                position: 'top-end',
                toast: true,
                showConfirmButton: true,
                timer: 4000 // Tampil selama 2 detik
            });
        } else {
            // Nominal bayar 0 atau kosong, tidak perlu warning, hanya hapus invalid class
            $("#NominalBayar").removeClass("is-invalid");
        }
    }
}
function Empty() {
    $("#ContainerAdditemsProduk").empty();
    $("#TotalBelanja").val("0");
    $("#NominalBayar").val("");
    $("#Kembalian").val("0");
}
// Save data ke server
$(document).on("submit", "#formTransaksi", function (e) {
    e.preventDefault(); // stop submit biasa
    // kumpulkan data form utama
    let formData = {
        IdTransaksi: $("input[name='IdTransaksi']").val(),
        IdPelanggan: $("select[name='IdPelanggan']").val(),
        Email: $("input[name='Email']").val(),
        Phone: $("input[name='Phone']").val(),
        Notes: $("textarea[name='Notes']").val(),
        IdMetodeBayar: $("select[name='IdMetodeBayar']").val(),
        Kembalian: $(this).find("#Kembalian").data("raw") ||
            $(this).find("input[name='Kembalian']").val().replace(/\./g, ""),
        NominalBayar: $(this).find("#NominalBayar").data("raw") ||
            $(this).find("input[name='NominalBayar']").val().replace(/\./g, ""),
        TotalBelanja: $(this).find("#TotalBelanja").data("raw") ||
            $(this).find("input[name='TotalBelanja']").val().replace(/\./g, ""),
        Items: []        
    };    
    // kumpulkan data items    
    $("#ContainerAdditemsProduk .transaksi-item").each(function () {                 
        let hargaJualElement = $(this).find("input[name='HargaJual']");
        let rawHargaJual = hargaJualElement.data("raw");
        let fallbackHargaJual = hargaJualElement.val() || "0";       
        let cleanedInputHargaJual = fallbackHargaJual.replace(/[^0-9]/g, "");
        let finalHargaJual = rawHargaJual > 0 ? rawHargaJual : parseFloat(cleanedInputHargaJual);  
        let itemHargaModalRaw = $(this).find(".hargaModal").data("raw") || 0;
        let item = {            
            IdProduk: $(this).find(".produk-id-hidden").val(),
            ProductUnitId: $(this).find(".SatuanProduk-id-hidden").val(),
            Stok: $(this).find("input[name='Stok']").val(),
            Profit: $(this).find(".profit").data("raw") ||
                $(this).find("input[name='Profit']").val().replace(/\./g, ""),
            Conversion: parseInt($(this).find(".SatuanProduk-dropdown").data("konversi")) || 1,
            HargaJual: finalHargaJual, 
            HargaModal: parseFloat(itemHargaModalRaw),
            Qty: $(this).find("input[name='Jumlah']").val()
        };
        formData.Items.push(item);
    });    
    //Validasi form
    if (!ValidasiForm(formData)) {
        return; // stop jika validasi gagal
    }
    // kirim via ajax
    $.ajax({
        url: "/Transaksi/Edit",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (res) {
            Swal.fire({
                icon: "success",
                title: "Sukses",
                text: "Data berhasil disimpan",
                position: 'top-end',
                toast: true
            }).then(() => {
                window.location.href = "/Transaksi/Index"; // redirect setelah sukses
            });
        },
        error: function (xhr, status, error) {
            let errorMsg = "Terjadi kesalahan tidak diketahui.";
            if (xhr.responseText) {
                try {
                    const json = JSON.parse(xhr.responseText);
                    errorMsg = json.message || xhr.responseText;
                } catch (e) {
                    errorMsg = xhr.responseText;
                }
            }
            Swal.fire({
                icon: "error",
                title: "Gagal",
                text: errorMsg,
                position: 'top-end',
                toast: true
            });
        }
    });
});
function ValidasiForm(formData) {
    let errors = [];
    if (!formData.IdPelanggan) {
        errors.push("Nama Customer tidak boleh kosong.");
    }
    if (!formData.Email) {
        errors.push("Email tidak boleh kosong.");
    }
    if (!formData.Phone) {
        errors.push("Phone tidak boleh kosong.");
    }
    if (!formData.IdMetodeBayar) {
        errors.push("Metode Pembayaran tidak boleh kosong.");
    }
    if (!formData.NominalBayar || formData.NominalBayar === "0") {
        errors.push("Nominal Bayar tidak boleh kosong.");
    }
    if (!formData.TotalBelanja || formData.TotalBelanja === "0") {
        errors.push("Total Belanja tidak boleh kosong.");
    }
    if (formData.Items.length === 0) {
        errors.push("Minimal 1 produk harus ditambahkan.");
    } else {
        formData.Items.forEach((item, index) => {
            if (!item.IdProduk) {
                errors.push(`Produk ke-${index + 1} tidak valid.`);
            }
            if (!item.Qty || item.Qty <= 0) {
                errors.push(`Qty produk ke-${index + 1} harus lebih dari 0.`);
            }
        });
    }
    if (errors.length > 0) {
        Swal.fire({
            icon: "warning",
            title: "Validasi Gagal",
            html: errors.join("<br>"),
            position: 'top-end',
            toast: true
        });
        return false; // return false jika ada error
    }
    return true; // return true jika lolos
}
function initautocomplete() {
    $("#IdPelanggan").select2({
        placeholder: "--Pilih--",
        allowClear: true,
        width: '100%'
    });
    $("#IdPelanggan").on("change", function () {
        const pelangganId = $(this).val();
        if (pelangganId) {
            $.ajax({
                url: '/Transaksi/GetPelangganDetail/' + pelangganId,
                type: 'GET',
                success: function (data) {
                    if (data) {
                        $("input[name='Email']").val(data.email);
                        $("input[name='Phone']").val(data.phone);
                    }
                }
            });
        }
    });
}
function LoadDetailsTransaksi() {
    const idTransaksi = $("#IdTransaksi").val();
    $.ajax({
        url: '/Transaksi/_LoadDetail',
        type: 'GET',
        data: { idTransaksi: idTransaksi },
        success: function (html) {            
            $("#ContainerAdditemsProduk").html(html);
            // Jalankan kalkulasi awal data dari DB
            HitungProfit();
            updateTotalBelanja();
            // Inisialisasi ulang Select2 untuk setiap item yang baru di-load dari DB
            $("#ContainerAdditemsProduk").find(".NamaProdukSelect").each(function () {
                initProductSelect2($(this));
            });
            //$("#ContainerAdditemsProduk").find(".SatuanProduk-dropdown, .NamaProdukSelect").each(function () {
            //    initProductSelect2($(this));
            //});
        }
    });
}
function formatRupiah(angka) {
    // Pastikan angka adalah integer mentah sebelum diformat
    let num = parseInt(angka) || 0;
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}
// Fungsi untuk mengubah nilai Rupiah berformat string menjadi integer mentah
function parseRupiah(value) {
    if (!value) return 0;
    let strValue = value.toString();
    // 1. Hapus 'Rp ' (dan spasi setelahnya)
    strValue = strValue.replace(/Rp\s*/g, '');
    // 2. Hapus pemisah ribuan (titik)
    strValue = strValue.replace(/\./g, '');
    // 3. 💡 PERBAIKAN: Jika ada koma, asumsikan itu desimal (seperti 250000,00),
    //    dan kita hanya ambil bagian BULAT (sebelum koma)
    const parts = strValue.split(',');
    // Ambil nilai integer bulat dari bagian pertama
    return parseInt(parts[0]) || 0;
}
let nominalBayarTimeout = null; // Variabel global untuk menyimpan ID timeout
function konfirmasiPembatalan(idTransaksi, noTransaksi) {    
    Swal.fire({
        title: 'Konfirmasi Pembatalan!',
        text: `Anda yakin ingin membatalkan transaksi ${noTransaksi}? Aksi ini TIDAK DAPAT diurungkan dan stok akan dikembalikan.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545', // Warna merah untuk 'Ya, Batalkan!'
        cancelButtonColor: '#007bff', // Warna biru untuk 'Tidak'
        confirmButtonText: 'Ya, Batalkan!',
        cancelButtonText: 'Tidak',
        position: 'top-end',
        toast: true
    }).then((result) => {
        if (result.isConfirmed) {
            // Jika user mengkonfirmasi, cari form yang sesuai dan submit
            // ID form adalah: form-batal-@item.IdTransaksi
            const formId = `#form-batal-${idTransaksi}`;
            // Tambahkan token anti-forgery secara manual sebelum submit (Wajib untuk [ValidateAntiForgeryToken])
            const antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
            if (antiForgeryToken) {
                // Pastikan form memiliki token jika Anda menggunakan [ValidateAntiForgeryToken] di Controller
                $(formId).prepend(`<input type="hidden" name="__RequestVerificationToken" value="${antiForgeryToken}">`);
            }
            $(formId).submit();
        }
    });
}
function GetProdukDetail(id, $card) {
    $.ajax({
        url: "/Transaksi/GetProdukDetail/" + id,
        type: "GET",
        success: function (product) {
            if (!product) {
                Swal.fire({
                    icon: "warning",
                    title: "Produk tidak ditemukan",
                    position: 'top-end',
                    toast: true
                });
                return;
            }
            mapProductToCard($card, product);
        }
    });
}
function mapProductToCard($card, product) {
    let modalDefault = 0;
    let jualDefault = 0;
    if (product.availableUnits && product.availableUnits.length > 0) {
        modalDefault = product.availableUnits[0].hargaModal;
        jualDefault = product.availableUnits[0].hargaJual;
    }
    $card.find("input[name='ScanBarcode']").val(product.barcode);
    $card.find(".produk-id-hidden").val(product.idProduk);
    $card.find(".hargaModal").val(modalDefault.toLocaleString("id-ID")).data("raw", modalDefault);
    $card.find(".hargaJual").val(jualDefault.toLocaleString("id-ID")).data("raw", jualDefault);
    $card.find("input[name='Stok']").val(product.stok);
    // 2. Update Select2
    const $select = $card.find(".produk-dropdown");
    const option = new Option(product.namaProduk, product.idProduk, true, true);
    $select.empty().append(option).trigger("change.select2");
    const $unitDrop = $card.find(".SatuanProduk-dropdown");
    $unitDrop.empty();
    $unitDrop.append('<option value="">-- Pilih Satuan --</option>');
    if (product.availableUnits && Array.isArray(product.availableUnits)) {
        product.availableUnits.forEach(function (u) {
            //$unitDrop.append(`<option value="${u.productUnitId}">${u.unitName}</option>`);
            $unitDrop.append(`<option value="${u.productUnitId}" data-konversi="${u.conversion || u.Conversion || 1}">${u.unitName}</option>`);
        });
        if (product.availableUnits.length > 0) {
            let first = product.availableUnits[0];
            $unitDrop.val(first.productUnitId).trigger("change");
        }
    } else {
        Swal.fire({
            icon: 'warning',
            title: 'Data Tidak Ditemukan',
            text: 'Data satuan (availableUnits) tidak ditemukan untuk produk ini.',
            confirmButtonText: 'Oke',
            position: 'top-end',
            toast: true,
            confirmButtonColor: '#f8bb86' // Warna orange khas warning
        });
    }
    // 4. Pilih satuan pertama secara otomatis
    if (product.availableUnits.length > 0) {
        let first = product.availableUnits[0];
        $unitDrop.val(first.productUnitId).trigger("change"); // Trigger change untuk memicu load detail
    }
}
function handleBarcodeScan(barcode, element) {
    const $card = $(element).closest(".transaksi-item");
    $.ajax({
        url: "/Transaksi/GetProductByBarcode",
        type: "GET",
        data: {
            barcode: barcode
        },
        success: function (res) {
            if (!res) {
                Swal.fire({
                    icon: "warning",
                    title: "Produk tidak ditemukan",
                    position: 'top-end',
                    toast: true
                });
                return;
            }
            clearProduct($card);
            GetProdukDetail(res.idProduk, $card);
        },
        error: function () {
            Swal.fire({
                icon: "error",
                position: 'top-end',
                toast: true,
                title: "Barcode tidak ditemukan"
            });
        }
    });
}
// Event Keypress (Enter)
$(document).off("keydown", 'input[name="ScanBarcode"]');
$(document).on("keydown", 'input[name="ScanBarcode"]', function (e) {
    if (e.key === "Enter") {
        e.preventDefault();
        const barcode = $(this).val().trim();
        if (barcode === "") return;
        handleBarcodeScan(barcode, this);
    }
});
// Event Klik Tombol Cari pada input barcode
$('#ContainerAdditemsProduk').off('click', '.btn-scan-barcode').on('click', '.btn-scan-barcode', function (e) {
    e.preventDefault();
    // Cari input barcode di dalam card yang sama
    const $input = $(this).closest('.transaksi-item').find('input[name="ScanBarcode"]');
    if ($input.length > 0) {
        const barcodeValue = $input.val();
        if (barcodeValue && barcodeValue.trim() !== "") {
            handleBarcodeScan(barcodeValue, this);
        } else {
            Swal.fire({
                icon: "warning",
                position: 'top-end',
                toast: true,
                title: "Masukkan barcode terlebih dahulu"
            });
        }
    }
});
function clearProduct($card) {
    $card.find("input[name='ScanBarcode']").val("");
    $card.find(".produk-id-hidden").val("");
    $card.find(".produk-dropdown")
        .empty();
    $card.find(".SatuanProduk-dropdown")
        .empty();
    $card.find(".SatuanProduk-id-hidden").val("");
    $card.find(".hargaModal")
        .val("")
        .data("raw", 0);
    $card.find(".hargaJual")
        .val("")
        .data("raw", 0);
    $card.find("input[name='Stok']").val("");
}
