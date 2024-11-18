# E-Commerce Book Store

This is an E-Commerce Book Store built with ASP.NET Core MVC, .NET 8, C#, and Entity Framework Core (Code First). It includes features such as product management, user roles, secure online payments, and an admin panel to manage products, users, and orders.

## Features

### Product Management
- Users can browse and add products to their cart.
- Admin can manage product listings and update them in real-time.
- Real-time updates to products are handled through EF-Core.

### User Roles & Authentication
- Four types of user accounts: Admin, Employee, Customer, and Company.
- Secure authentication with Facebook and Microsoft login integrations.

### Admin Panel
- The Admin and Employee roles have access to an intuitive admin panel.
- Admins can update or delete products, manage orders, and handle user accounts.
- Real-time notifications using Toastr and SweetAlert.

### Online Payments
- Integrated Stripe for secure online payments, allowing users to make seamless transactions.

### Notifications
- Real-time notifications to enhance user interaction, including Toastr and SweetAlert for seamless alerts.

## Technologies Used

- **Frontend**: ASP.NET Core MVC
- **Backend**: ASP.NET Core MVC, Web API
- **Database**: SQL Server, Entity Framework Core (Code First)
- **Payment Integration**: Stripe
- **User Authentication**: Facebook Login, Microsoft Login
- **Design Patterns**: Repository, Unit of Work
- **Architecture**: N-Tier Architecture

## Prerequisites

To run this application, you will need the following:

- **.NET SDK**: Version 8.0 or later
- **SQL Server**: A local or remote SQL Server instance

## Getting Started

Follow these steps to set up and run the application.

### 1. Clone the Repository

Clone the repository to your local machine using Git:

```bash
git clone https://github.com/abdallaahmedsd/e-commerce-bookstore.git
```

### 2. Open the Project

Open the project in your preferred IDE. Visual Studio 2022 is recommended for full ASP.NET Core and EF Core support. Simply open the solution file (.sln) in Visual Studio.

### 3. Configure the Database

- Open the `appsettings.json` file located in the root of the project.
- Update the connection string under `"ConnectionStrings": { "DefaultConnection": "Your_SQL_Server_Connection_String" }` to match your SQL Server configuration.

### 4. Build the Application

Navigate to the project directory and build the application:

```bash
dotnet build
```

### 5. Run the Application

Simply run the application to initialize the database and start the server. The application is configured to check for any pending migrations on startup and apply them automatically if needed.

```bash
dotnet run
```

## Acknowledgments

- **Bhrugen Patel** for the excellent course on Udemy that helped build this project.
