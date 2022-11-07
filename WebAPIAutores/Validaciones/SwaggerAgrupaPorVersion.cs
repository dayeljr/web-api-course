﻿using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebAPIAutores.Validaciones
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace;
            var versionAPI = namespaceControlador.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = versionAPI;
        }
    }
}
