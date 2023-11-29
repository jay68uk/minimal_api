using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Minimal_Api.App.Auth;
using Minimal_Api.App.Data;
using Minimal_Api.App.Endpoints.Internal;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.IncludeFields = true;
});

builder.Services.AddAuthentication(ApiKeySchemeConstants.SchemeName)
    .AddScheme<ApiAuthKeySchemeOptions, ApiKeyAuthHandler>(ApiKeySchemeConstants.SchemeName, _ => { });
builder.Services.AddAuthorization();

builder.Services.AddCors(options => { options.AddPolicy("AnyOrigin", x => x.AllowAnyOrigin()); });

builder.Services.AddEndpoints<Program>(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(builder.Configuration.GetValue<string>("Database:ConnectionString")!));
builder.Services.AddSingleton<DatabaseInitialiser>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

var databaseInitialiser = app.Services.GetRequiredService<DatabaseInitialiser>();
await databaseInitialiser.InitialiseAsync();

app.MapGet("/basic-get", () => "Very basic GET");

app.MapGet("get-with-route-param-explicit/{id:int}", (int id) => $"GET with route parameter {id}");

app.MapGet("get-with-route-param-implicit/{id}", (int id) => $"GET with route parameter {id}");

app.MapGet("get-with-query", (int id) => $"GET with query string {id}");

app.MapGet(@"get-with-regex/{id:regex(^\d{{3}}-\d{{2}}-\d{{4}}$)}", (string id) => $"GET with regex route parameter {id}");

app.UseEndpoints<Program>();

app.Run();