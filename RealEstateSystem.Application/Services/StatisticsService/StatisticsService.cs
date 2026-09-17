using System.IO;
using ClosedXML.Excel;
using RealEstateSystem.Application.DTOs.Response;
using RealEstateSystem.Application.Interfaces;
using RealEstateSystem.Domain.Exceptions;

namespace RealEstateSystem.Application.Services.StatisticsService
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public StatisticsService(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public async Task<DashboardOverviewResponse> GetDashboardOverviewAsync(CancellationToken cancellationToken)
        {
            var totalUsers = await _statisticsRepository.GetTotalUsersCountAsync(cancellationToken);
            var totalProperties = await _statisticsRepository.GetTotalPropertiesCountAsync(cancellationToken);
            var totalRevenue = await _statisticsRepository.GetTotalRevenueAsync(cancellationToken);
            var totalPremiumProperties = await _statisticsRepository.GetTotalPremiumPropertiesCountAsync(cancellationToken);

            return new DashboardOverviewResponse
            {
                TotalUsers = totalUsers,
                TotalProperties = totalProperties,
                TotalRevenue = totalRevenue,
                TotalPremiumProperties = totalPremiumProperties
            };
        }

        public async Task<List<MonthlyRevenueResponse>> GetMonthlyRevenueAsync(CancellationToken cancellationToken)
        {
            return await _statisticsRepository.GetMonthlyRevenueAsync(cancellationToken);
        }

        public async Task<List<MonthlyListingsResponse>> GetMonthlyListingsAsync(CancellationToken cancellationToken)
        {
            return await _statisticsRepository.GetMonthlyListingsAsync(cancellationToken);
        }

        public async Task<byte[]> ExportReportToExcelAsync(string reportType, CancellationToken cancellationToken)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Bao cao");

            if (reportType.Equals("revenue", StringComparison.OrdinalIgnoreCase))
            {
                worksheet.Cell(1, 1).Value = "Mã giao dịch";
                worksheet.Cell(1, 2).Value = "Email";
                worksheet.Cell(1, 3).Value = "Số tiền";
                worksheet.Cell(1, 4).Value = "Mã giao dịch cổng";
                worksheet.Cell(1, 5).Value = "Trạng thái";
                worksheet.Cell(1, 6).Value = "Thời gian thanh toán";

                var data = await _statisticsRepository.GetTransactionReportDataAsync(cancellationToken);
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.TransactionId.ToString();
                    worksheet.Cell(row, 2).Value = item.Email;
                    worksheet.Cell(row, 3).Value = item.Amount;
                    worksheet.Cell(row, 4).Value = item.TransactionCode;
                    worksheet.Cell(row, 5).Value = item.PaymentStatus;
                    if (item.PaidAt.HasValue)
                    {
                        worksheet.Cell(row, 6).Value = item.PaidAt.Value;
                    }
                    row++;
                }
            }
            else if (reportType.Equals("listings", StringComparison.OrdinalIgnoreCase))
            {
                worksheet.Cell(1, 1).Value = "Mã tin đăng";
                worksheet.Cell(1, 2).Value = "Tiêu đề";
                worksheet.Cell(1, 3).Value = "Email người đăng";
                worksheet.Cell(1, 4).Value = "Giá";
                worksheet.Cell(1, 5).Value = "Diện tích";
                worksheet.Cell(1, 6).Value = "Trạng thái duyệt";
                worksheet.Cell(1, 7).Value = "Ngày tạo";

                var data = await _statisticsRepository.GetPropertyReportDataAsync(cancellationToken);
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.PropertyId.ToString();
                    worksheet.Cell(row, 2).Value = item.Title;
                    worksheet.Cell(row, 3).Value = item.OwnerEmail;
                    worksheet.Cell(row, 4).Value = item.Price;
                    worksheet.Cell(row, 5).Value = item.Area;
                    worksheet.Cell(row, 6).Value = item.DisplayStatus;
                    worksheet.Cell(row, 7).Value = item.CreatedAt;
                    row++;
                }
            }
            else
            {
                throw new BadRequestException("Loại báo cáo không hợp lệ.");
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
