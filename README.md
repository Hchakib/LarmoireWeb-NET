# LarmoireWeb-NET

🪑 **LarmoireWeb** is an ASP.NET Core MVC web application built for an artisan woodworker to manage an online furniture store and renovation requests.

## 🔧 Features

- 🛒 Product listing, cart, and checkout
- 🧑‍🔧 Renovation request form
- 🛠️ Admin dashboard to manage products, categories, users, and orders
- 🔐 Role-based access (Admin vs Client)
- 🛍️ Order and cart management with session persistence

## 🧱 Project Structure

- `Controllers/` – Logic for routing requests (Products, Orders, Users, etc.)
- `Models/` – Entity classes (Product, User, Order, etc.)
- `Views/` – Razor Pages (dynamic HTML templates)
- `Services/` – Business logic layers
- `wwwroot/` – Static content (images, CSS, JS)
- `Data/` – EF Core DB context and seeders
- `appsettings.json` – Config settings

## 🛠️ Technologies

- ASP.NET Core MVC (.NET 6+)
- Entity Framework Core
- SQL Server
- Bootstrap 5
- Identity (User authentication and roles)

## 🚀 Getting Started

1. **Clone the project:**
   ```bash
   git clone https://github.com/Hchakib/LarmoireWeb-NET.git
