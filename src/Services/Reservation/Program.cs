using HotelManagement.Services.Reservation.Services;
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

// Add HTTP client for availability service
builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Availability"] 
        ?? throw new InvalidOperationException("Availability service URL not configured."));
});

// Add services
builder.Services.AddScoped<IReservationService, ReservationService>();

// Add controllers and API documentation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
    });

builder.Services.AddAuthorization();

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
