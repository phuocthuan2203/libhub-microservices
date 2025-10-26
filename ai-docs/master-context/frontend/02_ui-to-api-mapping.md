### **Action 5.2: Map UI Actions to Backend API Calls**

This table details the specific API call that the client-side JavaScript application will make to the **API Gateway** in response to a user's action.

### **Public User Flow (Unauthenticated)**

| User Action / Trigger | UI Location / Wireframe | API Call (to Gateway) | Request Details |
| --- | --- | --- | --- |
| Submits the login form | **Login Page** | POST /api/users/login | **Body:** { "email": "...", "password": "..." } |
| Submits the registration form | **Registration Page** | POST /api/users/register | **Body:** { "username": "...", "email": "...", "password": "..." } |

---

### **Authenticated User Flow (Role: "User")**

*All subsequent requests in this flow must include the JWT received from the login step in the request header.*

| User Action / Trigger | UI Location / Wireframe | API Call (to Gateway) | Request Details |
| --- | --- | --- | --- |
| **Page Loads** after login | **Book Catalog Page** | GET /api/books | **Headers:** Authorization: Bearer <token> |
| Submits a search query | **Book Catalog Page** | GET /api/books?search=... | **Headers:** Authorization: Bearer <token> |
| Clicks **"View Details"** | **Book Catalog Page** | GET /api/books/{id} | **URL Param:** id of the selected book.<br>**Headers:** Authorization: Bearer <token> |
| Clicks **"Borrow This Book"** | **Book Detail Page** | POST /api/loans | **Body:** { "bookId": ... }<br>**Headers:** Authorization: Bearer <token> |
| **Page Loads** | **My Loans Page** | GET /api/users/{userId}/loans | **URL Param:** userId of the logged-in user.<br>**Headers:** Authorization: Bearer <token> |
| Clicks the **"Return"** button | **My Loans Page** | PUT /api/loans/{id}/return | **URL Param:** id of the specific loan to be returned.<br>**Headers:** Authorization: Bearer <token> |
| Clicks **"Logout"** | Any Authenticated Page | (No API Call) | Client-side action to delete the stored JWT and redirect to the Login Page. |

---

### **Admin User Flow (Role: "Admin")**

*All requests in this flow require a JWT with an "Admin" role claim.*

| User Action / Trigger | UI Location / Wireframe | API Call (to Gateway) | Request Details |
| --- | --- | --- | --- |
| Clicks the **"Add New Book +"** button | **Admin Catalog Page** | (UI Navigation) | Navigates to the "Add New Book" form. No API call is made yet. |
| Submits the form to create a book | **Add New Book Page** | POST /api/books | **Body:** { "isbn": "...", "title": "...", "author": "...", "totalCopies": ... }<br>**Headers:** Authorization: Bearer <token> |
| Clicks the **"Edit"** button | **Admin Catalog Page** | GET /api/books/{id} | **URL Param:** id of the selected book. Fetches data to pre-populate the edit form.<br>**Headers:** Authorization: Bearer <token> |
| Submits the form to update a book | **Edit Book Page** | PUT /api/books/{id} | **URL Param:** id of the book being edited.<br>**Body:** { "title": "...", "author": "...", ... }<br>**Headers:** Authorization: Bearer <token> |
| Clicks **"Confirm Delete"** in the dialog | **Admin Catalog Page (Modal)** | DELETE /api/books/{id} | **URL Param:** id of the book to be deleted.<br>**Headers:** Authorization: Bearer <token> |

This mapping table completes the end-to-end design of the application's functionality. It serves as the definitive bridge between the frontend and backend, ensuring both development teams are aligned on how the system communicates.