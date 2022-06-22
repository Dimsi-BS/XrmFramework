using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Utils
{
    public class DebuggerAssemblyExporter : IAssemblyExporter
    {
        private readonly AssemblyExporter _mainExporter;
        private readonly IRegistrationService _registrationService;
        private readonly ISolutionContext _solutionContext;
        private readonly ICrmComponentConverter _converter;

        public DebuggerAssemblyExporter(IRegistrationService registrationService, ISolutionContext solutionContext, ICrmComponentConverter converter)
        {
            _registrationService = registrationService;
            _solutionContext = solutionContext;
            _converter = converter;
             _mainExporter = new AssemblyExporter(solutionContext, registrationService, converter);
       }

        public void UpdateAllComponents(IEnumerable<ICrmComponent> componentsToUpdate)
        {
            _mainExporter.UpdateAllComponents(componentsToUpdate);
        }

        /// <summary>
        /// Creates a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// <remarks>
        /// This method is in a partial file because it is implemented differently in the DeployUtils project
        /// </remarks>
        /// <param name="component"> The component to export</param>
        public void CreateComponent(ICrmComponent component)
        {
            component.Id = Guid.Empty;
            var registeringComponent = _converter.ToRegisterComponent(component);
            component.Id = _registrationService.Create(registeringComponent);
            registeringComponent.Id = component.Id;

            if (!component.DoAddToSolution) return;

            int? entityTypeCode = component.DoFetchTypeCode
                ? _registrationService.GetIntEntityTypeCode(component.EntityTypeName)
                : null;

            var addSolutionComponentRequest = _mainExporter.CreateAddSolutionComponentRequest(registeringComponent.ToEntityReference(), entityTypeCode);

            if (addSolutionComponentRequest != null)
            {
                _registrationService.Execute(addSolutionComponentRequest);
            }
        }

        /// <summary>
        /// Deletes a <see cref="ICrmComponent"/> on the Crm
        /// </summary>
        /// 
        /// This method is in a partial file because it is implemented differently in the RemoteDebugger.Client project
        public void DeleteComponent(ICrmComponent component)
        {
            _registrationService.Delete(component.EntityTypeName, component.Id);
        }

        public void UpdateComponent(ICrmComponent component)
        {
            _mainExporter.UpdateComponent(component);
        }

        public void InitExportMetadata(IEnumerable<Step> steps)
        {
            _mainExporter.InitExportMetadata(steps);
        }

        public void CreateAllComponents(IEnumerable<ICrmComponent> componentsToCreate)
        {
            _mainExporter.CreateAllComponents(componentsToCreate);
        }

        public IEnumerable<OrganizationRequest> ToDeleteRequestCollection(IEnumerable<ICrmComponent> componentsToDelete)
        {
            var sortedList = componentsToDelete.ToList();

            sortedList.Sort((x, y) => -x.Rank.CompareTo(y.Rank));

            return sortedList.Select(_mainExporter.ToDeleteRequest);
        }

        public IEnumerable<OrganizationRequest> ToUpdateRequestCollection(IEnumerable<ICrmComponent> componentsToUpdate)
        {
            return _mainExporter.ToUpdateRequestCollection(componentsToUpdate);
        }

        public void DeleteAllComponents(IEnumerable<ICrmComponent> componentsToDelete)
        {
            _mainExporter.DeleteAllComponents(componentsToDelete);
        }
    }
}