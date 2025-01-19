using ProductService.Api.Extensions;
using ProductService.Application;
using ProductService.Domain;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDomainServices();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddCustomJwtAuthentication(builder.Configuration);

var app = builder.Build();

var productDbConnString = builder.Configuration.GetConnectionString("ProductDatabase");
MySqlInitializer.EnsureDatabaseAndTables(productDbConnString);

//await app.MigrateDatabaseAsync<ProductDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
