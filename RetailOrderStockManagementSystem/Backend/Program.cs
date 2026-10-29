using Microsoft.EntityFrameworkCore;
using RetailOrderStockManagementSystem.Backend.Data;
using RetailOrderStockManagementSystem.Backend.Enums;
using RetailOrderStockManagementSystem.Backend.Mapper;
using RetailOrderStockManagementSystem.Backend.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RetailOrderStockManagementSystem.Backend.Security;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//registers jwt token generation
builder.Services.AddScoped<JwtTokenGenerator>();
//swagger api
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//adds mapper to container
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
//adds middleware to container
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
//adds jwt auth to protect endpoints
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       //token validation rules
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Aydience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
       };
   });
builder.Services.AddAuthorization();


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

app.UseExceptionHandler();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CORS 


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
