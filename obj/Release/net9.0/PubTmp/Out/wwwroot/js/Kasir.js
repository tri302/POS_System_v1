// ======================================================
// GLOBAL STATE
// ======================================================
// item: { id: productUnitId, IdProduk, name, price, quantity, image, availableUnits: [{productUnitId, unitName, hargaJual, stok, conversionValue}] }
let cart = [];
let currentTotal = 0;
let nominalBayarInput = null;
let modalKembalianElement = null;
let modalTotalBelanjaElement = null;
let btnBayar = null;
let currentPage = 1;
let currentQuery = "";
let currentCategoryId = "";
let currentIsBarcode = false;
let isSavingTransaction = false;
// ======================================================
// HELPERS
// ======================================================
function safeParseInt(v, fallback = 0) {
    const n = parseInt(String(v).replace(/[^\d\-]/g, ''), 10);
    return Number.isFinite(n) ? n : fallback;
}
function safeParseFloat(v, fallback = 0) {
    if (v === null || v === undefined) return fallback;
    const s = String(v).replace(/\./g, '').replace(',', '.').replace(/[^\d\.\-]/g, '');
    const n = parseFloat(s);
    return Number.isFinite(n) ? n : fallback;
}
function formatRupiah(price) {
    return new Intl.NumberFormat("id-ID", { style: "currency", currency: "IDR", minimumFractionDigits: 0 }).format(Number(price) || 0);
}
function escapeHtml(text) {
    if (text === null || text === undefined) return "";
    return String(text)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}
/* ============================================================================
   Menambahkan mapping `conversionValue` agar tidak bernilai undefined saat di-render.
   ============================================================================ */
function normalizeUnits(rawUnits) {
    let units = Array.isArray(rawUnits) ? rawUnits : [];
    if (!units.length && typeof rawUnits === "string") {
        try { units = JSON.parse(rawUnits); } catch (e) { units = []; }
    }
    return units.map(u => ({
        productUnitId: safeParseInt(u.productUnitId ?? u.ProductUnitId ?? u.ProductUnitID, 0),
        unitName: u.unitName ?? u.UnitName ?? u.Unit ?? u.namaUnit ?? "",
        hargaJual: safeParseFloat(u.hargaJual ?? u.HargaJual ?? u.price ?? u.Harga, 0),
        stok: safeParseInt(u.stok ?? u.Stok ?? u.stock ?? u.Stock, 0),
        conversionValue: safeParseInt(u.conversionValue ?? u.ConversionValue ?? u.Conversion ?? u.Konversi, 1) // Ditambahkan!
    }));
}
// ======================================================
// INPUT FORMAT (nominal bayar)
// ======================================================
function cleanAndFormatInput(rawValue) {
    const numericString = String(rawValue).replace(/[^\d]/g, '');
    const numericValue = parseInt(numericString) || 0;
    if (nominalBayarInput) {
        nominalBayarInput.value = numericValue > 0
            ? new Intl.NumberFormat('id-ID').format(numericValue)
            : '';
    }
    return numericValue;
}
// ======================================================
// CALCULATE CHANGE
// ======================================================
function calculateChange() {
    const total = currentTotal || 0;
    const paid = nominalBayarInput
        ? safeParseInt(nominalBayarInput.value.replace(/[^\d]/g, '')) || 0
        : 0;
    const change = paid - total;
    if (modalKembalianElement) modalKembalianElement.textContent = formatRupiah(Math.max(0, change));
    if (modalTotalBelanjaElement) modalTotalBelanjaElement.textContent = formatRupiah(total);
    if (btnBayar) btnBayar.disabled = !(paid >= total && total > 0);
}
// ======================================================
// FETCH PRODUCTS (AJAX)
// ======================================================
function fetchProducts({ page = 1, query = "", categoryId = "", isBarcode = false } = {}) {
    currentPage = page;
    currentQuery = query;
    currentCategoryId = categoryId;
    currentIsBarcode = isBarcode;
    if (typeof searchProductsUrl === "undefined") {
        console.warn("searchProductsUrl not defined - skipping fetchProducts.");
        return;
    }
    $.ajax({
        url: searchProductsUrl,
        type: "GET",
        data: { page, query, categoryId, isBarcode },
        success: function (data) {
            $("#productListContainer").html(data);
            attachPagingListeners();
        },
        error: function () {
            console.error("Gagal fetch products");
        }
    });
}
function attachPagingListeners() {
    $(document).off("click", ".page-btn").on("click", ".page-btn", function (e) {
        e.preventDefault();
        fetchProducts({
            page: safeParseInt($(this).data("page"), 1),
            query: currentQuery,
            categoryId: currentCategoryId,
            isBarcode: currentIsBarcode
        });
    });
}
/* ============================================================================
   Fungsi `addItemToCart`
   Parameter disamakan dan penataan variabel stok dasar dibersihkan agar konsisten.
   ============================================================================ */
function addItemToCart(idProduk, name, price, cartItemId, image, stokUtama, units) {
    const unitId = safeParseInt(cartItemId);
    const idP = safeParseInt(idProduk);
    const stokValid = safeParseInt(stokUtama, 0);
    const normUnits = normalizeUnits(units);
    const unitDetail = normUnits.find(u => u.productUnitId === unitId);
    const availableUnits = normUnits.length ? normUnits : (unitDetail ? [unitDetail] : []);
    const harga = unitDetail ? unitDetail.hargaJual : safeParseFloat(price, 0);
    // 1. CEK DUPLIKAT
    const existing = cart.find(item => item.productUnitId === unitId);
    if (existing) {
        Swal.fire({
            icon: 'info',
            title: 'Produk sudah ada',
            text: 'Produk ini sudah ada di keranjang belanja.',
            position: 'top-end',
            toast: true,
            timer: 5000,
            showConfirmButton: false
        });
        return;
    }
    // 2. Tambah item baru
    cart.push({
        productUnitId: unitId,
        id: unitId,
        IdProduk: idP,
        name: name,
        price: harga,
        quantity: 1,
        image: image || "",
        stok: stokValid, // Menyimpan stok dasar utama produk
        availableUnits: availableUnits
    });
    renderCart();
}
/* ============================================================================
   Fungsi `window.addToCart`  
   ============================================================================ */
window.addToCart = function (buttonElement) {
    const unitId = safeParseInt(buttonElement.data("id"));
    const stokUtama = safeParseInt(buttonElement.data("stok"));
    const idProduk = safeParseInt(buttonElement.data("idproduk"));
    const name = buttonElement.data("name");
    const price = safeParseFloat(buttonElement.data("price")) || 0;
    const image = buttonElement.data("image");
    let units = buttonElement.data("units");
    if (!units) units = [];
    if (typeof units === "string") {
        try { units = JSON.parse(units); } catch (e) { units = []; }
    }
    //Urutan parameter diubah agar sesuai target (stokUtama dulu baru units)
    addItemToCart(idProduk, name, price, unitId, image, stokUtama, units);
};
/* ============================================================================
   Fungsi `handleBarcodeScan`
   Menambahkan parameter stokUtama (product.stok) yang sebelumnya hilang agar stok tidak menjadi 0.
   ============================================================================ */
function handleBarcodeScan(barcode) {
    if (!barcode) return;
    $.ajax({
        url: "/Kasir/GetProductByBarcode",
        type: "GET",
        data: { barcode: String(barcode).trim() },
        success: function (product) {
            if (!product) {
                Swal.fire({
                    icon: 'info',
                    title: 'Warning',
                    text:'Produk tidak ditemukan.',
                    position: 'top-end',
                    toast: true,
                    timer: 5000,
                    showConfirmButton: false
                });
                return;
            }
            let units = Array.isArray(product.availableUnits) ? product.availableUnits : [];
            if (!units.length && typeof product.availableUnits === "string") {
                try { units = JSON.parse(product.availableUnits || "[]"); } catch (e) { units = []; }
            }
            if (units.length === 0) {
                Swal.fire({
                    icon: 'info',
                    title: 'Warning',
                    text: 'Data unit produk tidak tersedia.',
                    position: 'top-end',
                    toast: true,
                    timer: 5000,
                    showConfirmButton: false
                });
                return;
            }
            const normalized = normalizeUnits(units);
            const unit = normalized[0];
            if (!unit) {
                Swal.fire({
                    icon: 'info',
                    title: 'Warning',
                    text: 'Unit produk tidak valid.',
                    position: 'top-end',
                    toast: true,
                    timer: 5000,
                    showConfirmButton: false
                });
                return;
            }
            const stokUtama = safeParseInt(product.stok ?? product.Stok ?? unit.stok, 0);
            // Meloloskan parameter lengkap ke addItemToCart
            addItemToCart(
                product.idProduk ?? product.IdProduk ?? product.Id,
                product.namaProduk ?? product.NamaProduk ?? product.name,
                unit.hargaJual,
                unit.productUnitId,
                product.gambar ?? product.image ?? "",
                stokUtama, // Ditambahkan!
                normalized // Menggunakan unit yang sudah di-normalize
            );
            const cartEl = document.getElementById("cart-items");
            if (cartEl) cartEl.scrollIntoView({ behavior: "smooth" });
            $("#barcodeInput").focus();
        },
        error: function () {
            Swal.fire({
                icon: 'info',
                title: 'Warning',
                text: 'Barcode tidak ditemukan!.',
                position: 'top-end',
                toast: true,
                timer: 5000,
                showConfirmButton: false
            });
        }
    });
}
$(document).off("keypress", "#barcodeInput").on("keypress", "#barcodeInput", function (e) {
    if (e.which === 13) {
        e.preventDefault();
        const barcode = $(this).val().trim();
        $(this).val("");
        handleBarcodeScan(barcode);
    }
});
$(document).off("click", "#BtnbarcodeInput").on("click", "#BtnbarcodeInput", function () {
    const barcode = $("#barcodeInput").val().trim();
    $("#barcodeInput").val("");
    handleBarcodeScan(barcode);
});
/* ============================================================================
   Fungsi `renderCart`
   Mengubah properti penunjuk dari `u.ConversionValue` menjadi `u.conversionValue` (sesuai hasil `normalizeUnits`).
   ============================================================================ */
function renderCart() {
    const cartItemsElement = document.getElementById("cart-items");
    const cartTotalElement = document.getElementById("cart-total");
    const totalBayarEl = document.getElementById("total-bayar");
    const cartEmptyMessageElement = document.getElementById("cart-empty-message");
    const cartSummaryElement = document.getElementById("cart-summary");
    if (!cartItemsElement) return;
    cartItemsElement.innerHTML = "";
    let total = 0;
    if (cart.length === 0) {
        if (cartEmptyMessageElement) cartEmptyMessageElement.classList.remove("d-none");
        if (cartSummaryElement) cartSummaryElement.classList.add("d-none");
    } else {
        if (cartEmptyMessageElement) cartEmptyMessageElement.classList.add("d-none");
        if (cartSummaryElement) cartSummaryElement.classList.remove("d-none");
        cart.forEach(item => {
            const stokDasar = safeParseInt(item.stok || 0);
            const selectedUnit = (item.availableUnits || []).find(u => parseInt(u.productUnitId) === parseInt(item.id));
            if (selectedUnit) {
                item.price = safeParseFloat(selectedUnit.hargaJual, item.price || 0);
            }
            //Menggunakan properti huruf kecil 'conversionValue' dari normalizeUnits
            const conversionSelected = selectedUnit ? safeParseInt(selectedUnit.conversionValue, 1) : 1;
            const maxQtyBerdasarkanSatuan = Math.floor(stokDasar / conversionSelected);
            if (item.quantity > maxQtyBerdasarkanSatuan) {
                item.quantity = maxQtyBerdasarkanSatuan;
            }
            if (item.quantity < 1 && maxQtyBerdasarkanSatuan > 0) item.quantity = 1;
            const subtotal = (safeParseFloat(item.price) || 0) * (safeParseInt(item.quantity) || 0);
            total += subtotal;
            let unitOptionsHtml = "";
            (item.availableUnits || []).forEach(u => {
                const unitId = parseInt(u.productUnitId);                
                const isSelected = parseInt(u.productUnitId) === parseInt(item.id) ? 'selected' : '';
                const unitLabel = escapeHtml(u.unitName || "");                
                const nilaiKonversi = safeParseInt(u.conversionValue, 1);                
                //unitOptionsHtml += `<option value="${unitId}" data-konversi="${nilaiKonversi}" ${isSelected}>${unitLabel}</option>`;
                unitOptionsHtml += `<option value="${u.productUnitId}" data-conversion="${u.conversionValue || u.ConversionValue || 1}" ${isSelected}>${escapeHtml(u.unitName)}</option>`;
            });
            cartItemsElement.innerHTML += `
            <div class="d-flex align-items-center mb-3 border-bottom pb-2 cart-item" data-idproduk="${item.IdProduk}" data-current-unit-id="${item.id}">
                <img src="${escapeHtml(item.image || '/img/no-image.png')}" class="me-2 rounded" style="width:50px;height:50px;object-fit:cover;">
                <div class="flex-grow-1 text-start" style="min-width: 0;">
                    <p class="fw-bold mb-0 product-name" title="${escapeHtml(item.name)}">${escapeHtml(item.name)}</p>
                    <small class="text-muted d-block">Subtotal: ${formatRupiah(subtotal)}</small>
                    <p class="text-success fw-bold mb-0 cart-item-price-display">${formatRupiah(item.price)}</p>
                </div>
                <div class="d-flex align-items-center gap-2 m-1">
                    <select class="form-select form-select-sm cart-unit-select" data-idproduk="${item.IdProduk}" data-cart-id="${item.id}" style="width:120px;">
                        ${unitOptionsHtml}
                    </select>
                    <div class="input-group input-group-sm" style="width:100px;">
                        <button class="btn btn-outline-danger decrement-qty" data-id="${item.id}">-</button>
                        <input type="text" class="form-control text-center bg-white cart-item-qty" value="${item.quantity}" readonly>
                        <button class="btn btn-outline-success increment-qty" data-id="${item.id}">+</button>
                    </div>
                </div>
            </div>`;
        });
    }
    currentTotal = total;
    if (cartTotalElement) cartTotalElement.textContent = formatRupiah(total);
    if (totalBayarEl) totalBayarEl.textContent = formatRupiah(total);
    calculateChange();
}
window.updateCartDisplayWrapper = renderCart;
// ======================================================
// DOCUMENT READY & EVENT DELEGATIONS
// ======================================================
$(document).ready(function () {
    nominalBayarInput = document.getElementById("nominalBayar");
    modalKembalianElement = document.getElementById("modal-kembalian");
    modalTotalBelanjaElement = document.getElementById("modal-total-belanja");
    btnBayar = document.getElementById("btn-bayar");
    fetchProducts({ page: 1, query: "", categoryId: "", isBarcode: false });
    $("#nominalBayar").off("keyup change").on("keyup change", function () {
        cleanAndFormatInput($(this).val());
        calculateChange();
    });
    $(document).off("click", ".quick-cash-btn").on("click", ".quick-cash-btn", function () {
        const amount = safeParseInt($(this).data("amount")) || 0;
        if (nominalBayarInput) nominalBayarInput.value = new Intl.NumberFormat('id-ID').format(amount);
        calculateChange();
    });
    $("#searchInput").off("keyup change").on("keyup change", function () {
        const q = $(this).val();
        const cat = $(".category-filter-btn.active").data("category-id") || "";
        if (q.length >= 2 || q.length === 0) {
            fetchProducts({ page: 1, query: q, categoryId: cat, isBarcode: false });
        }
    });
    $("#category-filter-container").off("click", ".category-filter-btn").on("click", ".category-filter-btn", function () {
        $(".category-filter-btn").removeClass("active");
        $(this).addClass("active");
        $("#searchInput").val("");
        $("#barcodeInput").val("");
        fetchProducts({ page: 1, query: "", categoryId: $(this).data("category-id"), isBarcode: false });
    });
    /* ========================================================================
       Event Handler `.increment-qty`
       Menggunakan logika pembagian nilai konversi real-time agar sinkron dengan batas max Qty Berdasarkan Satuan.
       ======================================================================== */
    $(document).off("click", ".increment-qty").on("click", ".increment-qty", function () {
        const id = safeParseInt($(this).data("id"));
        const item = cart.find(i => parseInt(i.id) === id);
        if (!item) return;
        const stokDasar = safeParseInt(item.stok, 0);
        const selectedUnit = (item.availableUnits || []).find(u => parseInt(u.productUnitId) === parseInt(item.id));
        const conversion = selectedUnit ? safeParseInt(selectedUnit.conversionValue, 1) : 1;
        const maxStokUnitIni = Math.floor(stokDasar / conversion);
        if (item.quantity + 1 > maxStokUnitIni) {
            Swal.fire({
                icon: "warning",
                title: "Stok tidak cukup!",
                text: `Stok tersedia hanya ${stokDasar} Pcs. Untuk satuan ${selectedUnit?.unitName || "ini"}, maksimal pembelian adalah ${maxStokUnitIni} ${selectedUnit?.unitName || ""}.`,
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 5000,
                timerProgressBar: true
            });
            return;
        }
        item.quantity++;
        renderCart();
    });
    $(document).off("click", ".decrement-qty").on("click", ".decrement-qty", function () {
        const id = safeParseInt($(this).data("id"));
        const idx = cart.findIndex(i => parseInt(i.id) === id);
        if (idx === -1) return;
        cart[idx].quantity--;
        if (cart[idx].quantity <= 0) cart.splice(idx, 1);
        renderCart();
    });
    $(document).off("click", "#reset-cart").on("click", "#reset-cart", function () {
        Swal.fire({
            icon: 'warning',
            title: 'Konfirmasi',
            text: 'Kosongkan keranjang?',
            showCancelButton: true,
            position: 'top-end',
            toast: true,
            confirmButtonText: 'Ya',
            cancelButtonText: 'Tidak'
        }).then((result) => {
            if (result.isConfirmed) {
                cart = []; renderCart();
                Swal.fire({
                    icon: 'success',
                    title: 'Keranjang dikosongkan!',
                    position: 'top-end',
                    toast: true,
                    timer: 5000,
                    showConfirmButton: false
                });
            }
        });
    });
    $(document).off("change", ".cart-unit-select").on("change", ".cart-unit-select", function () {
        const $select = $(this);
        const newUnitId = safeParseInt($select.val());
        const oldUnitId = safeParseInt($select.data("cart-id"));
        const idProduk = safeParseInt($select.data("idproduk"));
        let cartIndex = cart.findIndex(i => parseInt(i.productUnitId) === oldUnitId);
        if (cartIndex === -1) cartIndex = cart.findIndex(i => parseInt(i.IdProduk) === idProduk);
        if (cartIndex === -1) {
            Swal.fire({
                icon: 'error',
                title: 'Warning',
                text: 'Item keranjang tidak ditemukan.',
                position: 'top-end',
                toast: true,
                timer: 5000,
                showConfirmButton: false
            });
            return;
        }
        const cartItem = cart[cartIndex];
        let newUnit = (cartItem.availableUnits || []).find(u => parseInt(u.productUnitId) === newUnitId);
        const applyNewUnit = (unitObj) => {
            if (!unitObj) {
                Swal.fire({
                    icon: 'error',
                    title: 'Warning',
                    text: 'Unit tidak tersedia.',
                    position: 'top-end',
                    toast: true,
                    timer: 5000,
                    showConfirmButton: false
                });
                $select.val(cartItem.productUnitId || cartItem.id);
                return;
            }
            const existing = cart.find(i => parseInt(i.productUnitId) === parseInt(unitObj.productUnitId));
            if (existing && existing !== cartItem) {
                const maxStok = Math.floor(safeParseInt(cartItem.stok) / safeParseInt(unitObj.conversionValue, 1));
                let combined = (existing.quantity || 0) + (cartItem.quantity || 0);
                if (combined > maxStok) combined = maxStok;
                existing.quantity = combined;
                cart.splice(cartIndex, 1);
            } else {
                cartItem.productUnitId = unitObj.productUnitId;
                cartItem.id = unitObj.productUnitId;
                cartItem.price = safeParseFloat(unitObj.hargaJual, cartItem.price);
                const maxStok = Math.floor(safeParseInt(cartItem.stok) / safeParseInt(unitObj.conversionValue, 1));
                if (cartItem.quantity > maxStok) cartItem.quantity = maxStok;
            }
            renderCart();
        };
        if (newUnit) {
            applyNewUnit(newUnit);
            return;
        }
        $.ajax({
            url: "/Kasir/GetUnitDetails",
            type: "GET",
            data: { productId: idProduk },
            success: function (unitsFromServer) {
                let units = Array.isArray(unitsFromServer) ? unitsFromServer : [];
                if (!units.length && typeof unitsFromServer === "string") {
                    try { units = JSON.parse(unitsFromServer); } catch (e) { units = []; }
                }
                units = normalizeUnits(units);
                cartItem.availableUnits = units;
                newUnit = units.find(u => parseInt(u.productUnitId) === newUnitId);
                applyNewUnit(newUnit);
            },
            error: function () {
                Swal.fire({
                    icon: 'info',
                    title: 'Warning',
                    text: 'Gagal mengambil detail unit.',
                    position: 'top-end',
                    toast: true,
                    timer: 5000,
                    showConfirmButton: false
                });
                $select.val(cartItem.productUnitId || cartItem.id);
            }
        });
    });
    $(document).off("click", "#btn-bayar").on("click", "#btn-bayar", function () {
        saveTransaction();
    });
    $(document).off("click", ".add-to-cart").on("click", ".add-to-cart", function () {
        addToCart($(this));
    });
    $("#BtnbarcodeInput").on("click", function () {
        $("#barcodeInput").focus();
    });
    if (typeof $.fn.validate === "function") {
        $("#checkoutForm").validate({
            submitHandler: function () { return false; },
            rules: {
                customerNameSelect: { required: true },
                paymentMethodSelect: { required: true },
                nominalBayar: { required: true, numberOrFormatted: true }
            },
            messages: {
                customerNameSelect: { required: "Pilih customer terlebih dahulu." },
                paymentMethodSelect: { required: "Pilih metode pembayaran." },
                nominalBayar: { required: "Masukkan nominal bayar.", numberOrFormatted: "Nominal harus berupa angka." }
            },
            errorClass: "is-invalid", validClass: "is-valid", errorElement: "div",
            errorPlacement: function (error, element) {
                error.addClass("text-danger small mt-1");
                if (element.parent(".input-group").length) { error.insertAfter(element.parent()); } else { error.insertAfter(element); }
            },
            highlight: function (element) { $(element).addClass("is-invalid").removeClass("is-valid"); },
            unhighlight: function (element) { $(element).removeClass("is-invalid").addClass("is-valid"); }
        });
        $.validator.addMethod("numberOrFormatted", function (value) {
            if (!value) return false;
            const n = String(value).replace(/[^\d\-]/g, "");
            return /^\d+$/.test(n);
        }, "Masukkan angka yang valid.");
    }
    $("#checkoutForm").off("submit").on("submit", function (e) {
        e.preventDefault();
        const $form = $(this);
        if ($form.data("validator")) {
            if (!$form.valid()) {
                const $firstErr = $form.find(".is-invalid").first();
                if ($firstErr.length) $firstErr.focus();
                Swal.fire({
                    icon: "warning",
                    title: "Form belum lengkap",
                    text: "Periksa field warning !!!.",
                    position: 'top-end',
                    toast: true
                });
                return;
            }
        }
        if (!cart || cart.length === 0) {
            Swal.fire({
                icon: "warning",
                title: "Keranjang kosong",
                position: 'top-end',
                toast: true,
                text: "Tambahkan produk terlebih dahulu."
            });
            return;
        }
        const total = currentTotal || 0;
        const nominalBayar = parseInt(($("#nominalBayar").val() || "").replace(/[^\d]/g, "")) || 0;
        if (nominalBayar < total) {
            Swal.fire({
                icon: "error",
                title: "Pembayaran kurang",
                position: 'top-end',
                toast: true,
                text: `Nominal bayar ${formatRupiah(nominalBayar)} kurang dari total ${formatRupiah(total)}.`
            });
            return;
        }
        saveTransaction();
    });
});
/* ============================================================================
   Fungsi `saveTransaction`  
   ============================================================================ */
async function saveTransaction() {
    if (isSavingTransaction) {
        console.warn("Request ganda diblokir oleh sistem memori.");
        return;
    }
    const rawIdPelanggan = $("#customerNameSelect").val();
    const rawIdMetode = $("#paymentMethodSelect").val();
    const rawNominal = ($("#nominalBayar").val() || "").toString();
    const idPelanggan = safeParseInt(rawIdPelanggan, 0);
    const idMetodeBayar = safeParseInt(rawIdMetode, 0);
    const nominalBayar = safeParseInt(rawNominal.replace(/[^\d]/g, ""), 0);
    const total = currentTotal || 0;
    if (!cart || cart.length === 0) {
        Swal.fire({
            icon: "warning",
            title: "Keranjang masih kosong!",
            position: 'top-end',
            toast: true,
            text: "Tambahkan produk terlebih dahulu."
        });
        return;
    }
    if (!idPelanggan || idPelanggan <= 0) {
        Swal.fire({
            icon: "warning",
            title: "Warning",
            position: 'top-end',
            toast: true,
            text: "Pilih customer terlebih dahulu!"
        });
        return;
    }
    if (!idMetodeBayar || idMetodeBayar <= 0) {
        Swal.fire({
            icon: "warning",
            title: "Warning",
            position: 'top-end',
            toast: true,
            text: "Pilih metode pembayaran!"
        });
        return;
    }
    if (nominalBayar < total) {
        Swal.fire({
            icon: "error",
            title: "Pembayaran kurang",
            position: 'top-end',
            toast: true,
            text: `Nominal bayar ${formatRupiah(nominalBayar)} kurang dari total ${formatRupiah(total)}.`
        });
        return;
    }
    for (const item of cart) {
        if (!safeParseInt(item.id)) {
            Swal.fire({
                icon: "error",
                title: "Warning",
                position: 'top-end',
                toast: true,
                text: `Gagal! Unit Produk (${item.name}) belum terdefinisi!`
            });
            return;
        }
    }
    isSavingTransaction = true;
    const btnSubmit = $("#btn-bayar");
    btnSubmit.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Menyimpan...');
    const items = cart.map(item => {
        // 1. CARI UNIT YANG SEDANG DIPILIH BERDASARKAN ID UNIT YANG AKTIF DI ITEM KERANJANG (item.productUnitId atau item.id)
        const targetUnitId = safeParseInt(item.productUnitId || item.id);
        const selectedUnit = (item.availableUnits || []).find(u => safeParseInt(u.productUnitId) === targetUnitId);
        let conversionVal = 1; // Default paling dasar (Pcs)
        if (selectedUnit) {
            // Ambil dari conversionValue hasil bentukan dari fungsi normalizeUnits()
            conversionVal = safeParseInt(selectedUnit.conversionValue) ||
                safeParseInt(selectedUnit.ConversionValue) ||
                safeParseInt(selectedUnit.Conversion) || 1;
        } else {
            // BAN SEREP: Jika objek unit di memori corrupt/hilang, coba paksa intip dari element select HTML di layar UI
            const unitSelectEl = $(`.cart-unit-select[data-idproduk="${item.IdProduk}"]`).filter(function () {
                return safeParseInt($(this).val()) === targetUnitId;
            });
            if (unitSelectEl.length > 0) {
                const selectedOption = unitSelectEl.find('option:selected');
                conversionVal = safeParseInt(selectedOption.attr('data-conversion')) ||
                    safeParseInt(selectedOption.data('conversion')) || 1;
            }
        }
        return {
            IdProduk: safeParseInt(item.IdProduk),
            ProductUnitId: targetUnitId,
            Qty: safeParseInt(item.quantity),
            HargaJual: safeParseFloat(item.price),
            Conversion: conversionVal // Mengunci nilai konversi asli dari DB (misal Dus = 40)
        };
    });
    //Payload item mengambil nilai konversi secara spesifik dari properti item unit terpilih
    const payload = {
        IdPelanggan: idPelanggan,
        IdMetodeBayar: idMetodeBayar,
        NominalBayar: nominalBayar,
        TotalBelanja: total,
        Items: items
    };    
    const tokenEl = document.querySelector("input[name='__RequestVerificationToken']");
    const token = tokenEl ? tokenEl.value : null;
    if (!token) {
        Swal.fire({
            icon: 'error',
            title: 'Warning',
            text: 'CSRF token tidak ditemukan. Reload halaman!',
            position: 'top-end',
            toast: true,
            timer: 5000,
            showConfirmButton: false
        });
        isSavingTransaction = false;
        btnSubmit.prop('disabled', false).text("Konfirmasi Bayar");
        return;
    }
    try {
        const res = await fetch("/Kasir/Save", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            },
            body: JSON.stringify(payload)
        });
        const text = await res.text();
        let json = null;
        try { json = JSON.parse(text); } catch (e) { json = null; }
        if (!res.ok) {
            const msg = (json && json.message) ? json.message : text || `HTTP ${res.status}`;
            Swal.fire({
                icon: 'error',
                title: 'Gagal',
                text: msg,
                position: 'top-end',
                toast: true,
                timer: 5000,
                showConfirmButton: false
            });
            return;
        }
        if (json && json.success === false) {
            Swal.fire({
                icon: 'error',
                title: 'Gagal',
                text: json.message,
                position: 'top-end',
                toast: true,
                timer: 5000,
                showConfirmButton: false
            });
            return;
        }
        const data = json || {};
        Swal.fire({
            icon: 'success',
            title: 'Berhasil',
            text: data.message || "Transaksi tersimpan.",
            position: 'top-end',
            toast: true,
            timer: 5000,
            showConfirmButton: false
        });
        $("#checkoutModal").modal("hide");
        cart = [];
        renderCart();
        $("#customerNameSelect").val("");
        $("#paymentMethodSelect").val("");
        $("#nominalBayar").val("");
        if (data.idTransaksi) {
            openReceipt(data.idTransaksi);
        }
    } catch (err) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: `Terjadi kesalahan koneksi: ${err.message}`,
            position: 'top-end',
            toast: true,
            timer: 5000,
            showConfirmButton: false,
            timerProgressBar: true
        });
    } finally {
        isSavingTransaction = false;
        btnSubmit.prop('disabled', false).text("Konfirmasi Bayar");
    }
}
// ======================================================
// PRINT & UTILITIES
// ======================================================
function openReceipt(idTransaksi) {
    if (!idTransaksi) return;
    // Ambil element iframe yang sudah ada di Index.cshtml
    const iframe = document.getElementById('receiptFrame');
    if (!iframe) {
        console.error("Elemen receiptFrame tidak ditemukan di halaman.");
        return;
    }
    // Set URL target ke controller cetak struk
    iframe.src = `/Kasir/PrintStruk?id=${idTransaksi}`;
    // Jalankan perintah cetak begitu konten di dalam iframe selesai dimuat
    iframe.onload = function () {
        try {
            iframe.contentWindow.focus();
            iframe.contentWindow.print();
        } catch (e) {
            console.error("Gagal memicu print pada iframe: ", e);
            // Backup plan jika iframe diblokir browser: buka jendela popup kecil sementara
            const printWindow = window.open(`/Kasir/PrintStruk?id=${idTransaksi}`, '_blank', 'width=350,height=600');
            if (printWindow) {
                printWindow.focus();
            }
        }
    };
}
function cetakStruk(idTransaksi) {
    // Disamakan agar tombol manual cetak ulang memanggil fungsi yang sama
    openReceipt(idTransaksi);
}
window._cart_debug = function () { return JSON.parse(JSON.stringify(cart)); };
window._cart_clear = function () { cart = []; renderCart(); };
window._add_raw = function (raw) {
    const units = normalizeUnits(raw.units || raw.availableUnits || []);
    const targetUnitId = raw.defaultUnitId || (units[0] && units[0].productUnitId);
    addItemToCart(raw.idProduk || raw.IdProduk || raw.Id, raw.namaProduk || raw.name, (units.find(u => u.productUnitId === targetUnitId) || units[0] || { hargaJual: 0 }).hargaJual, targetUnitId, raw.image || raw.gambar || "", raw.stok || 0, units);
};