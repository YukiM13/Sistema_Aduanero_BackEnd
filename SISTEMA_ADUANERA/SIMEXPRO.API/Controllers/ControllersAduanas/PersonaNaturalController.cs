using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SIMEXPRO.API.Models;
using SIMEXPRO.API.Models.ModelsAduana;
using SIMEXPRO.BussinessLogic.Services.EventoServices;
using SIMEXPRO.DataAccess;
using SIMEXPRO.Entities.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIMEXPRO.API.Controllers.ControllersAduanas
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaNaturalController : Controller
    {
        private readonly AduanaServices _aduanaServices;
        private readonly IMapper _mapper;

        public PersonaNaturalController(AduanaServices AduanaServices, IMapper mapper)
        {
            _aduanaServices = AduanaServices;
            _mapper = mapper;
        }

        [HttpGet("Listar")]
        public IActionResult Index()
        {
            var listado = _aduanaServices.ListarPersonaNatural();
            listado.Data = _mapper.Map<IEnumerable<PersonaNaturalViewModel>>(listado.Data);
            return Ok(listado);
        }


       [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromForm] PersonaNaturalViewModel model)
        {
            try
            {
                var storageService = new GoogleCloudStorageService();

                // Procesar ArchivoRTN
                if (model.ArchivoRTN == null || model.ArchivoRTN.Length == 0)
                {
                    return BadRequest("Por favor, sube un archivo válido para el RTN.");
                }
                var tempFilePathRTN = Path.GetTempFileName();
                using (var stream = new FileStream(tempFilePathRTN, FileMode.Create))
                {
                    await model.ArchivoRTN.CopyToAsync(stream);
                }
                var objectNameRTN = $"{Guid.NewGuid()}_{model.ArchivoRTN.FileName}";
                var fileUrlRTN = await storageService.SubirArchivoAsync(tempFilePathRTN, objectNameRTN);
                model.pena_ArchivoRTN = fileUrlRTN;

                // Procesar ArchivoDNI
                if (model.ArchivoDNI != null && model.ArchivoDNI.Length > 0)
                {
                    var tempFilePathDNI = Path.GetTempFileName();
                    using (var stream = new FileStream(tempFilePathDNI, FileMode.Create))
                    {
                        await model.ArchivoDNI.CopyToAsync(stream);
                    }
                    var objectNameDNI = $"{Guid.NewGuid()}_{model.ArchivoDNI.FileName}";
                    var fileUrlDNI = await storageService.SubirArchivoAsync(tempFilePathDNI, objectNameDNI);
                    model.pena_ArchivoDNI = fileUrlDNI;
                }

                // Procesar ArchivoNumeroRecibo
                if (model.ArchivoNumeroRecibo != null && model.ArchivoNumeroRecibo.Length > 0)
                {
                    var tempFilePathRecibo = Path.GetTempFileName();
                    using (var stream = new FileStream(tempFilePathRecibo, FileMode.Create))
                    {
                        await model.ArchivoNumeroRecibo.CopyToAsync(stream);
                    }
                    var objectNameRecibo = $"{Guid.NewGuid()}_{model.ArchivoNumeroRecibo.FileName}";
                    var fileUrlRecibo = await storageService.SubirArchivoAsync(tempFilePathRecibo, objectNameRecibo);
                    model.pena_ArchivoNumeroRecibo = fileUrlRecibo;
                }

                // Mapear el ViewModel a la entidad
                var persona = new tbPersonaNatural
                {
                    pena_DireccionExacta = model.pena_DireccionExacta,
                    pers_Id = model.pers_Id,
                    ciud_Id = model.ciud_Id,
                    pena_TelefonoFijo = model.pena_TelefonoFijo,
                    pena_TelefonoCelular = model.pena_TelefonoCelular,
                    pena_CorreoElectronico = model.pena_CorreoElectronico,
                    pena_CorreoAlternativo = model.pena_CorreoAlternativo,
                    pena_RTN = model.pena_RTN,
                    pena_ArchivoRTN = model.pena_ArchivoRTN,
                    pena_DNI = model.pena_DNI,
                    pena_ArchivoDNI = model.pena_ArchivoDNI,
                    pena_NumeroRecibo = model.pena_NumeroRecibo,
                    pena_ArchivoNumeroRecibo = model.pena_ArchivoNumeroRecibo,
                    pena_NombreArchRTN = model.pena_NombreArchRTN,
                    pena_NombreArchRecibo = model.pena_NombreArchRecibo,
                    pena_NombreArchDNI = model.pena_NombreArchDNI,
                    usua_UsuarioCreacion = model.usua_UsuarioCreacion,
                    pena_FechaCreacion = model.pena_FechaCreacion
                };

                // Insertar la persona en la base de datos
                var result = _aduanaServices.InsertarPersonaNatural(persona);

                if (result.Success)
                {
                    var requestStatus = (RequestStatus)result.Data;

                    if (requestStatus.Data == null)
                    {
                        return StatusCode(500, "Error: El ID de la persona no fue generado correctamente.");
                    }

                    int personaId = (int)requestStatus.Data;

                    return Ok(new
                    {
                        Message = "Persona insertada exitosamente.",
                        PersonaId = personaId,
                        DocumentUrls = new
                        {
                            Message = "Persona insertada exitosamente.",
                        }
                    });
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }



        [HttpPost("Editar")]
        public async Task<IActionResult> Update([FromForm] PersonaNaturalViewModel model)
        {
            try
            {
                var storageService = new GoogleCloudStorageService();

                // Procesar ArchivoRTN si se envía
                if (model.ArchivoRTN != null && model.ArchivoRTN.Length > 0)
                {
                    var tempFilePathRTN = Path.GetTempFileName();
                    using (var stream = new FileStream(tempFilePathRTN, FileMode.Create))
                    {
                        await model.ArchivoRTN.CopyToAsync(stream);
                    }
                    var objectNameRTN = $"{Guid.NewGuid()}_{model.ArchivoRTN.FileName}";
                    var fileUrlRTN = await storageService.SubirArchivoAsync(tempFilePathRTN, objectNameRTN);
                    model.pena_ArchivoRTN = fileUrlRTN;
                }

                // Procesar ArchivoDNI si se envía
                if (model.ArchivoDNI != null && model.ArchivoDNI.Length > 0)
                {
                    var tempFilePathDNI = Path.GetTempFileName();
                    using (var stream = new FileStream(tempFilePathDNI, FileMode.Create))
                    {
                        await model.ArchivoDNI.CopyToAsync(stream);
                    }
                    var objectNameDNI = $"{Guid.NewGuid()}_{model.ArchivoDNI.FileName}";
                    var fileUrlDNI = await storageService.SubirArchivoAsync(tempFilePathDNI, objectNameDNI);
                    model.pena_ArchivoDNI = fileUrlDNI;
                }

                // Procesar ArchivoNumeroRecibo si se envía
                if (model.ArchivoNumeroRecibo != null && model.ArchivoNumeroRecibo.Length > 0)
                {
                    var tempFilePathRecibo = Path.GetTempFileName();
                    using (var stream = new FileStream(tempFilePathRecibo, FileMode.Create))
                    {
                        await model.ArchivoNumeroRecibo.CopyToAsync(stream);
                    }
                    var objectNameRecibo = $"{Guid.NewGuid()}_{model.ArchivoNumeroRecibo.FileName}";
                    var fileUrlRecibo = await storageService.SubirArchivoAsync(tempFilePathRecibo, objectNameRecibo);
                    model.pena_ArchivoNumeroRecibo = fileUrlRecibo;
                }

                var item = _mapper.Map<tbPersonaNatural>(model);
                var respuesta = _aduanaServices.ActualizarPersonaNatural(item);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpPost("Eliminar")]
        public IActionResult Delete(PersonaNaturalViewModel personaNaturalViewModel)
        {
            var item = _mapper.Map<tbPersonaNatural>(personaNaturalViewModel);
            var respuesta = _aduanaServices.EliminarPersonaNatural(item);
            return Ok(respuesta);
        }
        
        [HttpPost("Finalizar")]
        public IActionResult Finalizar(PersonaNaturalViewModel personaNaturalViewModel)
        {
            var item = _mapper.Map<tbPersonaNatural>(personaNaturalViewModel);
            var respuesta = _aduanaServices.FinalizarPersonaNatural(item);
            return Ok(respuesta);
        }
    }
}
