using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRentalSystem.Application.Helpers.EmailTemplates
{
    public static class ContactEmailTemplateHelper
    {
        public static string GenerateHtml(string senderName, string senderEmail, string message)
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
                            padding: 10px 20px;
                            border-radius: 8px 8px 0 0;
                            font-size: 18px;
                            font-weight: bold;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .footer {{
                            font-size: 12px;
                            color: #777;
                            padding-top: 20px;
                            text-align: center;
                        }}
                        .highlight {{
                            color: #00B78E;
                            font-weight: bold;
                        }}
                        blockquote {{
                            border-left: 3px solid #00B78E;
                            padding-left: 15px;
                            color: #444;
                            margin: 10px 0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            🚗 New Contact Inquiry – CarRentalSystem
                        </div>
                        <div class='content'>
                            <p>Hello <strong>Support Team</strong>,</p>
                            <p>You have received a new message from the <strong>Contact Us</strong> form.</p>

                            <p><strong>Name:</strong> <span class='highlight'>{senderName}</span><br />
                               <strong>Email:</strong> <a href='mailto:{senderEmail}'>{senderEmail}</a></p>

                            <p><strong>Message:</strong></p>
                            <blockquote>
                                {message}
                            </blockquote>

                            <p>Please respond to the customer promptly to maintain our support standards.</p>

                            <p>– CarRentalSystem Notification Bot</p>
                        </div>
                        <div class='footer'>
                            This is an automated message. Do not reply directly.
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}

