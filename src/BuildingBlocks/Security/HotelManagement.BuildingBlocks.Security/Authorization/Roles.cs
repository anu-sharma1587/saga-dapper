namespace HotelManagement.BuildingBlocks.Security.Authorization;

public static class PolicyNames
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireManager = "RequireManager";
    public const string RequireStaff = "RequireStaff";
    public const string RequireGuest = "RequireGuest";
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Staff = "Staff";
    public const string Guest = "Guest";
}
