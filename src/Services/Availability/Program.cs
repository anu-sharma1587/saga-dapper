
using HotelManagement.Services.Availability.Configuration;
using HotelManagement.Services.Availability.Extensions;
using HotelManagement.Services.Availability.Middleware;
using DataAccess.DbConnectionProvider;
using DataAccess.Dapper;

var builder = WebApplication.CreateBuilder(args);
// Add Dapper and DataAccess DI
builder.Services.AddScoped<IDbConnectionFactory>(sp =>
    new DbConnectionFactory(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        builder.Configuration["DatabaseProvider"]));
builder.Services.AddScoped<IDapperDataRepository, DapperDataRepository>();

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddAvailabilityServices(builder.Configuration);
builder.Services.AddAvailabilityApi();
builder.Services.AddResiliencePolicies(builder.Configuration);
HotelManagement.Services.Availability.Extensions.OpenTelemetryExtensions.AddCustomOpenTelemetry(builder.Services, builder.Configuration);

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Seed development data
    await DataSeeder.SeedDataAsync(app.Services);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Ensure database is created and migrations are applied
// app.Services.EnsureDatabaseCreated();

app.Run();
