using Asp.Versioning;
using FluentValidation;
using HiLo.Extensions;
using HiLo.Feature.Game;
using HiLo.Feature.Player;
using HiLo.Feature.Player.CreatePlayer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddEndpointsApiExplorer()
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstitutionFormat = "VV";
        options.SubstituteApiVersionInUrl = true;
    })
    .EnableApiVersionBinding();
builder.Services.AddOpenApi();

builder.Services.AddValidatorsFromAssemblyContaining<CreatePlayerRequestValidator>();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();
app.MapPlayerEndpoints();
app.AddGameEndpoints();

await app.RunAsync();