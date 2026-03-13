# Girls Accommodation Room Management System (Girls RMS)

A secure, role-based web application built with **ASP.NET Core MVC (.NET 8)** to manage accommodation rooms for girl employees in a company/hostel setting.

## Features

### Roles & Permissions
- **Admin** — Full system control
  - Create/Edit/Delete rooms
  - Auto-generate beds on room creation
  - Create Warden & User accounts
  - Assign/Transfer/Remove room & bed allocations
  - View all complaints & statistics

- **Warden** — Daily operations
  - View all rooms & occupancy
  - Assign/Transfer girls to rooms/beds
  - Manage (view/update status) all complaints

- **User (Girl Employee)** — Personal access
  - View assigned room, bed & roommates
  - Submit & track personal maintenance complaints

### Key Modules
- Room & Bed Management (with automatic bed creation)
- Room Allocation & Transfer (with vacancy checks & validation)
- Complaint System (submit, track, update status)
- Role-based Authentication & Authorization (ASP.NET Core Identity)
- Responsive, modern UI with Bootstrap 5 + animations
- Dashboard statistics per role (total rooms, occupied/vacant beds, pending complaints, etc.)

## Technology Stack

- **Backend**: ASP.NET Core MVC (.NET 8)
- **Database**: SQL Server + Entity Framework Core
- **Authentication**: ASP.NET Core Identity (with roles: Admin, Warden, User)
- **Frontend**: Bootstrap 5, Bootstrap Icons, Animate.css
- **ORM**: Entity Framework Core (Code-First)
- **Other**: Chart.js (optional), Font Awesome (optional)

## Project Structure
GirlsAccommodationRMS/
├── Controllers/
│   ├── AccountController.cs
│   ├── DashboardController.cs
│   ├── RoomController.cs
│   ├── AllocationController.cs
│   ├── ComplaintController.cs
│   ├── MyRoomController.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── Room.cs
│   ├── Bed.cs
│   ├── RoomAllocation.cs
│   ├── Complaint.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── AdminDashboardViewModel.cs
│   ├── WardenDashboardViewModel.cs
│   ├── UserDashboardViewModel.cs
│   ├── AllocationViewModel.cs
│   ├── TransferViewModel.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Views/
│   ├── Home/
│   ├── Account/
│   ├── Dashboard/
│   ├── Room/
│   ├── Allocation/
│   ├── Complaint/
│   ├── MyRoom/
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
└── appsettings.json



## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022/2025 or VS Code

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/yourusername/GirlsAccommodationRMS.git
cd GirlsAccommodationRMS

dotnet restore

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GirlsRMS;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}

dotnet ef migrations add InitialCreate
dotnet ef database update
