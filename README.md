# 📚 Hệ Thống Quản Lý Nhà Sách (Bookstore Management System)

Một ứng dụng Desktop hiện đại dành cho việc quản lý nhà sách, được xây dựng theo mô hình Client-Server. Dự án sử dụng WPF cho giao diện người dùng và ASP.NET Core Web API cho hệ thống backend, mang lại trải nghiệm mượt mà, giao diện mang phong cách hiện đại (Navy Blue theme) và hiệu suất cao.

## 🚀 Công Nghệ Sử Dụng

Dự án được phân chia rõ ràng giữa Frontend và Backend với các công nghệ sau:

### Frontend (Desktop Client)
* **Framework:** WPF (Windows Presentation Foundation)
* **Architecture Pattern:** MVVM (Model-View-ViewModel)
* **UI Library:** WPF UI / Material Design

### Backend (Server)
* **Framework:** ASP.NET Core Web API
* **Communication:** RESTful API
* **Real-time Communication:** SignalR (Nâng cao)
* **Authentication:** JWT (JSON Web Token)
* **Validation:** FluentValidation
* **Architecture:** Tích hợp sẵn Dependency Injection (DI)

### Database & ORM
* **Database:** PostgreSQL (Host trên Neon)
* **ORM:** Entity Framework Core (EF Core)

---

## ✨ Tính Năng Chính

Ứng dụng cung cấp các tính năng quản lý toàn diện bao gồm:

* **Xác thực người dùng:** Đăng nhập an toàn (Authentication).
* **Giao diện & Điều hướng:** Main Layout & Navigation trực quan.
* **Quản lý sách:** Thêm, đọc, sửa, xóa (CRUD for books) và bộ lọc tìm kiếm sách (Filter Books).
* **Quản lý giao dịch:** Lập hóa đơn bán sách (Book Sale Invoice) và Lập phiếu thu tiền (Cash receipt).
* **Quản lý nhân sự:** Hệ thống quản lý nhân viên (Employee management).
* **Báo cáo thống kê:** Tự động lập báo cáo tháng (Make Monthly Report).

---

## 🛠 Hướng Dẫn Cài Đặt (Getting Started)

### Yêu cầu hệ thống
* .NET 10.0 trở lên
* Visual Studio 2022
* PostgreSQL (hoặc có thể dùng trực tiếp connection string của Neon)

### Các bước chạy dự án

1. **Clone Repository:**
   ```bash
   git clone [https://github.com/](https://github.com/)[Tên-Tài-Khoản-Của-Bạn]/[Tên-Repo].git
