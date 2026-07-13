//Hitung Profit per item & total keseluruhan
function HitungProfit() {
    $(".transaksi-item").each(function () {
        const $card = $(this);
        // AMBIL NILAI MENTAH DARI DATA ATTRIBUTE!
        const hargaJual = $card.find(".hargaJual").data("raw") || 0;
        const hargaModal = $card.find(".hargaModal").data("raw") || 0;
        const qty = parseInt($card.find(".jumlah").val()) || 0;
        const profit = (hargaJual - hargaModal) * qty;
        // Cek jika profit < 0 (kerugian) seperti yang terlihat di image_9e48a6.png
        // (Ini menunjukkan perhitungan sudah bekerja, tapi Harga Jual < Harga Modal)
        $card.find(".profit")
            .val(profit.toLocaleString("id-ID"))
            .data("raw", profit); // Simpan nilai mentah profit
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
//Autocomplete cari by nama produk
function initProductSelect2() {
    $('.produk-dropdown').select2({
        placeholder: "Cari produk...",
        minimumInputLength: 2, // Mulai cari setelah ketik 2 huruf
        allowClear: true,
        width: '100%',
        ajax: {
            url: '/Transaksi/SearchProdukAsync',
            dataType: 'json',
            delay: 250, // Delay saat mengetik
            data: function (params) {                
                return {
                    term: params.term 
                };
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
    });    
}
function cleanAndFormatMoneyInitialValue(elementId, forceZeroIfEmpty = false) {
    const $elem = $(`#${elementId}`);
    let rawValue = $elem.val() || "";
    let cleanedValue = 0;
    if (rawValue) {
        // Hapus 'Rp', hapus pemisah ribuan (titik)
        let tempValue = rawValue.replace(/Rp\s*/g, '').replace(/\./g, '');
        // 💡 PERBAIKAN: Jika ada koma, asumsikan itu adalah pemisah desimal, 
        // dan kita hanya ambil bagian BULAT (sebelum koma)
        const parts = tempValue.split(',');
        // Ambil nilai integer dari bagian BULAT
        cleanedValue = parseInt(parts[0]) || 0;
    }
    if (forceZeroIfEmpty && !rawValue) {
        cleanedValue = 0;
    }
    // 2. Format: Terapkan format "titik" ribuan
    let formattedValue = formatRupiah(cleanedValue);
    // 3. Update input field
    $elem.val(formattedValue);
    // 4. Simpan nilai mentah di data-raw 
    $elem.data("raw", cleanedValue);
}
//document ready
$(document).ready(function () {    
    // Inisialisasi autocomplete pelanggan
    initautocomplete();    
    // =======================================================
    // ✅ SATU-SATUNYA KODE EVENT HANDLER UNTUK NOMINAL BAYAR
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
            <div class="col-md-3">
                <label class="form-label fw-bold">Nama Produk</label>
                <div class="input-group">
                    <select class="form-select form-control produk-dropdown" style="width: 100%;">
                        <option value="">Pilih Produk</option>
                    </select>
                </div>
                <input type="hidden" name="IdProduk" class="produk-id-hidden"/>                
            </div>				
				<div class="col-md-3">
    <label class="form-label">Satuan Produk</label>
    <div class="form-group">
        <select class="form-control SatuanProduk-dropdown">
            <option value="">-- pilih satuan --</option>
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
                        <input name="Profit" class="form-control profit" readonly="true"/>
                        </div>
                        <span class="text-danger"></span>
                    </div>
                </div>
				<div class="col-md-4">
					<div class="form-group">
						<label>Qty</label>
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
        initProductSelect2($newCard.find(".produk-dropdown"));
    });
    //Hapus item transaksi
    $('#ContainerAdditemsProduk').on('click', '.BtnTrash', function () {
        $(this).closest('.card').remove();        
    });    
    //Tombol plus dengan validasi multi satuan (Pcs/Dus)
    $(document).on("click", ".btn-plus", function () {
        const $card = $(this).closest(".transaksi-item");
        const $input = $card.find(".jumlah");
        let val = parseInt($input.val()) || 0;
        // Ambil nilai batas maksimal stok yang sudah tampil di layar (sudah terkonversi)
        const maxStokTampil = parseInt($card.find("input[name='Stok']").val()) || 0;
        const unitName = $card.find(".SatuanProduk-dropdown option:selected").text() || "Unit";
        if (val + 1 > maxStokTampil) {
            Swal.fire({
                icon: "warning",
                title: "Stok tidak cukup!",
                text: `Maksimal pembelian untuk satuan ${unitName} adalah ${maxStokTampil} ${unitName}.`,
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
            return;
        }
        $input.val(val + 1);
        HitungProfit();
        updateTotalBelanja();
    });
    //Tombol minus
    $(document).on("click", ".btn-minus", function () {
        const $input = $(this).closest(".transaksi-item").find(".jumlah");
        let val = parseInt($input.val()) || 0;
        if (val > 0) { // Ubah dari val > 1 menjadi val > 0 agar bisa diset minimal 0
            $input.val(val - 1);     
            HitungProfit();
            updateTotalBelanja();
        }
    });
    //Jika qty diketik manual dengan validasi multi satuan
    $(document).on("input", ".jumlah", function () {
        const $card = $(this).closest(".transaksi-item");
        let qty = parseInt($(this).val().replace(/\./g, "")) || 0;
        if (qty < 0) {
            $(this).val(0);
            qty = 0;
        }
        const maxStokTampil = parseInt($card.find("input[name='Stok']").val()) || 0;
        const unitName = $card.find(".SatuanProduk-dropdown option:selected").text() || "Unit";
        if (qty > maxStokTampil) {
            Swal.fire({
                icon: "warning",
                title: "Stok tidak cukup!",
                text: `Maksimal pembelian untuk satuan ${unitName} adalah ${maxStokTampil} ${unitName}.`,
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
            $(this).val(maxStokTampil);
        }
        HitungProfit();
        updateTotalBelanja();
    });
    //FUNGSI INI WAJIB DIPANGGIL SAAT LOAD AWAL DARI DB!
    cleanAndFormatMoneyInitialValue("NominalBayar", true);
    cleanAndFormatMoneyInitialValue("Kembalian", true);
    initProductSelect2();
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
        // ✅ Perbaikan: AKTIFKAN tombol submit
        $("#BtnSubmit").attr("disabled", false);
    } else {
        // UANG KURANG (kembalian < 0)
        // 1. Update tampilan kembalian (tampilkan nilai negatif)
        $("#Kembalian").val("-" + formatRupiah(Math.abs(kembalian)));
        $("#Kembalian").data("raw", kembalian);
        // ✅ Nonaktifkan tombol submit karena uang kurang
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
    // Ambil IdTransaksi dari input hidden (jika ada, tambahkan di HTML jika belum ada)
    let idTransaksi = $("#IdTransaksi").val() || 0;
    // kumpulkan data form utama
    let formData = {
        IdTransaksi: parseInt(idTransaksi) || 0,
        IdPelanggan: $("#IdPelanggan").val(), // Menggunakan ID selektor yang lebih akurat
        IdMetodeBayar: parseInt($("#IdMetodeBayar").val()),
        Email: $("input[name='Email']").val(),
        Phone: $("input[name='Phone']").val(),
        Notes: $("textarea[name='Notes']").val(),
        Kembalian: $(this).find("#Kembalian").data("raw") !== undefined ? $(this).find("#Kembalian").data("raw") : $(this).find("input[name='Kembalian']").val().replace(/\./g, ""),
        NominalBayar: $(this).find("#NominalBayar").data("raw") !== undefined ? $(this).find("#NominalBayar").data("raw") : $(this).find("input[name='NominalBayar']").val().replace(/\./g, ""),
        TotalBelanja: $(this).find("#TotalBelanja").data("raw") !== undefined ? $(this).find("#TotalBelanja").data("raw") : $(this).find("input[name='TotalBelanja']").val().replace(/\./g, ""),
        Items: []
    };
    // kumpulkan data items    
    $("#ContainerAdditemsProduk .transaksi-item").each(function () {
        let $itemCard = $(this);
        let hargaJualElement = $itemCard.find("input[name='HargaJual']");
        let rawHargaJual = hargaJualElement.data("raw");
        let fallbackHargaJual = hargaJualElement.val() || "0";
        let cleanedInputHargaJual = fallbackHargaJual.replace(/[^0-9]/g, "");
        let finalHargaJual = rawHargaJual > 0 ? rawHargaJual : parseFloat(cleanedInputHargaJual);        
        let hargaModalRawItem = $itemCard.find(".hargaModal").data("raw") || 0;
        let item = {
            IdProduk: $itemCard.find(".produk-id-hidden").val(),
            ProductUnitId: $itemCard.find(".SatuanProduk-id-hidden").val(),
            //Stok: $itemCard.find("input[name='Stok']").val(),
            HargaJual: finalHargaJual,
            HargaModal: parseFloat(hargaModalRawItem),
            Qty: $itemCard.find("input[name='Jumlah']").val(),            
            Conversion: parseInt($itemCard.find(".SatuanProduk-dropdown").data("konversi")) || 1,
            Profit: $(this).find(".profit").data("raw") || $(this).find(".profit").val().replace(/\./g, "")
        };
        formData.Items.push(item);
    });
    //Validasi form
    if (!ValidasiForm(formData)) {
        return; // stop jika validasi gagal
    }
    // kirim via ajax
    $.ajax({
        url: "/Transaksi/Create",
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
                toast: true,
                position: "top-end"
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
                toast: true,
                position: "top-end"
            });
        }
    });
});
function ValidasiForm(formData) {
    let errors = [];
    if (!formData.IdPelanggan) errors.push("Nama Customer tidak boleh kosong.");
    if (!formData.Email) errors.push("Email tidak boleh kosong.");
    if (!formData.Phone) errors.push("Phone tidak boleh kosong.");
    if (!formData.IdMetodeBayar) errors.push("Metode Pembayaran tidak boleh kosong.");
    if (!formData.NominalBayar || formData.NominalBayar === "0") errors.push("Nominal Bayar tidak boleh kosong.");
    if (!formData.TotalBelanja || formData.TotalBelanja === "0") errors.push("Total Belanja tidak boleh kosong.");
    if (formData.Items.length === 0) {
        errors.push("Minimal 1 produk harus ditambahkan.");
    } else {
        $("#ContainerAdditemsProduk .transaksi-item").each(function (index) {
            const $card = $(this);
            const namaProduk = $card.find(".produk-dropdown option:selected").text() || `ke-${index + 1}`;
            const qty = parseInt($card.find("input[name='Jumlah']").val()) || 0;
            const stok = parseInt($card.find("input[name='Stok']").val()) || 0;
            if (!$card.find(".produk-id-hidden").val()) {
                errors.push(`Produk ${namaProduk} belum dipilih.`);
            }
            if (qty <= 0) {
                errors.push(`Qty produk ${namaProduk} harus lebih dari 0.`);
            }
            // VALIDASI STOK DI SINI
            if (qty > stok) {
                errors.push(`Stok produk ${namaProduk} tidak mencukupi (Tersedia: ${stok}, Diminta: ${qty}).`);
            }
        });
    }
    if (errors.length > 0) {
        Swal.fire({
            icon: "warning",
            title: "Validasi Gagal",
            html: errors.join("<br>"),
            toast: true,
            position: "top-end"
        });
        return false;
    }
    return true;
}
//Autocomplete untuk dropdown pelanggan/custumer
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
// PANGGIL INI SEKALI SAAT LOAD:
// cleanAndFormatMoneyInitialValue("TotalBelanja", true);
let nominalBayarTimeout = null; // Variabel global untuk menyimpan ID timeout
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
                title: "Masukkan barcode terlebih dahulu",
                position: 'top-end',
                toast: true
            });
        }
    }
});
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
                title: "Barcode tidak ditemukan",
                position: 'top-end',
                toast: true
            });
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
            $unitDrop.append(`<option value="${u.productUnitId}">${u.unitName}</option>`);
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
// untuk load harga jual, modal, stok berdasarkan satuan produk yang dipilih
// Ketika Satuan dipilih → load HargaJual, Modal, Stok dll
$(document).on("change", ".SatuanProduk-dropdown", function () {
    const $dropdown = $(this);
    const productUnitId = $dropdown.val();
    const $card = $dropdown.closest(".transaksi-item");
    if (!productUnitId) return;
    $.ajax({
        url: '/Transaksi/GetUnitDetail',
        type: 'GET',
        data: { productUnitId: productUnitId },
        success: function (data) {
            $card.find(".SatuanProduk-id-hidden").val(productUnitId);
            $card.find(".SatuanProduk-dropdown").data("konversi", data.conversion);
            // Simpan nilai stok dasar asli (Pcs) ke data attribute agar tidak hilang saat ganti-ganti satuan
            if ($card.find("input[name='Stok']").data("stok-asli") === undefined) {
                $card.find("input[name='Stok']").data("stok-asli", data.stok);
            }
            const stokDasarAsli = parseInt($card.find("input[name='Stok']").data("stok-asli")) || 0;
            const conversionValue = parseInt(data.conversion) || 1;
            // HITUNG STOK YANG DITAMPILKAN DI LAYAR (Contoh: 40 Pcs / 40 Konversi Dus = 1 Dus)
            const stokTampil = Math.floor(stokDasarAsli / conversionValue);
            $card.find("input[name='Stok']").val(stokTampil);
            $card.find(".hargaModal").val(data.hargaModal.toLocaleString("id-ID")).data("raw", data.hargaModal);
            $card.find(".hargaJual").val(data.hargaJual.toLocaleString("id-ID")).data("raw", data.hargaJual);
            // Reset input jumlah menjadi 1 setiap kali ganti satuan demi keamanan validasi
            $card.find(".jumlah").val(1);
            HitungProfit();
            updateTotalBelanja();
        },
        error: function (xhr, status, error) {
            let errorMessage = "Gagal mengambil detail harga dari server.";
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            Swal.fire({
                icon: "error",
                title: "Error",
                text: errorMessage,
                toast: true,
                position: 'top-end'
            });
        }
    });
});
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