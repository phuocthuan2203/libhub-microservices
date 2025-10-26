using LibHub.CatalogService.Application.Services;
using LibHub.CatalogService.Domain.Interfaces;
using LibHub.CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core for MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register application services and repositories
builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddScoped<BookApplicationService>();

// Add controllers
builder.Services.AddControllers();

// Add authentication and authorization (placeholder for JWT configuration)
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
