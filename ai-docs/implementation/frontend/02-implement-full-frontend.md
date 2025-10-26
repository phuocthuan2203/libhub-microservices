# Frontend Implementation Plan: Full Application

## Task 1: Implement the Full Frontend Application

**Objective:** Build the complete frontend for the LibHub application, from public pages to authenticated user and admin features, based on the master context.

**Preamble:** This plan is a sequential guide. Complete each phase before moving to the next. You have been provided with the frontend master context documents and are expected to have read and understood them, especially the UI Wireframes and the UI-to-API Mapping.

---
## Phase 2: Public Components & User Flow

**Goal:** Build the UI and logic for users who are not yet authenticated.

### Step 2.1: Implement UI Rendering (`ui.js`)
- **File to modify:** `src/LibHub.WebApp/js/ui.js`
- **Instructions:** Implement the functions that generate the HTML for the login and registration forms. These functions should return an HTML string.

```javascript
// In ui.js

function renderLoginForm() {
    return `
        <h2>Login</h2>
        <form id="login-form" class="auth-form">
            <div class="form-group">
                <label for="email">Email:</label>
                <input type="email" id="email" name="email" required>
            </div>
            <div class="form-group">
                <label for="password">Password:</label>
                <input type="password" id="password" name="password" required>
            </div>
            <button type="submit">Login</button>
        </form>
        <p>Don't have an account? <a href="#" id="show-register">Register Here</a></p>
    `;
}

function renderRegisterForm() {
    // TODO: Implement this function. It should return the HTML string for the registration form,
    // including fields for username, email, and password. Add a link with id="show-login".
}
Step 2.2: Implement API Calls (api.js)

File to modify: src/LibHub.WebApp/js/api.js

Instructions: Implement the login and register functions using fetch.

code
JavaScript
download
content_copy
expand_less
// In api.js

async function login(email, password) {
    // TODO: Implement the fetch POST request to `${API_BASE_URL}/users/login`.
    // The body should be a JSON string of { email, password }.
    // If response.ok is true, return response.json(). Otherwise, throw an error.
}

async function register(username, email, password) {
    // TODO: Implement the fetch POST request to `${API_BASE_URL}/users/register`.
}
Step 2.3: Implement Main Logic (app.js)

File to modify: src/LibHub.WebApp/js/app.js

Instructions: Wire up the public views and their event listeners.

code
JavaScript
download
content_copy
expand_less
// In app.js

const appRoot = document.getElementById('app-root');

function showLoginPage() {
    appRoot.innerHTML = ui.renderLoginForm();
    document.getElementById('login-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        // TODO: Get email/password, call api.login(), and for now, console.log the result.
    });
    document.getElementById('show-register').addEventListener('click', showRegisterPage);
}

function showRegisterPage() {
    // TODO: Render the register form and add its event listeners.
}

document.addEventListener('DOMContentLoaded', () => {
    // For now, the app always starts by showing the login page.
    showLoginPage();
});
Phase 3: Core Authentication Logic

Goal: Make the application "auth-aware" by implementing client-side JWT handling.

Step 3.1: Implement Token Management (auth.js)

File to modify: src/LibHub.WebApp/js/auth.js

Instructions: Implement all functions for managing the JWT in localStorage.

code
JavaScript
download
content_copy
expand_less
// In auth.js

function saveToken(token) {
    // TODO: Save the token to localStorage.
}

function getToken() {
    // TODO: Retrieve the token from localStorage.
}

function removeToken() {
    // TODO: Remove the token from localStorage.
}

function getUserInfo() {
    // TODO: Get token, decode its payload using atob() and JSON.parse(), and return the user object.
    // Return null if no token exists.
}
Step 3.2: Update API Calls to be Auth-Aware (api.js)

File to modify: src/LibHub.WebApp/js/api.js

Instructions: Create a helper to add the Authorization header automatically.

code
JavaScript
download
content_copy
expand_less
// In api.js

function getAuthHeaders() {
    const token = auth.getToken();
    const headers = { 'Content-Type': 'application/json' };
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }
    return headers;
}
Step 3.3: Update Main Logic (app.js)

File to modify: src/LibHub.WebApp/js/app.js

Instructions: Update the login/logout logic and create the initial "router."

code
JavaScript
download
content_copy
expand_less
// In app.js

function handleLoginSuccess(data) {
    auth.saveToken(data.accessToken);
    showCatalogView(); // Transition to the main app view.
}

function handleLogout() {
    auth.removeToken();
    showLoginPage();
}

// TODO: Update the login form's submit handler to call handleLoginSuccess.

document.addEventListener('DOMContentLoaded', () => {
    if (auth.getToken()) {
        showCatalogView();
    } else {
        showLoginPage();
    }
});
Phase 4: Authenticated User Components

Goal: Build the main application features for a standard user.

Step 4.1: Implement UI Rendering (ui.js)

File to modify: src/LibHub.WebApp/js/ui.js

Instructions: Create the render functions for the authenticated views. Add a renderNav function.

code
JavaScript
download
content_copy
expand_less
// In ui.js

function renderNav(userRole) {
    // TODO: Render nav links. Include "My Loans" and "Logout".
    // If userRole is 'Admin', also render "Add New Book".
}

function renderBookList(books, userRole) {
    // TODO: Return HTML string for the book grid.
    // Each book should have a "View Details" button with a `data-book-id`.
    // If userRole is 'Admin', also render "Edit" and "Delete" buttons.
}

function renderBookDetail(book) {
    // TODO: Return HTML for the detail view. Disable "Borrow" button if availableCopies is 0.
}

function renderMyLoans(loans) {
    // TODO: Return HTML for the loan history table.
}
Step 4.2: Implement Authenticated API Calls (api.js)

File to modify: src/LibHub.WebApp/js/api.js

Instructions: Implement all remaining API call functions. Remember to use getAuthHeaders().

code
JavaScript
download
content_copy
expand_less
// In api.js
async function getBooks() { /* ... */ }
async function getBookById(id) { /* ... */ }
async function borrowBook(bookId) { /* ... */ }
async function getMyLoans(userId) { /* ... */ }
async function returnBook(loanId) { /* ... */ }
// Admin functions
async function createBook(bookData) { /* ... */ }
async function updateBook(bookId, bookData) { /* ... */ }
async function deleteBook(bookId) { /* ... */ }
Step 4.3: Wire Up the Views in app.js

File to modify: src/LibHub.WebApp/js/app.js

Instructions: Create functions to orchestrate the authenticated views.

code
JavaScript
download
content_copy
expand_less
// In app.js

async function showCatalogView() {
    // TODO: 1. Get user role from auth.getUserInfo().
    // 2. Render nav.
    // 3. Call api.getBooks().
    // 4. Render book list into #app-root.
    // 5. Add event listeners for details, edit, delete, add book, etc.
}

async function showBookDetailView(bookId) {
    // TODO: Call api.getBookById(), render the detail view, and add event listener for borrowing.
}

async function showMyLoansView() {
    // TODO: Call api.getMyLoans(), render the view, and add event listeners for returning books.
}
Phase 5: Admin Components & Finalization

Goal: Implement admin-specific features and polish the application.

Step 5.1: Implement Admin UI and Logic

Files to modify: All .js files.

Instructions:

Role-Based Rendering: Ensure all ui.render...() functions correctly show/hide admin controls based on the user's role.

Add/Edit Form: In ui.js, create a reusable renderBookForm(book) function. In app.js, create showAddBookForm() and showEditBookForm(bookId) functions that use this renderer.

Wire Admin Actions: Connect the "Add New Book", "Edit", "Delete", and form submission events to their corresponding functions in api.js. Use a confirm() dialog for the delete action.

Step 5.2: Finalization (Error Handling and Polish)

Files to modify: All .js files and style.css.

Instructions:

Error Handling: Wrap all API calls in app.js with try...catch blocks. Display user-friendly error messages using alert(). In api.js, ensure you check response.ok and throw an error if the call fails.

Styling: In css/style.css, add styles to make the application usable and visually clean.

Task 2: Generate Implementation Report

Objective: After completing the implementation, generate a single, final report.

File to create: ai-docs/implementation/frontend/implementation-report.md

Instructions: Use the provided template to create a comprehensive report covering all phases.
