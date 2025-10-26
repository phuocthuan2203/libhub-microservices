# CatalogService API Specification (OpenAPI 3.0)

This document defines the RESTful API contract for the `CatalogService`. This service is the single source of truth for all book and inventory information in the LibHub system. It handles creating, reading, updating, and deleting books, as well as managing stock levels.

## API Contract (YAML)

The following is the formal, machine-readable API definition.

```yaml
openapi: 3.0.1
info:
  title: "CatalogService API"
  description: "Manages the library's book collection and inventory."
  version: "1.0.0"
servers:
  - url: http://localhost:5001
    description: Local development server

paths:
  /api/books:
    post:
      tags:
        - Books
      summary: "Adds a new book to the catalog"
      security:
        - BearerAuth: [] # Admin role required
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/CreateBookDto'
      responses:
        '201':
          description: "Book created successfully"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/BookDto'
    get:
      tags:
        - Books
      summary: "Retrieves a list of all books"
      responses:
        '200':
          description: "Successful operation"
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/BookDto'

  /api/books/{id}:
    get:
      tags:
        - Books
      summary: "Retrieves details for a single book"
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: integer
      responses:
        '200':
          description: "Successful operation"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/BookDto'
        '404':
          description: "Book not found"
    put:
      tags:
        - Books
      summary: "Updates the details of an existing book"
      security:
        - BearerAuth: [] # Admin role required
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: integer
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/UpdateBookDto'
      responses:
        '204':
          description: "Book updated successfully"
    delete:
      tags:
        - Books
      summary: "Deletes a book from the catalog"
      security:
        - BearerAuth: [] # Admin role required
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: integer
      responses:
        '204':
          description: "Book deleted successfully"

  /api/books/{id}/stock:
    put:
      tags:
        - Internal
      summary: "Updates the stock count of a book"
      description: "Internal endpoint used by LoanService during borrow/return operations."
      security:
        - BearerAuth: [] # Authenticated service call
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: integer
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/UpdateStockDto'
      responses:
        '204':
          description: "Stock updated successfully"

components:
  securitySchemes:
    BearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT
  schemas:
    BookDto:
      type: object
      properties:
        bookId:
          type: integer
        isbn:
          type: string
        title:
          type: string
        author:
          type: string
        genre:
          type: string
        totalCopies:
          type: integer
        availableCopies:
          type: integer
    CreateBookDto:
      type: object
      properties:
        isbn:
          type: string
        title:
          type: string
        author:
          type: string
        totalCopies:
          type: integer
    UpdateBookDto:
      type: object
      properties:
        title:
          type: string
        author:
          type: string
        genre:
          type: string
        description:
          type: string
    UpdateStockDto:
      type: object
      properties:
        changeAmount:
          type: integer
          description: "The amount to change the stock by, e.g., -1 for borrow, +1 for return."