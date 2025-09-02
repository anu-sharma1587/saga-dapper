namespace HotelManagement.Services.Availability.Events;

public static class EventTypes
{
    public static class ChangeTrigger
    {
        public const string PriceUpdate = "PriceUpdate";
        public const string InventoryUpdate = "InventoryUpdate";
        public const string InventoryBlock = "InventoryBlock";
        public const string SeasonalPeriod = "SeasonalPeriod";
        public const string PricingRule = "PricingRule";
        public const string SpecialEvent = "SpecialEvent";
        public const string DemandForecast = "DemandForecast";
        public const string Recalculation = "Recalculation";
        public const string Manual = "Manual";
    }

    public static class PriceFactorType
    {
        public const string Base = "Base";
        public const string SeasonalPeriod = "SeasonalPeriod";
        public const string PricingRule = "PricingRule";
        public const string SpecialEvent = "SpecialEvent";
        public const string DemandForecast = "DemandForecast";
        public const string Manual = "Manual";
    }
}
