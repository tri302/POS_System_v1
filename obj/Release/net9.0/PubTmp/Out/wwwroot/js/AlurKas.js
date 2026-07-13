$(document).ready(function () {
    // Create
    $("#btnCreate").on('click', function () {
        $.get("/AlurKas/Create", function (html) {
            $("#ContainerModalCreate").html(html);
            $("#ModalCreate").modal('show');
        });
    });
    // Edit
    $(".btnEdit").on('click', function () {
        var id = $(this).data("id");
        $.get("/AlurKas/Edit/" + id, function (html) {
            $("#ContainerModalEdit").html(html);
            $("#ModalEdit").modal('show');
        });
    });
    $(document).on('shown.bs.modal', '#ModalCreate', function () {
        const self = $(this);
        // Ambil tipe default dari radio button yang dicentang (biasanya value="1")
        var initialType = self.find('input[name="TipeAlurKas"]:checked').val();
        // Jika karena suatu hal tidak terdeteksi, default ke 1
        if (!initialType) {
            initialType = 1;
            self.find('input[name="TipeAlurKas"][value="1"]').prop('checked', true);
        }
        // Jalankan loader dropdown
        loadSourceDropdown(initialType);
        //Format Amount to indonesian currency
        const val = $('#Amount').val();
        if (val) {
            $('#AmountDisplay').val(val.replace(/\B(?=(\d{3})+(?!\d))/g, '.'));
        }
        //Set default datePicker to indonesian format
        $.datepicker.setDefaults($.datepicker.regional['id']);
        $('.date-picker').datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true,
            //showButtonPanel: true
        });
        //Validate form
        var form = $(this).find('form');
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
    });
    $(document).on('shown.bs.modal', '#ModalEdit', function () {
        //Set default datePicker to indonesian format
        $.datepicker.setDefaults($.datepicker.regional['id']);
        $('.date-picker').datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true,
            //showButtonPanel: true
        });
        // Tangkap nilai mentah (dari input hidden)
        let rawAmount = $('#Amount').val(); // Misal: "250000,00"
        if (rawAmount) {
            // Ganti pemisah desimal lokal (koma) menjadi titik (jika ada), lalu hapus. 
            //    Ini untuk mendapatkan nilai integer murni.
            //    Misal: "250000,00" menjadi "250000.00" (sebelumnya: "250000,00").
            //    Lalu kita HANYA mengambil bagian integer.
            let cleanedAmount = rawAmount.toString()
                .replace(',', '.'); // Ganti koma desimal ke titik (optional, tergantung konfigurasi server)
            // Ambil bagian integer saja (sebelum titik/koma desimal, jika ada)
            if (cleanedAmount.includes('.')) {
                cleanedAmount = cleanedAmount.split('.')[0];
            }
            // Hapus semua karakter non-digit (pastikan hanya angka integer yang tersisa)
            let rawValue = cleanedAmount.replace(/\D/g, ''); // Nilai: "250000"
            if (rawValue === '') {
                $('#Amount').val('');
                $('#AmountDisplay').val('');
                return;
            }
            // Simpan nilai integer murni kembali ke input hidden
            $('#Amount').val(rawValue); // Output: 250000
            // Format ulang pakai titik ribuan untuk tampilan
            const formattedAmount = rawValue.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
            // Masukkan ke field tampilan
            $('#AmountDisplay').val(formattedAmount); // Output: 250.000
        } else {
            $('#AmountDisplay').val('');
        }
        var form = $(this).find('form');
        form.removeData('validator');
        form.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
        // Ambil nilai-nilai yang sudah ada dari Partial View yang dirender
        var dropdown = $(this).find('#sumberAlurKasDropdown');
        // Ambil nilai tipe dari radio button yang ter-checked di modal edit
        var initialType = $(this).find('input[name="TipeAlurKas"]:checked').val(); // <-- Pastikan name="TipeAlurKas"
        // Ambil nilai Source ID yang disimpan di data attribute
        var savedSourceId = dropdown.data('saved-idcashflow');
        if (initialType) {
            // Panggil fungsi dengan nilai Tipe dan Source ID yang tersimpan
            loadSourceDropdownEdit(initialType, savedSourceId);
        }
    });
    //Event delegation untuk input amount    
    $(document).on('input', '#AmountDisplay', function () {
        // Ambil nilai dan hapus semua titik
        let value = $(this).val().replace(/\./g, '');
        // Ambil hanya digit
        let rawValue = value.replace(/\D/g, '');
        if (rawValue === '') {
            $(this).val('');
            $('#Amount').val('');
            return;
        }
        // Format ulang pakai titik ribuan
        const formatted = rawValue.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        // Tampilkan format
        $(this).val(formatted);
        // Simpan nilai mentah tanpa titik (ke hidden input)
        $('#Amount').val(rawValue);
    });
    //Load Data to list dropdown    
    function loadSourceDropdownEdit(selectedType, selectedSourceId) { // << TAMBAHKAN PARAMETER KEDUA
        var dropdown = $('#sumberAlurKasDropdown');
        dropdown.empty();
        dropdown.append('<option value="" disabled selected>-- Select Source --</option>'); // Tambah opsi default
        if (!selectedType) return;
        // Pastikan URL Anda benar: /AlurKas/GetSources/{IdCashFlow}
        // Jika IdCashFlow adalah Tipe (1/2) maka URL: /AlurKas/GetSources/1 atau /AlurKas/GetSources/2
        $.ajax({
            url: '/AlurKas/GetSources/' + selectedType,
            type: 'GET',
            cache: false,
            dataType: "json",
            success: function (data) {
                $.each(data, function (i, item) {
                    dropdown.append($('<option></option>').val(item.idCashFlow).text(item.cashFlowName));
                });
                // **PERBAIKAN UTAMA DI SINI:** Setelah data dimuat, set nilai yang tersimpan
                if (selectedSourceId) {
                    dropdown.val(selectedSourceId);
                }
            },
            error: function (xhr, status, error) {
                console.error("Gagal load sumber. Status:", status, error);
            }
        });
    }
    // Dropdown loader
    function loadSourceDropdown(IdTipeCashFlow) {
        var dropdown = $('#sumberAlurKasDropdown');
        dropdown.empty();
        // Beri indikasi loading agar user tahu data sedang diambil
        dropdown.append('<option value="" disabled selected>-- Select Source --</option>');
        dropdown.val('');
        if (!IdTipeCashFlow) return;
        $.ajax({
            url: '/AlurKas/GetSources/' + IdTipeCashFlow,
            type: 'GET',
            cache: false,
            dataType: "json",
            success: function (data) {
                $.each(data, function (i, item) {
                    dropdown.append($('<option></option>').val(item.idCashFlow).text(item.cashFlowName));
                });
            },
            error: function (xhr, status, error) {
                console.error("Gagal load sumber. Status:", status, error);
            }
        });
    }
    $('body').on('click', '.cashflow-type-label', function () {
        var inputId = $(this).attr('for');
        var selectedType = $('#' + inputId).val();
        loadSourceDropdown(selectedType);
    });
    // Submit create form
    $(document).on("submit", "form", function (e) {
        e.preventDefault();
        var form = $(this);
        // ambil nilai display        
        // Ini akan mengubah "4.000.000" menjadi "4000000"
        const amountRaw = $('#AmountDisplay').val().replace(/\D/g, "");
        $('#Amount').val(amountRaw); // <-- Isi hidden field Amount dengan nilai murni
        // Lakukan validasi menggunakan nilai angka murni
        if (!amountRaw || isNaN(amountRaw) || parseFloat(amountRaw) <= 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Warning',
                text: 'Isi Amount dengan benar dan lebih dari 0!',
                showConfirmButton: true,
                timer: 5000,
                toast: true,
                position: 'top-end'
            });
            $('#AmountDisplay').focus();
            return; // Hentikan proses submit AJAX
        }
        // VALIDASI SUMBER KAS (IdCashFlow) ---
        if ($("#sumberAlurKasDropdown").val() == "" || $("#sumberAlurKasDropdown").val() == null) {
            Swal.fire({
                icon: 'warning',
                title: 'Warning',
                text: 'Source tidak boleh kosong !!!',
                showConfirmButton: true,
                timer: 5000,
                toast: true,
                position: 'top-end'
            })
            $("#sumberAlurKasDropdown").focus()
            return;
        }
        // PENGATURAN FORMAT TANGGAL YANG BENAR UNTUK SERVER ---
        var dateVal = $('#Date').val();
        if (!dateVal) { // Cek jika tanggal kosong
            Swal.fire({
                icon: 'warning',
                title: 'Warning',
                text: 'Tanggal tidak boleh kosong !!!',
                showConfirmButton: true,
                timer: 5000,
                toast: true,
                position: 'top-end'
            })
            $("#Date").focus()
            return;
        }
        var parts = dateVal.split("/");
        if (parts.length === 3) {
            // ubah dari dd/MM/yyyy ke yyyy-MM-dd
            dateVal = `${parts[2]}-${parts[1]}-${parts[0]}`;
            // TIDAK PERLU lagi set val() ke hidden field
        }
        // Karena Anda menggunakan date-picker, format yang dikirim mungkin sudah benar.
        // Jika format tanggal di input adalah dd/MM/yyyy, Anda harus mengirimkannya sebagai yyyy-MM-dd
        // ke server. Kita akan memasukkannya secara manual ke data yang akan dikirim:
        // Ambil semua data form, lalu tambahkan/timpa Date dengan format yyyy-MM-dd
        var formData = form.serializeArray();
        // Cari field "Date" dan perbarui nilainya
        var dateFound = false;
        for (var i = 0; i < formData.length; i++) {
            if (formData[i].name === "Date") {
                formData[i].value = dateVal; // Gunakan format yyyy-MM-dd yang sudah dikonversi
                dateFound = true;
                break;
            }
        }
        // Jika Date tidak ada di formData (walaupun seharusnya ada), tambahkan
        if (!dateFound) {
            formData.push({ name: "Date", value: dateVal });
        }
        // *Controller C# Anda akan menerima nilai dari IdCashFlow, Amount, Date, dan Notes*
        $.ajax({
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            url: form.attr("action"),
            method: form.attr("method"),
            // Kirim data yang sudah dimodifikasi (termasuk tanggal dengan format server)
            data: $.param(formData),
            success: function (res) {
                // Jika Controller mengembalikan JSON { success: true, ... }
                if (res.success) {
                    console.log("Log Form", $.param(formData)); // Log data yang dikirim
                    Swal.fire({
                        icon: 'success',
                        title: 'Berhasil',
                        text: res.message,
                        showConfirmButton: true,
                        timer: 5000,
                        toast: true,
                        position: 'top-end'
                    }).then(() => {
                        window.location.href = "/AlurKas/Index";
                    });
                    // Jika Controller mengembalikan PartialView (karena ModelState.IsValid = false)
                } else {
                    // replace form with updated partial (validation errors)
                    // Ini berarti ada validasi server yang gagal, dan response 'res' berisi HTML partial view
                    form.parent().html(res);
                }
            },
            //Tambahkan ERROR handler untuk melihat masalah koneksi/server ---
            error: function (xhr, status, error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error Jaringan/Server',
                    text: 'Terjadi kesalahan saat mengirim data. Cek log konsol untuk detail.',
                    showConfirmButton: true,
                    toast: true,
                    position: 'top-end'
                });
                console.error("AJAX Error:", status, error);
                console.log("XHR Response:", xhr.responseText);
            }
        });
    });
});
// Fungsi untuk update data Summary (Total Modal, Masuk, Keluar, Toko) 🔥
function updateSummary() {
    $.ajax({
        url: '/AlurKas/GetSummaryData', // Panggil action GetSummaryData
        type: 'GET',
        dataType: 'json',
        success: function (newTotals) {
            console.log("AJAX: Summary diperbarui.", newTotals);
            // Perbarui elemen-elemen di _PartialIndexAlurKas.cshtml
            $('#totalModalIndex').text(newTotals.totalModal);
            $('#totalMasukIndex').text(newTotals.totalUangMasuk);
            $('#totalKeluarIndex').text(newTotals.totalUangKeluar);
            $('#totalTokoIndex').text(newTotals.totalUangToko);
            // Perbarui detail masuk/keluar di Total Uang Toko
            $('#masukTokoIndex').text(newTotals.totalUangMasuk);
            $('#keluarTokoIndex').text(newTotals.totalUangKeluar);
        },
        error: function (xhr, status, error) {
            console.error("Gagal mengambil data summary terbaru:", error);
        }
    });
}
// Fungsi untuk update Table Alur Kas
function updateTable() {
    $.ajax({
        url: '/AlurKas/GetAlurKasTableData',
        type: 'GET',
        success: function (htmlContent) {
            console.log("AJAX: Tabel diperbarui.");
            // Ganti isi <tbody> dengan Partial View yang baru
            $('#alurKasTableBody').html(htmlContent);
            // Re-bind event Edit setelah DOM diganti! (PENTING)
            $(".btnEdit").off('click').on('click', function () {
                var id = $(this).data("id");
                $.get("/AlurKas/Edit/" + id, function (html) {
                    $("#ContainerModalEdit").html(html);
                    $("#ModalEdit").modal('show');
                });
            });
        },
        error: function (xhr, status, error) {
            console.error("Gagal mengambil data tabel terbaru:", error);
        }
    });
}
//Koneksi SignalR untuk update data secara real-time
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/cashFlowHub")
    .build();
// DAFTARKAN LISTENER (Harus di luar connection.start())
// Ketika server mengirim sinyal "ReceiveDataUpdate", panggil updateSummary()
connection.on("ReceiveDataUpdate", () => {
    console.log("SignalR: Menerima sinyal pembaruan data.");
    // Pemicu AJAX call ke Controller untuk ambil data terbaru
    updateSummary();
    // 💡 Tambahkan juga updateTable() jika Anda ingin tabel di-refresh
    updateTable();
});
// START KONEKSI (Harus dipanggil setelah listener didaftarkan dan BUKAN di dalamnya)
connection.start()
    .then(() => console.log("SignalR terhubung."))
    .catch(err => console.error("Gagal konek SignalR:", err));