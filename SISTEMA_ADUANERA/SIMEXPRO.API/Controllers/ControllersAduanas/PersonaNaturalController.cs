using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SIMEXPRO.API.Models;
using SIMEXPRO.API.Models.ModelsAduana;
using SIMEXPRO.BussinessLogic.Services.EventoServices;
using SIMEXPRO.Entities.Entities;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Insert([FromForm] PersonaNaturalViewModel personaNaturalViewModel)
        {
            if (personaNaturalViewModel.ArchivoRTN == null || personaNaturalViewModel.ArchivoRTN.Length == 0)
                return BadRequest("Archivo no válido.");

            // Subir archivo a Google Cloud Storage
            using var stream = personaNaturalViewModel.ArchivoRTN.OpenReadStream();
            var storageService = new GoogleCloudStorageService();
            var fileUrl = await storageService.SubirArchivoAsync(stream, personaNaturalViewModel.ArchivoRTN.FileName, personaNaturalViewModel.ArchivoRTN.ContentType);

            // Guardar el URL del archivo en la base de datos
            var item = _mapper.Map<tbPersonaNatural>(personaNaturalViewModel);
            item.pena_ArchivoRTN = fileUrl; // Se guarda la URL en el modelo
            var respuesta = _aduanaServices.InsertarPersonaNatural(item);

            return Ok(respuesta);
        }


        [HttpPost("Editar")]
        public IActionResult Update(PersonaNaturalViewModel personaNaturalViewModel)
        {
            var item = _mapper.Map<tbPersonaNatural>(personaNaturalViewModel);
            var respuesta = _aduanaServices.ActualizarPersonaNatural(item);
            return Ok(respuesta);
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
