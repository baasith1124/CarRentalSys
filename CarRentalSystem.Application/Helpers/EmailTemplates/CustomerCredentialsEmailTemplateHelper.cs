using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Helpers.EmailTemplates
{
    public static class CustomerCredentialsEmailTemplateHelper
    {
        public static string GenerateHtml(string customerName, string email, string password)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{
                            font-family: 'Segoe UI', sans-serif;
                            background-color: #f9f9f9;
                            color: #333;
                            padding: 20px;
                        }}
                        .container {{
                            background-color: #ffffff;
                            border: 1px solid #dddddd;
                            border-radius: 8px;
                            padding: 20px;
                            max-width: 600px;
                            margin: auto;
                        }}
                        .header {{
                            background-color: #00B78E;
                            color: white;
                            padding: 20px;
                            border-radius: 8px 8px 0 0;
                            text-align: center;
                            font-size: 24px;
                            font-weight: bold;
                        }}
                        .content {{
                            padding: 30px 20px;
                        }}
                        .credentials-box {{
                            background-color: #f8f9fa;
                            border: 2px solid #00B78E;
                            border-radius: 8px;
                            padding: 20px;
                            margin: 20px 0;
                        }}
                        .credential-item {{
                            margin: 10px 0;
                            padding: 10px;
                            background-color: white;
                            border-radius: 4px;
                            border-left: 4px solid #00B78E;
                        }}
                        .label {{
                            font-weight: bold;
                            color: #00B78E;
                        }}
                        .value {{
                            font-family: 'Courier New', monospace;
                            background-color: #e9ecef;
                            padding: 5px 10px;
                            border-radius: 4px;
                            margin-left: 10px;
                        }}
                        .footer {{
                            font-size: 12px;
                            color: #777;
                            padding-top: 20px;
                            text-align: center;
                            border-top: 1px solid #eee;
                        }}
                        .warning {{
                            background-color: #fff3cd;
                            border: 1px solid #ffeaa7;
                            border-radius: 4px;
                            padding: 15px;
                            margin: 20px 0;
                            color: #856404;
                        }}
                        .highlight {{
                            color: #00B78E;
                            font-weight: bold;
                        }}
                        .button {{
                            display: inline-block;
                            background-color: #00B78E;
                            color: white;
                            padding: 12px 24px;
                            text-decoration: none;
                            border-radius: 6px;
                            margin: 20px 0;
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            🚗 Welcome to CarRentalSystem!
                        </div>
                        <div class='content'>
                            <p>Dear <strong>{customerName}</strong>,</p>
                            
                            <p>Welcome to <span class='highlight'>CarRentalSystem</span>! Your customer account has been successfully created by our administrator.</p>
                            
                            <p>Below are your login credentials:</p>
                            
                            <div class='credentials-box'>
                                <div class='credential-item'>
                                    <span class='label'>Email:</span>
                                    <span class='value'>{email}</span>
                                </div>
                                <div class='credential-item'>
                                    <span class='label'>Password:</span>
                                    <span class='value'>{password}</span>
                                </div>
                            </div>
                            
                            <div class='warning'>
                                <strong>⚠️ Important Security Notice:</strong><br/>
                                For your security, please change your password after your first login. Keep your credentials safe and do not share them with anyone.
                            </div>
                            
                            <p>You can now:</p>
                            <ul>
                                <li>Browse and book cars from our extensive collection</li>
                                <li>Manage your bookings and reservations</li>
                                <li>Update your profile information</li>
                                <li>Contact our support team for any assistance</li>
                            </ul>
                            
                            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                            
                            <p>Thank you for choosing CarRentalSystem!</p>
                            
                            <p>Best regards,<br/>
                            <strong>The CarRentalSystem Team</strong></p>
                        </div>
                        <div class='footer'>
                            This is an automated message. Please do not reply to this email.<br/>
                            For support, contact us at our support email address.
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}

