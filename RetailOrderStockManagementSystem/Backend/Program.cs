using RetailOrderStockManagementSystem.Backend.Services;
using RetailOrderStockManagementSystem.Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Registering our ProductService as a Singleton
builder.Services.AddSingleton<IProductService, ProductService>();

// 2. This line that builds the app
var app = builder.Build();

// 3. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//CORS 


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
