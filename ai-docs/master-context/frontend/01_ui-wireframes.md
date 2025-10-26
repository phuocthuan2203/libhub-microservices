### **Public User Flow (Unauthenticated)**

This flow covers the experience of a new or returning user before they have logged in.

### **Wireframe 1: Login Page**

- **Purpose:** To allow a returning user to authenticate and gain access to the system.

code Code

downloadcontent_copy

expand_less

```
    +------------------------------------------------------+
|                     LibHub                             |
|              Welcome Back! Please Login.             |
+------------------------------------------------------+
|                                                      |
|       Email Address: [___________________________]   |
|                                                      |
|            Password: [___________________________]   |
|                                                      |
|                     [    Login    ]                  |
|                                                      |
|     ----------------- or -----------------          |
|                                                      |
|          Don't have an account? [ Register Here ]    |
|                                                      |
+------------------------------------------------------+
```

- **Interaction Notes:**
    - Clicking **[ Login ]** submits the form.
    - Clicking **[ Register Here ]** navigates to the Registration Page.

---

### **Wireframe 2: Registration Page**

- **Purpose:** To allow a new user to create an account.

code Code

downloadcontent_copy

expand_less

```
    +------------------------------------------------------+
|                     LibHub                             |
|              Create Your New Account                 |
+------------------------------------------------------+
|                                                      |
|            Username: [___________________________]   |
|                                                      |
|       Email Address: [___________________________]   |
|                                                      |
|            Password: [___________________________]   |
|                                                      |
|     Confirm Password: [___________________________]   |
|                                                      |
|                     [   Register   ]                 |
|                                                      |
|     ----------------- or -----------------          |
|                                                      |
|        Already have an account? [ Login Here ]       |
|                                                      |
+------------------------------------------------------+
```

- **Interaction Notes:**
    - Clicking **[ Register ]** submits the registration form.
    - Clicking **[ Login Here ]** navigates back to the Login Page.

---

### **Authenticated User Flow (Role: "User")**

This flow covers the primary experience after a standard user has logged in.

### **Wireframe 3: Book Catalog Page (Main View)**

- **Purpose:** The main landing page for authenticated users. Allows browsing and searching for books.

code Code

downloadcontent_copy

expand_less

```
    +--------------------------------------------------------------------+
| LibHub                        [ Welcome, User! ] [My Profile] [My Loans] [Logout] |
+--------------------------------------------------------------------+
|                                                                    |
|  Search Catalog: [ Search by title, author, or ISBN...  ] [Search]  |
|                                                                    |
|  Results:                                                          |
|  +-------------------+  +-------------------+  +-------------------+ |
|  |    [Cover Art]    |  |    [Cover Art]    |  |    [Cover Art]    | |
|  | Title: Book One   |  | Title: Book Two   |  | Title: Book Three | |
|  | Author: A. Uthor  |  | Author: B. Writer |  | Author: C. Scribe | |
|  | [ View Details ]  |  | [ View Details ]  |  | [ View Details ]  | |
|  +-------------------+  +-------------------+  +-------------------+ |
|                                                                    |
|  ... more results ...                                              |
|                                                                    |
+--------------------------------------------------------------------+
```

- **Interaction Notes:**
    - Clicking **[ View Details ]** navigates to the Book Detail Page for that specific book.
    - Clicking **[My Profile]**, **[My Loans]**, or **[Logout]** navigates to the respective pages/actions.

---

### **Wireframe 4: Book Detail Page**

- **Purpose:** Shows all information for a single book and provides the primary action of borrowing.

code Code

downloadcontent_copy

expand_less

```
    +--------------------------------------------------------------------+
| LibHub                        [ Welcome, User! ] [My Profile] [My Loans] [Logout] |
+--------------------------------------------------------------------+
|                                                                    |
|  [ < Back to Catalog ]                                             |
|                                                                    |
|  +---------------+  Title: Book Two                                |
|  |   [Cover Art]   |  Author: B. Writer                              |
|  +---------------+  ISBN: 978-3-16-148410-0                        |
|                     Genre: Fiction                                 |
|                                                                    |
|  Description:                                                      |
|  A compelling story about a software engineer who discovers a      |
|  hidden secret within a legacy codebase...                         |
|                                                                    |
|  Copies Available: 3                                               |
|                                                                    |
|                     [    Borrow This Book    ]                      |
|                                                                    |
|  *Note: A success message appears after borrowing. Button is       |
|   disabled if 'Copies Available' is 0.                             |
|                                                                    |
+--------------------------------------------------------------------+
```

- **Interaction Notes:**
    - Clicking **[ Borrow This Book ]** initiates the borrow action.
    - Clicking **[ < Back to Catalog ]** navigates back to the Book Catalog Page.

---

### **Wireframe 5: My Loans Page**

- **Purpose:** Allows a user to view their current and past loans and to return books.

code Code

downloadcontent_copy

expand_less

```
    +--------------------------------------------------------------------+
| LibHub                        [ Welcome, User! ] [My Profile] [My Loans] [Logout] |
+--------------------------------------------------------------------+
|                                                                    |
|  [ < Back to Catalog ]                                             |
|                                                                    |
|  My Borrowing History                                              |
|                                                                    |
|  |--------------------|---------------|------------|--------------| |
|  | Book Title         | Checkout Date | Due Date   | Status/Action| |
|  |--------------------|---------------|------------|--------------| |
|  | Book One           | 2025-10-11    | 2025-10-25 | [ Return ]   | |
|  | Book Three         | 2025-10-15    | 2025-10-29 | [ Return ]   | |
|  | Another Book       | 2025-09-01    | 2025-09-15 | Returned     | |
|  |--------------------|---------------|------------|--------------| |
|                                                                    |
|  *Note: If no loans exist, a message "You have no borrowed         |
|   books." is displayed here.                                       |
|                                                                    |
+--------------------------------------------------------------------+
```

- **Interaction Notes:**
    - Clicking **[ Return ]** initiates the return action for that specific loan.

---

### **Admin User Flow (Role: "Admin")**

### **1. The Admin's Central Hub: The Book Catalog Page**

This is the primary interface where an admin oversees the collection and initiates all management actions. It looks similar to the user's view but is augmented with CRUD (Create, Read, Update, Delete) controls.

**Wireframe 6: Book Catalog Page (Admin View)**

code Code

downloadcontent_copy

expand_less

```
    +--------------------------------------------------------------------+
| LibHub (Admin Panel)  [ Welcome, Admin! ] [My Loans] [Logout] [Add New Book +] |
+--------------------------------------------------------------------+
|                                                                    |
|  Search Catalog: [ Search by title, author, or ISBN...  ] [Search]  |
|                                                                    |
|  Results:                                                          |
|  +-------------------+  +-------------------+  +-------------------+ |
|  |    [Cover Art]    |  |    [Cover Art]    |  |    [Cover Art]    | |
|  | Title: Book One   |  | Title: Book Two   |  | Title: Book Three | |
|  | Stock: 5/5 Avail. |  | Stock: 2/3 Avail. |  | Stock: 0/1 Avail. | |
|  | [View][Edit][Del] |  | [View][Edit][Del] |  | [View][Edit][Del] | |
|  +-------------------+  +-------------------+  +-------------------+ |
|                                                                    |
+--------------------------------------------------------------------+
```

- **Key Features:**
    - **Admin Panel Identifier:** Clearly indicates the user is in an administrative view.
    - **[Add New Book +]:** A primary call-to-action to begin the "Create" workflow.
    - **Stock Levels:** Admins see a clear "Available/Total" stock count for quick inventory assessment.
    - **Action Buttons ([View][Edit][Del]):** Each book entry has a set of controls for direct management.
- **Interaction Flow:**
    - Clicking **[Add New Book +]** navigates the admin to the "Add New Book" page.
    - Clicking **[View]** navigates to the standard "Book Detail Page" (Wireframe 4).
    - Clicking **[Edit]** navigates to the "Edit Book" page, pre-populating the form with that book's data.
    - Clicking **[Del]** triggers a confirmation dialog before proceeding with deletion.

---

### **2. The "Create" Workflow: Adding a New Book**

This flow is for expanding the library's collection.

**Wireframe 7: Add New Book Page**

code Code

downloadcontent_copy

expand_less

```
    +------------------------------------------------------+
|                     Add New Book                     |
|              [ < Back to Catalog ]                   |
+------------------------------------------------------+
|                                                      |
|                 ISBN*: [___________________________]  |
|                 Title*: [___________________________]  |
|                Author*: [___________________________]  |
|                 Genre: [___________________________]  |
|         Total Copies*: [_________] (#)                |
|                                                      |
|          Description: [ (Text Area for book summary) ]|
|                       [                             ]  |
|                       [                             ]  |
|                                                      |
|               [ Save Book ]  [ Cancel ]              |
|                                                      |
|  * Fields marked with an asterisk are required.      |
+------------------------------------------------------+
```

- **Key Features:**
    - **Clear Form Inputs:** Standard fields for all necessary book metadata.
    - **Required Field Indicators:** The UI clearly communicates which fields are mandatory for successful submission.
- **Interaction Flow:**
    - Admin fills out the form details. The UI should perform basic client-side validation (e.g., ensuring "Total Copies" is a number).
    - Clicking **[ Save Book ]** submits the form. The frontend sends a POST request to /api/books. Upon success, the user is redirected back to the Book Catalog, where the new book is now visible.
    - Clicking **[ Cancel ]** discards all changes and navigates back to the Book Catalog page.

---

### **3. The "Update" Workflow: Editing an Existing Book**

This flow is for correcting errors or updating information for a book already in the system.

**Wireframe 8: Edit Book Page**

code Code

downloadcontent_copy

expand_less

```
    +------------------------------------------------------+
|                     Edit Book: Book Two              |
|              [ < Back to Catalog ]                   |
+------------------------------------------------------+
|                                                      |
|                 ISBN*: [978-3-16-148410-0________]    |
|                Title*: [Book Two__________________]    |
|               Author*: [B. Writer________________]    |
|                 Genre: [Fiction_________________]    |
|         Total Copies*: [5________] (#)                |
|                                                      |
|          Description: [ (Text Area with content) ]    |
|                       [                             ]  |
|                                                      |
|             [ Update Book ]  [ Cancel ]              |
|                                                      |
+------------------------------------------------------+
```

- **Key Features:**
    - **Pre-populated Form:** The page loads with all fields already filled with the existing data for the selected book, making it easy to change only what's necessary.
- **Interaction Flow:**
    - Admin modifies the required fields.
    - Clicking **[ Update Book ]** submits the form. The frontend sends a PUT request to /api/books/{id}. On success, the user is redirected back to the Book Catalog.
    - Clicking **[ Cancel ]** discards changes and returns to the Book Catalog.

---

### **4. The "Delete" Workflow: Removing a Book**

This is a destructive action that requires explicit user confirmation to prevent accidental data loss.

**Wireframe 9: Deletion Confirmation Dialog**

*This is not a full page, but a modal dialog that appears over the Book Catalog page.*

code Code

downloadcontent_copy

expand_less

```
    +------------------------------------------------------+
| LibHub (Admin Panel)       [ Welcome, Admin! ] [My Loans] [Logout] [Add New Book +] |
+------------------------------------------------------+
|                                                      |
|  Search Catalog: [ Search by title, author, or ISBN...  ] [Search]  |
|                               +----------------------------------+
|  Results:                     |       Confirm Deletion           |
|  +-------------------+        |----------------------------------|
|  |    [Cover Art]    |        | Are you sure you want to delete  |
|  | Title: Book One   |        | "Book One"?                      |
|  | Stock: 5/5 Avail. |        |                                  |
|  | [View][Edit][Del] |        | This action cannot be undone.    |
|  +-------------------+        |                                  |
|                               | [ Confirm Delete ] [ Cancel ]    |
|                               +----------------------------------+
+--------------------------------------------------------------------+
```

- **Key Features:**
    - **Clear Warning:** The dialog explicitly states what is about to be deleted and that the action is permanent.
    - **Explicit Actions:** The buttons have clear, unambiguous labels.
- **Interaction Flow:**
    - This dialog appears after an admin clicks the **[Del]** button on any book in the catalog.
    - Clicking **[ Confirm Delete ]** closes the dialog and sends a DELETE request to /api/books/{id}. The catalog list then refreshes to show the book has been removed.
    - Clicking **[ Cancel ]** simply closes the dialog with no further action.