//
// Contains functions for rendering HTML content into the DOM.
//

function renderLoginForm() {
    return `
        <div class="auth-container">
            <h2>LibHub</h2>
            <p>Welcome Back! Please Login.</p>
            <form id="login-form" class="auth-form">
                <div class="form-group">
                    <label for="email">Email Address:</label>
                    <input type="email" id="email" name="email" required>
                </div>
                <div class="form-group">
                    <label for="password">Password:</label>
                    <input type="password" id="password" name="password" required>
                </div>
                <button type="submit" class="btn btn-primary">Login</button>
                <div class="auth-divider">
                    <span>or</span>
                </div>
                <p class="auth-link">Don't have an account? <a href="#" id="show-register">Register Here</a></p>
            </form>
        </div>
    `;
}

function renderRegisterForm() {
    return `
        <div class="auth-container">
            <h2>LibHub</h2>
            <p>Create Your New Account</p>
            <form id="register-form" class="auth-form">
                <div class="form-group">
                    <label for="username">Username:</label>
                    <input type="text" id="username" name="username" required>
                </div>
                <div class="form-group">
                    <label for="email">Email Address:</label>
                    <input type="email" id="email" name="email" required>
                </div>
                <div class="form-group">
                    <label for="password">Password:</label>
                    <input type="password" id="password" name="password" required>
                </div>
                <div class="form-group">
                    <label for="confirm-password">Confirm Password:</label>
                    <input type="password" id="confirm-password" name="confirm-password" required>
                </div>
                <button type="submit" class="btn btn-primary">Register</button>
                <div class="auth-divider">
                    <span>or</span>
                </div>
                <p class="auth-link">Already have an account? <a href="#" id="show-login">Login Here</a></p>
            </form>
        </div>
    `;
}

function renderNav(userRole) {
    const userInfo = getUserInfo();
    const username = userInfo ? userInfo.username || userInfo.name || 'User' : 'User';
    
    let navLinks = `
        <span class="nav-welcome">Welcome, ${username}!</span>
        <a href="#" id="nav-catalog" class="nav-link">Catalog</a>
        <a href="#" id="nav-my-loans" class="nav-link">My Loans</a>
    `;
    
    if (userRole === 'Admin') {
        navLinks += `<a href="#" id="nav-add-book" class="nav-link">Add New Book +</a>`;
    }
    
    navLinks += `<a href="#" id="nav-logout" class="nav-link">Logout</a>`;
    
    return navLinks;
}

function renderBookList(books, userRole) {
    if (!books || books.length === 0) {
        return '<p class="no-results">No books found in the catalog.</p>';
    }
    
    let html = '<div class="book-grid">';
    
    books.forEach(book => {
        const availableText = book.availableCopies > 0 ? 
            `${book.availableCopies} available` : 
            'Not available';
            
        html += `
            <div class="book-card">
                <div class="book-cover">📚</div>
                <h3 class="book-title">${book.title}</h3>
                <p class="book-author">by ${book.author}</p>
                <p class="book-availability">${availableText}</p>
                <div class="book-actions">
                    <button class="btn btn-secondary" data-book-id="${book.bookId}" onclick="showBookDetailView(${book.bookId})">View Details</button>
        `;
        
        if (userRole === 'Admin') {
            html += `
                    <button class="btn btn-warning" data-book-id="${book.bookId}" onclick="showEditBookForm(${book.bookId})">Edit</button>
                    <button class="btn btn-danger" data-book-id="${book.bookId}" onclick="confirmDeleteBook(${book.bookId})">Delete</button>
            `;
        }
        
        html += `
                </div>
            </div>
        `;
    });
    
    html += '</div>';
    return html;
}

function renderBookDetail(book) {
    const isAvailable = book.availableCopies > 0;
    const borrowButton = isAvailable ? 
        `<button class="btn btn-primary" onclick="borrowBookAction(${book.bookId})">Borrow This Book</button>` :
        `<button class="btn btn-primary" disabled>Not Available</button>`;
        
    return `
        <div class="book-detail">
            <button class="btn btn-link" onclick="showCatalogView()">< Back to Catalog</button>
            <div class="book-detail-content">
                <div class="book-detail-cover">📚</div>
                <div class="book-detail-info">
                    <h2>${book.title}</h2>
                    <p class="book-author">by ${book.author}</p>
                    <p class="book-isbn">ISBN: ${book.isbn || 'N/A'}</p>
                    <p class="book-genre">Genre: ${book.genre || 'N/A'}</p>
                    <p class="book-description">${book.description || 'No description available.'}</p>
                    <p class="book-copies">Copies Available: ${book.availableCopies}</p>
                    ${borrowButton}
                </div>
            </div>
        </div>
    `;
}

function renderMyLoans(loans) {
    if (!loans || loans.length === 0) {
        return '<p class="no-results">You have no borrowed books.</p>';
    }
    
    let html = `
        <button class="btn btn-link" onclick="showCatalogView()">< Back to Catalog</button>
        <h2>My Borrowing History</h2>
        <div class="loans-table">
            <table>
                <thead>
                    <tr>
                        <th>Book Title</th>
                        <th>Checkout Date</th>
                        <th>Due Date</th>
                        <th>Status/Action</th>
                    </tr>
                </thead>
                <tbody>
    `;
    
    loans.forEach(loan => {
        const checkoutDate = new Date(loan.checkoutDate).toLocaleDateString();
        const dueDate = new Date(loan.dueDate).toLocaleDateString();
        const statusAction = loan.status === 'CheckedOut' ? 
            `<button class="btn btn-primary btn-sm" onclick="returnBookAction(${loan.loanId})">Return</button>` :
            'Returned';
            
        html += `
            <tr>
                <td>${loan.bookTitle || 'Unknown Book'}</td>
                <td>${checkoutDate}</td>
                <td>${dueDate}</td>
                <td>${statusAction}</td>
            </tr>
        `;
    });
    
    html += `
                </tbody>
            </table>
        </div>
    `;
    
    return html;
}

function renderBookForm(book = null) {
    const isEdit = book !== null;
    const title = isEdit ? `Edit Book: ${book.title}` : 'Add New Book';
    const buttonText = isEdit ? 'Update Book' : 'Save Book';
    
    return `
        <div class="book-form-container">
            <button class="btn btn-link" onclick="showCatalogView()">< Back to Catalog</button>
            <h2>${title}</h2>
            <form id="book-form" class="book-form">
                <div class="form-group">
                    <label for="isbn">ISBN*:</label>
                    <input type="text" id="isbn" name="isbn" value="${book?.isbn || ''}" required>
                </div>
                <div class="form-group">
                    <label for="title">Title*:</label>
                    <input type="text" id="title" name="title" value="${book?.title || ''}" required>
                </div>
                <div class="form-group">
                    <label for="author">Author*:</label>
                    <input type="text" id="author" name="author" value="${book?.author || ''}" required>
                </div>
                <div class="form-group">
                    <label for="genre">Genre:</label>
                    <input type="text" id="genre" name="genre" value="${book?.genre || ''}">
                </div>
                <div class="form-group">
                    <label for="totalCopies">Total Copies*:</label>
                    <input type="number" id="totalCopies" name="totalCopies" value="${book?.totalCopies || ''}" min="1" required>
                </div>
                <div class="form-group">
                    <label for="description">Description:</label>
                    <textarea id="description" name="description" rows="4">${book?.description || ''}</textarea>
                </div>
                <div class="form-actions">
                    <button type="submit" class="btn btn-primary">${buttonText}</button>
                    <button type="button" class="btn btn-secondary" onclick="showCatalogView()">Cancel</button>
                </div>
            </form>
            <p class="form-note">* Fields marked with an asterisk are required.</p>
        </div>
    `;
}

function renderDeleteConfirmation(book) {
    return `
        <div class="modal-overlay" onclick="closeDeleteModal()">
            <div class="modal-content" onclick="event.stopPropagation()">
                <h3>Confirm Deletion</h3>
                <p>Are you sure you want to delete "${book.title}"?</p>
                <p class="warning-text">This action cannot be undone.</p>
                <div class="modal-actions">
                    <button class="btn btn-danger" onclick="confirmDeleteBookAction(${book.bookId})">Confirm Delete</button>
                    <button class="btn btn-secondary" onclick="closeDeleteModal()">Cancel</button>
                </div>
            </div>
        </div>
    `;
}
