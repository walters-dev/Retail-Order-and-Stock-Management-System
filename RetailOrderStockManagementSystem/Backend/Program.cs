using RetailOrderStockManagementSystem.Backend.Mapper;
var builder = WebApplication.CreateBuilder(args);

// Add services to the contain(er.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

var app = builder.Build();

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
