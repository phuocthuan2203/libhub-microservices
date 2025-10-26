# LibHub System Architecture

## 1. High-Level Overview

The LibHub system is designed using a **microservices architecture**. This architecture decomposes the application into a suite of small, independent, and loosely coupled services. All communication from the external client is managed through a single entry point, the API Gateway. Services are designed to be resilient and scalable, using a Service Registry for dynamic discovery.

---

## 2. Core Components

The system consists of the following key components:

*   **Web Client (Frontend):** The user-facing application, built with HTML, CSS, and JavaScript. It is the sole initiator of requests into the system. It communicates **only** with the API Gateway.

*   **API Gateway:** This is the single entry point for all incoming client requests. Its responsibilities are:
    *   **Request Routing:** Forwarding requests to the appropriate downstream microservice.
    *   **Authentication:** Verifying JWTs to secure the system.
    *   **Service Discovery:** Querying the Service Registry to find the network locations of other services.

*   **Microservices:** Each service is an independent application with its own database, built using ASP.NET Core and Clean Architecture.
    *   `UserService`: Manages user identity, registration, and authentication.
    *   `CatalogService`: Manages the book catalog and inventory.
    *   `LoanService`: Manages the business logic for borrowing and returning books.

*   **Service Registry (Consul):** A central server that acts as a "phone book" for the services.
    *   **Registration:** On startup, each microservice registers its own location (IP and port) with Consul.
    *   **Discovery:** The API Gateway queries Consul at runtime to find out where to send requests.
    *   **Health Checking:** Consul monitors the health of each service instance.

---

## 3. Communication and Request Flow

The standard request lifecycle follows this pattern:

1.  **Registration:** On system startup, each microservice (`UserService`, `CatalogService`, `LoanService`) registers itself with the **Service Registry**.
2.  **Client Request:** The **Web Client** sends an HTTP request to the **API Gateway**.
3.  **Discovery & Routing:** The **API Gateway** receives the request, queries the **Service Registry** to get the current address of the target service (e.g., `CatalogService`), and then forwards the request to that address.
4.  **Inter-Service Communication (if needed):** A service may need to communicate with another service (e.g., `LoanService` calling `CatalogService`). This communication also uses the Service Registry for discovery.

---

## 4. Architecture Diagram (PlantUML)

The following PlantUML code describes the visual layout and relationships between the components.

```plantuml
@startuml
!theme plain
skinparam componentStyle uml2
title LibHub Architecture with Service Discovery

actor "Web Client" as Client

package "LibHub Ecosystem" {
  [API Gateway] as Gateway
  [Service Registry\n(Consul)] as Registry

  package "Microservices" {
    [CatalogService] as Catalog
    [UserService] as UserSvc
    [LoanService] as Loan
  }
}

Client --> Gateway : " "

' Step 2 & 3: Discovery and Routing
Gateway -> Registry : 2. Discover("CatalogService")
Registry --> Gateway : Returns IP:Port list
Gateway -> Catalog : 3. Route Request

' Step 1: Registration
Catalog ..> Registry : 1. Register
UserSvc ..> Registry : 1. Register
Loan ..> Registry : 1. Register

note right of Registry
  Services register on startup
  and send periodic heartbeats
  to prove they are healthy.
end note

@enduml