(function () {
    //const display = document.getElementById('AmountDisplay');
    const hidden = document.getElementById('Amount');
    function setCaret(el, pos) {
        if (el.setSelectionRange) {
            el.focus();
            el.setSelectionRange(pos, pos);
        }
    }
    function formatNumberWithDots(value) {
        // hanya angka (hapus karakter non-digit)
        let v = value.replace(/\D/g, '');
        if (v === '') return '';
        // tambahkan pemisah ribuan (titik)
        return v.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    }
    function rawFromFormatted(formatted) {
        return formatted.replace(/\D/g, ''); // semua non-digit dihapus
    }
    display.addEventListener('input', function (e) {
        const el = e.target;
        const oldVal = el.value;
        const oldPos = el.selectionStart;
        // hitung berapa digit di sebelah kiri kursor sebelum formatting
        const leftPart = oldVal.slice(0, oldPos);
        const digitsLeftBefore = (leftPart.match(/\d/g) || []).length;
        // format
        const formatted = formatNumberWithDots(oldVal);
        el.value = formatted;
        // tulis ke hidden (mentah)
        hidden.value = rawFromFormatted(formatted);
        // pindahkan caret: cari posisi di mana harus diposisikan kembali
        // hitung posisi berdasarkan jumlah digit di kiri yang ingin dipertahankan
        let pos = 0;
        let digitsSeen = 0;
        for (let i = 0; i < el.value.length; i++) {
            if (/\d/.test(el.value[i])) digitsSeen++;
            pos++;
            if (digitsSeen >= digitsLeftBefore) break;
        }
        setCaret(el, pos);
    });
    // opsi: saat blur, kalau kosong set ke "0" atau kosongkan
    display.addEventListener('blur', function () {
        if (!this.value) {
            this.value = '';
            hidden.value = '';
        }
    });
    // inisialisasi jika ada nilai awal di hidden
    (function init() {
        if (hidden.value) {
            display.value = formatNumberWithDots(hidden.value.toString());
        }
    })();
})();
