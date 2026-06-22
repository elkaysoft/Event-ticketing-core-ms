namespace ETS.Domain.Common
{
    public class EmailTemplateConstant
    {
        public const string SetupPassword = @"<div style=""max-width: 600px;margin: 0 auto;padding: 20px;font-family: Arial, sans-serif;background-color: #fff;color: #333;""><div style=""margin-bottom: 20px;""><img src=""https://res.cloudinary.com/diitoyah1/image/upload/q_auto/f_auto/v1782022242/adexswag-logo_mkt46p.png"" style=""max-width: 150px;"" alt=""AdexSwagNation Logo"" /><p>Powered by AdexSwagNation</p></div><div style=""color:#333;margin-top: 50px;margin-bottom: 200px;""><h1 style=""font-size: 18px;font-weight: bold;"">Account Activation</h1><p style=""font-size: 14px;margin: 10px 0;"">Dear [ADMIN_NAME]</p><p style=""font-size: 14px;margin: 20px 0;"">Your admin account has been successfully created. Please click the link below to set your new password:</p><a href=""[CHANGE_PASSWORD_URL]"" style=""background:#0e156f;color:white;padding:10px 20px;text-align:center;text-decoration:none;border-radius:5px;display:inline-block;font-weight:bold;margin:20px 0;"">Set Your Password</a><p style=""font-size: 14px;margin: 10px 0;""><br /><br />AdexSwagNation Team</p></div></div>";
    }
}
