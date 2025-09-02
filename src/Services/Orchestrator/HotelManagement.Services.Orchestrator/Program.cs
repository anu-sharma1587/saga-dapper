using HotelManagement.Services.Orchestrator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Reservation", c => c.BaseAddress = new Uri(builder.Configuration["Services:Reservation"]!));
builder.Services.AddHttpClient("Payment", c => c.BaseAddress = new Uri(builder.Configuration["Services:Payment"]!));
builder.Services.AddHttpClient("Billing", c => c.BaseAddress = new Uri(builder.Configuration["Services:Billing"]!));
builder.Services.AddHttpClient("AvailabilityPricing", c => c.BaseAddress = new Uri(builder.Configuration["Services:AvailabilityPricing"]!));
builder.Services.AddHttpClient("Search", c => c.BaseAddress = new Uri(builder.Configuration["Services:Search"]!));
builder.Services.AddHttpClient("Reporting", c => c.BaseAddress = new Uri(builder.Configuration["Services:Reporting"]!));
builder.Services.AddHttpClient("CheckInOut", c => c.BaseAddress = new Uri(builder.Configuration["Services:CheckInOut"]!));
builder.Services.AddHttpClient("Housekeeping", c => c.BaseAddress = new Uri(builder.Configuration["Services:Housekeeping"]!));
builder.Services.AddHttpClient("Maintenance", c => c.BaseAddress = new Uri(builder.Configuration["Services:Maintenance"]!));
builder.Services.AddHttpClient("Notifications", c => c.BaseAddress = new Uri(builder.Configuration["Services:Notifications"]!));
builder.Services.AddHttpClient("Loyalty", c => c.BaseAddress = new Uri(builder.Configuration["Services:Loyalty"]!));
builder.Services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
