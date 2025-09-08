# AutomationAndIntegration
│
│
├── Data/
│   ├── WebshopContext.cs   # EF Core DbContext
│   └── SeedData.cs         # Init testdata
│
│
├── Models/
│   ├── User.cs
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
│
│
├── Services/
│   ├── AuthService.cs      # Login/registration
│   ├── UserService.cs      # Handling of Users
│   └── ProductService.cs   # Handling of products
│
├── Helpers/
│   └── MenuHelper.cs		# Helpers for rendering menu and logics
│
└── Program.cs              # Main for starting system