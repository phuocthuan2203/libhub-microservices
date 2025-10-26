using LibHub.LoanService.Application.Interfaces;
using LibHub.LoanService.Application.Services;
using LibHub.LoanService.Domain.Interfaces;
using LibHub.LoanService.Infrastructure.HttpClients;
using LibHub.LoanService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure EF Core for MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LoanDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register application services and repositories
builder.Services.AddScoped<ILoanRepository, EfLoanRepository>();
builder.Services.AddScoped<LoanApplicationService>();

// Register and configure the typed HttpClient for the CatalogService
builder.Services.AddHttpClient<ICatalogService, CatalogServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://catalogservice");
});

// Add authentication and authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Authentication:Authority"];
        options.RequireHttpsMetadata = false;
        options.Audience = "loanservice";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
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
