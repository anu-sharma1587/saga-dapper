using HotelManagement.Services.Reporting.Services;
using DataAccess;
using DataAccess.Dapper;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection factory
builder.Services.AddScoped<IDbConnectionFactory>(provider => 
    new DbConnectionFactory(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        builder.Configuration.GetValue<string>("DatabaseProvider") ?? "PostgreSQL"
    ));

// Configure Dapper data repository
builder.Services.AddScoped<IDapperDataRepository, DapperDataRepository>();

// Add services
builder.Services.AddScoped<IReportingService, ReportingService>();

// Add controllers and API documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Database initialization will be handled by migrations separately

app.Run();
