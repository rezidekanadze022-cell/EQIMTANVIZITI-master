using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using EQIMTANVIZITI.Models;

namespace EQIMTANVIZITI.DependencyInjection.SMTP;

public class SMTPService
{
    public static string SenderEmail = "rezidekanadze022@gmail.com";
    public static string AppPassword = "adnf aohu pkrr yask";
    public static string SenderName = "EQIMTANVIZITI";
    public static string SmtpHost = "smtp.gmail.com";
    public static int SmtpPort = 587;

    public static string GenerateCode()
        => RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

    public static async Task<string> SendEmail(User user)
    {
        string code = GenerateCode();
        user.Code = code;
        user.CodeExpiresAt = DateTime.UtcNow.AddMinutes(15);

        string displayName = !string.IsNullOrWhiteSpace(user.FullName)
            ? user.FullName
            : (!string.IsNullOrWhiteSpace(user.FirstName) ? $"{user.FirstName} {user.LastName}".Trim() : user.Email);

        string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin:0; padding:0; background-color:#f3f4f6;
             font-family:Arial,Helvetica,sans-serif;'>

    <div style='max-width:600px; margin:40px auto; padding:20px;'>

        <div style='background-color:#ffffff; border-radius:16px;
                    overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08);'>

            <div style='background:linear-gradient(135deg,#4f46e5,#7c3aed);
                        padding:35px 20px; text-align:center; color:#ffffff;'>
                <h1 style='margin:0; font-size:28px;'>
                    Email Verification
                </h1>
                <p style='margin:12px 0 0; font-size:15px;'>
                    One more step to get started!
                </p>
            </div>

            <div style='padding:35px 30px; color:#374151;'>

                <h2 style='font-size:22px; color:#111827; margin-top:0;'>
                    Hello, {WebUtility.HtmlEncode(displayName)}!
                </h2>

                <p style='font-size:16px; line-height:1.7;'>
                    Thank you for joining us. To verify your email address,
                    please use the verification code below.
                </p>

                <div style='background-color:#f5f3ff;
                            border:2px dashed #8b5cf6;
                            border-radius:12px;
                            padding:24px 15px;
                            margin:28px 0;
                            text-align:center;'>

                    <p style='margin:0 0 12px; color:#6b7280;
                              font-size:13px; text-transform:uppercase;
                              letter-spacing:2px;'>
                        Your Verification Code
                    </p>

                    <div style='font-size:36px; font-weight:bold;
                                color:#5b21b6; letter-spacing:8px;'>
                        {code}
                    </div>
                </div>

                <p style='font-size:14px; line-height:1.7; color:#6b7280;'>
                    Enter this code on the verification page to confirm
                    your email address. Please don't share this code with
                    anyone.
                </p>

                <p style='font-size:14px; line-height:1.7; color:#6b7280;'>
                    If you didn't request this verification, you can
                    safely ignore this email.
                </p>

                <hr style='border:none; border-top:1px solid #e5e7eb;
                           margin:30px 0;'>

                <p style='text-align:center; color:#9ca3af;
                          font-size:12px; margin-bottom:0;'>
                    &copy; {DateTime.UtcNow.Year} EQIMTANVIZITI.
                    All rights reserved.
                </p>

            </div>
        </div>
    </div>

</body>
</html>";

        try
        {
            using var mail = new MailMessage();
            mail.From = new MailAddress(SenderEmail, SenderName);
            mail.To.Add(user.Email);
            mail.Subject = "Verification Code";
            mail.IsBodyHtml = true;
            mail.Body = htmlContent;

            using var smtpClient = new SmtpClient(SmtpHost)
            {
                Port = SmtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(SenderEmail, AppPassword)
            };

            await smtpClient.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SMTP WARNING] Failed to send email to {user.Email}: {ex.Message}");
            Console.WriteLine($"[DEV VERIFICATION CODE] {user.Email}: {code}");
        }

        return code;
    }
}
