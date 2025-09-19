using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.AdminAssistant;
using CarRentalSystem.Domain.Entities;
using System.Drawing;
using System.Drawing.Imaging;

namespace CarRentalSystem.Application.Features.AdminAssistant
{
    public class ChartGenerationService : IChartGenerationService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public ChartGenerationService(
            ICustomerRepository customerRepository,
            IBookingRepository bookingRepository,
            ICarRepository carRepository,
            IInvoiceRepository invoiceRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _carRepository = carRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<byte[]> GenerateChartAsync(ChartRequest request, CancellationToken cancellationToken = default)
        {
            switch (request.DataType.ToLower())
            {
                case "bookings":
                    return await GenerateBookingChartAsync(request, cancellationToken);
                case "revenue":
                    return await GenerateRevenueChartAsync(request, cancellationToken);
                case "customers":
                    return await GenerateCustomerChartAsync(request, cancellationToken);
                case "cars":
                    return await GenerateCarChartAsync(request, cancellationToken);
                default:
                    throw new ArgumentException($"Unsupported data type: {request.DataType}");
            }
        }

        private async Task<byte[]> GenerateBookingChartAsync(ChartRequest request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
            
            // Group bookings by date for the chart
            var bookingData = bookings
                .GroupBy(b => b.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            return CreateChart(bookingData.Select(x => x.Date.ToString("MM/dd")).ToArray(),
                             bookingData.Select(x => x.Count).ToArray(),
                             "Bookings Over Time",
                             "Date",
                             "Number of Bookings",
                             request.ChartType);
        }

        private async Task<byte[]> GenerateRevenueChartAsync(ChartRequest request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
            
            // Group bookings by date for revenue chart
            var revenueData = bookings
                .GroupBy(b => b.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(b => b.TotalCost) })
                .OrderBy(x => x.Date)
                .ToList();

            return CreateChart(revenueData.Select(x => x.Date.ToString("MM/dd")).ToArray(),
                             revenueData.Select(x => (int)x.Revenue).ToArray(),
                             "Revenue Over Time",
                             "Date",
                             "Revenue ($)",
                             request.ChartType);
        }

        private async Task<byte[]> GenerateCustomerChartAsync(ChartRequest request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllCustomersAsync(cancellationToken);
            
            // Group customers by KYC status
            var kycVerified = customers.Count(c => c.IsKycVerified);
            var kycNotVerified = customers.Count(c => !c.IsKycVerified);

            return CreatePieChart(new[] { "KYC Verified", "KYC Not Verified" },
                                new[] { kycVerified, kycNotVerified },
                                "Customer KYC Status");
        }

        private async Task<byte[]> GenerateCarChartAsync(ChartRequest request, CancellationToken cancellationToken)
        {
            var cars = await _carRepository.GetAllCarsAsync(cancellationToken);
            
            // Group cars by approval status
            var statusGroups = cars
                .GroupBy(c => c.CarApprovalStatus?.Name ?? "Unknown")
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            return CreatePieChart(statusGroups.Select(x => x.Status).ToArray(),
                                statusGroups.Select(x => x.Count).ToArray(),
                                "Car Approval Status");
        }

        private byte[] CreateChart(string[] labels, int[] values, string title, string xLabel, string yLabel, string chartType)
        {
            const int width = 800;
            const int height = 600;
            const int margin = 50;

            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            
            // Clear background
            graphics.Clear(Color.White);
            
            // Set up fonts
            using var titleFont = new Font("Arial", 16, FontStyle.Bold);
            using var labelFont = new Font("Arial", 12);
            using var valueFont = new Font("Arial", 10);
            
            // Draw title
            var titleSize = graphics.MeasureString(title, titleFont);
            graphics.DrawString(title, titleFont, Brushes.Black, 
                (width - titleSize.Width) / 2, 10);
            
            // Calculate chart area
            var chartWidth = width - 2 * margin;
            var chartHeight = height - 2 * margin - 50;
            var chartX = margin;
            var chartY = margin + 40;
            
            if (values.Length == 0) return BitmapToByteArray(bitmap);
            
            var maxValue = values.Max();
            var barWidth = chartWidth / values.Length;
            
            // Draw bars or lines based on chart type
            if (chartType.ToLower() == "bar")
            {
                DrawBarChart(graphics, values, labels, chartX, chartY, chartWidth, chartHeight, barWidth, maxValue);
            }
            else if (chartType.ToLower() == "line")
            {
                DrawLineChart(graphics, values, labels, chartX, chartY, chartWidth, chartHeight, maxValue);
            }
            else
            {
                // Default to bar chart
                DrawBarChart(graphics, values, labels, chartX, chartY, chartWidth, chartHeight, barWidth, maxValue);
            }
            
            // Draw axes labels
            graphics.DrawString(xLabel, labelFont, Brushes.Black, 
                chartX + chartWidth / 2 - 50, chartY + chartHeight + 20);
            
            // Draw Y-axis label (rotated)
            var yLabelSize = graphics.MeasureString(yLabel, labelFont);
            graphics.TranslateTransform(20, chartY + chartHeight / 2);
            graphics.RotateTransform(-90);
            graphics.DrawString(yLabel, labelFont, Brushes.Black, 0, 0);
            graphics.ResetTransform();
            
            return BitmapToByteArray(bitmap);
        }

        private void DrawBarChart(Graphics graphics, int[] values, string[] labels, 
            int chartX, int chartY, int chartWidth, int chartHeight, int barWidth, int maxValue)
        {
            var colors = new[] { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple };
            
            for (int i = 0; i < values.Length; i++)
            {
                var barHeight = (int)((double)values[i] / maxValue * chartHeight);
                var x = chartX + i * barWidth;
                var y = chartY + chartHeight - barHeight;
                
                using var brush = new SolidBrush(colors[i % colors.Length]);
                graphics.FillRectangle(brush, x, y, barWidth - 2, barHeight);
                
                // Draw value on top of bar
                if (barHeight > 20)
                {
                    graphics.DrawString(values[i].ToString(), new Font("Arial", 8), 
                        Brushes.Black, x + barWidth / 2 - 10, y - 15);
                }
                
                // Draw label
                graphics.DrawString(labels[i], new Font("Arial", 8), 
                    Brushes.Black, x, chartY + chartHeight + 5);
            }
        }

        private void DrawLineChart(Graphics graphics, int[] values, string[] labels, 
            int chartX, int chartY, int chartWidth, int chartHeight, int maxValue)
        {
            if (values.Length < 2) return;
            
            var points = new PointF[values.Length];
            var stepX = (float)chartWidth / (values.Length - 1);
            
            for (int i = 0; i < values.Length; i++)
            {
                var x = chartX + i * stepX;
                var y = chartY + chartHeight - (float)values[i] / maxValue * chartHeight;
                points[i] = new PointF(x, y);
            }
            
            // Draw line
            using var pen = new Pen(Color.Blue, 3);
            graphics.DrawLines(pen, points);
            
            // Draw points
            foreach (var point in points)
            {
                graphics.FillEllipse(Brushes.Red, point.X - 3, point.Y - 3, 6, 6);
            }
            
            // Draw labels
            for (int i = 0; i < labels.Length; i++)
            {
                graphics.DrawString(labels[i], new Font("Arial", 8), 
                    Brushes.Black, chartX + i * stepX - 20, chartY + chartHeight + 5);
            }
        }

        private byte[] CreatePieChart(string[] labels, int[] values, string title)
        {
            const int width = 600;
            const int height = 400;
            
            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            
            // Clear background
            graphics.Clear(Color.White);
            
            // Set up fonts
            using var titleFont = new Font("Arial", 16, FontStyle.Bold);
            using var labelFont = new Font("Arial", 10);
            
            // Draw title
            var titleSize = graphics.MeasureString(title, titleFont);
            graphics.DrawString(title, titleFont, Brushes.Black, 
                (width - titleSize.Width) / 2, 10);
            
            if (values.Length == 0) return BitmapToByteArray(bitmap);
            
            var total = values.Sum();
            var colors = new[] { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple, Color.Yellow };
            
            // Draw pie chart
            var pieRect = new Rectangle(50, 50, 300, 300);
            var startAngle = 0f;
            
            for (int i = 0; i < values.Length; i++)
            {
                var sweepAngle = (float)values[i] / total * 360;
                
                using var brush = new SolidBrush(colors[i % colors.Length]);
                graphics.FillPie(brush, pieRect, startAngle, sweepAngle);
                
                // Draw label
                var labelX = 400;
                var labelY = 80 + i * 30;
                graphics.FillRectangle(brush, labelX, labelY, 15, 15);
                graphics.DrawString($"{labels[i]} ({values[i]})", labelFont, 
                    Brushes.Black, labelX + 20, labelY);
                
                startAngle += sweepAngle;
            }
            
            return BitmapToByteArray(bitmap);
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return stream.ToArray();
        }
    }
}
