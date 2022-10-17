using System;
using System.Collections.Generic;
using System.Reflection;
using Deploy;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using CustomApi = XrmFramework.DeployUtils.Model.CustomApi;
using CustomApiRequestParameter = Deploy.CustomApiRequestParameter;
using CustomApiResponseProperty = Deploy.CustomApiResponseProperty;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Utils;

/// <summary>
///     Imports Components from <see cref="System" /> or <see cref="Deploy" /> to <see cref="ICrmComponent" />
/// </summary>
internal interface IAssemblyImporter
{
	/// <summary>
	///     Creates a <see cref="IAssemblyContext" /> with only the <see cref="AssemblyInfo" /> Property computed from an
	///     <see cref="Assembly" />
	/// </summary>
	/// <param name="assembly"></param>
	/// <returns>
	///     <see cref="IAssemblyContext" />
	/// </returns>
	AssemblyInfo CreateAssemblyFromLocal(Assembly assembly);

	/// <summary>
	///     Creates a <see cref="IAssemblyContext" /> with only the <see cref="AssemblyInfo" /> Property computed from a
	///     <see cref="Deploy.PluginAssembly" />
	/// </summary>
	/// <param name="assembly"></param>
	/// <returns>
	///     <see cref="IAssemblyContext" />
	/// </returns>
	IAssemblyContext CreateAssemblyFromRemote(PluginAssembly assembly);

	/// <summary>
	///     Creates a <see cref="IAssemblyContext" /> with only the <see cref="AssemblyInfo" /> Property computed from a
	///     <see cref="Deploy.PluginAssembly" />
	/// </summary>
	/// <param name="assembly"></param>
	/// <returns>
	///     <see cref="IAssemblyContext" />
	/// </returns>
	IAssemblyContext CreateAssemblyFromRemote(AssemblyInfo assemblyInfo);

	/// <summary>
	///     Creates a <see cref="PluginPackage" /> from a local Assembly
	/// </summary>
	/// <param name="assembly"></param>
	/// <returns></returns>
	PluginPackage CreatePackageFromLocal(AssemblyInfo assembly);

	/// <summary>
	///     Creates a <see cref="PluginPackage" /> from the remote
	/// </summary>
	/// <param name="package"></param>
	/// <returns></returns>
	PluginPackage CreatePackageFromRemote(Deploy.PluginPackage package);

	/// <summary>
	///     Creates a <see cref="Plugin" /> from a local Assembly <see cref="Type" />
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	Plugin CreatePluginFromType(Type type);

	/// <summary>
	///     Creates a <see cref="Plugin" /> as a CustomWorkflow from a local Assembly <see cref="Type" />
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	Plugin CreateWorkflowFromType(Type type);

	/// <summary>
	///     Creates a <see cref="CustomApi" /> from a local Assembly <see cref="Type" />
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	CustomApi CreateCustomApiFromType(Type type);

	/// <summary>
	///     Creates a <see cref="Step" /> from a remote Assembly <see cref="Deploy.SdkMessageProcessingStep" />
	///     and the <see cref="Deploy.SdkMessageProcessingStepImage" /> registered on the remote Assembly
	/// </summary>
	/// <param name="sdkStep"></param>
	/// <param name="sdkImages"></param>
	/// <returns></returns>
	Step CreateStepFromRemote(SdkMessageProcessingStep sdkStep, IEnumerable<SdkMessageProcessingStepImage> sdkImages);

	/// <summary>
	///     Creates a <see cref="Plugin" /> from a remote Assembly <see cref="Deploy.PluginType" />
	///     and the <see cref="Step" /> already parsed
	/// </summary>
	/// <param name="pluginType"></param>
	/// <param name="steps"></param>
	/// <returns></returns>
	Plugin CreatePluginFromRemote(PluginType pluginType, IEnumerable<Step> steps);

	/// <summary>
	///     Creates a <see cref="CustomApi" /> from a remote Assembly <see cref="Deploy.CustomApi" />,
	///     the <see cref="Deploy.CustomApiRequestParameter" />
	///     and the <see cref="Deploy.CustomApiResponseProperty" /> registered on the remote Assembly
	/// </summary>
	/// <param name="customApi"></param>
	/// <param name="requestParameters"></param>
	/// <param name="responseProperties"></param>
	/// <returns></returns>
	CustomApi CreateCustomApiFromRemote(Deploy.CustomApi customApi,
		IEnumerable<CustomApiRequestParameter> requestParameters,
		IEnumerable<CustomApiResponseProperty> responseProperties);
}