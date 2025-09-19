using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.AdminAssistant;
using CarRentalSystem.Domain.Entities;
using OfficeOpenXml;
using System.Text;

namespace CarRentalSystem.Application.Features.AdminAssistant
{
    public class ReportGenerationService : IReportGenerationService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICarRepository _carRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public ReportGenerationService(
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

        public async Task<byte[]> GenerateExcelReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Report");

            switch (request.ReportType.ToLower())
            {
                case "customers":
                    await GenerateCustomerExcelReportAsync(worksheet, request, cancellationToken);
                    break;
                case "bookings":
                    await GenerateBookingExcelReportAsync(worksheet, request, cancellationToken);
                    break;
                case "cars":
                    await GenerateCarExcelReportAsync(worksheet, request, cancellationToken);
                    break;
                case "invoices":
                    await GenerateInvoiceExcelReportAsync(worksheet, request, cancellationToken);
                    break;
            }

            return package.GetAsByteArray();
        }

        public async Task<byte[]> GeneratePdfReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            // For PDF generation, we'll create a simple HTML-based PDF
            // In a real implementation, you might use libraries like iTextSharp or PuppeteerSharp
            var htmlContent = await GenerateHtmlReportAsync(request, cancellationToken);
            
            // Convert HTML to PDF (simplified - in production, use a proper HTML to PDF library)
            return Encoding.UTF8.GetBytes(htmlContent);
        }

        public async Task<byte[]> GenerateWordReportAsync(ReportRequest request, CancellationToken cancellationToken = default)
        {
            // For Word generation, we'll create a simple HTML-based document
            // In a real implementation, you might use libraries like DocumentFormat.OpenXml
            var htmlContent = await GenerateHtmlReportAsync(request, cancellationToken);
            
            // Convert HTML to Word (simplified - in production, use a proper HTML to Word library)
            return Encoding.UTF8.GetBytes(htmlContent);
        }

        private async Task GenerateCustomerExcelReportAsync(ExcelWorksheet worksheet, ReportRequest request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllCustomersAsync(cancellationToken);
            
            // Headers
            worksheet.Cells[1, 1].Value = "Customer ID";
            worksheet.Cells[1, 2].Value = "Full Name";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Address";
            worksheet.Cells[1, 5].Value = "KYC Verified";
            worksheet.Cells[1, 6].Value = "Total Bookings";

            // Data
            int row = 2;
            foreach (var customer in customers)
            {
                worksheet.Cells[row, 1].Value = customer.Id.ToString();
                worksheet.Cells[row, 2].Value = customer.FullName;
                worksheet.Cells[row, 3].Value = customer.Email;
                worksheet.Cells[row, 4].Value = customer.Address ?? "";
                worksheet.Cells[row, 5].Value = customer.IsKycVerified ? "Yes" : "No";
                worksheet.Cells[row, 6].Value = customer.Bookings?.Count ?? 0;
                row++;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
        }

        private async Task GenerateBookingExcelReportAsync(ExcelWorksheet worksheet, ReportRequest request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
            
            // Headers
            worksheet.Cells[1, 1].Value = "Booking ID";
            worksheet.Cells[1, 2].Value = "Customer Name";
            worksheet.Cells[1, 3].Value = "Car Model";
            worksheet.Cells[1, 4].Value = "Pickup Date";
            worksheet.Cells[1, 5].Value = "Return Date";
            worksheet.Cells[1, 6].Value = "Total Cost";
            worksheet.Cells[1, 7].Value = "Booking Status";
            worksheet.Cells[1, 8].Value = "Payment Status";

            // Data
            int row = 2;
            foreach (var booking in bookings)
            {
                worksheet.Cells[row, 1].Value = booking.BookingId.ToString();
                worksheet.Cells[row, 2].Value = booking.Customer?.FullName ?? "";
                worksheet.Cells[row, 3].Value = booking.Car?.Model ?? "";
                worksheet.Cells[row, 4].Value = booking.PickupDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 5].Value = booking.ReturnDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 6].Value = booking.TotalCost;
                worksheet.Cells[row, 7].Value = booking.BookingStatus?.Name ?? "";
                worksheet.Cells[row, 8].Value = booking.PaymentStatus?.Name ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
        }

        private async Task GenerateCarExcelReportAsync(ExcelWorksheet worksheet, ReportRequest request, CancellationToken cancellationToken)
        {
            var cars = await _carRepository.GetAllCarsAsync(cancellationToken);
            
            // Headers
            worksheet.Cells[1, 1].Value = "Car ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Model";
            worksheet.Cells[1, 4].Value = "Rate Per Day";
            worksheet.Cells[1, 5].Value = "Year";
            worksheet.Cells[1, 6].Value = "Transmission";
            worksheet.Cells[1, 7].Value = "Fuel Type";
            worksheet.Cells[1, 8].Value = "Approval Status";

            // Data
            int row = 2;
            foreach (var car in cars)
            {
                worksheet.Cells[row, 1].Value = car.CarId.ToString();
                worksheet.Cells[row, 2].Value = car.Name;
                worksheet.Cells[row, 3].Value = car.Model;
                worksheet.Cells[row, 4].Value = car.RatePerDay;
                worksheet.Cells[row, 5].Value = car.Year?.ToString() ?? "";
                worksheet.Cells[row, 6].Value = car.Transmission ?? "";
                worksheet.Cells[row, 7].Value = car.FuelType ?? "";
                worksheet.Cells[row, 8].Value = car.CarApprovalStatus?.Name ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
        }

        private async Task GenerateInvoiceExcelReportAsync(ExcelWorksheet worksheet, ReportRequest request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);
            
            // Headers
            worksheet.Cells[1, 1].Value = "Invoice ID";
            worksheet.Cells[1, 2].Value = "Payment ID";
            worksheet.Cells[1, 3].Value = "Generated At";
            worksheet.Cells[1, 4].Value = "File Path";

            // Data
            int row = 2;
            foreach (var invoice in invoices)
            {
                worksheet.Cells[row, 1].Value = invoice.InvoiceId.ToString();
                worksheet.Cells[row, 2].Value = invoice.PaymentId.ToString();
                worksheet.Cells[row, 3].Value = invoice.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[row, 4].Value = invoice.FilePath;
                row++;
            }

            // Auto-fit columns
            worksheet.Cells.AutoFitColumns();
        }

        private async Task<string> GenerateHtmlReportAsync(ReportRequest request, CancellationToken cancellationToken)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head><title>Car Rental Report</title></head><body>");
            html.AppendLine($"<h1>{request.ReportType.ToUpper()} Report</h1>");
            html.AppendLine($"<p>Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");

            switch (request.ReportType.ToLower())
            {
                case "customers":
                    await GenerateCustomerHtmlReportAsync(html, request, cancellationToken);
                    break;
                case "bookings":
                    await GenerateBookingHtmlReportAsync(html, request, cancellationToken);
                    break;
                case "cars":
                    await GenerateCarHtmlReportAsync(html, request, cancellationToken);
                    break;
                case "invoices":
                    await GenerateInvoiceHtmlReportAsync(html, request, cancellationToken);
                    break;
            }

            html.AppendLine("</body></html>");
            return html.ToString();
        }

        private async Task GenerateCustomerHtmlReportAsync(StringBuilder html, ReportRequest request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetAllCustomersAsync(cancellationToken);
            
            html.AppendLine("<table border='1'><tr>");
            html.AppendLine("<th>Customer ID</th><th>Full Name</th><th>Email</th><th>Address</th><th>KYC Verified</th><th>Total Bookings</th>");
            html.AppendLine("</tr>");

            foreach (var customer in customers)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{customer.Id}</td>");
                html.AppendLine($"<td>{customer.FullName}</td>");
                html.AppendLine($"<td>{customer.Email}</td>");
                html.AppendLine($"<td>{customer.Address ?? ""}</td>");
                html.AppendLine($"<td>{(customer.IsKycVerified ? "Yes" : "No")}</td>");
                html.AppendLine($"<td>{customer.Bookings?.Count ?? 0}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
        }

        private async Task GenerateBookingHtmlReportAsync(StringBuilder html, ReportRequest request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync(cancellationToken);
            
            html.AppendLine("<table border='1'><tr>");
            html.AppendLine("<th>Booking ID</th><th>Customer Name</th><th>Car Model</th><th>Pickup Date</th><th>Return Date</th><th>Total Cost</th><th>Status</th>");
            html.AppendLine("</tr>");

            foreach (var booking in bookings)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{booking.BookingId}</td>");
                html.AppendLine($"<td>{booking.Customer?.FullName ?? ""}</td>");
                html.AppendLine($"<td>{booking.Car?.Model ?? ""}</td>");
                html.AppendLine($"<td>{booking.PickupDate:yyyy-MM-dd}</td>");
                html.AppendLine($"<td>{booking.ReturnDate:yyyy-MM-dd}</td>");
                html.AppendLine($"<td>${booking.TotalCost:F2}</td>");
                html.AppendLine($"<td>{booking.BookingStatus?.Name ?? ""}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
        }

        private async Task GenerateCarHtmlReportAsync(StringBuilder html, ReportRequest request, CancellationToken cancellationToken)
        {
            var cars = await _carRepository.GetAllCarsAsync(cancellationToken);
            
            html.AppendLine("<table border='1'><tr>");
            html.AppendLine("<th>Car ID</th><th>Name</th><th>Model</th><th>Rate Per Day</th><th>Year</th><th>Transmission</th><th>Fuel Type</th>");
            html.AppendLine("</tr>");

            foreach (var car in cars)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{car.CarId}</td>");
                html.AppendLine($"<td>{car.Name}</td>");
                html.AppendLine($"<td>{car.Model}</td>");
                html.AppendLine($"<td>${car.RatePerDay:F2}</td>");
                html.AppendLine($"<td>{car.Year?.ToString() ?? ""}</td>");
                html.AppendLine($"<td>{car.Transmission ?? ""}</td>");
                html.AppendLine($"<td>{car.FuelType ?? ""}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
        }

        private async Task GenerateInvoiceHtmlReportAsync(StringBuilder html, ReportRequest request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);
            
            html.AppendLine("<table border='1'><tr>");
            html.AppendLine("<th>Invoice ID</th><th>Payment ID</th><th>Generated At</th><th>File Path</th>");
            html.AppendLine("</tr>");

            foreach (var invoice in invoices)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{invoice.InvoiceId}</td>");
                html.AppendLine($"<td>{invoice.PaymentId}</td>");
                html.AppendLine($"<td>{invoice.GeneratedAt:yyyy-MM-dd HH:mm:ss}</td>");
                html.AppendLine($"<td>{invoice.FilePath}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
        }
    }
}
