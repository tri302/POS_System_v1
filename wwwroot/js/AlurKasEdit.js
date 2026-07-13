$(document).ready(function () {
    // Definisikan fungsi untuk memuat ulang dropdown
    function loadSourceDropdown(selectedType) {
        console.log("Memuat data untuk Tipe:", selectedType);
        $.ajax({
            url: '/AlurKas/GetSources',
            type: 'GET',
            data: { type: selectedType },
            cache: false, // Penting: Matikan caching
            success: function (data) {
                var dropdown = $('#sumberAlurKasDropdown');
                dropdown.empty();
                // Tambahkan opsi default
                dropdown.append('<option value="" disabled selected>-- Pilih salah satu opsi --</option>');
                // Tambahkan opsi baru
                $.each(data, function (i, item) {
                    // Pastikan properti 'value' dan 'text' digunakan
                    dropdown.append($('<option></option>').val(item.value).text(item.text));
                });
            },
            error: function (xhr, status, error) {
                console.error("Gagal load sumber:", status, error);
                // Menggunakan SweetAlert2 untuk penanganan error
                Swal.fire({
                    icon: 'error',
                    title: 'Oops... Terjadi Kesalahan',
                    text: 'Gagal memuat data sumber. Silakan coba beberapa saat lagi.',
                    confirmButtonColor: '#3085d6',
                    toast: true,
                    position: 'top-end',
                    confirmButtonText: 'Oke'
                });
            }
        });
    }
    // Event Listener pada Label Tombol
    // Tangkap klik pada elemen label yang berfungsi sebagai tombol Tipe
    $('.cashflow-type-label').on('click', function () {
        var inputId = $(this).attr('for');
        var selectedType = $('#' + inputId).val();
        loadSourceDropdown(selectedType);
    });
    // Panggil fungsi saat halaman dimuat
    // Ambil nilai radio button yang sedang 'checked' (yaitu "Masuk" secara default)
    var initialType = $('input[type="radio"][name="btnradio"]:checked').val();
    if (initialType) {
        loadSourceDropdown(initialType);
    }
});
$(document).ready(function () {
    // Set default regional ke Indonesian
    $.datepicker.setDefaults($.datepicker.regional['id']);
    // Inisialisasi datepicker pada elemen dengan class 'date-picker'
    $('.date-picker').datepicker({
        // Format tanggal untuk tampilan dd/mm/yyyy
        dateFormat: 'dd/mm/yy',
        // Tambahkan opsi lain jika diperlukan, contoh:
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true
    });
    // Re-enable client-side validation
    var form = $('#sumberAlurKasDropdown').closest('form');
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
});