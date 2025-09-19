using CarRentalSystem.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Features.KYC.Commands.AdminApproveOrRejectKYC
{
    public class AdminApproveOrRejectKYCCommandHandler : IRequestHandler<AdminApproveOrRejectKYCCommand, bool>
    {
        private readonly IKYCRepository _kycRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailService _emailService;

        public AdminApproveOrRejectKYCCommandHandler(IKYCRepository kycRepository, ICustomerRepository customerRepository, IEmailService emailService)
        {
            _kycRepository = kycRepository;
            _customerRepository = customerRepository;
            _emailService = emailService;
        }

        public async Task<bool> Handle(AdminApproveOrRejectKYCCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Processing KYC {request.KYCId} with status {request.NewStatus}");
            
            var kyc = await _kycRepository.GetKYCByIdAsync(request.KYCId, cancellationToken);
            if (kyc == null) 
            {
                Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: KYC {request.KYCId} not found");
                return false;
            }

            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Found KYC with current status: {kyc.Status}");

            // Optional: check if already approved/rejected
            if (kyc.Status == "Approved" || kyc.Status == "Rejected")
            {
                Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: KYC {request.KYCId} already processed with status {kyc.Status}");
                return false;
            }

            // Update status
            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Updating KYC {request.KYCId} to status {request.NewStatus}");
            var result = await _kycRepository.UpdateKYCStatusAsync(request.KYCId, request.NewStatus, cancellationToken);
            Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Update result: {result}");

            if (result && request.IsApproved)
            {
                // Send email notification for KYC approval
                try
                {
                    var customer = await _customerRepository.GetCustomerByIdAsync(kyc.UserId, cancellationToken);
                    if (customer != null && !string.IsNullOrEmpty(customer.Email))
                    {
                        var subject = "KYC Verification Approved - Car Rental System";
                        var body = $@"
                            <html>
                            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                                    <h2 style='color: #28a745; text-align: center;'>🎉 KYC Verification Approved!</h2>
                                    
                                    <p>Dear {customer.FullName},</p>
                                    
                                    <p>Great news! Your KYC (Know Your Customer) verification has been <strong style='color: #28a745;'>approved</strong> by our admin team.</p>
                                    
                                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                                        <h3 style='color: #495057; margin-top: 0;'>What's Next?</h3>
                                        <ul style='color: #6c757d;'>
                                            <li>✅ Your identity has been verified</li>
                                            <li>🚗 You can now proceed with car bookings</li>
                                            <li>💳 Complete your payment to confirm your booking</li>
                                            <li>📧 You'll receive booking confirmation via email</li>
                                        </ul>
                                    </div>
                                    
                                    <p>You can now continue with your car rental booking process. If you have any pending bookings, please proceed to complete the payment.</p>
                                    
                                    <div style='text-align: center; margin: 30px 0;'>
                                        <a href='http://localhost:5130/Dashboard/Bookings' 
                                           style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                            View My Bookings
                                        </a>
                                    </div>
                                    
                                    <p style='color: #6c757d; font-size: 14px;'>
                                        If you have any questions, please contact our support team at 
                                        <a href='mailto:support@carrental.com' style='color: #007bff;'>support@carrental.com</a>
                                    </p>
                                    
                                    <hr style='border: none; border-top: 1px solid #dee2e6; margin: 30px 0;'>
                                    <p style='color: #6c757d; font-size: 12px; text-align: center;'>
                                        This is an automated message from Car Rental System. Please do not reply to this email.
                                    </p>
                                </div>
                            </body>
                            </html>";

                        await _emailService.SendEmailAsync(customer.Email, subject, body);
                        Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Email sent to {customer.Email} for KYC approval");
                    }
                    else
                    {
                        Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Could not find customer or email for user {kyc.UserId}");
                    }
                }
                catch (Exception emailEx)
                {
                    Console.WriteLine($"AdminApproveOrRejectKYCCommandHandler: Failed to send email notification: {emailEx.Message}");
                    // Don't fail the KYC approval if email fails
                }
            }

            //  store remarks in a separate table if designed (not implemented here)
            return result;
        }
    }
}
