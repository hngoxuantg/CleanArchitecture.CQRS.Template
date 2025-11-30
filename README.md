# Web API Clean Architecture with CQRS & Shared Services

Clean architecture Web API template with advanced CQRS pattern, MediatR, Shared Business Services, JWT authentication and refresh token implementation.

## Overview

.NET 10 Web API enterprise template implementing clean architecture with CQRS (Command Query Responsibility Segregation) pattern, MediatR, **Shared Business Services**, JWT authentication, refresh tokens, repository pattern, and production-grade patterns.

This template implements CQRS by **separating business logic into Shared Services**, keeping handlers thin and focused solely on orchestration.

## Features

- **Clean Architecture** - Domain, Application, Infrastructure, API layers
- **Advanced CQRS Pattern** - Command/Query with **Shared Business Services**
- **Separation of Concerns** - Handlers orchestrate, Services contain business logic
- **Single Responsibility Principle** - Each handler, service, and class has one clear purpose
- **JWT Authentication** - Access tokens with refresh token mechanism
- **Repository Pattern** - Generic repository with Unit of Work
- **API Versioning** - Endpoint versioning support
- **Entity Framework Core** - Code-first database approach
- **Global Exception Handling** - Centralized error management with custom exceptions
- **FluentValidation** - Request validation pipeline
- **ASP.NET Core Identity** - User management and authentication
- **AutoMapper** - Object-to-object mapping
- **Structured Logging** - Request/Response logging middleware

## Project Structure

```
Project.API/                          # Controllers, middleware, configuration
  ├── Controllers/V1/                 # Versioned API controllers
  ├── Extensions/                     # Service registration extensions
  ├── Middlewares/                    # Custom middlewares
  └── Program.cs                      # Application entry point

Project.Application/                  # CQRS Commands/Queries, Business logic
  ├── Features/                       # Feature-based organization
  │   ├── Auth/                       # Authentication feature
  │   │   ├── Commands/               # Login, Logout, Refresh, Register
  │   │   │   └── Login/              # Each command in own folder
  │   │   │       ├── LoginCommand.cs
  │   │   │       └── LoginCommandHandler.cs
  │   │   ├── Requests/               # Request DTOs
  │   │   ├── Validators/             # FluentValidation validators
  │   │   └── Shared/                 # Shared business services
  │   │       ├── Interfaces/         # Service interfaces
  │   │       │   └── IAuthService.cs
  │   │       └── Services/           # Business logic services
  │   │           └── AuthService.cs
  │   │
  │   └── Categories/                 # Category CRUD feature
  │       ├── Commands/               # Write operations
  │       │   ├── CreateCategory/
  │       │   ├── UpdateCategory/
  │       │   └── DeleteCategory/
  │       ├── Queries/                # Read operations
  │       │   └── GetById/
  │       ├── Request/                # Request/Response DTOs
  │       ├── Validators/             # Validation rules
  │       └── Shared/                 # Business services
  │           ├── Interfaces/
  │           │   ├── ICategoryCreationService.cs
  │           │   ├── ICategoryUpdateService.cs
  │           │   ├── ICategoryDeletionService.cs
  │           │   └── ICategoryQueryService.cs
  │           └── Services/
  │               ├── CategoryCreationService.cs
  │               ├── CategoryUpdateService.cs
  │               ├── CategoryDeletionService.cs
  │               └── CategoryQueryService.cs
  │
  └── Common/                         # Shared application code
      ├── DTOs/                       # Data Transfer Objects
      ├── Exceptions/                 # Custom exceptions
      ├── Interfaces/                 # Common interfaces
      └── Mappers/                    # AutoMapper profiles

Project.Infrastructure/               # Data access, external services
  ├── Data/
  │   ├── Contexts/                   # DbContext
  │   ├── Repositories/               # Repository implementations
  │   └── Migrations/                 # EF Core migrations
  └── ExternalServices/               # External integrations
      ├── TokenServices/              # JWT service
      └── StorageServices/            # File storage

Project.Domain/                       # Core business entities
  ├── Entities/                       # Domain entities
  ├── Interfaces/                     # Domain interfaces
  └── Enums/                          # Enumerations

Project.Common/                       # Cross-cutting concerns
  ├── Constants/                      # Application constants
  ├── Models/                         # Common models
  ├── Options/                        # Configuration options
  └── Extensions/                     # Extension methods

Project.UnitTest/                     # Unit tests (TODO)
```

## CQRS Pattern with Shared Services

This template implements CQRS architecture with a clear separation of concerns:

### Architecture Flow
```
Controller → Command → CommandHandler → Shared Service → Repository
                ↓                              ↓
            Orchestration              Business Logic
```

✅ **Benefits:**
- Handlers are thin (5-10 lines) - only orchestration
- Business logic in dedicated services - reusable
- Easy to test - mock services in handlers
- Easy to maintain - one service per responsibility
- Can reuse services across multiple handlers
- Follows SOLID principles strictly

### Example: Create Category Flow

**1. Request DTO** - Input from client
```csharp
// Features/Categories/Request/CreateCategoryRequest.cs
public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
}
```

**2. Validator** - Validation rules
```csharp
// Features/Categories/Validators/CreateCategoryRequestValidator.cs
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);
    }
}
```

**3. Command** - Wraps request
```csharp
// Features/Categories/Commands/CreateCategory/CreateCategoryCommand.cs
public record CreateCategoryCommand(CreateCategoryRequest Request) 
    : IRequest<CategoryDto>;
```

**4. Handler** - Thin orchestration layer
```csharp
// Features/Categories/Commands/CreateCategory/CreateCategoryCommandHandler.cs
public class CreateCategoryCommandHandler 
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryCreationService _categoryCreationService;
    
    public CreateCategoryCommandHandler(ICategoryCreationService service)
    {
        _categoryCreationService = service;
    }

    public async Task<CategoryDto> Handle(
        CreateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        // Handler only orchestrates - delegates to service
        return await _categoryCreationService.CreateCategoryAsync(
            request.Request, 
            cancellationToken);
    }
}
```

**5. Shared Service** - Business logic
```csharp
// Features/Categories/Shared/Services/CategoryCreationService.cs
public class CategoryCreationService : ICategoryCreationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public async Task<CategoryDto> CreateCategoryAsync(
        CreateCategoryRequest request, 
        CancellationToken cancellationToken)
    {
        // Business logic: validation, mapping, persistence
        if (await IsValidCategoryNameAsync(request.Name))
            throw new ValidatorException("Name", "Category name already exists");

        Category category = _mapper.Map<Category>(request);
        await _unitOfWork.CategoryRepository.CreateAsync(category, cancellationToken);
        
        return _mapper.Map<CategoryDto>(category);
    }

    private async Task<bool> IsValidCategoryNameAsync(string name)
    {
        return await _unitOfWork.CategoryRepository
            .IsExistsAsync(nameof(Category.Name), name);
    }
}
```

**6. Controller** - API endpoint
```csharp
// Controllers/V1/CategoriesController.cs
[HttpPost]
public async Task<IActionResult> CreateCategoryAsync(
    [FromBody] CreateCategoryRequest request,
    CancellationToken cancellationToken)
{
    var result = await _sender.Send(
        new CreateCategoryCommand(request), 
        cancellationToken);

    return Ok(new ApiResponse<CategoryDto>
    {
        Success = true,
        Message = "Category created successfully",
        Data = result
    });
}
```



## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- SQL Server (LocalDB, Express, or full instance)
- Visual Studio 2022+ or VS Code

### Setup Instructions

1. **Clone repository**
```bash
git clone https://github.com/hngoxuantg/CleanArchitecture.CQRS.Template.git
cd CleanArchitecture.CQRS.Template
dotnet restore
```

2. **Update connection string** in `Project.API/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanArchCQRS;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

3. **Update JWT settings** in `appsettings.json`
```json
{
  "AppSettings": {
    "JwtConfig": {
      "Secret": "your-super-secret-key-minimum-32-characters-long",
      "ValidIssuer": "YourApp",
      "ValidAudience": "YourAppUsers",
      "AccessTokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    }
  }
}
```

4. **Apply database migrations**
```bash
dotnet ef database update --project Project.Infrastructure --startup-project Project.API
```

5. **Run the application**
```bash
dotnet run --project Project.API
```

API available at:
- **HTTPS:** `https://localhost:7001`
- **HTTP:** `http://localhost:5001`
- **Swagger:** `https://localhost:7001/swagger`

## Authentication Flow

### JWT with Refresh Token

1. **Login**
   - `POST /api/v1/auth/login`
   - Receive: `accessToken` (15 min) + `refreshToken` (7 days)

2. **Access Protected Endpoints**
   - Header: `Authorization: Bearer {accessToken}`

3. **Refresh Token**
   - `POST /api/v1/auth/refresh`
   - Send expired `accessToken` + `refreshToken`
   - Receive: new `accessToken` + new `refreshToken`

4. **Logout**
   - `POST /api/v1/auth/logout`
   - Invalidates `refreshToken`

### Default Users (Seeded)

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |

## Architecture Components

### Core Patterns

| Component | Purpose | Location |
|-----------|---------|----------|
| **MediatR** | CQRS dispatcher | Commands/Queries → Handlers |
| **Shared Services** | Business logic | Features/*/Shared/Services |
| **Repository Pattern** | Data access abstraction | Infrastructure/Data/Repositories |
| **Unit of Work** | Transaction management | Infrastructure/Data/Repositories |
| **Dependency Injection** | IoC container | Program.cs, Extensions |

### Application Services

| Service | Responsibility |
|---------|----------------|
| `IAuthService` | Login, logout, register, token refresh |
| `ICategoryCreationService` | Category creation logic |
| `ICategoryUpdateService` | Category update logic |
| `ICategoryDeletionService` | Category deletion logic |
| `ICategoryQueryService` | Category query logic |
| `IJwtTokenService` | JWT generation & validation |
| `ICurrentUserService` | Current user context |
| `IFileService` | File upload/storage |

### Custom Exceptions

| Exception | Usage | HTTP Status |
|-----------|-------|-------------|
| `ValidatorException` | Validation errors | 400 Bad Request |
| `NotFoundException` | Entity not found | 404 Not Found |
| `UnauthorizedException` | Auth failures | 401 Unauthorized |
| `ForbiddenException` | Permission denied | 403 Forbidden |

### Middlewares

| Middleware | Purpose |
|------------|---------|
| `ExceptionHandlingMiddleware` | Global exception handling, logging |
| `RequestResponseLoggingMiddleware` | Log all requests/responses |

## API Endpoints

### Authentication
```
POST   /api/v1/auth/login          - User login
POST   /api/v1/auth/register       - User registration
POST   /api/v1/auth/refresh        - Refresh access token
POST   /api/v1/auth/logout         - Logout user
```

### Categories (Protected - requires authentication)
```
POST   /api/v1/categories          - Create category [Admin only]
GET    /api/v1/categories/{id}     - Get category by ID
PUT    /api/v1/categories/{id}     - Update category [Admin only]
DELETE /api/v1/categories/{id}     - Delete category [Admin only]
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanArchCQRS;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "AppSettings": {
    "JwtConfig": {
      "Secret": "your-super-secret-key-at-least-32-characters-long-for-security",
      "ValidIssuer": "YourApplicationName",
      "ValidAudience": "YourApplicationUsers",
      "AccessTokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    },
    "AllowedHosts": "*"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### CORS Configuration

Configured in `Project.API/Extensions/CorsExtension.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## Adding New Features

### Step-by-step Guide

Let's add a new **Product** feature:

**1. Create folder structure**
```
Project.Application/Features/Products/
  ├── Commands/
  │   └── CreateProduct/
  │       ├── CreateProductCommand.cs
  │       └── CreateProductCommandHandler.cs
  ├── Queries/
  │   └── GetById/
  │       ├── GetProductByIdQuery.cs
  │       └── GetProductByIdQueryHandler.cs
  ├── Request/
  │   ├── CreateProductRequest.cs
  │   └── UpdateProductRequest.cs
  ├── Validators/
  │   └── CreateProductRequestValidator.cs
  └── Shared/
      ├── Interfaces/
      │   └── IProductCreationService.cs
      └── Services/
          └── ProductCreationService.cs
```

**2. Create Request DTO**
```csharp
public class CreateProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
```

**3. Create Validator**
```csharp
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}
```

**4. Create Command & Handler**
```csharp
public record CreateProductCommand(CreateProductRequest Request) 
    : IRequest<ProductDto>;

public class CreateProductCommandHandler 
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductCreationService _service;
    
    public async Task<ProductDto> Handle(CreateProductCommand request, ...)
    {
        return await _service.CreateProductAsync(request.Request, cancellationToken);
    }
}
```

**5. Create Shared Service**
```csharp
public class ProductCreationService : IProductCreationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public async Task<ProductDto> CreateProductAsync(
        CreateProductRequest request, 
        CancellationToken cancellationToken)
    {
        // Business logic here
        var product = _mapper.Map<Product>(request);
        await _unitOfWork.ProductRepository.CreateAsync(product, cancellationToken);
        return _mapper.Map<ProductDto>(product);
    }
}
```

**6. Register Service** in `ServiceExtension.cs`
```csharp
services.AddScoped<IProductCreationService, ProductCreationService>();
```

**7. Create Controller**
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        CreateProductRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateProductCommand(request), 
            cancellationToken);
        return Ok(new ApiResponse<ProductDto> { Data = result });
    }
}
```



## Technology Stack

- **.NET 10.0** - Framework
- **ASP.NET Core 10.0** - Web API
- **Entity Framework Core 10.0** - ORM
- **MediatR** - CQRS mediator
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **ASP.NET Core Identity** - Authentication
- **JWT Bearer** - Token authentication
- **Swagger/OpenAPI** - API documentation
- **SQL Server** - Database



## Roadmap / TODO

- [ ] Add Unit Tests (xUnit, Moq, FluentAssertions)
- [ ] Add Integration Tests
- [ ] Implement Pagination for GetAll queries
- [ ] Add Redis caching layer
- [ ] Implement Background Jobs (Hangfire)
- [ ] Add API Rate Limiting
- [ ] Implement Email service
- [ ] Add Health Checks
- [ ] Docker support
- [ ] CI/CD pipeline (GitHub Actions)

## Contributing

Contributions are welcome! This is a learning project.

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Author

**Ngô Xuân Hải**  
Third-year Computer Science Student  
GitHub: [@hngoxuantg](https://github.com/hngoxuantg)

## Acknowledgments

- Inspired by Jason Taylor's Clean Architecture template
- CQRS pattern from Martin Fowler
- Community best practices from .NET community

---

# Web API Clean Architecture với CQRS & Shared Services

Template Web API clean architecture với CQRS pattern nâng cao, MediatR, Shared Business Services, JWT authentication và refresh token.

## Tổng quan

Template .NET 10 Web API enterprise triển khai clean architecture với CQRS (Command Query Responsibility Segregation) pattern, MediatR, **Shared Business Services**, JWT authentication, refresh tokens, repository pattern, và các production-grade patterns.

Template này implement CQRS bằng cách **tách business logic vào Shared Services**, giữ cho handlers mỏng và chỉ tập trung vào orchestration.

## Tính năng

- **Clean Architecture** - Phân tầng Domain, Application, Infrastructure, API
- **Advanced CQRS Pattern** - Command/Query với **Shared Business Services**
- **Separation of Concerns** - Handler điều phối, Service chứa business logic
- **Single Responsibility Principle** - Mỗi handler, service, class có một trách nhiệm rõ ràng
- **JWT Authentication** - Access token với cơ chế refresh token
- **Repository Pattern** - Generic repository với Unit of Work
- **API Versioning** - Hỗ trợ phiên bản endpoint
- **Entity Framework Core** - Code-first database approach
- **Global Exception Handling** - Quản lý lỗi tập trung với custom exceptions
- **FluentValidation** - Pipeline validation cho request
- **ASP.NET Core Identity** - Quản lý user và authentication
- **AutoMapper** - Object-to-object mapping
- **Structured Logging** - Middleware logging request/response

## Cấu trúc Project

```
Project.API/                          # Controllers, middleware, configuration
  ├── Controllers/V1/                 # API controllers có versioning
  ├── Extensions/                     # Service registration extensions
  ├── Middlewares/                    # Custom middlewares
  └── Program.cs                      # Application entry point

Project.Application/                  # CQRS Commands/Queries, Business logic
  ├── Features/                       # Tổ chức theo tính năng
  │   ├── Auth/                       # Tính năng Authentication
  │   │   ├── Commands/               # Login, Logout, Refresh, Register
  │   │   │   └── Login/              # Mỗi command có folder riêng
  │   │   │       ├── LoginCommand.cs
  │   │   │       └── LoginCommandHandler.cs
  │   │   ├── Requests/               # Request DTOs
  │   │   ├── Validators/             # FluentValidation validators
  │   │   └── Shared/                 # Shared business services
  │   │       ├── Interfaces/         # Service interfaces
  │   │       │   └── IAuthService.cs
  │   │       └── Services/           # Business logic services
  │   │           └── AuthService.cs
  │   │
  │   └── Categories/                 # Tính năng CRUD Category
  │       ├── Commands/               # Thao tác ghi
  │       │   ├── CreateCategory/
  │       │   ├── UpdateCategory/
  │       │   └── DeleteCategory/
  │       ├── Queries/                # Thao tác đọc
  │       │   └── GetById/
  │       ├── Request/                # Request/Response DTOs
  │       ├── Validators/             # Validation rules
  │       └── Shared/                 # Business services
  │           ├── Interfaces/
  │           │   ├── ICategoryCreationService.cs
  │           │   ├── ICategoryUpdateService.cs
  │           │   ├── ICategoryDeletionService.cs
  │           │   └── ICategoryQueryService.cs
  │           └── Services/
  │               ├── CategoryCreationService.cs
  │               ├── CategoryUpdateService.cs
  │               ├── CategoryDeletionService.cs
  │               └── CategoryQueryService.cs
  │
  └── Common/                         # Shared application code
      ├── DTOs/                       # Data Transfer Objects
      ├── Exceptions/                 # Custom exceptions
      ├── Interfaces/                 # Common interfaces
      └── Mappers/                    # AutoMapper profiles

Project.Infrastructure/               # Data access, external services
  ├── Data/
  │   ├── Contexts/                   # DbContext
  │   ├── Repositories/               # Repository implementations
  │   └── Migrations/                 # EF Core migrations
  └── ExternalServices/               # External integrations
      ├── TokenServices/              # JWT service
      └── StorageServices/            # File storage

Project.Domain/                       # Core business entities
  ├── Entities/                       # Domain entities
  ├── Interfaces/                     # Domain interfaces
  └── Enums/                          # Enumerations

Project.Common/                       # Cross-cutting concerns
  ├── Constants/                      # Application constants
  ├── Models/                         # Common models
  ├── Options/                        # Configuration options
  └── Extensions/                     # Extension methods

Project.UnitTest/                     # Unit tests (TODO)
```

## CQRS Pattern Nâng cao với Shared Services

Template này triển khai CQRS architecture với separation of concerns rõ ràng:

### Architecture Flow
```
Controller → Command → CommandHandler → Shared Service → Repository
                ↓                              ↓
          Orchestration              Business Logic
```

✅ **Benefits:**
- Handlers mỏng (5-10 dòng) - chỉ orchestration
- Business logic trong dedicated services - reusable
- Dễ test - mock services trong handlers
- Dễ maintain - một service một responsibility
- Có thể reuse services cho nhiều handlers
- Tuân thủ SOLID principles nghiêm ngặt

### Ví dụ: Luồng Create Category

**1. Request DTO** - Input từ client
```csharp
// Features/Categories/Request/CreateCategoryRequest.cs
public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
}
```

**2. Validator** - Validation rules
```csharp
// Features/Categories/Validators/CreateCategoryRequestValidator.cs
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name là bắt buộc")
            .MaximumLength(100);
    }
}
```

**3. Command** - Wrap request
```csharp
// Features/Categories/Commands/CreateCategory/CreateCategoryCommand.cs
public record CreateCategoryCommand(CreateCategoryRequest Request) 
    : IRequest<CategoryDto>;
```

**4. Handler** - Lớp điều phối mỏng
```csharp
// Features/Categories/Commands/CreateCategory/CreateCategoryCommandHandler.cs
public class CreateCategoryCommandHandler 
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryCreationService _categoryCreationService;
    
    public CreateCategoryCommandHandler(ICategoryCreationService service)
    {
        _categoryCreationService = service;
    }

    public async Task<CategoryDto> Handle(
        CreateCategoryCommand request, 
        CancellationToken cancellationToken)
    {
        // Handler chỉ điều phối - delegate cho service
        return await _categoryCreationService.CreateCategoryAsync(
            request.Request, 
            cancellationToken);
    }
}
```

**5. Shared Service** - Business logic
```csharp
// Features/Categories/Shared/Services/CategoryCreationService.cs
public class CategoryCreationService : ICategoryCreationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public async Task<CategoryDto> CreateCategoryAsync(
        CreateCategoryRequest request, 
        CancellationToken cancellationToken)
    {
        // Business logic: validation, mapping, persistence
        if (await IsValidCategoryNameAsync(request.Name))
            throw new ValidatorException("Name", "Tên category đã tồn tại");

        Category category = _mapper.Map<Category>(request);
        await _unitOfWork.CategoryRepository.CreateAsync(category, cancellationToken);
        
        return _mapper.Map<CategoryDto>(category);
    }

    private async Task<bool> IsValidCategoryNameAsync(string name)
    {
        return await _unitOfWork.CategoryRepository
            .IsExistsAsync(nameof(Category.Name), name);
    }
}
```

**6. Controller** - API endpoint
```csharp
// Controllers/V1/CategoriesController.cs
[HttpPost]
public async Task<IActionResult> CreateCategoryAsync(
    [FromBody] CreateCategoryRequest request,
    CancellationToken cancellationToken)
{
    var result = await _sender.Send(
        new CreateCategoryCommand(request), 
        cancellationToken);

    return Ok(new ApiResponse<CategoryDto>
    {
        Success = true,
        Message = "Tạo category thành công",
        Data = result
    });
}
```



## Bắt đầu

### Yêu cầu

- .NET 10.0 SDK trở lên
- SQL Server (LocalDB, Express, hoặc full instance)
- Visual Studio 2022+ hoặc VS Code

### Hướng dẫn Setup

1. **Clone repository**
```bash
git clone https://github.com/hngoxuantg/CleanArchitecture.CQRS.Template.git
cd CleanArchitecture.CQRS.Template
dotnet restore
```

2. **Cập nhật connection string** trong `Project.API/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanArchCQRS;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

3. **Cập nhật JWT settings** trong `appsettings.json`
```json
{
  "AppSettings": {
    "JwtConfig": {
      "Secret": "your-super-secret-key-minimum-32-characters-long",
      "ValidIssuer": "YourApp",
      "ValidAudience": "YourAppUsers",
      "AccessTokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    }
  }
}
```

4. **Apply database migrations**
```bash
dotnet ef database update --project Project.Infrastructure --startup-project Project.API
```

5. **Chạy ứng dụng**
```bash
dotnet run --project Project.API
```

API có sẵn tại:
- **HTTPS:** `https://localhost:7001`
- **HTTP:** `http://localhost:5001`
- **Swagger:** `https://localhost:7001/swagger`

## Luồng Authentication

### JWT với Refresh Token

1. **Login**
   - `POST /api/v1/auth/login`
   - Nhận: `accessToken` (15 phút) + `refreshToken` (7 ngày)

2. **Truy cập Protected Endpoints**
   - Header: `Authorization: Bearer {accessToken}`

3. **Refresh Token**
   - `POST /api/v1/auth/refresh`
   - Gửi `accessToken` hết hạn + `refreshToken`
   - Nhận: `accessToken` mới + `refreshToken` mới

4. **Logout**
   - `POST /api/v1/auth/logout`
   - Vô hiệu hóa `refreshToken`

### Users Mặc định (Đã seed)

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |

## Các Components trong Kiến trúc

### Core Patterns

| Component | Mục đích | Vị trí |
|-----------|----------|--------|
| **MediatR** | CQRS dispatcher | Commands/Queries → Handlers |
| **Shared Services** | Business logic | Features/*/Shared/Services |
| **Repository Pattern** | Abstraction truy cập data | Infrastructure/Data/Repositories |
| **Unit of Work** | Quản lý transaction | Infrastructure/Data/Repositories |
| **Dependency Injection** | IoC container | Program.cs, Extensions |

### Application Services

| Service | Trách nhiệm |
|---------|-------------|
| `IAuthService` | Login, logout, register, refresh token |
| `ICategoryCreationService` | Logic tạo category |
| `ICategoryUpdateService` | Logic cập nhật category |
| `ICategoryDeletionService` | Logic xóa category |
| `ICategoryQueryService` | Logic query category |
| `IJwtTokenService` | Tạo & validate JWT |
| `ICurrentUserService` | Context user hiện tại |
| `IFileService` | Upload/lưu trữ file |

### Custom Exceptions

| Exception | Sử dụng | HTTP Status |
|-----------|---------|-------------|
| `ValidatorException` | Lỗi validation | 400 Bad Request |
| `NotFoundException` | Không tìm thấy entity | 404 Not Found |
| `UnauthorizedException` | Lỗi xác thực | 401 Unauthorized |
| `ForbiddenException` | Không có quyền | 403 Forbidden |

### Middlewares

| Middleware | Mục đích |
|------------|----------|
| `ExceptionHandlingMiddleware` | Global exception handling, logging |
| `RequestResponseLoggingMiddleware` | Log tất cả requests/responses |

## API Endpoints

### Authentication
```
POST   /api/v1/auth/login          - Đăng nhập
POST   /api/v1/auth/register       - Đăng ký user
POST   /api/v1/auth/refresh        - Refresh access token
POST   /api/v1/auth/logout         - Đăng xuất
```

### Categories (Bảo vệ - yêu cầu authentication)
```
POST   /api/v1/categories          - Tạo category [Chỉ Admin]
GET    /api/v1/categories/{id}     - Lấy category theo ID
PUT    /api/v1/categories/{id}     - Cập nhật category [Chỉ Admin]
DELETE /api/v1/categories/{id}     - Xóa category [Chỉ Admin]
```

## Cấu hình

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanArchCQRS;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "AppSettings": {
    "JwtConfig": {
      "Secret": "your-super-secret-key-at-least-32-characters-long-for-security",
      "ValidIssuer": "YourApplicationName",
      "ValidAudience": "YourApplicationUsers",
      "AccessTokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    },
    "AllowedHosts": "*"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Cấu hình CORS

Được cấu hình trong `Project.API/Extensions/CorsExtension.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## Thêm Tính năng Mới

### Hướng dẫn từng bước

Thêm tính năng **Product** mới:

**1. Tạo cấu trúc folder**
```
Project.Application/Features/Products/
  ├── Commands/
  │   └── CreateProduct/
  │       ├── CreateProductCommand.cs
  │       └── CreateProductCommandHandler.cs
  ├── Queries/
  │   └── GetById/
  │       ├── GetProductByIdQuery.cs
  │       └── GetProductByIdQueryHandler.cs
  ├── Request/
  │   ├── CreateProductRequest.cs
  │   └── UpdateProductRequest.cs
  ├── Validators/
  │   └── CreateProductRequestValidator.cs
  └── Shared/
      ├── Interfaces/
      │   └── IProductCreationService.cs
      └── Services/
          └── ProductCreationService.cs
```

**2. Tạo Request DTO**
```csharp
public class CreateProductRequest
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
```

**3. Tạo Validator**
```csharp
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}
```

**4. Tạo Command & Handler**
```csharp
public record CreateProductCommand(CreateProductRequest Request) 
    : IRequest<ProductDto>;

public class CreateProductCommandHandler 
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductCreationService _service;
    
    public async Task<ProductDto> Handle(CreateProductCommand request, ...)
    {
        return await _service.CreateProductAsync(request.Request, cancellationToken);
    }
}
```

**5. Tạo Shared Service**
```csharp
public class ProductCreationService : IProductCreationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public async Task<ProductDto> CreateProductAsync(
        CreateProductRequest request, 
        CancellationToken cancellationToken)
    {
        // Business logic ở đây
        var product = _mapper.Map<Product>(request);
        await _unitOfWork.ProductRepository.CreateAsync(product, cancellationToken);
        return _mapper.Map<ProductDto>(product);
    }
}
```

**6. Đăng ký Service** trong `ServiceExtension.cs`
```csharp
services.AddScoped<IProductCreationService, ProductCreationService>();
```

**7. Tạo Controller**
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        CreateProductRequest request, 
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new CreateProductCommand(request), 
            cancellationToken);
        return Ok(new ApiResponse<ProductDto> { Data = result });
    }
}
```



## Technology Stack

- **.NET 10.0** - Framework
- **ASP.NET Core 10.0** - Web API
- **Entity Framework Core 10.0** - ORM
- **MediatR** - CQRS mediator
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **ASP.NET Core Identity** - Authentication
- **JWT Bearer** - Token authentication
- **Swagger/OpenAPI** - API documentation
- **SQL Server** - Database



## Roadmap / TODO

- [ ] Thêm Unit Tests (xUnit, Moq, FluentAssertions)
- [ ] Thêm Integration Tests
- [ ] Implement Pagination cho GetAll queries
- [ ] Thêm Redis caching layer
- [ ] Implement Background Jobs (Hangfire)
- [ ] Thêm API Rate Limiting
- [ ] Implement Email service
- [ ] Thêm Health Checks
- [ ] Hỗ trợ Docker
- [ ] CI/CD pipeline (GitHub Actions)

## Đóng góp

Chào mừng mọi đóng góp! Đây là một learning project.

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/TinhNangMoi`)
3. Commit changes (`git commit -m 'Thêm tính năng mới'`)
4. Push lên branch (`git push origin feature/TinhNangMoi`)
5. Mở Pull Request

## Author

**Ngô Xuân Hải**  
Software Engineer  
GitHub: [@hngoxuantg](https://github.com/hngoxuantg)

## Acknowledgments

- Inspired by Jason Taylor's Clean Architecture template
- CQRS pattern from Martin Fowler
- Community best practices from .NET community