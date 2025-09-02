using HotelManagement.Services.Orchestrator.DTOs;
using HotelManagement.Services.Orchestrator.Models;
using System.Net.Http.Json;

namespace HotelManagement.Services.Orchestrator.Services;

public class SagaOrchestrator : ISagaOrchestrator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SagaOrchestrator> _logger;

    public SagaOrchestrator(IHttpClientFactory httpClientFactory, ILogger<SagaOrchestrator> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<SagaResult> StartReservationSagaAsync(StartReservationSagaRequest request)
    {
        var sagaId = Guid.NewGuid();
        var state = new SagaState
        {
            Id = sagaId,
            Type = "ReservationSaga",
            StartedAt = DateTime.UtcNow,
            Steps = new()
        };
        try
        {
            // 1. Create Reservation
            state.Steps.Add(new SagaStep { Name = "CreateReservation", Status = SagaStepStatus.Pending });
            var reservationClient = _httpClientFactory.CreateClient("Reservation");
            var reservationResponse = await reservationClient.PostAsJsonAsync("/api/reservations", new
            {
                request.HotelId,
                request.RoomTypeId,
                request.GuestId,
                request.CheckInDate,
                request.CheckOutDate,
                request.NumberOfRooms,
                request.NumberOfGuests
            });
            if (!reservationResponse.IsSuccessStatusCode)
                throw new Exception("Reservation creation failed");
            var reservation = await reservationResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid reservationId = reservation.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 2. Create AvailabilityPricing
            state.Steps.Add(new SagaStep { Name = "CreateAvailabilityPricing", Status = SagaStepStatus.Pending });
            var apClient = _httpClientFactory.CreateClient("AvailabilityPricing");
            var apResponse = await apClient.PostAsJsonAsync("/api/availabilitypricing", new
            {
                HotelId = request.HotelId,
                Date = request.CheckInDate,
                AvailableRooms = request.NumberOfRooms,
                PricePerNight = request.PricePerNight,
                RoomType = request.RoomTypeId
            });
            if (!apResponse.IsSuccessStatusCode)
                throw new Exception("AvailabilityPricing creation failed");
            var ap = await apResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid apId = ap.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 3. Search
            state.Steps.Add(new SagaStep { Name = "CreateSearch", Status = SagaStepStatus.Pending });
            var searchClient = _httpClientFactory.CreateClient("Search");
            var searchResponse = await searchClient.PostAsJsonAsync("/api/search", new
            {
                QueryText = $"Hotel:{request.HotelId},RoomType:{request.RoomTypeId},Date:{request.CheckInDate}",
                Type = "Reservation"
            });
            if (!searchResponse.IsSuccessStatusCode)
                throw new Exception("Search creation failed");
            var search = await searchResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid searchId = search.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 4. Reporting
            state.Steps.Add(new SagaStep { Name = "CreateReportJob", Status = SagaStepStatus.Pending });
            var reportingClient = _httpClientFactory.CreateClient("Reporting");
            var reportResponse = await reportingClient.PostAsJsonAsync("/api/reporting", new
            {
                Type = "Reservation",
                RequestedAt = DateTime.UtcNow
            });
            if (!reportResponse.IsSuccessStatusCode)
                throw new Exception("ReportJob creation failed");
            var report = await reportResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid reportId = report.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 5. Process Payment
            state.Steps.Add(new SagaStep { Name = "ProcessPayment", Status = SagaStepStatus.Pending });
            var paymentClient = _httpClientFactory.CreateClient("Payment");
            var paymentResponse = await paymentClient.PostAsJsonAsync("/api/payment", new
            {
                ReservationId = reservationId,
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMethod = request.PaymentMethod,
                PaymentProvider = request.PaymentProvider
            });
            if (!paymentResponse.IsSuccessStatusCode)
                throw new Exception("Payment failed");
            var payment = await paymentResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid paymentId = payment.paymentId;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 6. Create Invoice
            state.Steps.Add(new SagaStep { Name = "CreateInvoice", Status = SagaStepStatus.Pending });
            var billingClient = _httpClientFactory.CreateClient("Billing");
            var invoiceResponse = await billingClient.PostAsJsonAsync("/api/billing", new
            {
                ReservationId = reservationId,
                GuestId = request.GuestId,
                Amount = request.Amount,
                Currency = request.Currency
            });
            if (!invoiceResponse.IsSuccessStatusCode)
                throw new Exception("Invoice creation failed");
            var invoice = await invoiceResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid invoiceId = invoice.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 7. CheckInOut
            state.Steps.Add(new SagaStep { Name = "CreateCheckInOut", Status = SagaStepStatus.Pending });
            var checkInOutClient = _httpClientFactory.CreateClient("CheckInOut");
            var checkInOutResponse = await checkInOutClient.PostAsJsonAsync("/api/checkinout", new
            {
                ReservationId = reservationId,
                GuestId = request.GuestId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate
            });
            if (!checkInOutResponse.IsSuccessStatusCode)
                throw new Exception("CheckInOut creation failed");
            var checkInOut = await checkInOutResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid checkInOutId = checkInOut.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 8. Housekeeping
            state.Steps.Add(new SagaStep { Name = "CreateHousekeepingTask", Status = SagaStepStatus.Pending });
            var housekeepingClient = _httpClientFactory.CreateClient("Housekeeping");
            var housekeepingResponse = await housekeepingClient.PostAsJsonAsync("/api/housekeeping", new
            {
                RoomId = request.RoomTypeId,
                GuestId = request.GuestId,
                CheckInDate = request.CheckInDate
            });
            if (!housekeepingResponse.IsSuccessStatusCode)
                throw new Exception("HousekeepingTask creation failed");
            var housekeeping = await housekeepingResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid housekeepingId = housekeeping.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 9. Maintenance
            state.Steps.Add(new SagaStep { Name = "CreateMaintenanceRequest", Status = SagaStepStatus.Pending });
            var maintenanceClient = _httpClientFactory.CreateClient("Maintenance");
            var maintenanceResponse = await maintenanceClient.PostAsJsonAsync("/api/maintenance", new
            {
                RoomId = request.RoomTypeId,
                Issue = "Initial check"
            });
            if (!maintenanceResponse.IsSuccessStatusCode)
                throw new Exception("MaintenanceRequest creation failed");
            var maintenance = await maintenanceResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid maintenanceId = maintenance.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 10. Notifications
            state.Steps.Add(new SagaStep { Name = "SendNotification", Status = SagaStepStatus.Pending });
            var notificationsClient = _httpClientFactory.CreateClient("Notifications");
            var notificationResponse = await notificationsClient.PostAsJsonAsync("/api/notifications", new
            {
                RecipientId = request.GuestId,
                Message = "Reservation confirmed"
            });
            if (!notificationResponse.IsSuccessStatusCode)
                throw new Exception("Notification sending failed");
            var notification = await notificationResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid notificationId = notification.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            // 11. Loyalty
            state.Steps.Add(new SagaStep { Name = "UpdateLoyaltyAccount", Status = SagaStepStatus.Pending });
            var loyaltyClient = _httpClientFactory.CreateClient("Loyalty");
            var loyaltyResponse = await loyaltyClient.PostAsJsonAsync("/api/loyalty", new
            {
                GuestId = request.GuestId,
                Points = 100,
                Reason = "Reservation"
            });
            if (!loyaltyResponse.IsSuccessStatusCode)
                throw new Exception("Loyalty update failed");
            var loyalty = await loyaltyResponse.Content.ReadFromJsonAsync<dynamic>();
            Guid loyaltyId = loyalty.id;
            state.Steps[^1].Status = SagaStepStatus.Completed;

            state.IsCompleted = true;
            state.CompletedAt = DateTime.UtcNow;
            return new SagaResult { SagaId = sagaId, Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saga failed, starting compensation");
            await CompensateAsync(state);
            return new SagaResult { SagaId = sagaId, Success = false, Error = ex.Message };
        }
    }

    private async Task CompensateAsync(SagaState state)
    {
        // Example: If payment succeeded but invoice failed, refund payment
        for (int i = state.Steps.Count - 1; i >= 0; i--)
        {
            var step = state.Steps[i];
            if (step.Status == SagaStepStatus.Completed)
            {
                try
                {
                    switch (step.Name)
                    {
                        case "ProcessPayment":
                            var paymentClient = _httpClientFactory.CreateClient("Payment");
                            await paymentClient.PostAsync($"/api/payment/{step.Error}/compensate-refund", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateReservation":
                            var reservationClient = _httpClientFactory.CreateClient("Reservation");
                            await reservationClient.PostAsync($"/api/reservations/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateAvailabilityPricing":
                            var apClient = _httpClientFactory.CreateClient("AvailabilityPricing");
                            await apClient.PostAsync($"/api/availabilitypricing/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateSearch":
                            var searchClient = _httpClientFactory.CreateClient("Search");
                            await searchClient.PostAsync($"/api/search/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateReportJob":
                            var reportingClient = _httpClientFactory.CreateClient("Reporting");
                            await reportingClient.PostAsync($"/api/reporting/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateInvoice":
                            var billingClient = _httpClientFactory.CreateClient("Billing");
                            await billingClient.PostAsync($"/api/billing/{step.Error}/compensate-void", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateCheckInOut":
                            var checkInOutClient = _httpClientFactory.CreateClient("CheckInOut");
                            await checkInOutClient.PostAsync($"/api/checkinout/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateHousekeepingTask":
                            var housekeepingClient = _httpClientFactory.CreateClient("Housekeeping");
                            await housekeepingClient.PostAsync($"/api/housekeeping/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "CreateMaintenanceRequest":
                            var maintenanceClient = _httpClientFactory.CreateClient("Maintenance");
                            await maintenanceClient.PostAsync($"/api/maintenance/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "SendNotification":
                            var notificationsClient = _httpClientFactory.CreateClient("Notifications");
                            await notificationsClient.PostAsync($"/api/notifications/{step.Error}/compensate-cancel", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                        case "UpdateLoyaltyAccount":
                            var loyaltyClient = _httpClientFactory.CreateClient("Loyalty");
                            await loyaltyClient.PostAsync($"/api/loyalty/{step.Error}/compensate-revert-points", null);
                            step.Status = SagaStepStatus.Compensated;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    step.Error = ex.Message;
                    step.Status = SagaStepStatus.Failed;
                }
            }
        }
        state.IsCompensated = true;
    }
}
