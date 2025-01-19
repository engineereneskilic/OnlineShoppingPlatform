
# Multiple Layered ASP.NET Core API Project - Online Shopping Platform

## Table of Contents
1. [Introduction](#introduction)
2. [Technologies Used](#technologies-used)
3. [Project Structure](#project-structure)
   - [Presentation Layer](#presentation-layer)
   - [Business Layer](#business-layer)
   - [Data Access Layer](#data-access-layer)
4. [Key Features](#key-features)
   - [Data Models](#data-models)
   - [Authentication and Authorization](#authentication-and-authorization)
   - [Middleware](#middleware)
   - [Action Filters](#action-filters)
   - [Model Validation](#model-validation)
   - [Dependency Injection](#dependency-injection)
   - [Exception Handling](#exception-handling)
5. [Output Examples](#output-examples)
   - [Orders Output](#orders-output)
   - [Products Output](#products-output)
   - [Users Output](#users-output)
   - [Maintenance Output](#maintenance-output)
6. [Technical Details](#technical-details)
   - [Repository Pattern](#repository-pattern)
   - [Unit of Work Pattern](#unit-of-work-pattern)
   - [Caching and Pagination](#caching-and-pagination)
7. [Setup and Usage](#setup-and-usage)
8. [API Endpoints](#api-endpoints)
   - [Users](#users)
   - [Products](#products)
   - [Orders](#orders)
   - [Maintenance](#maintenance)
9. [Future Enhancements](#future-enhancements)
10. [License](#license)

---

## Introduction

This **Multiple Layered ASP.NET Core API Project** is a comprehensive solution for managing an **Online Shopping Platform**. It features a **clean architecture**, leveraging **Entity Framework Core**, **JWT Authentication**, and **custom middleware** to ensure robust functionality and scalability. The project showcases the application of advanced development principles such as **Repository Pattern**, **Unit of Work Pattern**, and **Dependency Injection**, making it a prime example of a maintainable and scalable backend architecture.

---

## Technologies Used

- **Framework**: ASP.NET Core 7.0
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core (Code First)
- **Authentication**: JSON Web Token (JWT)
- **Middleware**: Custom Logging and Maintenance Middleware
- **Validation**: FluentValidation for request validation
- **Caching and Pagination**: Improves performance for large datasets
- **Design Patterns**:
  - Repository Pattern
  - Unit of Work Pattern
  - Singleton Pattern

---

## Project Structure

### **1. Presentation Layer**
- **Responsibilities**:
  - Handles HTTP requests via **Controllers**.
  - Processes requests and routes them to the appropriate service.
  - Implements global exception handling and logging.
- **Examples**:
  - UserController: Handles user registration and authentication.
  - OrderController: Manages order creation and retrieval.

### **2. Business Layer**
- **Responsibilities**:
  - Contains the core application logic.
  - Ensures validation and business rules are enforced.
  - Communicates with the Data Access Layer.
- **Services**:
  - UserService: Validates user input, encrypts passwords, and generates JWT tokens.
  - OrderService: Ensures stock availability and calculates the total amount of orders.

### **3. Data Access Layer**
- **Responsibilities**:
  - Manages database interactions via **Entity Framework Core**.
  - Encapsulates CRUD operations using the **Repository Pattern**.
  - Ensures atomic transactions using the **Unit of Work Pattern**.
- **Example Repositories**:
  - UserRepository: Fetches and updates user data.
  - OrderRepository: Handles order-related database operations.

---

## Key Features

### **1. Data Models**
#### **User**
- **Properties**:
  - Id: Primary key.
  - FirstName, LastName, Email: Personal details.
  - Role: Defines user permissions (e.g., Admin, Customer).
  - Password: Encrypted using **Data Protection**.
- **Role Management**:
  - Admins manage users, products, and orders.
  - Customers place orders and view their data.

#### **Product**
- **Properties**:
  - Id, ProductName: Identification details.
  - Price, StockQuantity: Tracks inventory.

#### **Order**
- **Properties**:
  - OrderDate, TotalAmount: Transaction details.
  - Associated with multiple products using the OrderProduct relationship.

---

### **2. Authentication and Authorization**
- Implements **JWT Authentication** to secure endpoints.
- Supports role-based access:
  - **Admin**: Full access to the API.
  - **Customer**: Restricted to order and product-related endpoints.

---

### **3. Middleware**
#### **Logging Middleware**:
Logs each incoming request with:
- Timestamp
- URL
- User ID (if authenticated)

#### **Maintenance Middleware**:
- Checks if the system is in maintenance mode.
- Blocks incoming requests and returns a maintenance message.

---

### **4. Action Filters**
- Restricts access to certain endpoints based on time intervals.
- Example: Allow product updates only between 9:00 AM - 6:00 PM.

---

### **5. Model Validation**
- Uses **FluentValidation**:
  - Ensures email is valid.
  - Prevents negative stock quantities.
  - Enforces required fields.

---

### **6. Dependency Injection**
- Services and repositories are injected using ASP.NET Core's built-in DI container:

```csharp
services.AddScoped<IUserService, UserService>();
services.AddScoped<IOrderRepository, OrderRepository>();
```

---

### **7. Exception Handling**
- Implements global exception handling:
  - Captures and logs unhandled exceptions.
  - Returns a consistent error response format.

---

## Output Examples

### **1. Orders Output**
```json
[
  {
    "orderId": 1,
    "orderDate": "2025-01-14T21:43:48.0421237",
    "orderStatus": 1,
    "products": [
      {
        "productId": 1,
        "productName": "Iphone 16 Pro Max",
        "unitPrice": 120000,
        "quantity": 3
      }
    ],
    "totalAmount": 240000
  }
]
```
### **2. Products Output**
```json
[
  {
    "productId": 1,
    "productName": "Iphone 16 Pro Max",
    "price": 120000,
    "stockQuantity": 75
  },
  {
    "productId": 2,
    "productName": "Samsung Galaxy S25 Ultra",
    "price": 110000,
    "stockQuantity": 65
  }
]
```
### **3. Users Output**
```json
[
  {
    "userId": 1,
    "userName": "eneskilic",
    "email": "enes@example.com",
    "firstName": "Enes",
    "lastName": "Kılıç",
    "phoneNumber": "053666",
    "birthDate": "1998-03-25T00:00:00",
    "userType": 0
  }
]
```

### **4. Maintenance Output**
```json
[
  {
    "maintenanceId": -1,
    "isActive": false,
    "startTime": "2025-01-16T13:30:00",
    "endTime": "2025-01-16T19:00:00",
    "message": "Initial maintenance setup"
  }
]
```

---

## API Endpoints

### **Users**
| HTTP Method | Endpoint             | Description                          | Authentication |
|-------------|----------------------|--------------------------------------|----------------|
| POST        | /api/users/register | Registers a new user                 | No             |
| POST        | /api/users/login    | Logs in and generates a JWT token    | No             |
| GET         | /api/users          | Retrieves all users (Admin only)     | Admin          |
| GET         | /api/users/{id}     | Retrieves a specific user by ID      | Admin          |
| DELETE      | /api/users/{id}     | Deletes a specific user (Admin only) | Admin          |

### **Products**
| HTTP Method | Endpoint             | Description                          | Authentication |
|-------------|----------------------|--------------------------------------|----------------|
| GET         | /api/products        | Retrieves a paginated list of products | No           |
| GET         | /api/products/{id}   | Retrieves a specific product by ID   | No             |
| POST        | /api/products        | Adds a new product (Admin only)      | Admin          |
| PUT         | /api/products/{id}   | Updates an existing product (Admin only) | Admin     |
| DELETE      | /api/products/{id}   | Deletes a product (Admin only)       | Admin          |

### **Orders**
| HTTP Method | Endpoint             | Description                          | Authentication |
|-------------|----------------------|--------------------------------------|----------------|
| GET         | /api/orders          | Retrieves all orders for the current user | Customer  |
| GET         | /api/orders/{id}     | Retrieves a specific order by ID     | Customer       |
| POST        | /api/orders          | Places a new order                   | Customer       |
| DELETE      | /api/orders/{id}     | Cancels a specific order             | Customer       |

### **Maintenance**
| HTTP Method | Endpoint                   | Description                                  | Authentication |
|-------------|----------------------------|----------------------------------------------|----------------|
| GET         | /api/maintenance           | Retrieves maintenance mode status           | No             |
| POST        | /api/maintenance           | Enables maintenance mode (Admin only)       | Admin          |
| DELETE      | /api/maintenance           | Disables maintenance mode (Admin only)      | Admin          |
| PUT         | /api/maintenance/toggle/{id}| Toggles maintenance mode status by ID       | Admin          |
| PUT         | /api/maintenance/toggle    | Toggles the status of the most recent entry | Admin          |

---

## Future Enhancements
- Add support for third-party payment integration.
- Implement real-time notifications for order status updates.
- Enhance reporting capabilities with advanced analytics.

---

## License
This project is licensed under the MIT License.
