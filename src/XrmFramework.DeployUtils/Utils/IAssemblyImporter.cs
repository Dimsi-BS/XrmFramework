using System;
using System.Collections.Generic;
using System.Reflection;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    /// <summary>
    /// Imports Components from <see cref="System"/> or <see cref="Deploy"/> to <see cref="ICrmComponent"/>
    /// </summary>
    interface IAssemblyImporter
    {
        /// <summary>
        /// Creates a <see cref="IAssemblyContext"/> with only the <see cref="AssemblyInfo"/> Property computed from an <see cref="Assembly"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns><see cref="IAssemblyContext"/></returns>
        IAssemblyContext CreateAssemblyFromLocal(Assembly assembly);

        /// <summary>
        /// Creates a <see cref="IAssemblyContext"/> with only the <see cref="AssemblyInfo"/> Property computed from a <see cref="Deploy.PluginAssembly"/>
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns><see cref="IAssemblyContext"/></returns>
        IAssemblyContext CreateAssemblyFromRemote(Deploy.PluginAssembly assembly);

        /// <summary>
        /// Creates a <see cref="Plugin"/> from a local Assembly <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Plugin CreatePluginFromType(Type type);

        /// <summary>
        /// Creates a <see cref="Plugin"/> as a CustomWorkflow from a local Assembly <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Plugin CreateWorkflowFromType(Type type);

        /// <summary>
        /// Creates a <see cref="CustomApi"/> from a local Assembly <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        CustomApi CreateCustomApiFromType(Type type);

        /// <summary>
        /// Creates a <see cref="Step"/> from a remote Assembly <see cref="Deploy.SdkMessageProcessingStep"/>
        /// and the <see cref="Deploy.SdkMessageProcessingStepImage"/> registered on the remote Assembly
        /// </summary>
        /// <param name="sdkStep"></param>
        /// <param name="sdkImages"></param>
        /// <returns></returns>
        Step CreateStepFromRemote(Deploy.SdkMessageProcessingStep sdkStep, IEnumerable<Deploy.SdkMessageProcessingStepImage> sdkImages);

        /// <summary>
        /// Creates a <see cref="Plugin"/> from a remote Assembly <see cref="Deploy.PluginType"/>
        /// and the <see cref="Step"/> already parsed
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="steps"></param>
        /// <returns></returns>
        Plugin CreatePluginFromRemote(Deploy.PluginType pluginType, IEnumerable<Step> steps);

        /// <summary>
        /// Creates a <see cref="CustomApi"/> from a remote Assembly <see cref="Deploy.CustomApi"/>,
        /// the <see cref="Deploy.CustomApiRequestParameter"/>
        /// and the <see cref="Deploy.CustomApiResponseProperty"/> registered on the remote Assembly
        /// </summary>
        /// <param name="customApi"></param>
        /// <param name="requestParameters"></param>
        /// <param name="responseProperties"></param>
        /// <returns></returns>
        CustomApi CreateCustomApiFromRemote(Deploy.CustomApi customApi,
                                            IEnumerable<Deploy.CustomApiRequestParameter> requestParameters,
                                            IEnumerable<Deploy.CustomApiResponseProperty> responseProperties);
    }
}