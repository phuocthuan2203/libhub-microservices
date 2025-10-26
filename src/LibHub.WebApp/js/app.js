//
// Main application logic, router, and event listeners.
// This is the entry point for the frontend application.
//

const appRoot = document.getElementById('app-root');

function showLoginPage() {
    appRoot.innerHTML = renderLoginForm();
    
    document.getElementById('login-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        
        try {
            const result = await login(email, password);
            handleLoginSuccess(result);
        } catch (error) {
            console.error('Login error:', error);
            alert('Login failed: ' + error.message);
        }
    });
    
    document.getElementById('show-register').addEventListener('click', (e) => {
        e.preventDefault();
        showRegisterPage();
    });
}

function showRegisterPage() {
    appRoot.innerHTML = renderRegisterForm();
    
    document.getElementById('register-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        const username = document.getElementById('username').value;
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirm-password').value;
        
        if (password !== confirmPassword) {
            alert('Passwords do not match');
            return;
        }
        
        try {
            const result = await register(username, email, password);
            console.log('Registration successful:', result);
            alert('Registration successful! Please login.');
            showLoginPage();
        } catch (error) {
            console.error('Registration error:', error);
            alert('Registration failed: ' + error.message);
        }
    });
    
    document.getElementById('show-login').addEventListener('click', (e) => {
        e.preventDefault();
        showLoginPage();
    });
}

function handleLoginSuccess(data) {
    saveToken(data.accessToken);
    showCatalogView();
}

function handleLogout() {
    removeToken();
    showLoginPage();
}

async function showCatalogView() {
    try {
        const userInfo = getUserInfo();
        const userRole = userInfo ? userInfo.role : 'User';
        
        // Update navigation
        document.getElementById('main-nav').innerHTML = renderNav(userRole);
        
        // Add navigation event listeners
        document.getElementById('nav-catalog').addEventListener('click', (e) => {
            e.preventDefault();
            showCatalogView();
        });
        
        document.getElementById('nav-my-loans').addEventListener('click', (e) => {
            e.preventDefault();
            showMyLoansView();
        });
        
        if (document.getElementById('nav-add-book')) {
            document.getElementById('nav-add-book').addEventListener('click', (e) => {
                e.preventDefault();
                showAddBookForm();
            });
        }
        
        document.getElementById('nav-logout').addEventListener('click', (e) => {
            e.preventDefault();
            handleLogout();
        });
        
        // Fetch and display books
        const books = await getBooks();
        
        let html = `
            <div class="catalog-header">
                <h2>Book Catalog</h2>
                <div class="search-container">
                    <input type="text" id="search-input" placeholder="Search by title, author, or ISBN..." class="search-input">
                    <button id="search-btn" class="btn btn-secondary">Search</button>
                </div>
            </div>
            ${renderBookList(books, userRole)}
        `;
        
        appRoot.innerHTML = html;
        
        // Add search functionality
        document.getElementById('search-btn').addEventListener('click', performSearch);
        document.getElementById('search-input').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                performSearch();
            }
        });
        
    } catch (error) {
        console.error('Error loading catalog:', error);
        appRoot.innerHTML = `<div class="error">Error loading catalog: ${error.message}</div>`;
    }
}

async function performSearch() {
    const searchTerm = document.getElementById('search-input').value;
    try {
        // For now, we'll filter client-side since the API might not support search yet
        const books = await getBooks();
        const filteredBooks = books.filter(book => 
            book.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
            book.author.toLowerCase().includes(searchTerm.toLowerCase()) ||
            (book.isbn && book.isbn.includes(searchTerm))
        );
        
        const userInfo = getUserInfo();
        const userRole = userInfo ? userInfo.role : 'User';
        
        const bookListContainer = document.querySelector('.book-grid').parentElement;
        bookListContainer.innerHTML = renderBookList(filteredBooks, userRole);
    } catch (error) {
        console.error('Search error:', error);
        alert('Search failed: ' + error.message);
    }
}

async function showBookDetailView(bookId) {
    try {
        const book = await getBookById(bookId);
        appRoot.innerHTML = renderBookDetail(book);
    } catch (error) {
        console.error('Error loading book details:', error);
        appRoot.innerHTML = `<div class="error">Error loading book details: ${error.message}</div>`;
    }
}

async function showMyLoansView() {
    try {
        const userInfo = getUserInfo();
        if (!userInfo || !userInfo.userId) {
            throw new Error('User information not available');
        }
        
        const loans = await getMyLoans(userInfo.userId);
        appRoot.innerHTML = renderMyLoans(loans);
    } catch (error) {
        console.error('Error loading loans:', error);
        appRoot.innerHTML = `<div class="error">Error loading loans: ${error.message}</div>`;
    }
}

async function borrowBookAction(bookId) {
    try {
        await borrowBook(bookId);
        alert('Book borrowed successfully!');
        showCatalogView(); // Refresh the catalog
    } catch (error) {
        console.error('Error borrowing book:', error);
        alert('Failed to borrow book: ' + error.message);
    }
}

async function returnBookAction(loanId) {
    try {
        await returnBook(loanId);
        alert('Book returned successfully!');
        showMyLoansView(); // Refresh the loans view
    } catch (error) {
        console.error('Error returning book:', error);
        alert('Failed to return book: ' + error.message);
    }
}

// Admin Functions
function showAddBookForm() {
    appRoot.innerHTML = renderBookForm();
    
    document.getElementById('book-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const formData = new FormData(e.target);
        const bookData = {
            isbn: formData.get('isbn'),
            title: formData.get('title'),
            author: formData.get('author'),
            genre: formData.get('genre'),
            totalCopies: parseInt(formData.get('totalCopies')),
            description: formData.get('description')
        };
        
        try {
            await createBook(bookData);
            alert('Book created successfully!');
            showCatalogView();
        } catch (error) {
            console.error('Error creating book:', error);
            alert('Failed to create book: ' + error.message);
        }
    });
}

async function showEditBookForm(bookId) {
    try {
        const book = await getBookById(bookId);
        appRoot.innerHTML = renderBookForm(book);
        
        document.getElementById('book-form').addEventListener('submit', async (e) => {
            e.preventDefault();
            
            const formData = new FormData(e.target);
            const bookData = {
                isbn: formData.get('isbn'),
                title: formData.get('title'),
                author: formData.get('author'),
                genre: formData.get('genre'),
                totalCopies: parseInt(formData.get('totalCopies')),
                description: formData.get('description')
            };
            
            try {
                await updateBook(bookId, bookData);
                alert('Book updated successfully!');
                showCatalogView();
            } catch (error) {
                console.error('Error updating book:', error);
                alert('Failed to update book: ' + error.message);
            }
        });
    } catch (error) {
        console.error('Error loading book for edit:', error);
        alert('Failed to load book: ' + error.message);
    }
}

async function confirmDeleteBook(bookId) {
    try {
        const book = await getBookById(bookId);
        const modalHtml = renderDeleteConfirmation(book);
        
        // Create modal container if it doesn't exist
        let modalContainer = document.getElementById('modal-container');
        if (!modalContainer) {
            modalContainer = document.createElement('div');
            modalContainer.id = 'modal-container';
            document.body.appendChild(modalContainer);
        }
        
        modalContainer.innerHTML = modalHtml;
    } catch (error) {
        console.error('Error loading book for deletion:', error);
        alert('Failed to load book: ' + error.message);
    }
}

function closeDeleteModal() {
    const modalContainer = document.getElementById('modal-container');
    if (modalContainer) {
        modalContainer.innerHTML = '';
    }
}

async function confirmDeleteBookAction(bookId) {
    try {
        await deleteBook(bookId);
        alert('Book deleted successfully!');
        closeDeleteModal();
        showCatalogView();
    } catch (error) {
        console.error('Error deleting book:', error);
        alert('Failed to delete book: ' + error.message);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    console.log('LibHub App Initialized');
    if (getToken()) {
        showCatalogView();
    } else {
        showLoginPage();
    }
});
