using CarRentalSystem.Application.Common.Interfaces;
using CarRentalSystem.Application.Contracts.Booking;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace CarRentalSystem.Infrastructure.External.Services
{
    public class PdfService : IPdfService
    {
        public async Task<byte[]> GenerateReceiptPdfAsync(BookingDto booking, string paymentId, decimal amount)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 25, 25);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            
            document.Open();

            // Add title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            var title = new Paragraph("PAYMENT RECEIPT", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Add receipt details
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);

            // Receipt ID and Date
            var receiptInfo = new Paragraph();
            receiptInfo.Add(new Chunk("Receipt ID: ", boldFont));
            receiptInfo.Add(new Chunk(paymentId, normalFont));
            receiptInfo.Add(new Chunk("\nDate: ", boldFont));
            receiptInfo.Add(new Chunk(DateTime.Now.ToString("MMM dd, yyyy HH:mm"), normalFont));
            receiptInfo.SpacingAfter = 15;
            document.Add(receiptInfo);

            // Customer Information
            var customerInfo = new Paragraph();
            customerInfo.Add(new Chunk("Customer Information", boldFont));
            customerInfo.SpacingAfter = 10;
            document.Add(customerInfo);

            var customerDetails = new Paragraph();
            customerDetails.Add(new Chunk("Name: ", boldFont));
            customerDetails.Add(new Chunk(booking.CustomerName, normalFont));
            customerDetails.Add(new Chunk("\nEmail: ", boldFont));
            customerDetails.Add(new Chunk("N/A", normalFont));
            customerDetails.SpacingAfter = 15;
            document.Add(customerDetails);

            // Booking Information
            var bookingInfo = new Paragraph();
            bookingInfo.Add(new Chunk("Booking Information", boldFont));
            bookingInfo.SpacingAfter = 10;
            document.Add(bookingInfo);

            var bookingDetails = new Paragraph();
            bookingDetails.Add(new Chunk("Booking ID: ", boldFont));
            bookingDetails.Add(new Chunk(booking.BookingId.ToString(), normalFont));
            bookingDetails.Add(new Chunk("\nCar: ", boldFont));
            bookingDetails.Add(new Chunk($"{booking.CarName} {booking.Car.Model}", normalFont));
            bookingDetails.Add(new Chunk("\nPickup Date: ", boldFont));
            bookingDetails.Add(new Chunk(booking.PickupDate.ToString("MMM dd, yyyy HH:mm"), normalFont));
            bookingDetails.Add(new Chunk("\nReturn Date: ", boldFont));
            bookingDetails.Add(new Chunk(booking.ReturnDate.ToString("MMM dd, yyyy HH:mm"), normalFont));
            bookingDetails.Add(new Chunk("\nPickup Location: ", boldFont));
            bookingDetails.Add(new Chunk(booking.PickupLocation, normalFont));
            bookingDetails.Add(new Chunk("\nDrop Location: ", boldFont));
            bookingDetails.Add(new Chunk(booking.DropLocation, normalFont));
            bookingDetails.SpacingAfter = 15;
            document.Add(bookingDetails);

            // Payment Information
            var paymentInfo = new Paragraph();
            paymentInfo.Add(new Chunk("Payment Information", boldFont));
            paymentInfo.SpacingAfter = 10;
            document.Add(paymentInfo);

            var paymentDetails = new Paragraph();
            paymentDetails.Add(new Chunk("Amount Paid: ", boldFont));
            paymentDetails.Add(new Chunk($"${amount:F2}", normalFont));
            paymentDetails.Add(new Chunk("\nPayment Method: ", boldFont));
            paymentDetails.Add(new Chunk("Credit Card (Stripe)", normalFont));
            paymentDetails.Add(new Chunk("\nStatus: ", boldFont));
            paymentDetails.Add(new Chunk("PAID", normalFont));
            paymentDetails.SpacingAfter = 20;
            document.Add(paymentDetails);

            // Thank you message
            var thankYou = new Paragraph("Thank you for choosing our car rental service!", normalFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 20
            };
            document.Add(thankYou);

            document.Close();
            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(BookingDto booking, string invoiceId)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 40, 40, 40, 40);
            var writer = PdfWriter.GetInstance(document, memoryStream);
            
            document.Open();

            // Define brand colors
            var primaryBlue = new BaseColor(35, 35, 255); // #2323FF
            var lightBlue = new BaseColor(74, 74, 255);   // #4A4AFF
            var darkBlue = new BaseColor(26, 26, 204);    // #1A1ACC
            var accentPurple = new BaseColor(240, 147, 251); // #F093FB
            var lightGray = new BaseColor(248, 249, 250);
            var darkGray = new BaseColor(52, 58, 64);

            // Add header with brand styling
            var headerTable = new PdfPTable(2);
            headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new float[] { 2f, 1f });

            // Company info cell
            var companyCell = new PdfPCell();
            companyCell.Border = Rectangle.NO_BORDER;
            companyCell.Padding = 0;

            var companyFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, primaryBlue);
            var companyName = new Paragraph("CarRental", companyFont);
            companyName.SpacingAfter = 5;

            var taglineFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, darkGray);
            var tagline = new Paragraph("Drive Your Dreams", taglineFont);
            tagline.SpacingAfter = 10;

            var contactFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, darkGray);
            var contactInfo = new Paragraph();
            contactInfo.Add(new Chunk("📧 support@carrental.com\n", contactFont));
            contactInfo.Add(new Chunk("📞 +94 777 353 481\n", contactFont));
            contactInfo.Add(new Chunk("🌐 www.carrental.com", contactFont));

            companyCell.AddElement(companyName);
            companyCell.AddElement(tagline);
            companyCell.AddElement(contactInfo);

            // Invoice title cell
            var titleCell = new PdfPCell();
            titleCell.Border = Rectangle.NO_BORDER;
            titleCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            titleCell.VerticalAlignment = Element.ALIGN_TOP;

            var invoiceTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 28, primaryBlue);
            var invoiceTitle = new Paragraph("INVOICE", invoiceTitleFont);
            invoiceTitle.Alignment = Element.ALIGN_RIGHT;
            invoiceTitle.SpacingAfter = 5;

            var invoiceIdFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, darkGray);
            var invoiceIdText = new Paragraph($"#{invoiceId}", invoiceIdFont);
            invoiceIdText.Alignment = Element.ALIGN_RIGHT;

            titleCell.AddElement(invoiceTitle);
            titleCell.AddElement(invoiceIdText);

            headerTable.AddCell(companyCell);
            headerTable.AddCell(titleCell);
            headerTable.SpacingAfter = 20;
            document.Add(headerTable);

            // Add decorative line with simple approach
            document.Add(new Paragraph("_".PadRight(80, '_'), FontFactory.GetFont(FontFactory.HELVETICA, 8, primaryBlue)));
            document.Add(new Paragraph(" "));

            // Add invoice details
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLACK);
            var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, darkGray);

            // Invoice details table
            var detailsTable = new PdfPTable(2);
            detailsTable.WidthPercentage = 100;
            detailsTable.SetWidths(new float[] { 1f, 1f });

            // Left column - Invoice details
            var leftCell = new PdfPCell();
            leftCell.Border = Rectangle.NO_BORDER;
            leftCell.Padding = 10;
            leftCell.BackgroundColor = lightGray;

            var invoiceDetailsTitle = new Paragraph("Invoice Details", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryBlue));
            invoiceDetailsTitle.SpacingAfter = 8;
            leftCell.AddElement(invoiceDetailsTitle);

            var invoiceDetails = new Paragraph();
            invoiceDetails.Add(new Chunk("Invoice Date: ", boldFont));
            invoiceDetails.Add(new Chunk(DateTime.Now.ToString("MMM dd, yyyy"), normalFont));
            invoiceDetails.Add(new Chunk("\nDue Date: ", boldFont));
            invoiceDetails.Add(new Chunk(DateTime.Now.AddDays(30).ToString("MMM dd, yyyy"), normalFont));
            invoiceDetails.Add(new Chunk("\nPayment Terms: ", boldFont));
            invoiceDetails.Add(new Chunk("Net 30", normalFont));
            leftCell.AddElement(invoiceDetails);

            // Right column - Customer details
            var rightCell = new PdfPCell();
            rightCell.Border = Rectangle.NO_BORDER;
            rightCell.Padding = 10;
            rightCell.BackgroundColor = lightGray;

            var customerTitle = new Paragraph("Bill To", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryBlue));
            customerTitle.SpacingAfter = 8;
            rightCell.AddElement(customerTitle);

            var customerDetails = new Paragraph();
            customerDetails.Add(new Chunk(booking.CustomerName, boldFont));
            customerDetails.Add(new Chunk("\nCustomer ID: ", smallFont));
            customerDetails.Add(new Chunk(booking.BookingId.ToString().Substring(0, 8).ToUpper(), smallFont));
            customerDetails.Add(new Chunk("\nBooking Reference: ", smallFont));
            customerDetails.Add(new Chunk($"#{booking.BookingId.ToString().Substring(0, 8).ToUpper()}", smallFont));
            rightCell.AddElement(customerDetails);

            detailsTable.AddCell(leftCell);
            detailsTable.AddCell(rightCell);
            detailsTable.SpacingAfter = 20;
            document.Add(detailsTable);

            // Booking Information Section
            var bookingSectionTitle = new Paragraph("Rental Information", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, primaryBlue));
            bookingSectionTitle.SpacingAfter = 10;
            document.Add(bookingSectionTitle);

            var bookingTable = new PdfPTable(2);
            bookingTable.WidthPercentage = 100;
            bookingTable.SetWidths(new float[] { 1f, 1f });

            // Left booking details
            var bookingLeftCell = new PdfPCell();
            bookingLeftCell.Border = Rectangle.NO_BORDER;
            bookingLeftCell.Padding = 8;

            var bookingLeft = new Paragraph();
            bookingLeft.Add(new Chunk("🚗 Vehicle: ", boldFont));
            bookingLeft.Add(new Chunk($"{booking.CarName}", normalFont));
            bookingLeft.Add(new Chunk("\n📅 Pickup Date: ", boldFont));
            bookingLeft.Add(new Chunk(booking.PickupDate.ToString("MMM dd, yyyy 'at' HH:mm"), normalFont));
            bookingLeft.Add(new Chunk("\n📍 Pickup Location: ", boldFont));
            bookingLeft.Add(new Chunk(booking.PickupLocation, normalFont));
            bookingLeftCell.AddElement(bookingLeft);

            // Right booking details
            var bookingRightCell = new PdfPCell();
            bookingRightCell.Border = Rectangle.NO_BORDER;
            bookingRightCell.Padding = 8;

            var bookingRight = new Paragraph();
            bookingRight.Add(new Chunk("🆔 Booking ID: ", boldFont));
            bookingRight.Add(new Chunk($"#{booking.BookingId.ToString().Substring(0, 8).ToUpper()}", normalFont));
            bookingRight.Add(new Chunk("\n📅 Return Date: ", boldFont));
            bookingRight.Add(new Chunk(booking.ReturnDate.ToString("MMM dd, yyyy 'at' HH:mm"), normalFont));
            bookingRight.Add(new Chunk("\n📍 Drop Location: ", boldFont));
            bookingRight.Add(new Chunk(booking.DropLocation, normalFont));
            bookingRightCell.AddElement(bookingRight);

            bookingTable.AddCell(bookingLeftCell);
            bookingTable.AddCell(bookingRightCell);
            bookingTable.SpacingAfter = 20;
            document.Add(bookingTable);

            // Invoice Items Section
            var itemsTitle = new Paragraph("Invoice Items", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, primaryBlue));
            itemsTitle.SpacingAfter = 10;
            document.Add(itemsTitle);

            // Create a table for the invoice items
            var table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 3f, 1f, 1f, 1f });

            // Table headers with brand styling
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
            var headerCell1 = new PdfPCell(new Phrase("Description", headerFont));
            var headerCell2 = new PdfPCell(new Phrase("Days", headerFont));
            var headerCell3 = new PdfPCell(new Phrase("Rate/Day", headerFont));
            var headerCell4 = new PdfPCell(new Phrase("Amount", headerFont));

            headerCell1.BackgroundColor = primaryBlue;
            headerCell2.BackgroundColor = primaryBlue;
            headerCell3.BackgroundColor = primaryBlue;
            headerCell4.BackgroundColor = primaryBlue;
            headerCell1.HorizontalAlignment = Element.ALIGN_LEFT;
            headerCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            headerCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            headerCell4.HorizontalAlignment = Element.ALIGN_RIGHT;
            headerCell1.Padding = 8;
            headerCell2.Padding = 8;
            headerCell3.Padding = 8;
            headerCell4.Padding = 8;

            table.AddCell(headerCell1);
            table.AddCell(headerCell2);
            table.AddCell(headerCell3);
            table.AddCell(headerCell4);

            // Calculate days and rates
            var days = (int)(booking.ReturnDate - booking.PickupDate).TotalDays;
            var dailyRate = booking.TotalCost / days;

            // Add rental item with better styling
            var cell1 = new PdfPCell(new Phrase($"🚗 Car Rental - {booking.CarName}", normalFont));
            var cell2 = new PdfPCell(new Phrase(days.ToString(), normalFont));
            var cell3 = new PdfPCell(new Phrase($"${dailyRate:F2}", normalFont));
            var cell4 = new PdfPCell(new Phrase($"${booking.TotalCost:F2}", normalFont));

            cell1.HorizontalAlignment = Element.ALIGN_LEFT;
            cell2.HorizontalAlignment = Element.ALIGN_CENTER;
            cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.Padding = 8;
            cell2.Padding = 8;
            cell3.Padding = 8;
            cell4.Padding = 8;

            table.AddCell(cell1);
            table.AddCell(cell2);
            table.AddCell(cell3);
            table.AddCell(cell4);

            // Add empty row for spacing
            var emptyCell1 = new PdfPCell(new Phrase("", normalFont));
            var emptyCell2 = new PdfPCell(new Phrase("", normalFont));
            var emptyCell3 = new PdfPCell(new Phrase("", normalFont));
            var emptyCell4 = new PdfPCell(new Phrase("", normalFont));
            emptyCell1.Border = Rectangle.NO_BORDER;
            emptyCell2.Border = Rectangle.NO_BORDER;
            emptyCell3.Border = Rectangle.NO_BORDER;
            emptyCell4.Border = Rectangle.NO_BORDER;
            emptyCell1.Padding = 4;
            emptyCell2.Padding = 4;
            emptyCell3.Padding = 4;
            emptyCell4.Padding = 4;

            table.AddCell(emptyCell1);
            table.AddCell(emptyCell2);
            table.AddCell(emptyCell3);
            table.AddCell(emptyCell4);

            // Add subtotal row
            var subtotalCell1 = new PdfPCell(new Phrase("Subtotal", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, darkGray)));
            var subtotalCell2 = new PdfPCell(new Phrase("", normalFont));
            var subtotalCell3 = new PdfPCell(new Phrase("", normalFont));
            var subtotalCell4 = new PdfPCell(new Phrase($"${booking.TotalCost:F2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, darkGray)));

            subtotalCell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            subtotalCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            subtotalCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            subtotalCell4.HorizontalAlignment = Element.ALIGN_RIGHT;
            subtotalCell1.Colspan = 3;
            subtotalCell1.Border = Rectangle.NO_BORDER;
            subtotalCell2.Border = Rectangle.NO_BORDER;
            subtotalCell3.Border = Rectangle.NO_BORDER;
            subtotalCell4.Border = Rectangle.NO_BORDER;
            subtotalCell1.Padding = 8;
            subtotalCell2.Padding = 8;
            subtotalCell3.Padding = 8;
            subtotalCell4.Padding = 8;

            table.AddCell(subtotalCell1);
            table.AddCell(subtotalCell2);
            table.AddCell(subtotalCell3);
            table.AddCell(subtotalCell4);

            // Add total row with brand styling
            var totalCell1 = new PdfPCell(new Phrase("TOTAL", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE)));
            var totalCell2 = new PdfPCell(new Phrase("", normalFont));
            var totalCell3 = new PdfPCell(new Phrase("", normalFont));
            var totalCell4 = new PdfPCell(new Phrase($"${booking.TotalCost:F2}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE)));

            totalCell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            totalCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            totalCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            totalCell4.HorizontalAlignment = Element.ALIGN_RIGHT;
            totalCell1.Colspan = 3;
            totalCell1.BackgroundColor = primaryBlue;
            totalCell2.BackgroundColor = primaryBlue;
            totalCell3.BackgroundColor = primaryBlue;
            totalCell4.BackgroundColor = primaryBlue;
            totalCell1.Padding = 12;
            totalCell2.Padding = 12;
            totalCell3.Padding = 12;
            totalCell4.Padding = 12;

            table.AddCell(totalCell1);
            table.AddCell(totalCell2);
            table.AddCell(totalCell3);
            table.AddCell(totalCell4);

            table.SpacingAfter = 25;
            document.Add(table);

            // Payment status section
            var paymentStatusTable = new PdfPTable(2);
            paymentStatusTable.WidthPercentage = 100;
            paymentStatusTable.SetWidths(new float[] { 1f, 1f });

            // Payment status
            var paymentCell = new PdfPCell();
            paymentCell.Border = Rectangle.NO_BORDER;
            paymentCell.Padding = 10;
            paymentCell.BackgroundColor = lightGray;

            var paymentTitle = new Paragraph("Payment Information", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryBlue));
            paymentTitle.SpacingAfter = 8;
            paymentCell.AddElement(paymentTitle);

            var paymentDetails = new Paragraph();
            paymentDetails.Add(new Chunk("Status: ", boldFont));
            var statusColor = booking.PaymentStatus == "Paid" ? BaseColor.GREEN : BaseColor.ORANGE;
            paymentDetails.Add(new Chunk(booking.PaymentStatus ?? "Pending", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, statusColor)));
            paymentDetails.Add(new Chunk("\nMethod: ", boldFont));
            paymentDetails.Add(new Chunk("Credit Card (Stripe)", normalFont));
            paymentDetails.Add(new Chunk("\nDate: ", boldFont));
            paymentDetails.Add(new Chunk(DateTime.Now.ToString("MMM dd, yyyy"), normalFont));
            paymentCell.AddElement(paymentDetails);

            // Contact info
            var contactCell = new PdfPCell();
            contactCell.Border = Rectangle.NO_BORDER;
            contactCell.Padding = 10;
            contactCell.BackgroundColor = lightGray;

            var contactTitle = new Paragraph("Need Help?", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryBlue));
            contactTitle.SpacingAfter = 8;
            contactCell.AddElement(contactTitle);

            var contactDetails = new Paragraph();
            contactDetails.Add(new Chunk("📞 +94 777 353 481\n", smallFont));
            contactDetails.Add(new Chunk("📧 support@carrental.com\n", smallFont));
            contactDetails.Add(new Chunk("💬 WhatsApp Support Available", smallFont));
            contactCell.AddElement(contactDetails);

            paymentStatusTable.AddCell(paymentCell);
            paymentStatusTable.AddCell(contactCell);
            paymentStatusTable.SpacingAfter = 20;
            document.Add(paymentStatusTable);

            // Add decorative line
            document.Add(new Paragraph("_".PadRight(80, '_'), FontFactory.GetFont(FontFactory.HELVETICA, 8, primaryBlue)));
            document.Add(new Paragraph(" "));

            // Footer message
            var footerMessage = new Paragraph("Thank you for choosing CarRental! Drive safely and enjoy your journey.", 
                FontFactory.GetFont(FontFactory.HELVETICA, 10, darkGray))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 15
            };
            document.Add(footerMessage);

            var footerTagline = new Paragraph("Drive Your Dreams", 
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, primaryBlue))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 5
            };
            document.Add(footerTagline);

            document.Close();
            return memoryStream.ToArray();
        }
    }
}
