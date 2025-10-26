# LibHub Database Schemas

## 1. Core Architectural Principle: Database per Service

This document defines the database schemas for the LibHub microservices. It is **critically important** to understand that these schemas represent **three separate, independent databases**.

The architecture follows the **Database per Service** pattern. This means:
*   Each microservice is the sole owner of its own data and database schema.
*   There are **no foreign key relationships (`Ref`) or direct links** between the tables of different services at the database level.
*   Data consistency and relationships across services are managed at the **application layer** through API calls (e.g., using the Saga pattern), not through the database.

The AI agent must adhere to this principle when generating EF Core entities and `DbContext` configurations.

---

## 2. Database Schemas (DBML)

The following is the formal, machine-readable definition for each service's database schema, written in DBML (Database Markup Language).

### Schema 1: `UserService` (`user_db`)

This schema is the single source of truth for user identity, credentials, and roles.

```dbml
// Database: user_db
// Description: Manages user accounts, authentication, and profiles.

Table Users {
  user_id integer [pk, increment]
  username varchar(100) [unique, not null]
  email varchar(255) [unique, not null]
  hashed_password varchar(255) [not null]
  role varchar(50) [not null, default: 'User', note: "e.g., 'User', 'Admin'"]
  created_at timestamp [not null, default: `now()`]
  updated_at timestamp
}

Schema 2: CatalogService (catalog_db)
This schema is the authority for all book information, including descriptive metadata and inventory stock levels.
code
Dbml
// Database: catalog_db
// Description: Manages the library's book collection and inventory.

Table Books {
  book_id integer [pk, increment]
  isbn varchar(13) [unique, not null, note: 'Unique business identifier for the book edition']
  title varchar(255) [not null]
  author varchar(255) [not null]
  genre varchar(100)
  description text
  total_copies integer [not null, default: 1, note: 'The total number of physical copies owned by the library']
  available_copies integer [not null, default: 1, note: 'Copies currently available for loan. Must be <= total_copies']
  created_at timestamp [not null, default: `now()`]
  updated_at timestamp
}

Schema 3: LoanService (loan_db)
This schema manages the transactional data of borrowing and returning books. It holds references to entities in other services by their ID only, not as database foreign keys.
code
Dbml
// Database: loan_db
// Description: Manages the lifecycle of book loans.

Table Loans {
  loan_id integer [pk, increment]
  user_id integer [not null, note: 'This is a REFERENCE, not a foreign key. It points to a User in the UserService.']
  book_id integer [not null, note: 'This is a REFERENCE, not a foreign key. It points to a Book in the CatalogService.']
  status varchar(50) [not null, note: 'e.g., CheckedOut, Returned, PENDING']
  checkout_date timestamp [not null, default: `now()`]
  due_date timestamp [not null]
  return_date timestamp [note: 'Null until the book is returned']
}