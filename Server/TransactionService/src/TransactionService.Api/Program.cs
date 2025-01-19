using TransactionService.Domain;
using TransactionService.Infrastructure;
using TransactionService.Application;
using TransactionService.Api.Extensions;

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

var transactionDbConnString = builder.Configuration.GetConnectionString("TransactionDatabase");
PostgresInitializer.EnsureDatabaseAndTables(transactionDbConnString);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
