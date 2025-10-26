# Frontend Implementation Plan: Phase 1 - Scaffolding

## Task 1: Create the Frontend Project Structure and HTML Shell

**Objective:** Create the foundational file structure for a clean, vanilla JavaScript Single-Page Application (SPA) and implement the main `index.html` shell.

**Preamble:** You have been provided with the frontend master context documents. You are expected to have read and understood:
- `00_frontend-architecture-context.md`
- `01_ui-wireframes.md`

Your task is to create the necessary directories and files as specified below.

---
### **Step 1: Create the Project Directory and File Structure**

**Instructions:** Create the following directory and file structure within the `src/` folder of the monorepo. This will house the entire frontend application.

```bash
# Commands to be executed from the root of the monorepo

# 1. Create the main web application directory
mkdir -p src/LibHub.WebApp/css
mkdir -p src/LibHub.WebApp/js

# 2. Create the empty files
touch src/LibHub.WebApp/index.html
touch src/LibHub.WebApp/css/style.css
touch src/LibHub.WebApp/js/app.js
touch src/LibHub.WebApp/js/api.js
touch src/LibHub.WebApp/js/auth.js
touch src/LibHub.WebApp/js/ui.js
Step 2: Target File Structure

Instructions: After scaffolding, the project structure for the frontend should look like this. You will populate index.html in the next step.

code
Code
download
content_copy
expand_less
/src/LibHub.WebApp/
|-- index.html      <-- The single HTML page for the SPA
|-- css/
|   `-- style.css   <-- For all CSS styles
`-- js/
    |-- app.js      <-- Main application logic, routing, and event handling
    |-- api.js      <-- All fetch() calls to the backend API Gateway
    |-- auth.js     <-- JWT handling (saving, retrieving, deleting token)
    `-- ui.js       <-- Functions for rendering HTML components into the DOM
Step 3: Implement the HTML Shell (index.html)

File to create/modify: src/LibHub.WebApp/index.html

Instructions: Implement a basic HTML5 document. This will be the only HTML page for the entire application. It should contain a header, a main content area that will be dynamically updated by JavaScript, and script tags to load all the JavaScript modules.

code
Html
play_circle
download
content_copy
expand_less
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>LibHub</title>
    <link rel="stylesheet" href="css/style.css">
</head>
<body>
    <header>
        <div class="container">
            <h1>LibHub</h1>
            <nav id="main-nav">
                <!-- Navigation links will be dynamically inserted here by ui.js -->
            </nav>
        </div>
    </header>

    <main id="app-root" class="container">
        <!-- All dynamic content (login forms, book lists, etc.) will be rendered here by ui.js -->
    </main>

    <footer>
        <div class="container">
            <p>&copy; 2025 LibHub Microservices Project</p>
        </div>
    </footer>

    <!-- Load JavaScript files. Order can be important. -->
    <script src="js/auth.js"></script>
    <script src="js/api.js"></script>
    <script src="js/ui.js"></script>
    <script src="js/app.js"></script>
</body>
</html>
Step 4: Add Placeholder Content to JavaScript and CSS Files

Instructions: To ensure the files are set up correctly, add placeholder comments to each of the newly created .js and .css files.

css/style.css
code
CSS
download
content_copy
expand_less
/*
 * LibHub Main Stylesheet
 */

body {
    font-family: sans-serif;
    margin: 0;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 1rem;
}

/* Add more base styles as needed */
js/app.js
code
JavaScript
download
content_copy
expand_less
//
// Main application logic, router, and event listeners.
// This is the entry point for the frontend application.
//
document.addEventListener('DOMContentLoaded', () => {
    console.log('LibHub App Initialized');
    // TODO: Add initial routing logic here (e.g., check if logged in).
});
js/api.js
code
JavaScript
download
content_copy
expand_less
//
// Handles all communication with the backend API Gateway.
//
const API_BASE_URL = 'http://localhost:5000/api'; // The address of the API Gateway

// TODO: Implement functions for login, register, getBooks, etc.
js/auth.js
code
JavaScript
download
content_copy
expand_less
//
// Manages JWT authentication token.
//
const TOKEN_KEY = 'libhub_jwt';

// TODO: Implement saveToken, getToken, removeToken, etc.
js/ui.js
code
JavaScript
download
content_copy
expand_less
//
// Contains functions for rendering HTML content into the DOM.
//
function renderLoginForm() {
    // TODO: Return the HTML string for the login form.
}

// TODO: Implement other render functions (renderRegisterForm, renderBookList, etc.)
Task 2: Generate Implementation Report

Objective: After completing the scaffolding, generate a report.

File to create: ai-docs/implementation/frontend/01-scaffolding-report.md

Instructions: Use the following template.

Report Template
code
Markdown
download
content_copy
expand_less
# Frontend Implementation Report: Phase 1 - Scaffolding

## 1. Completed Tasks
- [x] Created the main project directory `src/LibHub.WebApp`.
- [x] Created the CSS and JS subdirectory structure.
- [x] Created the core application files: `index.html`, `style.css`, `app.js`, `api.js`, `auth.js`, and `ui.js`.
- [x] Implemented the main HTML shell in `index.html` with the `#app-root` for dynamic content.
- [x] Added placeholder comments and base code to all CSS and JavaScript files.

## 2. Pending Tasks & Notes
- The project is now fully scaffolded and ready for the implementation of the public components (Login/Registration) in the next phase.
