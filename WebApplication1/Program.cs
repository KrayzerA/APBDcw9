using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Endpoints;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Cw9Context>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddValidatorsFromAssemblyContaining<AssignAClientToTheTripModelValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var endpoints = app.MapGroup("api");
endpoints.RegisterTripsEndpoints();
endpoints.RegisterClientsEndpoints();

app.UseHttpsRedirection();
app.Run();
