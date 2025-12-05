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
| admin | Admin123@ | Admin |

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

### Core Framework
- **.NET 10.0** - Target framework
- **ASP.NET Core** - Web API framework

### Data Access
- **Entity Framework Core 9.0.7** - ORM
- **Microsoft.EntityFrameworkCore.SqlServer 9.0.7** - SQL Server provider
- **Microsoft.EntityFrameworkCore.Design 9.0.8** - EF Core design-time tools

### CQRS & Messaging
- **MediatR 13.1.0** - CQRS mediator pattern implementation
- **MediatR.Extensions.Microsoft.DependencyInjection 11.1.0** - DI integration

### Validation & Mapping
- **FluentValidation 12.0.0** - Fluent validation rules
- **FluentValidation.AspNetCore 11.3.1** - ASP.NET Core integration
- **AutoMapper 14.0.0** - Object-to-object mapping

### Authentication & Security
- **ASP.NET Core Identity 8.0.18** - User management
- **Microsoft.AspNetCore.Authentication.JwtBearer 8.0.18** - JWT authentication
- **System.IdentityModel.Tokens.Jwt 8.13.1** - JWT token handling

### API Features
- **Swashbuckle.AspNetCore 6.6.2** - Swagger/OpenAPI documentation
- **Microsoft.AspNetCore.Mvc.Versioning 5.1.0** - API versioning
- **Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer 5.1.0** - API version explorer

### Database
- **SQL Server** - Production database



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

## License

This project is licensed under the [MIT License](LICENSE).
