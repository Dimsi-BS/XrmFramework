using System;
using System.ServiceModel;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Commanding.Abstractions.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace $safeprojectname$.Extensions
{
    internal class XrmFrameworkHttpResponseHandler : IHttpResponseHandler
    {
        private readonly ILogger<XrmFrameworkHttpResponseHandler> _logger;

        public XrmFrameworkHttpResponseHandler(ILogger<XrmFrameworkHttpResponseHandler> logger)
        {
            _logger = logger;
        }

        public Task<IActionResult> CreateResponseFromException<TCommand>(TCommand command, Exception ex)
        {
            var exTemp = ex;

            while (exTemp.InnerException != null)
            {
                exTemp = exTemp.InnerException;
            }

            if (exTemp is FaultException<OrganizationServiceFault> fault)
            {
                _logger.LogError($"Erreur 400 : { fault.Detail }");

                return Task.FromResult((IActionResult)new BadRequestObjectResult(fault.Detail));
            }

            _logger.LogError($"Exception : {ex.Message} ({ex.GetType().FullName}) {ex.StackTrace}");

            _logger.LogError($"Inner Exception : {exTemp.Message} ({exTemp.GetType().FullName}) {exTemp.StackTrace}");


            return null;
        }

        public Task<IActionResult> CreateResponse<TCommand, TResult>(TCommand command, TResult result)
            => null;

        public Task<IActionResult> CreateResponse<TCommand>(TCommand command)
            => null;

        public Task<IActionResult> CreateValidationFailureResponse<TCommand>(TCommand command, ValidationResult validationResult)
        {
            var actionJson = JsonConvert.SerializeObject(command);

            _logger.LogInformation($"Payload en erreur : {actionJson}");

            return null;
        }
    }
}
