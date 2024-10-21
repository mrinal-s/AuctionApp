

---

# BidRestAPI - .NET Core Auction Bidding API

## Overview

BidRestAPI is a .NET Core API designed for an auction bidding system. It provides functionalities for user authentication, auction management, bid placement, and more. The solution follows a layered architecture with controllers, business logic, and data repositories to handle various aspects of the auction system.

---

## Project Structure

### 1. **BidRestAPI**
   - **Controllers**: Contains the API endpoints responsible for handling HTTP requests and responses.
     - `AuctionController.cs`: Manages operations related to auctions, such as creating and managing auction items.
     - `AuthController.cs`: Handles user authentication (login, registration, and token generation).
     - `BidController.cs`: Manages bid-related operations, such as placing bids on auction items.
     - `UserController.cs`: Handles user-related operations, including registration and profile updates.
   - **Enums**: Holds enumerations used across the API for managing constant values (e.g., user roles, bid statuses).
   - **Exceptions**: Custom exception handling logic for managing application errors.
   - **Extension**: Utility and extension methods to enhance functionality.
   - **appsettings.json**: Configuration file for environment-specific settings like connection strings, authentication keys, etc.
   - **Program.cs**: The entry point of the application where services are configured and the application is run.

### 2. **BusinessLogic**
   - This layer contains the service interfaces and their implementations, defining the business logic of the application.
     - **Interfaces**: 
       - `IAuthService.cs`: Defines authentication-related methods (e.g., registration, login).
       - `IAuctionService.cs`: Defines methods for auction-related functionalities.
       - `IBidService.cs`: Methods related to bidding operations.
       - `IUserService.cs`: Manages user profile updates and retrieval.
       - `IHashingService.cs`: Contains methods for password hashing.
       - `ITokenService.cs`: Manages JWT token generation for authentication.
     - **Implementations**:
       - `AuthService.cs`: Implements user authentication and registration logic.
       - `AuctionService.cs`: Implements auction management logic.
       - `BidService.cs`: Implements bid placement logic.

### 3. **DataRepository**
   - This layer manages data access and storage operations. It contains repository services for handling database interactions.
     - `AuctionService.cs`: Manages auction-related database operations.
     - `AuthService.cs`: Manages user authentication and data storage (e.g., registering new users, updating user info).
     - `BidService.cs`: Handles bid-related data operations.

### 4. **BidRestApiTests**
   - Contains unit tests to ensure the functionality of the various components, services, and controllers in the project.

---

## Installation and Setup

### Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core)
- A running instance of SQL Server or an alternative database for storing data.
- (Optional) Tools like Postman to test the API endpoints.


 

---

## API Endpoints

### User Authentication
- **POST** `/api/auth/register` - Register a new user.
- **POST** `/api/auth/login` - Login and get a JWT token.

### Auctions
- **POST** `/api/auction` - Create a new auction item.
- **GET** `/api/auction/{id}` - Get details of an auction item.

### Bids
- **POST** `/api/bid` - Place a bid on an auction item.
- **GET** `/api/bid/{id}` - Get details of a specific bid.

---

## Running Tests

Navigate to the `BidRestApiTests` project and run the tests using the following command:

```bash
dotnet test
```

---

## Contribution Guidelines

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them with clear commit messages.
4. Push your changes and submit a pull request.

---

## License

This project is licensed under the MIT License. See the LICENSE file for more information.


