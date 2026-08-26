using WHMS.Persistence;
using WHMS.Persistence.SeedData.Core;
using WHMS.Persistence.SeedData.Extensions;
using WHMS.Application;
using WHMS.Infrastructure;
using WHMS.Api;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentationServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddPersistenceServices(builder.Configuration, builder.Environment);
builder.Services.AddSeedDataServices();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();
await app.SeedEssentialDataAsync();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.SeedDevelopmentDataAsync();
}

app.UseHttpsRedirection();

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();


app.Run();

