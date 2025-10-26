# UserService API Specification (OpenAPI 3.0)

This document defines the RESTful API contract for the `UserService`. This service is the sole authority for managing user identity, registration, and authentication within the LibHub system. All interactions related to user accounts must go through this API.

## API Contract (YAML)

The following is the formal, machine-readable API definition.

```yaml
openapi: 3.0.1
info:
  title: "UserService API"
  description: "Manages user identity, registration, and authentication for the LibHub system."
  version: "1.0.0"
servers:
  - url: http://localhost:5002
    description: Local development server

paths:
  /api/users/register:
    post:
      tags:
        - Users
      summary: "Register a new user account"
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/RegisterUserDto'
      responses:
        '201':
          description: "User created successfully"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/UserDto'
        '400':
          description: "Invalid input or user already exists"
          
  /api/users/login:
    post:
      tags:
        - Users
      summary: "Authenticates a user and returns a JWT"
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/LoginDto'
      responses:
        '200':
          description: "Authentication successful"
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/TokenDto'
        '401':
          description: "Invalid credentials"

  /api/users/{id}:
    get:
      tags:
        - Users
      summary: "Retrieves the profile for a specific user"
      security:
        - BearerAuth: []
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
                $ref: '#/components/schemas/UserDto'
        '401':
          description: "Unauthorized"
        '403':
          description: "Forbidden"
        '404':
          description: "User not found"

components:
  securitySchemes:
    BearerAuth:
      type: http
      scheme: bearer
      bearerFormat: JWT
  schemas:
    RegisterUserDto:
      type: object
      properties:
        username:
          type: string
        email:
          type: string
          format: email
        password:
          type: string
          format: password
    LoginDto:
      type: object
      properties:
        email:
          type: string
          format: email
        password:
          type: string
          format: password
    UserDto:
      type: object
      properties:
        userId:
          type: integer
        username:
          type: string
        email:
          type: string
    TokenDto:
      type: object
      properties:
        accessToken:
          type: string