# Frontend Architecture Context

## 1. High-Level Overview

The frontend is a **single-page application (SPA)** built with raw HTML, CSS, and JavaScript. It acts as the "Web Client" in the LibHub microservices ecosystem.

## 2. The Golden Rule of Communication

There is one critical rule for all frontend development:

**The frontend application MUST ONLY communicate with the API Gateway.**

It should have no knowledge of the individual microservices (`UserService`, `CatalogService`, etc.). The API Gateway is the single, unified entry point for all backend interactions. The base URL for all API calls will be the address of the API Gateway (e.g., `http://localhost:5000`).

## 3. System Diagram (Frontend Perspective)

The following diagram illustrates this relationship.

```plantuml
@startuml
!theme plain
skinparam componentStyle uml2
title Frontend Communication Architecture

package "Your Browser" {
  [Frontend Application\n(HTML, CSS, JS)] as WebApp
}

package "Backend System" {
    [API Gateway] as Gateway

    package "Internal Microservices" {
        [UserService]
        [CatalogService]
        [LoanService]
    }
}

WebApp --> Gateway : All API Calls (e.g., /api/books, /api/loans)

note right of WebApp
  The frontend is completely
  decoupled from the internal
  backend structure.
end note
@enduml