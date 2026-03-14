/**
 * Expense Control App - SPA
 * Estructura: estado, autenticación, layout, API, vistas y utilidades.
 */

// ========== CONFIGURACIÓN Y ESTADO ==========
const API_BASE_URL = '/api';
const TOKEN_KEY = 'authToken';
const USER_KEY = 'currentUser';

let currentUser = null;
let authToken = null;
let currentPage = 'dashboard';
let categories = [];
let monthlyTrendsChart = null;
let expenseCategoriesChart = null;

// ========== AUTENTICACIÓN ==========

function isAuthenticated() {
    return !!(authToken && currentUser);
}

function getCurrentUser() {
    return currentUser;
}

/**
 * Valida el token con el backend. Si falla, limpia sesión.
 * @returns {Promise<boolean>} true si hay sesión válida
 */
async function restoreSession() {
    const token = localStorage.getItem(TOKEN_KEY);
    if (!token) return false;
    try {
        const response = await fetch(`${API_BASE_URL}/auth/me`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        if (!response.ok) {
            if (response.status === 401 || response.status === 403) {
                clearSession();
                return false;
            }
            return false;
        }
        const user = await response.json();
        authToken = token;
        currentUser = user;
        localStorage.setItem(USER_KEY, JSON.stringify(user));
        return true;
    } catch (_e) {
        clearSession();
        return false;
    }
}

function clearSession() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    authToken = null;
    currentUser = null;
}

function handleUnauthorized() {
    clearSession();
    renderAuthView();
    showToast('Session expired. Please sign in again.', 'warning');
}

// ========== API CON AUTH ==========

async function fetchWithAuth(url, options = {}) {
    const headers = { ...options.headers };
    if (authToken) headers['Authorization'] = `Bearer ${authToken}`;
    const response = await fetch(url, { ...options, headers });
    if (response.status === 401 || response.status === 403) {
        handleUnauthorized();
        throw new Error('Unauthorized');
    }
    return response;
}

// ========== LAYOUT Y RENDERIZADO ==========

function hideLoader() {
    const el = document.getElementById('appLoader');
    if (el) el.style.display = 'none';
}

function renderAuthView() {
    document.getElementById('appLayout').style.display = 'none';
    document.getElementById('appLayout').setAttribute('aria-hidden', 'true');
    document.getElementById('authLayout').style.display = 'flex';
    document.getElementById('authLayout').setAttribute('aria-hidden', 'false');
    document.getElementById('authLoginCard').style.display = 'block';
    document.getElementById('authRegisterCard').style.display = 'none';
    hideLoginError();
    hideRegisterError();
    const emailInput = document.getElementById('loginEmail');
    if (emailInput) setTimeout(() => emailInput.focus(), 100);
}

function renderAppView() {
    document.getElementById('authLayout').style.display = 'none';
    document.getElementById('authLayout').setAttribute('aria-hidden', 'true');
    document.getElementById('appLayout').style.display = 'block';
    document.getElementById('appLayout').setAttribute('aria-hidden', 'false');

    const nameEl = document.getElementById('userName');
    if (nameEl && currentUser) {
        nameEl.textContent = [currentUser.firstName, currentUser.lastName].filter(Boolean).join(' ') || currentUser.email || 'User';
    }

    document.querySelectorAll('.nav-link-page').forEach(link => {
        link.classList.toggle('active', link.getAttribute('data-page') === currentPage);
    });
    document.querySelector('.navbar-brand')?.setAttribute('data-page', 'dashboard');

    document.querySelectorAll('.app-page').forEach(page => {
        const id = page.id;
        const pageName = id.replace('Page', '');
        page.style.display = (pageName === currentPage) ? 'block' : 'none';
    });

    switch (currentPage) {
        case 'dashboard': loadDashboard(); break;
        case 'transactions': loadTransactions(); loadCategories(); break;
        case 'categories': loadCategories(); break;
        case 'reports': break;
    }
}

function renderLayout() {
    if (isAuthenticated()) {
        renderAppView();
    } else {
        renderAuthView();
    }
    hideLoader();
}

// ========== NAVEGACIÓN ==========

function navigateTo(pageName) {
    if (!isAuthenticated()) return;
    currentPage = pageName;
    renderAppView();
}

function showPage(pageName) { navigateTo(pageName); }

// Delegación de clics para data-page
document.addEventListener('click', (e) => {
    const link = e.target.closest('[data-page]');
    if (!link || !link.getAttribute('data-page')) return;
    e.preventDefault();
    navigateTo(link.getAttribute('data-page'));
});

// ========== LOGIN / REGISTRO ==========

function showLoginError(msg) {
    const el = document.getElementById('loginError');
    if (el) { el.textContent = msg || ''; el.style.display = msg ? 'block' : 'none'; }
}
function hideLoginError() { showLoginError(''); }
function showRegisterError(msg) {
    const el = document.getElementById('registerError');
    if (el) { el.textContent = msg || ''; el.style.display = msg ? 'block' : 'none'; }
}
function hideRegisterError() { showRegisterError(''); }

async function login(email, password) {
    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        const data = await response.json().catch(() => ({}));
        if (!response.ok) {
            showLoginError(data.message || 'Invalid email or password.');
            return;
        }
        authToken = data.token;
        currentUser = data.user;
        localStorage.setItem(TOKEN_KEY, authToken);
        localStorage.setItem(USER_KEY, JSON.stringify(currentUser));
        renderLayout();
        navigateTo('dashboard');
    } catch (_e) {
        showLoginError('Unable to connect. Please try again.');
    }
}

async function register(payload) {
    try {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        const data = await response.json().catch(() => ({}));
        if (!response.ok) {
            showRegisterError(data.message || 'Registration failed.');
            return;
        }
        authToken = data.token;
        currentUser = data.user;
        localStorage.setItem(TOKEN_KEY, authToken);
        localStorage.setItem(USER_KEY, JSON.stringify(currentUser));
        renderLayout();
        navigateTo('dashboard');
    } catch (_e) {
        showRegisterError('Unable to connect. Please try again.');
    }
}

function logout() {
    clearSession();
    renderAuthView();
    hideLoader();
}

// ========== UI: TOAST ==========

function showToast(message, type) {
    const container = document.getElementById('toastContainer');
    if (!container) return;
    const id = 'toast-' + Date.now();
    const el = document.createElement('div');
    el.className = `toast align-items-center text-bg-${type === 'danger' ? 'danger' : type === 'success' ? 'success' : 'primary'} border-0`;
    el.setAttribute('role', 'alert');
    el.innerHTML = `<div class="d-flex"><div class="toast-body">${escapeHtml(message)}</div><button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button></div>`;
    container.appendChild(el);
    const toast = new bootstrap.Toast(el, { delay: 5000 });
    el.addEventListener('hidden.bs.toast', () => el.remove());
    toast.show();
}

function escapeHtml(text) {
    if (text == null) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// ========== INICIALIZACIÓN ==========

async function initializeApp() {
    const valid = await restoreSession();
    renderLayout();
    setupEventListeners();
    setDefaultDates();
}

function setupEventListeners() {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            showLoginError('');
            const email = document.getElementById('loginEmail').value.trim();
            const password = document.getElementById('loginPassword').value;
            const btn = document.getElementById('loginSubmit');
            const btnText = btn?.querySelector('.btn-text');
            const btnLoading = btn?.querySelector('.btn-loading');
            if (btn) { btn.disabled = true; if (btnText) btnText.classList.add('d-none'); if (btnLoading) btnLoading.classList.remove('d-none'); }
            await login(email, password);
            if (btn) { btn.disabled = false; if (btnText) btnText.classList.remove('d-none'); if (btnLoading) btnLoading.classList.add('d-none'); }
        });
        ['loginEmail', 'loginPassword'].forEach(id => {
            document.getElementById(id)?.addEventListener('input', hideLoginError);
        });
    }

    const loginPasswordToggle = document.getElementById('loginPasswordToggle');
    if (loginPasswordToggle) {
        loginPasswordToggle.addEventListener('click', () => {
            const input = document.getElementById('loginPassword');
            const icon = loginPasswordToggle.querySelector('i');
            if (input.type === 'password') { input.type = 'text'; if (icon) { icon.className = 'fas fa-eye-slash'; } loginPasswordToggle.setAttribute('aria-label', 'Hide password'); }
            else { input.type = 'password'; if (icon) { icon.className = 'fas fa-eye'; } loginPasswordToggle.setAttribute('aria-label', 'Show password'); }
        });
    }

    document.getElementById('linkToRegister')?.addEventListener('click', (e) => {
        e.preventDefault();
        document.getElementById('authLoginCard').style.display = 'none';
        document.getElementById('authRegisterCard').style.display = 'block';
        hideRegisterError();
        document.getElementById('registerFirstName')?.focus();
    });
    document.getElementById('linkToLogin')?.addEventListener('click', (e) => {
        e.preventDefault();
        document.getElementById('authRegisterCard').style.display = 'none';
        document.getElementById('authLoginCard').style.display = 'block';
        hideLoginError();
        document.getElementById('loginEmail')?.focus();
    });

    const registerForm = document.getElementById('registerFormElement');
    if (registerForm) {
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            hideRegisterError();
            const password = document.getElementById('registerPassword').value;
            const confirm = document.getElementById('registerConfirmPassword').value;
            if (password !== confirm) { showRegisterError('Passwords do not match.'); return; }
            const btn = document.getElementById('registerSubmit');
            const btnText = btn?.querySelector('.btn-text');
            const btnLoading = btn?.querySelector('.btn-loading');
            if (btn) { btn.disabled = true; if (btnText) btnText.classList.add('d-none'); if (btnLoading) btnLoading.classList.remove('d-none'); }
            await register({
                firstName: document.getElementById('registerFirstName').value.trim(),
                lastName: document.getElementById('registerLastName').value.trim(),
                email: document.getElementById('registerEmail').value.trim(),
                password,
                confirmPassword: confirm
            });
            if (btn) { btn.disabled = false; if (btnText) btnText.classList.remove('d-none'); if (btnLoading) btnLoading.classList.add('d-none'); }
        });
        ['registerFirstName', 'registerLastName', 'registerEmail', 'registerPassword', 'registerConfirmPassword'].forEach(id => {
            document.getElementById(id)?.addEventListener('input', hideRegisterError);
        });
    }

    document.getElementById('btnLogout')?.addEventListener('click', (e) => { e.preventDefault(); logout(); });

    document.getElementById('transactionType')?.addEventListener('change', handleTransactionTypeChange);
}

function setDefaultDates() {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    const pad = (d) => d.toISOString().split('T')[0];
    const set = (id, val) => { const el = document.getElementById(id); if (el) el.value = val; };
    set('filterStartDate', pad(firstDay));
    set('filterEndDate', pad(lastDay));
    set('reportStartDate', pad(firstDay));
    set('reportEndDate', pad(lastDay));
    set('transactionDate', pad(today));
}

// ========== DASHBOARD ==========

async function loadDashboard() {
    if (!isAuthenticated()) return;
    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/dashboard`);
        if (!response.ok) return;
        const data = await response.json();
        const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = typeof val === 'number' ? '$' + val.toFixed(2) : val; };
        set('totalIncome', data.totalIncome);
        set('totalExpenses', data.totalExpenses);
        set('currentBalance', data.currentBalance);
        set('monthlyBalance', data.monthlyBalance);
        updateRecentTransactions(data.recentTransactions || []);
        loadMonthlyTrendsChart(data.monthlyTrends || []);
        loadExpenseCategoriesChart(data.topExpenseCategories || []);
    } catch (_e) { /* handleUnauthorized ya mostró login */ }
}

function updateRecentTransactions(transactions) {
    const container = document.getElementById('recentTransactions');
    if (!container) return;
    if (!transactions.length) {
        container.innerHTML = '<div class="empty-state"><i class="fas fa-receipt"></i><h5>No transactions yet</h5><p>Start by adding your first transaction</p></div>';
        return;
    }
    const rows = transactions.map(t => `
        <tr>
            <td>${escapeHtml(t.description)}</td>
            <td>${new Date(t.date).toLocaleDateString()}</td>
            <td><span class="badge ${t.type === 'Income' ? 'bg-success' : 'bg-danger'}">${escapeHtml(t.type)}</span></td>
            <td>${escapeHtml(t.categoryName || '-')}</td>
            <td class="${t.type === 'Income' ? 'income' : 'expense'}">$${Number(t.amount).toFixed(2)}</td>
        </tr>`).join('');
    container.innerHTML = `<div class="table-responsive"><table class="table table-hover"><thead><tr><th>Description</th><th>Date</th><th>Type</th><th>Category</th><th>Amount</th></tr></thead><tbody>${rows}</tbody></table></div>`;
}

function loadMonthlyTrendsChart(trends) {
    const ctx = document.getElementById('monthlyTrendsChart')?.getContext('2d');
    if (!ctx) return;
    if (monthlyTrendsChart) monthlyTrendsChart.destroy();
    monthlyTrendsChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: trends.map(t => t.monthName),
            datasets: [
                { label: 'Income', data: trends.map(t => t.income), borderColor: '#198754', backgroundColor: 'rgba(25,135,84,0.1)', tension: 0.4 },
                { label: 'Expenses', data: trends.map(t => t.expenses), borderColor: '#dc3545', backgroundColor: 'rgba(220,53,69,0.1)', tension: 0.4 }
            ]
        },
        options: { responsive: true, maintainAspectRatio: false, scales: { y: { beginAtZero: true } }, plugins: { legend: { position: 'top' } } }
    });
}

function loadExpenseCategoriesChart(cats) {
    const ctx = document.getElementById('expenseCategoriesChart')?.getContext('2d');
    if (!ctx) return;
    if (expenseCategoriesChart) expenseCategoriesChart.destroy();
    expenseCategoriesChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: cats.map(c => c.categoryName),
            datasets: [{ data: cats.map(c => c.totalAmount), backgroundColor: cats.map(c => c.categoryColor || '#6c757d'), borderWidth: 2, borderColor: '#fff' }]
        },
        options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
    });
}

// ========== TRANSACCIONES ==========

async function loadTransactions() {
    if (!isAuthenticated()) return;
    const container = document.getElementById('transactionsTable');
    if (!container) return;
    try {
        const params = new URLSearchParams();
        const start = document.getElementById('filterStartDate')?.value;
        const end = document.getElementById('filterEndDate')?.value;
        const cat = document.getElementById('filterCategory')?.value;
        const type = document.getElementById('filterType')?.value;
        if (start) params.append('startDate', start);
        if (end) params.append('endDate', end);
        if (cat) params.append('categoryId', cat);
        if (type) params.append('type', type);
        const response = await fetchWithAuth(`${API_BASE_URL}/transactions?${params}`);
        if (!response.ok) return;
        const list = await response.json();
        displayTransactions(Array.isArray(list) ? list : []);
    } catch (_e) {
        container.innerHTML = '<div class="empty-state"><p class="text-danger">Unable to load transactions.</p></div>';
    }
}

function displayTransactions(transactions) {
    const container = document.getElementById('transactionsTable');
    if (!container) return;
    if (!transactions.length) {
        container.innerHTML = '<div class="empty-state"><i class="fas fa-receipt"></i><h5>No transactions found</h5><p>Try adjusting filters or add a new transaction</p></div>';
        return;
    }
    const rows = transactions.map(t => `
        <tr>
            <td>${escapeHtml(t.description)}</td>
            <td>${new Date(t.date).toLocaleDateString()}</td>
            <td><span class="badge ${t.type === 'Income' ? 'bg-success' : 'bg-danger'}">${escapeHtml(t.type)}</span></td>
            <td>${escapeHtml(t.categoryName || '-')}</td>
            <td class="${t.type === 'Income' ? 'income' : 'expense'}">$${Number(t.amount).toFixed(2)}</td>
            <td>${escapeHtml(t.paymentMethod || '-')}</td>
            <td>
                <button type="button" class="btn btn-sm btn-outline-primary me-1" onclick="editTransaction(${t.id})"><i class="fas fa-edit"></i></button>
                <button type="button" class="btn btn-sm btn-outline-danger" onclick="deleteTransaction(${t.id})"><i class="fas fa-trash"></i></button>
            </td>
        </tr>`).join('');
    container.innerHTML = `<div class="table-responsive"><table class="table table-hover"><thead><tr><th>Description</th><th>Date</th><th>Type</th><th>Category</th><th>Amount</th><th>Payment</th><th>Actions</th></tr></thead><tbody>${rows}</tbody></table></div>`;
}

function clearFilters() {
    setDefaultDates();
    const el = document.getElementById('filterCategory');
    if (el) el.value = '';
    const el2 = document.getElementById('filterType');
    if (el2) el2.value = '';
    loadTransactions();
}

function handleTransactionTypeChange() {
    const type = document.getElementById('transactionType')?.value;
    const select = document.getElementById('transactionCategory');
    if (!select) return;
    select.innerHTML = '<option value="">Select Category</option>';
    categories.filter(c => c.type === type).forEach(c => select.add(new Option(c.name, c.id)));
}

function showAddTransactionModal() {
    const modal = document.getElementById('addTransactionModal');
    if (modal) new bootstrap.Modal(modal).show();
}

async function saveTransaction() {
    const form = document.getElementById('addTransactionForm');
    if (!form?.checkValidity()) { form?.reportValidity(); return; }
    const payload = {
        description: document.getElementById('transactionDescription').value,
        amount: parseFloat(document.getElementById('transactionAmount').value),
        type: document.getElementById('transactionType').value,
        date: document.getElementById('transactionDate').value,
        notes: document.getElementById('transactionNotes').value,
        location: document.getElementById('transactionLocation').value,
        paymentMethod: document.getElementById('transactionPaymentMethod').value,
        isRecurring: document.getElementById('transactionRecurring').checked,
        categoryId: parseInt(document.getElementById('transactionCategory').value, 10)
    };
    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/transactions`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        if (!response.ok) { const d = await response.json().catch(() => ({})); showToast(d.message || 'Failed to add transaction.', 'danger'); return; }
        bootstrap.Modal.getInstance(document.getElementById('addTransactionModal'))?.hide();
        form.reset();
        showToast('Transaction added.', 'success');
        loadTransactions();
        loadDashboard();
    } catch (_e) { showToast('Failed to add transaction.', 'danger'); }
}

// ========== CATEGORÍAS ==========

async function loadCategories() {
    if (!isAuthenticated()) return;
    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/categories`);
        if (!response.ok) return;
        categories = await response.json();
        populateCategorySelects();
        const container = document.getElementById('categoriesTable');
        if (container) displayCategories();
    } catch (_e) { /* handled */ }
}

function populateCategorySelects() {
    const sel1 = document.getElementById('transactionCategory');
    const sel2 = document.getElementById('filterCategory');
    if (sel1) { sel1.innerHTML = '<option value="">Select Category</option>'; categories.forEach(c => sel1.add(new Option(c.name, c.id))); }
    if (sel2) { sel2.innerHTML = '<option value="">All</option>'; categories.forEach(c => sel2.add(new Option(c.name, c.id))); }
}

function displayCategories() {
    const container = document.getElementById('categoriesTable');
    if (!container) return;
    if (!categories.length) {
        container.innerHTML = '<div class="empty-state"><i class="fas fa-tags"></i><h5>No categories yet</h5><p>Add your first category</p></div>';
        return;
    }
    const rows = categories.map(c => `
        <tr>
            <td><span class="category-color" style="background-color:${escapeHtml(c.color || '#6c757d')}"></span>${escapeHtml(c.name)}</td>
            <td><span class="badge ${c.type === 'Income' ? 'bg-success' : 'bg-danger'}">${escapeHtml(c.type)}</span></td>
            <td>${escapeHtml(c.description || '-')}</td>
            <td><span class="badge" style="background-color:${escapeHtml(c.color || '#6c757d')}">${escapeHtml(c.color || '')}</span></td>
            <td><i class="${escapeHtml(c.icon || 'fas fa-tag')}"></i></td>
            <td>
                <button type="button" class="btn btn-sm btn-outline-primary me-1" onclick="editCategory(${c.id})"><i class="fas fa-edit"></i></button>
                <button type="button" class="btn btn-sm btn-outline-danger" onclick="deleteCategory(${c.id})"><i class="fas fa-trash"></i></button>
            </td>
        </tr>`).join('');
    container.innerHTML = `<div class="table-responsive"><table class="table table-hover"><thead><tr><th>Name</th><th>Type</th><th>Description</th><th>Color</th><th>Icon</th><th>Actions</th></tr></thead><tbody>${rows}</tbody></table></div>`;
}

function showAddCategoryModal() {
    const modal = document.getElementById('addCategoryModal');
    if (modal) new bootstrap.Modal(modal).show();
}

async function saveCategory() {
    const form = document.getElementById('addCategoryForm');
    if (!form?.checkValidity()) { form?.reportValidity(); return; }
    const payload = {
        name: document.getElementById('categoryName').value,
        description: document.getElementById('categoryDescription').value,
        type: document.getElementById('categoryType').value,
        color: document.getElementById('categoryColor').value,
        icon: document.getElementById('categoryIcon').value
    };
    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/categories`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
        if (!response.ok) { const d = await response.json().catch(() => ({})); showToast(d.message || 'Failed to add category.', 'danger'); return; }
        bootstrap.Modal.getInstance(document.getElementById('addCategoryModal'))?.hide();
        form.reset();
        showToast('Category added.', 'success');
        loadCategories();
    } catch (_e) { showToast('Failed to add category.', 'danger'); }
}

// ========== REPORTES ==========

async function generatePdfReport() {
    const start = document.getElementById('reportStartDate')?.value;
    const end = document.getElementById('reportEndDate')?.value;
    if (!start || !end) { showToast('Select start and end dates.', 'warning'); return; }
    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/reports/pdf?startDate=${encodeURIComponent(start)}&endDate=${encodeURIComponent(end)}`);
        if (!response.ok) return;
        const blob = await response.blob();
        const url = URL.createObjectURL(blob);
        window.open(url, '_blank');
        setTimeout(() => URL.revokeObjectURL(url), 60000);
    } catch (_e) { showToast('Failed to generate PDF.', 'danger'); }
}

async function generateExcelReport() {
    const start = document.getElementById('reportStartDate')?.value;
    const end = document.getElementById('reportEndDate')?.value;
    if (!start || !end) { showToast('Select start and end dates.', 'warning'); return; }
    try {
        const response = await fetchWithAuth(`${API_BASE_URL}/reports/excel?startDate=${encodeURIComponent(start)}&endDate=${encodeURIComponent(end)}`);
        if (!response.ok) return;
        const blob = await response.blob();
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `report-${start}-${end}.xlsx`;
        a.click();
        setTimeout(() => URL.revokeObjectURL(url), 10000);
    } catch (_e) { showToast('Failed to generate Excel.', 'danger'); }
}

// ========== PLACEHOLDERS ==========

function editTransaction(id) { showToast('Edit transaction coming soon.', 'info'); }
function deleteTransaction(id) { if (confirm('Delete this transaction?')) showToast('Delete coming soon.', 'info'); }
function editCategory(id) { showToast('Edit category coming soon.', 'info'); }
function deleteCategory(id) { if (confirm('Delete this category?')) showToast('Delete coming soon.', 'info'); }

// ========== ENTRADA ==========

document.addEventListener('DOMContentLoaded', initializeApp);
