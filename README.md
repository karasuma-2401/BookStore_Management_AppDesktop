# 📚 Bookstore Management System

The "Bookstore Management System" is a comprehensive technological solution built on a modern Client-Server architecture. This system automates commercial processes ranging from inventory management and sales to complex operations like human resource management, timekeeping, and automated payroll calculation.

The Frontend application is developed using WPF (.NET 10.0), delivering a smooth and intuitive user interface, seamlessly integrated with a robust Backend powered by ASP.NET Core Web API.

## 🚀 Technology Stack

The system applies a clear separation between Frontend and Backend to ensure high security and maintainability:

### Frontend (Desktop Client)

- **Framework & Language:** WPF on the latest .NET 10.0 platform.
- **Architecture:** MVVM (Model-View-ViewModel) utilizing CommunityToolkit.Mvvm to strictly separate business logic and user interface.
- **User Interface (UI):** Lepo.co WPF UI library combined with Material Design to deliver a modern Navy Blue theme.

### Backend (Server)

- **Framework:** ASP.NET Core Web API acting as the central data processing hub.
- **Security & Authentication:** JSON Web Token (JWT) implemented with a stateless mechanism.
- **Real-time Processing:** SignalR integrated for instant, multi-threaded updates of invoice and payment statuses.
- **Data Validation:** FluentValidation ensures the integrity of incoming data.
- **Architecture:** Source code is organized following Clean Architecture principles with built-in Dependency Injection (DI).
- **Background Processing:** `IHostedService` (BackgroundService) automates attendance scanning and absence processing.

### Database & ORM

- **Database:** PostgreSQL deployed on the Neon cloud platform, supporting flexible serverless capabilities.
- **ORM:** Entity Framework Core (EF Core) optimizes database operations and dynamic LINQ queries.

---

## ✨ Key Features

The application provides comprehensive management functionalities, including:

- **Authentication & Authorization:** Secure login with encrypted passwords, role-based navigation, and authorization (Admin/Staff).
- **Book & Inventory Management:** Full CRUD operations for books, advanced search, and real-time inventory updates upon import/export transactions.
- **Sales & Promotions:** Flexible invoice creation, shopping cart support, Voucher discount application, and customer debt recording.
- **Payment & Debt Management:** Detailed customer profile management, debt tracking, receipt generation, and PDF/Excel invoice exporting.
- **HR & Timekeeping:** Visual shift scheduling, automated Kiosk check-ins, and automatic payroll calculation based on actual worked shifts and absence penalty rules.
- **Reporting & Statistics:** Automated monthly reports for revenue, inventory fluctuations, and debt, visualized with dynamic charts and Excel export support.

---

## ⚙️ CI/CD & Deployment

The project integrates a fully automated pipeline via GitHub Actions to control code quality and streamline deployment:

- **Continuous Integration (CI):** Automatically restores packages, builds the entire solution (`dotnet build`), and runs integration tests whenever changes are pushed to the main branch.
- **Continuous Deployment (CD):** Automatically builds in Release mode (`dotnet publish`) and packages the Desktop application into a `.zip` file containing the standalone `.exe` whenever a new Git Tag is created.
- **Automated Release:** The standalone installer is automatically pushed and distributed to end-users via GitHub Releases.
- **Flexible Configuration:** Easily modify the API server address via the `appsettings.json` file without recompiling the source code.

---

## 👥 Team Members & Task Assignment

This project was collaboratively developed by Team 06 over a 12-week period. Each member contributed 20% to the overall project.

| Member Name          | Student ID | Primary Responsibilities                                                                                                                                                            |
| :------------------- | :--------- | :---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Huỳnh Gia Thịnh**  | 24521680   | Feasibility Study, Software Requirements, Architecture Design, Process Design, Main UI & Navigation, Auth Module, Employee Module, Shift Module, Invoice Module, Regulation Module. |
| **Lê Minh Thắng**    | 24521603   | Feasibility Study, Software Requirements, Architecture Design, UI Design, Auth Module, Customer Module, Shift Module, Payment Module, **Software Testing**.                         |
| **Huỳnh Đơn Thuần**  | 24521736   | Feasibility Study, Software Requirements, Data Design, UI Design, Book Module, Customer Module, Report Module, Payment Module, **Deployment & Operations**.                         |
| **Trương Nhật Tiến** | 24521782   | Feasibility Study, Software Requirements, Class Diagram Design, UI Design, Book Module, Import Module, Report Module, Voucher Module.                                               |
| **Lê Hữu Trung**     | 24521882   | Feasibility Study, Software Requirements, Process Design, Employee Module, Import Module, Invoice Module, Voucher Module.                                                           |

---

## 🛠 Getting Started

### Prerequisites

- Visual Studio (2022 or 2026).
- .NET 10.0 SDK environment.
- PostgreSQL (Local installation or Neon Cloud Connection String).

### Installation & Setup

1. **Clone the Repository:**

```bash
   git clone https://github.com/karasuma-2401/BookStore_Management_AppDesktop.git
```

2. Backend Configuration:

- Navigate to the Backend directory of the project.
- Update your PostgreSQL connection string in the appsettings.json file.
- Open Terminal/Package Manager Console and run EF Core Migration commands to initialize the database.
- Run the Backend Server using Visual Studio or the dotnet run command.

3. Frontend Configuration (Desktop Client):

- Open the WPF application Solution using Visual Studio.
- Ensure the BaseUrl path connecting to the API in the Client's appsettings.json matches the port the Backend is listening on.
- Restore NuGet Packages using the dotnet restore command.
- Build and run the Desktop application.
