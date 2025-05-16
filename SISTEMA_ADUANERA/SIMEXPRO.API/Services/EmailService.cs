using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System;

namespace SIMEXPRO.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> EnviarCorreoAsync(string destino, string codigo)
        {
            try
            {
                var smtp = new SmtpClient(_config["EmailSettings:SmtpServer"])
                {
                    Port = int.Parse(_config["EmailSettings:Port"]),
                    Credentials = new NetworkCredential(
                        _config["EmailSettings:SenderEmail"],
                        _config["EmailSettings:SenderPassword"]
                    ),
                    EnableSsl = true
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_config["EmailSettings:SenderEmail"]),
                    Subject = "Tu código de verificación",
                    Body = $@"
                        <html>
                        <head>
                            <style>
                                .container {{
                                    font-family: Arial, sans-serif;
                                    background-color: #f9f9f9;
                                    padding: 30px;
                                    border-radius: 8px;
                                    max-width: 600px;
                                    margin: auto;
                                    box-shadow: 0 0 10px rgba(0,0,0,0.1);
                                }}
                                .header {{
                                    background-color: #002f6c;
                                    padding: 20px;
                                    text-align: center;
                                    color: white;
                                    border-radius: 8px 8px 0 0;
                                }}
                                .content {{
                                    padding: 20px;
                                    background-color: white;
                                    border-radius: 0 0 8px 8px;
                                    text-align: center;
                                }}
                                .code {{
                                    font-size: 24px;
                                    font-weight: bold;
                                    color: #002f6c;
                                    margin-top: 20px;
                                    letter-spacing: 2px;
                                }}
                                .footer {{
                                    margin-top: 30px;
                                    font-size: 12px;
                                    color: #888;
                                    text-align: center;
                                }}
                                .img{{
                                    width: 200px;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <img class='img' src='https://firebasestorage.googleapis.com/v0/b/fir-upload-pdf-d2c3f.firebasestorage.app/o/logos%2FLOGO.png?alt=media&token=714aa0c5-5772-4f29-a50f-eb9583964ed0' />
                                    <h2>Servicios Aduaneros</h2>
                                </div>
                                <div class='content'>
                                    <p>Estimado/a usuario:</p>
                                    <p>Gracias por utilizar nuestros servicios. Aquí tienes tu código de verificación:</p>
                                    <div class='code'>{codigo}</div>
                                    <p>Por favor, no compartas este código con nadie.</p>
                                    <div class='footer'>
                                        &copy; {DateTime.Now.Year} Frontier Logistic. Todos los derechos reservados.
                                    </div>
                                </div>
                            </div>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };

                mensaje.To.Add(destino);

                await smtp.SendMailAsync(mensaje);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
