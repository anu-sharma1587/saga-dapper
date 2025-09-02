using HotelManagement.Services.Guest.Services;
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
builder.Services.AddScoped<IGuestService, GuestService>();
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

app.Run();
