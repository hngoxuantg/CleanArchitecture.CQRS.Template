# Web API Clean Architecture with CQRS & Shared Services

Clean architecture Web API template with advanced CQRS pattern, MediatR, Shared Business Services, JWT authentication, refresh token implementation, Hangfire background jobs, and automated email notifications.

## Overview

.NET 10 Web API template with Clean Architecture, CQRS pattern, MediatR, JWT authentication, Hangfire background jobs, and SMTP email notifications. Implements **Shared Business Services** pattern with thin handlers for orchestration.

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
- **Rate Limiting** - Per-user, per-IP, and login attempt rate limiting policies
- **Background Jobs** - Hangfire for asynchronous task processing
- **Email Service** - SMTP email sending with HTML templates
- **Notification System** - Automated email notifications for business events
- **Memory Caching** - In-memory caching for frequently accessed data

## Project Structure

```
Project.API/                    # Controllers, middleware, configuration
Project.Application/            # CQRS Commands/Queries, Business logic
  ├── Features/                 # Feature-based organization (Auth, Categories)
  │   └── */Shared/Services/    # Business logic services
  └── Common/                   # DTOs, Interfaces, Exceptions
Project.Infrastructure/         # Data access, external services
  ├── Data/                     # DbContext, Repositories, Migrations
  ├── BackgroundJobs/           # Hangfire implementation
  └── ExternalServices/         # JWT, Email, Storage services
Project.Domain/                 # Core business entities
Project.Common/                 # Cross-cutting concerns
Project.UnitTest/               # Unit tests
```

## CQRS Pattern with Shared Services

**Architecture Flow:**
```
Command Handler → Write Service → Repository
Query Handler   → Read Service  → Repository
```

**Key Features:**
- **Thin Handlers**: Only orchestration (5-10 lines)
- **Fat Services**: All business logic
- **Write/Read Separation**: Different services for commands vs queries
- **Email Notifications**: Background jobs for business events

**Example Implementation:**
```csharp
// Thin Handler
public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
{
    return await _categoryWriteService.CreateCategoryAsync(request.Request, ct);
}

// Fat Service with Business Logic
public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct)
{
    // Validation, mapping, persistence, email notification
    var category = _mapper.Map<Category>(request);
    await _unitOfWork.CategoryRepository.CreateAsync(category, ct);
    _backgroundJob.EnqueueSendEmail(emailDto); // Background notification
    return _mapper.Map<CategoryDto>(category);
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

2. **Update connection strings** in `Project.API/appsettings.json`
```json
{
  "ConnectionStrings": {
    "PrimaryDbConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanArchCQRS;Trusted_Connection=true;TrustServerCertificate=true;",
    "HangfireDbConnection": "Server=(localdb)\\mssqllocaldb;Database=HangfireDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

3. **Update JWT settings** in `appsettings.json`
```json
{
  "AppSettings": {
    "JwtConfig": {
      "Secret": "your-super-secret-key-minimum-32-characters-long",
      "ValidIssuer": "https://localhost:7191",
      "ValidAudience": "http://localhost:5174",
      "TokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    }
  }
}
```

4. **Configure Email Settings** in `appsettings.json`
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "your-email@gmail.com",
    "DisplayName": "YourApp"
  }
}
```

5. **Apply database migrations**
```bash
dotnet ef database update --project Project.Infrastructure --startup-project Project.API
```

6. **Run the application**
```bash
dotnet run --project Project.API
```

API available at:
- **HTTPS:** `https://localhost:7191`
- **HTTP:** `http://localhost:5246`
- **Swagger:** `https://localhost:7191/swagger`
- **Hangfire Dashboard:** `https://localhost:7191/hangfire`

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
| `IAuthWriteService` | Login, logout, token refresh |
| `ICategoryWriteService` | Category create, update, delete operations |
| `ICategoryReadService` | Category query operations |
| `IJwtTokenService` | JWT generation & validation |
| `ICurrentUserService` | Current user context |
| `IMailService` | Email sending with templates and attachments |
| `IBackgroundJobService` | Asynchronous task queueing with Hangfire |
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

## Background Jobs & Email System

**Hangfire Integration:**
- Background job processing with SQL Server storage
- Dashboard: `/hangfire` endpoint
- Automatic retry on failures

**Email Service:**
- SMTP with MailKit library  
- HTML templates for notifications
- Background job queue for async sending
- Automatic notifications on category creation

**Example Usage:**
```csharp
_backgroundJob.EnqueueSendEmail(new EmailDto 
{
    To = "user@example.com",
    Subject = "Category Created",
    TemplateName = "CategoryCreated",
    TemplateData = new Dictionary<string, string> 
    {
        { "CategoryName", "Electronics" },
        { "CreatedDate", DateTime.Now.ToString() }
    }
});
```

## Caching Strategy

In-memory caching for read operations with automatic invalidation:

```csharp
// Cache on GET, invalidate on PUT/DELETE
if (_cache.TryGetValue($"Category_{id}", out CategoryDto? cached))
    return cached;

result = await _sender.Send(new GetCategoryByIdQuery(id));
_cache.Set($"Category_{id}", result, TimeSpan.FromMinutes(10));
```

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
POST   /api/v1/categories                    - Create category [Admin only]
GET    /api/v1/categories/{id}               - Get category by ID
PUT    /api/v1/categories/{id}               - Update category [Admin only]
PATCH  /api/v1/categories/{id}/description   - Update category description [Admin only]
DELETE /api/v1/categories/{id}               - Delete category [Admin only]
```

*Note: GetAll/List endpoints are not yet implemented - planned for future release with pagination support.*

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "PrimaryDbConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanArchCQRS;Trusted_Connection=true;TrustServerCertificate=true;",
    "HangfireDbConnection": "Server=(localdb)\\mssqllocaldb;Database=HangfireDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "AppSettings": {
    "JwtConfig": {
      "Secret": "your-super-secret-key-at-least-32-characters-long-for-security",
      "ValidIssuer": "https://localhost:7191",
      "ValidAudience": "http://localhost:5174",
      "TokenExpirationMinutes": 15,
      "RefreshTokenExpirationDays": 7
    },
    "RateLimit": {
      "AuthenticatedUser": {
        "PermitLimit": 200,
        "WindowMinutes": 1
      },
      "AnonymousUser": {
        "PermitLimit": 50,
        "WindowMinutes": 1
      },
      "LoginAttempts": {
        "PermitLimit": 5,
        "WindowMinutes": 15
      }
    }
  },
  "AdminAccount": {
    "Account": {
      "UserName": "admin",
      "Password": "Admin123@",
      "FullName": "Your Full Name",
      "Email": "admin@example.com",
      "PhoneNumber": "0987654321"
    }
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "your-email@gmail.com",
    "DisplayName": "WebAPI_Project"
  },
  "AllowedCors": {
    "Origins": [
      "http://localhost:5174",
      "https://localhost:5174"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
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

**Framework:** .NET 10, ASP.NET Core  
**Data:** Entity Framework Core 9.0.7, SQL Server  
**CQRS:** MediatR 13.1.0  
**Validation:** FluentValidation 12.0.0  
**Mapping:** AutoMapper 14.0.0  
**Authentication:** JWT Bearer, ASP.NET Core Identity  
**Background Jobs:** Hangfire 1.8.19  
**Email:** MailKit 4.9.0, MimeKit 4.9.0  
**Testing:** xUnit, NSubstitute, AutoFixture

## Unit Testing

**Focus:** Business logic in Shared Services  
**Tools:** xUnit, NSubstitute (mocking), AutoFixture (test data)  
**Coverage:** Authentication and Category services

```bash
dotnet test                                    # Run all tests
dotnet test /p:CollectCoverage=true           # With coverage
```



## Roadmap / TODO

- [ ] Expand Unit Test coverage for all services
- [ ] Add Integration Tests
- [ ] Implement Pagination for GetAll queries
- [ ] Add Redis caching layer
- [ ] Add Health Checks
- [ ] Docker support
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Extend email templates for more business events
- [ ] Add push notifications
- [ ] Implement file upload/download endpoints

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
