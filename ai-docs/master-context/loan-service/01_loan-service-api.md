# LoanService API Specification (OpenAPI 3.0)

This document defines the RESTful API contract for the `LoanService`. This service orchestrates the core business process of borrowing and returning books. It manages the state of all loans and communicates with other services to ensure data consistency.

## API Contract (YAML)

The following is the formal, machine-readable API definition.

```yaml
openapi: 3.0.1
info:
  title: "LoanService API"
  description: "Manages the business process of borrowing and returning books."
  version: "1.0.0"
servers:
  - url: http://localhost:5003
    description: Local development server

paths:
  /api/loans:
    post:
      tags:
        - Loans
      summary: "Borrows a book for the authenticated user"
      security:
        - BearerAuth: []
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/CreateLoanDto'
      responses:
        '201':
          description: "Loan created successfully"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/LoanDto'
        '404':
          description: "Book not found"
        '409':
          description: "Book is not available for loan (out of stock)"

  /api/loans/{id}/return:
    put:
      tags:
        - Loans
      summary: "Returns a borrowed book"
      security:
        - BearerAuth: []
      parameters:
        - name: id
          in: path
          required: true
          schema:
            type: integer
      responses:
        '204':
          description: "Book returned successfully"
        '404':
          description: "Loan not found"

  /api/users/{userId}/loans:
    get:
      tags:
        - Loans
      summary: "Retrieves the borrowing history for a specific user"
      security:
        - BearerAuth: []
      parameters:
        - name: userId
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
                type: array
                items:
                  $ref: '#/components/schemas/LoanDto'

components:
  securitySchemes:
    BearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT
  schemas:
    CreateLoanDto:
      type: object
      properties:
        bookId:
          type: integer
    LoanDto:
      type: object
      properties:
        loanId:
          type: integer
        userId:
          type: integer
        bookId:
          type: integer
        status:
          type: string
          enum: [CheckedOut, Returned, FAILED]
        checkoutDate:
          type: string
          format: date-time
        dueDate:
          type: string
          format: date-time
        returnDate:
          type: string
          format: date-time