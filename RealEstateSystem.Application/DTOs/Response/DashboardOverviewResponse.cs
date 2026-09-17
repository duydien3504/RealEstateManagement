namespace RealEstateSystem.Application.DTOs.Response
{
    public class DashboardOverviewResponse
    {
        public int TotalUsers { get; set; }
        public int TotalProperties { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalPremiumProperties { get; set; }
    }
}
