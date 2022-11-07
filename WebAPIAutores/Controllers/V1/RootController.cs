using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHETEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHETEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateoas.Add(new DatoHETEOAS(enlace: Url.Link("ObtenerRoot", new { }),
                descripcion: "self", metodo: "GET"));

            datosHateoas.Add(new DatoHETEOAS(enlace: Url.Link("obtenerAutores", new { }),
                descripcion: "autores", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHETEOAS(enlace: Url.Link("crearAutor", new { }),
                    descripcion: "autor-crear", metodo: "POST"));

                datosHateoas.Add(new DatoHETEOAS(enlace: Url.Link("crearLibro", new { }),
                    descripcion: "libro-crear", metodo: "POST"));
            }

            return datosHateoas;
        }
    }
}
