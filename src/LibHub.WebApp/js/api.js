//
// Handles all communication with the backend API Gateway.
//
const API_BASE_URL = 'http://localhost:5000/api'; // The address of the API Gateway

async function login(email, password) {
    const response = await fetch(`${API_BASE_URL}/users/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ email, password })
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Login failed');
    }

    return await response.json();
}

async function register(username, email, password) {
    const response = await fetch(`${API_BASE_URL}/users/register`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, email, password })
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Registration failed');
    }

    return await response.json();
}

function getAuthHeaders() {
    const token = getToken();
    const headers = { 'Content-Type': 'application/json' };
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }
    return headers;
}

async function getBooks() {
    const response = await fetch(`${API_BASE_URL}/books`, {
        method: 'GET',
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to fetch books');
    }

    return await response.json();
}

async function getBookById(id) {
    const response = await fetch(`${API_BASE_URL}/books/${id}`, {
        method: 'GET',
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to fetch book details');
    }

    return await response.json();
}

async function borrowBook(bookId) {
    const response = await fetch(`${API_BASE_URL}/loans`, {
        method: 'POST',
        headers: getAuthHeaders(),
        body: JSON.stringify({ bookId })
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to borrow book');
    }

    return await response.json();
}

async function getMyLoans(userId) {
    const response = await fetch(`${API_BASE_URL}/users/${userId}/loans`, {
        method: 'GET',
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to fetch loans');
    }

    return await response.json();
}

async function returnBook(loanId) {
    const response = await fetch(`${API_BASE_URL}/loans/${loanId}/return`, {
        method: 'PUT',
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to return book');
    }

    return await response.json();
}

async function createBook(bookData) {
    const response = await fetch(`${API_BASE_URL}/books`, {
        method: 'POST',
        headers: getAuthHeaders(),
        body: JSON.stringify(bookData)
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to create book');
    }

    return await response.json();
}

async function updateBook(bookId, bookData) {
    const response = await fetch(`${API_BASE_URL}/books/${bookId}`, {
        method: 'PUT',
        headers: getAuthHeaders(),
        body: JSON.stringify(bookData)
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to update book');
    }

    return await response.json();
}

async function deleteBook(bookId) {
    const response = await fetch(`${API_BASE_URL}/books/${bookId}`, {
        method: 'DELETE',
        headers: getAuthHeaders()
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error || 'Failed to delete book');
    }

    return true;
}
