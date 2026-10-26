using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Read the connection string saved in User Secrets.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "DefaultConnection was not found. Check your User Secrets.");

// Register the context and configure it to use PostgreSQL.
builder.Services.AddDbContext<RetailDbContext>(options =>
    options.UseNpgsql(connectionString, postgres =>
    {
        postgres.MapEnum<OrderStatus.Status>("order_status", "public");
        postgres.MapEnum<Users.UserTypes>("user_type", "public");
    }));


var app = builder.Build();

// Temporary connection check.
using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider.GetRequiredService<RetailDbContext>();

    var connected = await context.Database.CanConnectAsync();

    Console.WriteLine(connected
        ? "Supabase connection successful."
        : "Supabase connection failed.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//CORS 


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
