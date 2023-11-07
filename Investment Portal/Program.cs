using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
using Investment_Portal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure your DbContext here
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Register repositories and other services
builder.Services.AddTransient<IClient, ClientRepo>();
builder.Services.AddTransient<IAdvisor, AdvisorRepo>();
builder.Services.AddTransient<IInvestment, InvestmentRepo>();
builder.Services.AddTransient<IStrategy, StrategyRepo>();
builder.Services.AddTransient<IStrategies, StrategiesRepository>();

//builder.Services.AddTransient<InvestmentRepo, InvestmentRepo>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddApiVersioning();
builder.Services.AddSwaggerGen();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigins",
//        builder => builder.WithOrigins("http://localhost:3000") // Add your front-end URL here
//                          .AllowAnyMethod()
//                          .AllowAnyHeader());
//});

builder.Services.AddCors(options => { options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); });


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger(
    options => options.RouteTemplate = $"swagger/{ApiConstants.ServiceName}/{{documentName}}/swagger.json");





app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger/incinvest";
    options.SwaggerEndpoint($"/swagger/{ApiConstants.ServiceName}/v1/swagger.json", "V1");
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

//new changes done