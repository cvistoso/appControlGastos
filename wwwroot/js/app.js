// Expense Control App - Main JavaScript File

// Global variables
let currentUser = null;
let authToken = null;
let categories = [];
let monthlyTrendsChart = null;
let expenseCategoriesChart = null;

// API Base URL
const API_BASE_URL = '/api';

// Initialize app when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});

// Initialize the application
function initializeApp() {
    // Check if user is already logged in
    const token = localStorage.getItem('authToken');
    if (token) {
        authToken = token;
        loadUserProfile();
    } else {
        showLoginPage();
    }

    // Set up event listeners
    setupEventListeners();
    
    // Set default dates
    setDefaultDates();
}

// Set up event listeners
function setupEventListeners() {
    // Login form
    document.getElementById('loginForm').addEventListener('submit', handleLogin);
    
    // Register form
    document.getElementById('registerFormElement').addEventListener('submit', handleRegister);
    
    // Transaction type change
    document.getElementById('transactionType').addEventListener('change', handleTransactionTypeChange);
    
    // Recurring transaction checkbox
    document.getElementById('transactionRecurring').addEventListener('change', handleRecurringChange);
}

// Set default dates
function setDefaultDates() {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    
    document.getElementById('filterStartDate').value = firstDay.toISOString().split('T')[0];
    document.getElementById('filterEndDate').value = lastDay.toISOString().split('T')[0];
    document.getElementById('reportStartDate').value = firstDay.toISOString().split('T')[0];
    document.getElementById('reportEndDate').value = lastDay.toISOString().split('T')[0];
    document.getElementById('transactionDate').value = today.toISOString().split('T')[0];
}

// Authentication functions
async function handleLogin(e) {
    e.preventDefault();
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });
        
        if (response.ok) {
            const data = await response.json();
            authToken = data.token;
            currentUser = data.user;
            
            localStorage.setItem('authToken', authToken);
            localStorage.setItem('currentUser', JSON.stringify(currentUser));
            
            showMainApp();
        } else {
            const error = await response.json();
            showAlert('Invalid email or password', 'danger');
        }
    } catch (error) {
        showAlert('Login failed. Please try again.', 'danger');
    }
}

async function handleRegister(e) {
    e.preventDefault();
    
    const firstName = document.getElementById('registerFirstName').value;
    const lastName = document.getElementById('registerLastName').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('registerConfirmPassword').value;
    
    if (password !== confirmPassword) {
        showAlert('Passwords do not match', 'danger');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ firstName, lastName, email, password, confirmPassword })
        });
        
        if (response.ok) {
            const data = await response.json();
            authToken = data.token;
            currentUser = data.user;
            
            localStorage.setItem('authToken', authToken);
            localStorage.setItem('currentUser', JSON.stringify(currentUser));
            
            showMainApp();
        } else {
            const error = await response.json();
            showAlert('Registration failed. Email may already exist.', 'danger');
        }
    } catch (error) {
        showAlert('Registration failed. Please try again.', 'danger');
    }
}

function logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    authToken = null;
    currentUser = null;
    showLoginPage();
}

// Page navigation
function showLoginPage() {
    hideAllPages();
    document.getElementById('loginPage').style.display = 'block';
}

function showMainApp() {
    hideAllPages();
    document.getElementById('dashboardPage').style.display = 'block';
    loadUserProfile();
    loadDashboard();
}

function showPage(pageName) {
    console.log('showPage called with:', pageName);
    if (!authToken) {
        console.log('No auth token, showing login page');
        showLoginPage();
        return;
    }
    
    hideAllPages();
    const pageElement = document.getElementById(pageName + 'Page');
    console.log('Page element found:', pageElement);
    if (pageElement) {
        pageElement.style.display = 'block';
    }
    
    switch (pageName) {
        case 'dashboard':
            console.log('Loading dashboard');
            loadDashboard();
            break;
        case 'transactions':
            console.log('Loading transactions');
            console.log('About to call loadTransactions()');
            
            // Update debug info immediately
            const debugText = document.getElementById('debugText');
            if (debugText) {
                debugText.textContent = 'Page loaded, calling loadTransactions...';
            }
            
            loadTransactions();
            console.log('About to call loadCategories()');
            loadCategories();
            console.log('Both functions called');
            break;
        case 'reports':
            console.log('Loading reports');
            // Reports page doesn't need special loading
            break;
        case 'categories':
            console.log('Loading categories');
            loadCategories();
            break;
    }
}

function hideAllPages() {
    const pages = document.querySelectorAll('.page, #loginPage');
    pages.forEach(page => page.style.display = 'none');
}

// User profile
async function loadUserProfile() {
    if (!authToken) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/auth/me`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        if (response.ok) {
            currentUser = await response.json();
            document.getElementById('userName').textContent = `${currentUser.firstName} ${currentUser.lastName}`;
        }
    } catch (error) {
        console.error('Failed to load user profile:', error);
    }
}

// Dashboard functions
async function loadDashboard() {
    if (!authToken) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/dashboard`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        if (response.ok) {
            const data = await response.json();
            updateDashboardKPIs(data);
            updateRecentTransactions(data.recentTransactions);
            loadMonthlyTrendsChart(data.monthlyTrends);
            loadExpenseCategoriesChart(data.topExpenseCategories);
        }
    } catch (error) {
        console.error('Failed to load dashboard:', error);
    }
}

function updateDashboardKPIs(data) {
    document.getElementById('totalIncome').textContent = `$${data.totalIncome.toFixed(2)}`;
    document.getElementById('totalExpenses').textContent = `$${data.totalExpenses.toFixed(2)}`;
    document.getElementById('currentBalance').textContent = `$${data.currentBalance.toFixed(2)}`;
    document.getElementById('monthlyBalance').textContent = `$${data.monthlyBalance.toFixed(2)}`;
}

function updateRecentTransactions(transactions) {
    const container = document.getElementById('recentTransactions');
    
    if (transactions.length === 0) {
        container.innerHTML = '<div class="empty-state"><i class="fas fa-receipt"></i><h5>No transactions yet</h5><p>Start by adding your first transaction</p></div>';
        return;
    }
    
    const table = `
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th>Description</th>
                        <th>Date</th>
                        <th>Type</th>
                        <th>Category</th>
                        <th>Amount</th>
                    </tr>
                </thead>
                <tbody>
                    ${transactions.map(transaction => `
                        <tr>
                            <td>${transaction.description}</td>
                            <td>${new Date(transaction.date).toLocaleDateString()}</td>
                            <td><span class="badge ${transaction.type === 'Income' ? 'bg-success' : 'bg-danger'}">${transaction.type}</span></td>
                            <td>
                                <span class="category-color" style="background-color: ${transaction.categoryColor}"></span>
                                ${transaction.categoryName}
                            </td>
                            <td class="${transaction.type === 'Income' ? 'income' : 'expense'}">$${transaction.amount.toFixed(2)}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
    
    container.innerHTML = table;
}

function loadMonthlyTrendsChart(trends) {
    const ctx = document.getElementById('monthlyTrendsChart').getContext('2d');
    
    if (monthlyTrendsChart) {
        monthlyTrendsChart.destroy();
    }
    
    monthlyTrendsChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: trends.map(t => t.monthName),
            datasets: [{
                label: 'Income',
                data: trends.map(t => t.income),
                borderColor: '#198754',
                backgroundColor: 'rgba(25, 135, 84, 0.1)',
                tension: 0.4
            }, {
                label: 'Expenses',
                data: trends.map(t => t.expenses),
                borderColor: '#dc3545',
                backgroundColor: 'rgba(220, 53, 69, 0.1)',
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return '$' + value.toFixed(2);
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    position: 'top'
                }
            }
        }
    });
}

function loadExpenseCategoriesChart(categories) {
    const ctx = document.getElementById('expenseCategoriesChart').getContext('2d');
    
    if (expenseCategoriesChart) {
        expenseCategoriesChart.destroy();
    }
    
    expenseCategoriesChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: categories.map(c => c.categoryName),
            datasets: [{
                data: categories.map(c => c.totalAmount),
                backgroundColor: categories.map(c => c.categoryColor),
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
}

// Transaction functions
async function loadTransactions() {
    console.log('loadTransactions called');
    
    // Update debug info
    const debugText = document.getElementById('debugText');
    if (debugText) {
        debugText.textContent = 'Loading transactions...';
    }
    
    if (!authToken) {
        console.log('No auth token, returning');
        if (debugText) {
            debugText.textContent = 'No auth token available';
        }
        return;
    }
    
    console.log('Auth token available, proceeding...');
    
    try {
        // Get filter values safely
        const startDate = document.getElementById('filterStartDate')?.value || '';
        const endDate = document.getElementById('filterEndDate')?.value || '';
        const categoryId = document.getElementById('filterCategory')?.value || '';
        const type = document.getElementById('filterType')?.value || '';
        
        console.log('Filters:', { startDate, endDate, categoryId, type });
        
        const params = new URLSearchParams();
        if (startDate) params.append('startDate', startDate);
        if (endDate) params.append('endDate', endDate);
        if (categoryId) params.append('categoryId', categoryId);
        if (type) params.append('type', type);
        
        const url = `${API_BASE_URL}/transactions?${params}`;
        console.log('Fetching from URL:', url);
        
        // Add timeout to prevent hanging
        const controller = new AbortController();
        const timeoutId = setTimeout(() => {
            console.log('Request timeout, aborting...');
            controller.abort();
        }, 10000); // 10 second timeout
        
        console.log('About to make fetch request...');
        const response = await fetch(url, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            },
            signal: controller.signal
        });
        
        clearTimeout(timeoutId);
        console.log('Response received, status:', response.status);
        
        if (response.ok) {
            const transactions = await response.json();
            console.log('Transactions received:', transactions.length);
            
            if (debugText) {
                debugText.textContent = `Loaded ${transactions.length} transactions successfully`;
            }
            
            displayTransactions(transactions);
        } else {
            console.error('Response not ok:', response.status, response.statusText);
            const errorText = await response.text();
            console.error('Error response:', errorText);
            
            if (debugText) {
                debugText.textContent = `Error: ${response.status} - ${response.statusText}`;
            }
        }
    } catch (error) {
        console.error('Failed to load transactions:', error);
        if (debugText) {
            if (error.name === 'AbortError') {
                debugText.textContent = 'Error: Request timeout (10s)';
            } else {
                debugText.textContent = `Error: ${error.message}`;
            }
        }
    }
}

function displayTransactions(transactions) {
    console.log('displayTransactions called with:', transactions);
    const container = document.getElementById('transactionsTable');
    console.log('Container found:', container);
    
    // Update debug info
    const debugText = document.getElementById('debugText');
    if (debugText) {
        debugText.textContent = `Displaying ${transactions.length} transactions`;
    }
    
    if (!container) {
        console.error('transactionsTable container not found!');
        if (debugText) {
            debugText.textContent = 'Error: transactionsTable container not found!';
        }
        return;
    }
    
    if (transactions.length === 0) {
        console.log('No transactions, showing empty state');
        if (debugText) {
            debugText.textContent = 'No transactions found - showing empty state';
        }
        container.innerHTML = '<div class="empty-state"><i class="fas fa-receipt"></i><h5>No transactions found</h5><p>Try adjusting your filters or add a new transaction</p></div>';
        return;
    }
    
    console.log('Displaying', transactions.length, 'transactions');
    
    const table = `
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th>Description</th>
                        <th>Date</th>
                        <th>Type</th>
                        <th>Category</th>
                        <th>Amount</th>
                        <th>Payment Method</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${transactions.map(transaction => `
                        <tr>
                            <td>${transaction.description}</td>
                            <td>${new Date(transaction.date).toLocaleDateString()}</td>
                            <td><span class="badge ${transaction.type === 'Income' ? 'bg-success' : 'bg-danger'}">${transaction.type}</span></td>
                            <td>
                                <span class="category-color" style="background-color: ${transaction.categoryColor}"></span>
                                ${transaction.categoryName}
                            </td>
                            <td class="${transaction.type === 'Income' ? 'income' : 'expense'}">$${transaction.amount.toFixed(2)}</td>
                            <td>${transaction.paymentMethod || '-'}</td>
                            <td>
                                <button class="btn btn-sm btn-outline-primary me-1" onclick="editTransaction(${transaction.id})">
                                    <i class="fas fa-edit"></i>
                                </button>
                                <button class="btn btn-sm btn-outline-danger" onclick="deleteTransaction(${transaction.id})">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
    
    container.innerHTML = table;
}

function clearFilters() {
    document.getElementById('filterStartDate').value = '';
    document.getElementById('filterEndDate').value = '';
    document.getElementById('filterCategory').value = '';
    document.getElementById('filterType').value = '';
    loadTransactions();
}

async function testLoadTransactions() {
    console.log('=== DEBUG TEST START ===');
    console.log('Auth token:', authToken ? 'Present' : 'Missing');
    console.log('API Base URL:', API_BASE_URL);
    
    if (!authToken) {
        console.error('No auth token available');
        alert('No auth token available. Please login first.');
        return;
    }
    
    try {
        const url = `${API_BASE_URL}/transactions`;
        console.log('Testing URL:', url);
        
        const response = await fetch(url, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        console.log('Response status:', response.status);
        console.log('Response headers:', response.headers);
        
        if (response.ok) {
            const transactions = await response.json();
            console.log('Raw response:', transactions);
            console.log('Transactions count:', transactions.length);
            
            if (transactions.length > 0) {
                console.log('First transaction:', transactions[0]);
                displayTransactions(transactions);
            } else {
                console.log('No transactions in response');
                displayTransactions([]);
            }
        } else {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            alert(`Error: ${response.status} - ${errorText}`);
        }
    } catch (error) {
        console.error('Test error:', error);
        alert(`Test error: ${error.message}`);
    }
    
    console.log('=== DEBUG TEST END ===');
}

// Category functions
async function loadCategories() {
    if (!authToken) return;
    
    try {
        const response = await fetch(`${API_BASE_URL}/categories`, {
            headers: {
                'Authorization': `Bearer ${authToken}`
            }
        });
        
        if (response.ok) {
            categories = await response.json();
            populateCategorySelects();
            displayCategories();
        }
    } catch (error) {
        console.error('Failed to load categories:', error);
    }
}

function populateCategorySelects() {
    const transactionCategorySelect = document.getElementById('transactionCategory');
    const filterCategorySelect = document.getElementById('filterCategory');
    
    // Clear existing options
    transactionCategorySelect.innerHTML = '<option value="">Select Category</option>';
    filterCategorySelect.innerHTML = '<option value="">All Categories</option>';
    
    categories.forEach(category => {
        const option1 = new Option(category.name, category.id);
        const option2 = new Option(category.name, category.id);
        transactionCategorySelect.add(option1);
        filterCategorySelect.add(option2);
    });
}

function displayCategories() {
    const container = document.getElementById('categoriesTable');
    
    const table = `
        <div class="table-responsive">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Type</th>
                        <th>Description</th>
                        <th>Color</th>
                        <th>Icon</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${categories.map(category => `
                        <tr>
                            <td>
                                <span class="category-color" style="background-color: ${category.color}"></span>
                                ${category.name}
                            </td>
                            <td><span class="badge ${category.type === 'Income' ? 'bg-success' : 'bg-danger'}">${category.type}</span></td>
                            <td>${category.description || '-'}</td>
                            <td><span class="badge" style="background-color: ${category.color}">${category.color}</span></td>
                            <td><i class="${category.icon || 'fas fa-tag'}"></i></td>
                            <td>
                                <button class="btn btn-sm btn-outline-primary me-1" onclick="editCategory(${category.id})">
                                    <i class="fas fa-edit"></i>
                                </button>
                                <button class="btn btn-sm btn-outline-danger" onclick="deleteCategory(${category.id})">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        </div>
    `;
    
    container.innerHTML = table;
}

// Modal functions
function showAddTransactionModal() {
    const modal = new bootstrap.Modal(document.getElementById('addTransactionModal'));
    modal.show();
}

function showAddCategoryModal() {
    const modal = new bootstrap.Modal(document.getElementById('addCategoryModal'));
    modal.show();
}

function showLoginForm() {
    document.getElementById('loginForm').style.display = 'block';
    document.getElementById('registerForm').style.display = 'none';
}

function showRegisterForm() {
    document.getElementById('loginForm').style.display = 'none';
    document.getElementById('registerForm').style.display = 'block';
}

// Transaction form handlers
function handleTransactionTypeChange() {
    const type = document.getElementById('transactionType').value;
    const categorySelect = document.getElementById('transactionCategory');
    
    // Clear and repopulate categories based on type
    categorySelect.innerHTML = '<option value="">Select Category</option>';
    
    const filteredCategories = categories.filter(cat => cat.type === type);
    filteredCategories.forEach(category => {
        const option = new Option(category.name, category.id);
        categorySelect.add(option);
    });
}

function handleRecurringChange() {
    const isRecurring = document.getElementById('transactionRecurring').checked;
    // Add recurring frequency field if needed
}

async function saveTransaction() {
    const form = document.getElementById('addTransactionForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    const transactionData = {
        description: document.getElementById('transactionDescription').value,
        amount: parseFloat(document.getElementById('transactionAmount').value),
        type: document.getElementById('transactionType').value,
        date: document.getElementById('transactionDate').value,
        notes: document.getElementById('transactionNotes').value,
        location: document.getElementById('transactionLocation').value,
        paymentMethod: document.getElementById('transactionPaymentMethod').value,
        isRecurring: document.getElementById('transactionRecurring').checked,
        categoryId: parseInt(document.getElementById('transactionCategory').value)
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/transactions`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(transactionData)
        });
        
        if (response.ok) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('addTransactionModal'));
            modal.hide();
            form.reset();
            showAlert('Transaction added successfully!', 'success');
            loadTransactions();
            loadDashboard();
        } else {
            const error = await response.json();
            showAlert('Failed to add transaction: ' + (error.message || 'Unknown error'), 'danger');
        }
    } catch (error) {
        showAlert('Failed to add transaction. Please try again.', 'danger');
    }
}

async function saveCategory() {
    const form = document.getElementById('addCategoryForm');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    const categoryData = {
        name: document.getElementById('categoryName').value,
        description: document.getElementById('categoryDescription').value,
        type: document.getElementById('categoryType').value,
        color: document.getElementById('categoryColor').value,
        icon: document.getElementById('categoryIcon').value
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/categories`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authToken}`
            },
            body: JSON.stringify(categoryData)
        });
        
        if (response.ok) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('addCategoryModal'));
            modal.hide();
            form.reset();
            showAlert('Category added successfully!', 'success');
            loadCategories();
        } else {
            const error = await response.json();
            showAlert('Failed to add category: ' + (error.message || 'Unknown error'), 'danger');
        }
    } catch (error) {
        showAlert('Failed to add category. Please try again.', 'danger');
    }
}

// Report functions
async function generatePdfReport() {
    const startDate = document.getElementById('reportStartDate').value;
    const endDate = document.getElementById('reportEndDate').value;
    
    if (!startDate || !endDate) {
        showAlert('Please select both start and end dates', 'warning');
        return;
    }
    
    const url = `${API_BASE_URL}/reports/pdf?startDate=${startDate}&endDate=${endDate}`;
    window.open(url, '_blank');
}

async function generateExcelReport() {
    const startDate = document.getElementById('reportStartDate').value;
    const endDate = document.getElementById('reportEndDate').value;
    
    if (!startDate || !endDate) {
        showAlert('Please select both start and end dates', 'warning');
        return;
    }
    
    const url = `${API_BASE_URL}/reports/excel?startDate=${startDate}&endDate=${endDate}`;
    window.open(url, '_blank');
}

// Utility functions
function showAlert(message, type) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    // Insert at the top of the main content
    const container = document.querySelector('.container-fluid');
    container.insertBefore(alertDiv, container.firstChild);
    
    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}

// Placeholder functions for future implementation
function editTransaction(id) {
    showAlert('Edit transaction functionality coming soon!', 'info');
}

function deleteTransaction(id) {
    if (confirm('Are you sure you want to delete this transaction?')) {
        showAlert('Delete transaction functionality coming soon!', 'info');
    }
}

function editCategory(id) {
    showAlert('Edit category functionality coming soon!', 'info');
}

function deleteCategory(id) {
    if (confirm('Are you sure you want to delete this category?')) {
        showAlert('Delete category functionality coming soon!', 'info');
    }
}
