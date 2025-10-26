# API Gateway Contract

This document provides a consolidated list of all API endpoints available to the frontend, as exposed by the API Gateway. This is the **data contract** for all frontend development.

---
### **Authentication Endpoints (Public)**

*   **`POST /api/users/register`**
    *   **Description:** Creates a new user account.
    *   **Request Body:** `{ "username": "...", "email": "...", "password": "..." }`
    *   **Response Body (Success):** `{ "userId": 1, "username": "...", "email": "..." }`

*   **`POST /api/users/login`**
    *   **Description:** Authenticates a user.
    *   **Request Body:** `{ "email": "...", "password": "..." }`
    *   **Response Body (Success):** `{ "accessToken": "ey..." }`

---
### **Catalog Endpoints (Requires Auth)**

*   **`GET /api/books`**
    *   **Description:** Gets a list of all books.
    *   **Response Body (Success):** `[ { "bookId": 1, "title": "...", "author": "..." }, ... ]`

*   **`GET /api/books/{id}`**
    *   **Description:** Gets details for a single book.
    *   **Response Body (Success):** `{ "bookId": 1, "title": "...", "author": "...", "availableCopies": 3 }`

*   **`POST /api/books`** `(Admin Only)`
    *   **Description:** Creates a new book.
    *   **Request Body:** `{ "isbn": "...", "title": "...", "author": "...", "totalCopies": 5 }`

*   **`PUT /api/books/{id}`** `(Admin Only)`
    *   **Description:** Updates an existing book.
    *   **Request Body:** `{ "title": "...", "author": "..." }`

*   **`DELETE /api/books/{id}`** `(Admin Only)`
    *   **Description:** Deletes a book.

---
### **Loan Endpoints (Requires Auth)**

*   **`POST /api/loans`**
    *   **Description:** Borrows a book for the logged-in user.
    *   **Request Body:** `{ "bookId": 123 }`
    *   **Response Body (Success):** `{ "loanId": 1, "userId": 1, "bookId": 123, "status": "CheckedOut", ... }`

*   **`PUT /api/loans/{id}/return`**
    *   **Description:** Returns a previously borrowed book.

*   **`GET /api/users/{userId}/loans`**
    *   **Description:** Gets the loan history for a specific user.
    *   **Response Body (Success):** `[ { "loanId": 1, ... }, { "loanId": 2, ... } ]`