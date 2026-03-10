/* ================================================================
   SABORES EXÓTICOS · Heladería Artesanal
   app.js — Versión 2.0
   Autores: Isaac Núñez · Nicolas Betancourt · Daniel Rojas · 2026
================================================================ */

/* ================================================================
   CONFIG — URL base de la API (ajusta al desplegar)
================================================================ */
const API_BASE = 'http://localhost:5230/api';

/* ================================================================
   MÓDULO 1 — DATOS (fallback local mientras API no responda)
================================================================ */
const products = [
    { id: 1, name: "Lúcuma", region: "Amazonas", color: "#F5B731", desc: "Fruta peruana de textura cremosa y sabor caramelizado. Suministrada por comunidades del Putumayo.", price: 4500, prepTime: 3 },
    { id: 2, name: "Maracuyá", region: "Caribe", color: "#FF7043", desc: "Tropical y refrescante, con equilibrio perfecto entre dulce y ácido. Origen Golfo de Morrosquillo.", price: 4200, prepTime: 3 },
    { id: 3, name: "Guanábana", region: "Chocó", color: "#26C281", desc: "Cremosidad excepcional con notas frescas. Comunidades afrocolombianas del Chocó biogeográfico.", price: 4800, prepTime: 4 },
    { id: 4, name: "Coco Tostado", region: "Caribe", color: "#E8A87C", desc: "Coco caramelizado con trozos crujientes. Costas atlánticas, cultivos orgánicos de pequeños productores.", price: 5000, prepTime: 4 },
    { id: 5, name: "Matcha Orgánico", region: "Sierra Nevada", color: "#4CAF73", desc: "Té verde ceremonial con sabor terroso. Cultivado en huertos experimentales de la Sierra Nevada.", price: 5500, prepTime: 3 },
    { id: 6, name: "Lavanda", region: "Llanos", color: "#9B7EDE", desc: "Notas florales suaves y aromáticas. Lavanda silvestre recolectada por comunidades llaneras.", price: 5200, prepTime: 3 },
    { id: 7, name: "Pistacho Siciliano", region: "Sierra Nevada", color: "#6BBF6E", desc: "Pistachos importados molidos artesanalmente. Uno de nuestros sabores emblema con textura única.", price: 6000, prepTime: 5 },
    { id: 8, name: "Rosas y Frambuesa", region: "Llanos", color: "#F06292", desc: "Fusión floral y frutal. Pétalos de rosa cultivados en el piedemonte llanero por mujeres campesinas.", price: 5800, prepTime: 4 }
];

const inventory = [
    { id: 1, name: "Pulpa de Lúcuma", origin: "Putumayo", stock: 18, unit: "kg", min: 5 },
    { id: 2, name: "Pulpa de Maracuyá", origin: "Golfo Morrosquillo", stock: 22, unit: "kg", min: 8 },
    { id: 3, name: "Pulpa de Guanábana", origin: "Chocó", stock: 3, unit: "kg", min: 5 },
    { id: 4, name: "Coco Rallado", origin: "Caribe", stock: 12, unit: "kg", min: 4 },
    { id: 5, name: "Matcha Premium", origin: "Sierra Nevada", stock: 2, unit: "kg", min: 3 },
    { id: 6, name: "Esencia Lavanda", origin: "Llanos", stock: 6, unit: "L", min: 2 },
    { id: 7, name: "Pistacho Importado", origin: "Sicilia / Imp.", stock: 9, unit: "kg", min: 3 },
    { id: 8, name: "Mermelada Frambuesa", origin: "Llanos", stock: 7, unit: "kg", min: 3 },
    { id: 9, name: "Crema de Leche", origin: "Local", stock: 40, unit: "L", min: 10 },
    { id: 10, name: "Azúcar Orgánica", origin: "Cauca", stock: 35, unit: "kg", min: 10 }
];

const suppliers = [
    { id: 1, name: "Cabildo Indígena Uitoto", region: "Amazonas", products: "Pulpa lúcuma, cacao artesanal", cert: "Comercio Justo", active: true },
    { id: 2, name: "Coop. Afro Chocó", region: "Chocó", products: "Guanábana, borojo", cert: "Orgánico", active: true },
    { id: 3, name: "Asoc. Pesc. Morrosquillo", region: "Caribe", products: "Maracuyá, coco", cert: "Agroecológico", active: true },
    { id: 4, name: "Mujeres Llaneras", region: "Llanos Orientales", products: "Lavanda, rosas, frambuesa", cert: "Comercio Justo", active: true },
    { id: 5, name: "Red Sierra Nevada", region: "Sierra Nevada", products: "Matcha experimental, hierbas", cert: "Orgánico", active: false }
];

/* ================================================================
   MÓDULO 2 — ESTADO
================================================================ */
let order = [];
let ticketCounter = 1;
let kitchenOrders = [];
let allSales = [];
let salesCounter = 1;
let alertTimer = null;
let currentFilter = 'all';

/* ================================================================
   MÓDULO 3 — CURSOR (desactivado — se usa el cursor normal del sistema)
================================================================ */
// cursor personalizado desactivado

/* ================================================================
   MÓDULO 4 — RELOJ EN NAV
================================================================ */
function updateNavTime() {
    const el = document.getElementById('navTime');
    if (!el) return;
    const now = new Date();
    el.textContent = now.toLocaleTimeString('es-CO', { hour: '2-digit', minute: '2-digit' });
}
updateNavTime();
setInterval(updateNavTime, 30000);

/* ================================================================
   MÓDULO 5 — NAVEGACIÓN
================================================================ */
function showPage(pageId, btn) {
    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
    document.querySelectorAll('.nav-links button').forEach(b => b.classList.remove('active'));
    document.getElementById(pageId).classList.add('active');
    if (btn) btn.classList.add('active');

    if (pageId === 'cocinaPage') renderKitchen();
    if (pageId === 'erpPage') refreshErp();
}

function showErpPanel(panelId, btn) {
    document.querySelectorAll('.erp-panel').forEach(p => p.classList.remove('active'));
    document.querySelectorAll('.erp-sidebar-nav button').forEach(b => b.classList.remove('active'));
    document.getElementById(panelId).classList.add('active');
    if (btn) btn.classList.add('active');
}

/* ================================================================
   MÓDULO 6 — CATÁLOGO Y FILTROS
================================================================ */
function filterProducts(region, btn) {
    currentFilter = region;
    document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
    if (btn) btn.classList.add('active');
    renderProducts();
}

function renderProducts() {
    const grid = document.getElementById('productsGrid');
    const list = currentFilter === 'all'
        ? products
        : products.filter(p => p.region === currentFilter);

    grid.innerHTML = list.map((p, i) => `
    <div class="prod-card" style="animation-delay:${i * 0.07}s">
      <div class="prod-card-stripe" style="background:${p.color}"></div>
      <div class="prod-card-inner">
        <div class="prod-top">
          <span class="prod-region" style="color:${p.color}">${p.region}</span>
          <span class="prod-prep">${p.prepTime} min</span>
        </div>
        <h3>${p.name}</h3>
        <p class="prod-desc">${p.desc}</p>
        <div class="prod-footer">
          <span class="prod-price">$${p.price.toLocaleString('es-CO')}</span>
        </div>
        <button class="add-btn" onclick="addToOrder(${p.id})" id="addBtn${p.id}">
          <span class="add-btn-icon">+</span>
          <span>Agregar al pedido</span>
        </button>
      </div>
    </div>
  `).join('');
}

/* ================================================================
   MÓDULO 7 — CARRITO
================================================================ */
function addToOrder(pid) {
    const product = products.find(p => p.id === pid);
    const existing = order.find(i => i.id === pid);

    if (existing) {
        existing.quantity++;
    } else {
        order.push({ ...product, quantity: 1 });
    }

    // Feedback visual en el botón
    const btn = document.getElementById('addBtn' + pid);
    if (btn) {
        btn.classList.add('added');
        const icon = btn.querySelector('.add-btn-icon');
        const text = btn.querySelector('span:last-child');
        if (icon) icon.textContent = '✓';
        if (text) text.textContent = 'Agregado';
        setTimeout(() => {
            btn.classList.remove('added');
            if (icon) icon.textContent = '+';
            if (text) text.textContent = 'Agregar al pedido';
        }, 1200);
    }

    updateCart();
}

function removeFromOrder(idx) {
    order.splice(idx, 1);
    updateCart();
}

function clearCart() {
    order = [];
    updateCart();
}

function updateCart() {
    const itemsDiv = document.getElementById('orderItems');
    const totalQty = order.reduce((s, i) => s + i.quantity, 0);
    const countEl = document.getElementById('orderCount');
    const clearBtn = document.getElementById('clearCartBtn');
    const cartBadge = document.getElementById('cartBadge');

    // Actualizar contador
    if (countEl) countEl.textContent = totalQty;
    if (cartBadge) {
        cartBadge.textContent = totalQty;
        cartBadge.style.display = totalQty > 0 ? 'inline-flex' : 'none';
    }
    if (clearBtn) clearBtn.style.display = order.length ? 'block' : 'none';

    if (!order.length) {
        itemsDiv.innerHTML = `
      <div class="cart-empty">
        <div class="cart-empty-icon">◯</div>
        <p>Agrega helados para comenzar</p>
      </div>`;
        document.getElementById('totalPrice').textContent = '0';
        document.getElementById('waitTime').textContent = '0';
        document.getElementById('checkoutBtn').disabled = true;
        return;
    }

    itemsDiv.innerHTML = order.map((item, i) => `
    <div class="o-item">
      <div class="o-item-color" style="background:${item.color}"></div>
      <div class="o-item-info">
        <div class="o-item-name">${item.name}</div>
        <div class="o-item-qty">× ${item.quantity}</div>
      </div>
      <span class="o-item-price">$${(item.price * item.quantity).toLocaleString('es-CO')}</span>
      <button class="o-remove" onclick="removeFromOrder(${i})" title="Eliminar">✕</button>
    </div>
  `).join('');

    const total = order.reduce((s, i) => s + i.price * i.quantity, 0);
    const maxTime = Math.max(...order.map(i => i.prepTime));
    const waitTime = maxTime + Math.floor(totalQty / 2);

    document.getElementById('totalPrice').textContent = total.toLocaleString('es-CO');
    document.getElementById('waitTime').textContent = waitTime;
    document.getElementById('checkoutBtn').disabled = false;

    // Actualizar stat en hero
    const heroStat = document.getElementById('heroStatOrders');
    if (heroStat) heroStat.textContent = allSales.length;
}

/* ================================================================
   MÓDULO 8 — CHECKOUT Y TICKET
================================================================ */
document.addEventListener('DOMContentLoaded', () => {
    const checkoutBtn = document.getElementById('checkoutBtn');
    if (!checkoutBtn) return;

    checkoutBtn.addEventListener('click', async () => {
        const total = order.reduce((s, i) => s + i.price * i.quantity, 0);
        const totalQty = order.reduce((s, i) => s + i.quantity, 0);
        const maxTime = Math.max(...order.map(i => i.prepTime));
        const wait = maxTime + Math.floor(totalQty / 2);
        const turnStr = String(ticketCounter).padStart(3, '0');
        const name = document.getElementById('customerName')?.value.trim() || 'Cliente';

        // Intentar enviar a API
        try {
            const payload = {
                customerName: name,
                customerEmail: null,
                notes: null,
                items: order.map(i => ({ productId: i.id, quantity: i.quantity }))
            };
            await fetch(`${API_BASE}/orders`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });
        } catch (_) { /* API no disponible, continúa en modo offline */ }

        // Mostrar ticket
        document.getElementById('ticketNum').textContent = turnStr;
        document.getElementById('ticketTotal').textContent = total.toLocaleString('es-CO');
        document.getElementById('ticketTime').textContent = wait;
        document.getElementById('ticketModal').classList.add('open');

        // Enviar a cocina
        kitchenOrders.push({
            id: ticketCounter,
            turnStr,
            customerName: name,
            items: [...order],
            total,
            wait,
            status: 'pending',
            createdAt: Date.now()
        });

        // Registrar venta
        allSales.unshift({
            n: salesCounter++,
            turn: turnStr,
            items: order.map(i => `${i.name} ×${i.quantity}`).join(', '),
            total,
            wait,
            date: new Date().toLocaleString('es-CO'),
            status: 'Confirmado'
        });

        updateKitchenBadge();
        ticketCounter++;
        order = [];
        if (document.getElementById('customerName'))
            document.getElementById('customerName').value = '';
        updateCart();
    });
});

function closeTicket() {
    document.getElementById('ticketModal').classList.remove('open');
}

function updateKitchenBadge() {
    const pending = kitchenOrders.filter(o => o.status === 'pending').length;
    const badge = document.getElementById('kitchenBadge');
    if (badge) badge.textContent = pending;
}

/* ================================================================
   MÓDULO 9 — COCINA (Kanban)
================================================================ */
function renderKitchen() {
    const pending = kitchenOrders.filter(o => o.status === 'pending');
    const progress = kitchenOrders.filter(o => o.status === 'progress');
    const done = kitchenOrders.filter(o => o.status === 'done');

    document.getElementById('kPending').textContent = pending.length;
    document.getElementById('kProgress').textContent = progress.length;
    document.getElementById('kDone').textContent = done.length;

    // Lane counts
    const lP = document.getElementById('laneCountPending');
    const lPr = document.getElementById('laneCountProgress');
    const lD = document.getElementById('laneCountDone');
    if (lP) lP.textContent = pending.length;
    if (lPr) lPr.textContent = progress.length;
    if (lD) lD.textContent = done.length;

    document.getElementById('colPending').innerHTML = buildKitchenCards(pending, 'pending');
    document.getElementById('colProgress').innerHTML = buildKitchenCards(progress, 'progress');
    document.getElementById('colDone').innerHTML = buildKitchenCards(done, 'done');
}

function buildKitchenCards(list, status) {
    if (!list.length) return '<div class="k-empty">Sin pedidos</div>';

    return list.map(o => {
        const elapsed = Math.floor((Date.now() - o.createdAt) / 60000);
        const isUrgent = elapsed > o.wait;
        const itemsHtml = o.items.map(i =>
            `<div class="k-item-row">
        <span>${i.name}</span>
        <span>×${i.quantity}</span>
      </div>`
        ).join('');

        let actions = '';
        if (status === 'pending') {
            actions = `
        <button class="k-btn k-btn-start" onclick="moveOrder(${o.id},'progress')">▶ Iniciar</button>
        <button class="k-btn k-btn-del" onclick="moveOrder(${o.id},'cancel')" title="Cancelar">✕</button>`;
        } else if (status === 'progress') {
            actions = `
        <button class="k-btn k-btn-done" onclick="moveOrder(${o.id},'done')">✓ Listo</button>
        <button class="k-btn k-btn-del" onclick="moveOrder(${o.id},'cancel')" title="Cancelar">✕</button>`;
        } else {
            actions = `
        <button class="k-btn k-btn-del" style="flex:1" onclick="moveOrder(${o.id},'cancel')">Archivar</button>`;
        }

        return `
      <div class="k-order-card">
        <div class="k-card-top">
          <span class="k-turn">#${o.turnStr}</span>
          <span class="k-elapsed${isUrgent ? ' urgent' : ''}">${elapsed} min</span>
        </div>
        ${o.customerName && o.customerName !== 'Cliente' ? `<div style="font-size:.72rem;color:var(--muted);padding-bottom:.5rem">${o.customerName}</div>` : ''}
        <div class="k-items">${itemsHtml}</div>
        <div class="k-actions">${actions}</div>
      </div>`;
    }).join('');
}

function moveOrder(id, newStatus) {
    if (newStatus === 'cancel') {
        kitchenOrders = kitchenOrders.filter(o => o.id !== id);
    } else {
        const o = kitchenOrders.find(o => o.id === id);
        if (o) o.status = newStatus;
    }
    updateKitchenBadge();
    renderKitchen();

    const msgs = { progress: '🔥 Pedido en preparación', done: '✓ Pedido listo para entrega', cancel: 'Pedido cancelado' };
    showAlert(msgs[newStatus] || 'Estado actualizado');
}

/* ================================================================
   MÓDULO 10 — ERP: ACTUALIZACIÓN GENERAL
================================================================ */
function refreshErp() {
    renderInventory();
    renderSuppliers();
    renderSalesTable();
    renderInvoices();
    renderDashboard();
}

/* ================================================================
   MÓDULO 11 — ERP: DASHBOARD
================================================================ */
function renderDashboard() {
    const today = new Date().toLocaleDateString('es-CO', {
        weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
    });
    const dashDate = document.getElementById('dashDate');
    if (dashDate) dashDate.textContent = today;

    // KPIs
    const totalDay = allSales.reduce((s, x) => s + x.total, 0);
    const kpiSalesDay = document.getElementById('kpiSalesDay');
    const kpiSalesSub = document.getElementById('kpiSalesSub');
    if (kpiSalesDay) kpiSalesDay.textContent = '$' + totalDay.toLocaleString('es-CO');
    if (kpiSalesSub) kpiSalesSub.textContent = allSales.length + ' pedidos';

    const freq = buildProductFrequency();
    const best = Object.entries(freq).sort((a, b) => b[1] - a[1])[0];
    const kpiBest = document.getElementById('kpiBest');
    if (kpiBest) kpiBest.textContent = best ? best[0] : '—';

    const avgTime = allSales.length
        ? Math.round(allSales.reduce((s, x) => s + x.wait, 0) / allSales.length) : 0;
    const kpiAvgTime = document.getElementById('kpiAvgTime');
    if (kpiAvgTime) kpiAvgTime.textContent = avgTime + ' min';

    const rptClients = document.getElementById('rptClients');
    if (rptClients) rptClients.textContent = allSales.length;

    renderRecentOrdersTable();
    renderSalesBars(freq);

    // Hero stat
    const heroStat = document.getElementById('heroStatOrders');
    if (heroStat) heroStat.textContent = allSales.length;
}

function buildProductFrequency() {
    const freq = {};
    allSales.forEach(s => {
        s.items.split(', ').forEach(it => {
            const name = it.split(' ×')[0];
            freq[name] = (freq[name] || 0) + 1;
        });
    });
    return freq;
}

function renderRecentOrdersTable() {
    const tbody = document.getElementById('recentOrdersBody');
    if (!tbody) return;
    if (!allSales.length) {
        tbody.innerHTML = '<tr><td colspan="5" class="table-empty">Sin pedidos aún</td></tr>';
        return;
    }
    tbody.innerHTML = allSales.slice(0, 10).map(s => `
    <tr>
      <td><strong style="color:var(--terra)">#${s.turn}</strong></td>
      <td style="color:var(--muted);font-size:.78rem">${s.items.substring(0, 42)}${s.items.length > 42 ? '…' : ''}</td>
      <td><strong>$${s.total.toLocaleString('es-CO')}</strong></td>
      <td style="color:var(--muted)">${s.wait} min</td>
      <td><span class="pill pill-green">${s.status}</span></td>
    </tr>
  `).join('');
}

function renderSalesBars(freq) {
    const barsDiv = document.getElementById('salesBars');
    if (!barsDiv) return;
    const maxFreq = Math.max(1, ...Object.values(freq));

    if (!Object.keys(freq).length) {
        barsDiv.innerHTML = '<div class="bars-empty">Sin datos de ventas</div>';
        return;
    }

    barsDiv.innerHTML = products.map(p => {
        const count = freq[p.name] || 0;
        const height = Math.round((count / maxFreq) * 140) + 4;
        return `
      <div class="bar-col">
        <div class="bar-val">${count}</div>
        <div class="bar-fill" style="height:${height}px; --bar-color:${p.color}"></div>
        <div class="bar-lbl">${p.name.split(' ')[0]}</div>
      </div>`;
    }).join('');
}

/* ================================================================
   MÓDULO 12 — ERP: INVENTARIO
================================================================ */
function renderInventory() {
    const tbody = document.getElementById('inventoryBody');
    if (!tbody) return;
    tbody.innerHTML = inventory.map(item => {
        const isLow = item.stock <= item.min;
        const isWarning = item.stock <= item.min * 1.5 && !isLow;
        const pillClass = isLow ? 'pill-red' : isWarning ? 'pill-yellow' : 'pill-green';
        const pillLabel = isLow ? 'Stock Bajo' : isWarning ? 'Revisar' : 'OK';
        return `
      <tr>
        <td><strong>${item.name}</strong></td>
        <td style="color:var(--muted)">${item.origin}</td>
        <td><strong style="color:${isLow ? 'var(--terra)' : 'inherit'}">${item.stock}</strong></td>
        <td style="color:var(--muted)">${item.unit}</td>
        <td style="color:var(--muted)">${item.min}</td>
        <td><span class="pill ${pillClass}">${pillLabel}</span></td>
        <td>
          <button class="erp-btn erp-btn-sm erp-btn-outline" onclick="editStock(${item.id})">Editar</button>
        </td>
      </tr>`;
    }).join('');
}

function editStock(id) {
    const item = inventory.find(i => i.id === id);
    if (!item) return;
    const newVal = prompt(`Stock de "${item.name}" (${item.stock} ${item.unit}):`, item.stock);
    if (newVal !== null && !isNaN(parseFloat(newVal))) {
        item.stock = parseFloat(newVal);
        renderInventory();
        showAlert(`Stock de "${item.name}" actualizado`);
    }
}

function saveInventoryItem() {
    const name = document.getElementById('invName').value.trim();
    const origin = document.getElementById('invOrigin').value.trim();
    const stock = parseFloat(document.getElementById('invStock').value) || 0;
    const unit = document.getElementById('invUnit').value;
    const min = parseFloat(document.getElementById('invMin').value) || 0;

    if (!name) { showAlert('⚠ Ingresa el nombre del insumo'); return; }

    inventory.push({ id: inventory.length + 1, name, origin, stock, unit, min });
    closeModal('modalInventory');
    renderInventory();
    showAlert('Insumo registrado correctamente');
    ['invName', 'invOrigin', 'invStock', 'invMin'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = '';
    });
}

/* ================================================================
   MÓDULO 13 — ERP: PROVEEDORES
================================================================ */
function renderSuppliers() {
    const tbody = document.getElementById('suppliersBody');
    if (!tbody) return;
    tbody.innerHTML = suppliers.map(s => `
    <tr>
      <td><strong>${s.name}</strong></td>
      <td style="color:var(--muted)">${s.region}</td>
      <td style="font-size:.78rem;color:var(--muted)">${s.products}</td>
      <td><span class="pill pill-green">${s.cert}</span></td>
      <td><span class="pill ${s.active ? 'pill-green' : 'pill-gray'}">${s.active ? 'Activo' : 'Inactivo'}</span></td>
      <td>
        <button class="erp-btn erp-btn-sm ${s.active ? 'erp-btn-outline' : 'erp-btn-green'}" onclick="toggleSupplier(${s.id})">
          ${s.active ? 'Desactivar' : 'Activar'}
        </button>
      </td>
    </tr>
  `).join('');
}

function saveSupplier() {
    const name = document.getElementById('supName').value.trim();
    const region = document.getElementById('supRegion').value;
    const cert = document.getElementById('supCert').value;
    const prods = document.getElementById('supProducts').value.trim();

    if (!name) { showAlert('⚠ Ingresa el nombre del proveedor'); return; }

    suppliers.push({ id: suppliers.length + 1, name, region, products: prods, cert, active: true });
    closeModal('modalSupplier');
    renderSuppliers();
    showAlert('Proveedor registrado correctamente');
    ['supName', 'supProducts'].forEach(id => {
        const el = document.getElementById(id);
        if (el) el.value = '';
    });
}

function toggleSupplier(id) {
    const s = suppliers.find(x => x.id === id);
    if (s) { s.active = !s.active; renderSuppliers(); }
}

/* ================================================================
   MÓDULO 14 — ERP: VENTAS Y FACTURAS
================================================================ */
function renderSalesTable() {
    const tbody = document.getElementById('salesBody');
    if (!tbody) return;
    if (!allSales.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="table-empty">Sin ventas registradas</td></tr>';
        return;
    }
    tbody.innerHTML = allSales.map(s => `
    <tr>
      <td style="color:var(--muted)">#${s.n}</td>
      <td><strong style="color:var(--terra)">#${s.turn}</strong></td>
      <td style="font-size:.78rem;color:var(--muted)">${s.items.substring(0, 45)}${s.items.length > 45 ? '…' : ''}</td>
      <td><strong>$${s.total.toLocaleString('es-CO')}</strong></td>
      <td style="color:var(--muted)">${s.wait} min</td>
      <td style="font-size:.75rem;color:var(--muted2)">${s.date}</td>
      <td><span class="pill pill-green">${s.status}</span></td>
    </tr>
  `).join('');
}

function renderInvoices() {
    const tbody = document.getElementById('invoicesBody');
    if (!tbody) return;
    if (!allSales.length) {
        tbody.innerHTML = '<tr><td colspan="7" class="table-empty">Sin facturas generadas</td></tr>';
        return;
    }
    tbody.innerHTML = allSales.map((s, i) => {
        const iva = Math.round(s.total * 0.19);
        const subtotal = s.total - iva;
        const invoiceN = String(allSales.length - i).padStart(4, '0');
        return `
      <tr>
        <td><strong>FV-${invoiceN}</strong></td>
        <td style="color:var(--terra)">#${s.turn}</td>
        <td style="font-size:.75rem;color:var(--muted)">${s.items.substring(0, 35)}${s.items.length > 35 ? '…' : ''}</td>
        <td>$${subtotal.toLocaleString('es-CO')}</td>
        <td style="color:var(--muted)">$${iva.toLocaleString('es-CO')}</td>
        <td><strong>$${s.total.toLocaleString('es-CO')}</strong></td>
        <td style="font-size:.75rem;color:var(--muted2)">${s.date}</td>
      </tr>`;
    }).join('');
}

function exportCSV() {
    if (!allSales.length) { showAlert('⚠ No hay ventas para exportar'); return; }
    const headers = ['#', 'Turno', 'Productos', 'Total (COP)', 'Tiempo (min)', 'Fecha', 'Estado'];
    const rows = allSales.map(s =>
        [s.n, s.turn, `"${s.items}"`, s.total, s.wait, s.date, s.status].join(',')
    );
    const csv = [headers.join(','), ...rows].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `ventas_sabores_exoticos_${new Date().toISOString().split('T')[0]}.csv`;
    a.click();
    URL.revokeObjectURL(url);
    showAlert('Archivo CSV descargado');
}

/* ================================================================
   MÓDULO 15 — MODALES
================================================================ */
function openModal(id) { document.getElementById(id).classList.add('open'); }
function closeModal(id) { document.getElementById(id).classList.remove('open'); }
function closeTicket() { document.getElementById('ticketModal').classList.remove('open'); }

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.erp-modal-overlay').forEach(overlay => {
        overlay.addEventListener('click', e => {
            if (e.target === overlay) overlay.classList.remove('open');
        });
    });
    document.getElementById('ticketModal')?.addEventListener('click', e => {
        if (e.target === document.getElementById('ticketModal'))
            document.getElementById('ticketModal').classList.remove('open');
    });
});

/* ================================================================
   MÓDULO 16 — TOAST
================================================================ */
function showAlert(msg) {
    const el = document.getElementById('alertBanner');
    const msgEl = document.getElementById('alertMsg');
    if (!el || !msgEl) return;
    msgEl.textContent = msg;
    el.classList.add('show');
    clearTimeout(alertTimer);
    alertTimer = setTimeout(() => el.classList.remove('show'), 3200);
}

/* ================================================================
   MÓDULO 17 — INICIALIZACIÓN
================================================================ */
(function init() {
    renderProducts();
    renderInventory();
    renderSuppliers();

    const dashDate = document.getElementById('dashDate');
    if (dashDate) {
        dashDate.textContent = new Date().toLocaleDateString('es-CO', {
            weekday: 'long', year: 'numeric', month: 'long', day: 'numeric'
        });
    }
})();