using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();

            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private async Task<bool> EsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var resultado = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");

            return resultado.Succeeded;
        }

        public async Task GenerarEnlaces(AutorDTO autorDto)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirURLHelper();

            autorDto.Enlaces.Add(new DatoHETEOAS(
                enlace: Url.Link("obtenerAutor", new { id = autorDto.Id }),
                descripcion: "autor-obtener",
                metodo: "GET"));

            if (esAdmin)
            {
                autorDto.Enlaces.Add(new DatoHETEOAS(
                enlace: Url.Link("actualizarAutor", new { id = autorDto.Id }),
                descripcion: "autor-actualizar",
                metodo: "PUT"));

                autorDto.Enlaces.Add(new DatoHETEOAS(
                    enlace: Url.Link("removerAutor", new { id = autorDto.Id }),
                    descripcion: "autor-remover",
                    metodo: "DELETE"));
            }
        }
    }
}
