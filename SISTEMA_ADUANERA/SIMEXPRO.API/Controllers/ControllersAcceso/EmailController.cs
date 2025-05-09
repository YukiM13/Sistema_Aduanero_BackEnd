using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SIMEXPRO.API.Models.ModelsAcceso;
using SIMEXPRO.API.Services;

namespace SIMEXPRO.API.Controllers.ControllersAcceso
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("EnviarCodigo")]
        public async Task<IActionResult> EnviarCodigo([FromBody] EmailRequest request)
        {
            if (string.IsNullOrEmpty(request.CorreoDestino) || string.IsNullOrEmpty(request.Codigo))
                return BadRequest("Correo y código son requeridos.");

            var resultado = await _emailService.EnviarCorreoAsync(request.CorreoDestino, request.Codigo);

            if (resultado)
                return Ok("Correo enviado correctamente.");
            else
                return StatusCode(500, "Error al enviar el correo.");
        }
    }
}
