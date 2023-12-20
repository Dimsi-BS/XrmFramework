using System.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Converters;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Importers;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Configuration;

/// <summary>
///     Configures the necessary services and parameters of the project
/// </summary>
static class ServiceCollectionHelper
{
	/// <summary>
	///     Configures the required objects used during Deploy, such as :
	///     <list type="bullet">
	///         <item><see cref="IRegistrationService" />, the service used for communicating with the CRM</item>
	///         <item>
	///             <see cref="AutoMapper.IMapper" />, used for conversion between <see cref="Deploy" /> and
	///             <see cref="Model" /> objects
	///             as well as cloning
	///         </item>
	///         <item><see cref="DeploySettings" />, an object that contains information on the target <c>Solution</c></item>
	///         <item>The configuration of all other implemented interfaces</item>
	///     </list>
	/// </summary>
	/// <param name="projectName">Name of the target solution</param>
	/// <returns><see cref="IServiceProvider" /> the service provider used to instantiate every object needed</returns>
	public static IServiceProvider ConfigureForDeploy(string projectName)
	{
		var serviceCollection = InitServiceCollection();

		serviceCollection.AddScoped<IAssemblyExporter, AssemblyExporter>();

		var targetSolutionName = GetTargetSolutionName(projectName);
		var connectionString = GetSelectedConnectionString();

		serviceCollection.Configure<DeploySettings>((s) =>
		{
			s.ConnectionString = connectionString;
			s.PluginSolutionUniqueName = targetSolutionName;
		});

		serviceCollection.AddScoped<IOrganizationService, CrmServiceClient>(_ =>
			new CrmServiceClient(connectionString));

		return serviceCollection.BuildServiceProvider();
	}

	/// <summary>
	///     Configures the base <see cref="IServiceCollection" /> required for deploy,
	///     for more functionalities you can add them in the returned <see cref="IServiceCollection" />
	/// </summary>
	/// <returns>
	///     <see cref="IServiceCollection" />
	/// </returns>
	public static IServiceCollection InitServiceCollection()
	{
		var serviceCollection = new ServiceCollection();

		serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
		serviceCollection.AddScoped<ISolutionContext, SolutionContext>();
		serviceCollection.AddScoped<IAssemblyImporter, AssemblyImporter>();
		serviceCollection.AddScoped<ICrmComponentComparer, CrmComponentComparer>();
		serviceCollection.AddScoped<ICrmComponentConverter, CrmComponentConverter>();
		serviceCollection.AddScoped<AssemblyDiffFactory>();
		serviceCollection.AddSingleton<IAssemblyFactory, AssemblyFactory>();
		serviceCollection.AddSingleton<RegistrationHelper>();

		// Searches every AutoMapper Profiles declared in this Assembly and configures a mapper according to them
		serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
		return serviceCollection;
	}

	/// <summary>
	///     Retrieves the config files to get the target solution name as defined in xrmFramework.config
	/// </summary>
	/// <param name="projectName">, The name of the local project</param>
	/// <returns>The Name of the Target Solution</returns>
	public static string GetTargetSolutionName(string projectName)
	{
		var xrmFrameworkConfigSection = ConfigHelper.GetSection();

		var projectConfig = xrmFrameworkConfigSection.Projects.OfType<ProjectElement>()
			.FirstOrDefault(p => p.Name == projectName);

		if (projectConfig == null)
		{
			var defaultColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(
				$@"No reference to the project {projectName} has been found in the xrmFramework.config file.");
			Console.ForegroundColor = defaultColor;
			Environment.Exit(1);
		}

		return projectConfig.TargetSolution;
	}

	/// <summary>
	///     Retrieves the selected connection string as defined in xrmFramework.config
	/// </summary>
	/// <returns></returns>
	private static string GetSelectedConnectionString()
	{
		var xrmFrameworkConfigSection = ConfigHelper.GetSection();
		return ConfigurationManager.ConnectionStrings[xrmFrameworkConfigSection.SelectedConnection]
			.ConnectionString;
	}
}