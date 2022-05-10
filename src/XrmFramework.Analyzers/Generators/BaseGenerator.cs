using BoDi;
using Microsoft.CodeAnalysis;
using XrmFramework.Analyzers.IO;

namespace XrmFramework.Analyzers.Generators
{
	public abstract class BaseGenerator<TValueProvider> : IIncrementalGenerator
	{
		/// <inheritdoc />
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			var mainContainer = GetObjectContainer();


		}

		protected virtual IncrementalValueProvider<TValueProvider> InitValueProvider(
			IncrementalGeneratorInitializationContext context)
		{
			throw new NotImplementedException();
		}

		protected virtual IncrementalValuesProvider<TValueProvider> InitValuesProvider(
			IncrementalGeneratorInitializationContext context)
		{
			throw new NotImplementedException();
		}

		private IObjectContainer GetObjectContainer()
		{
			var container = new ObjectContainer();

			container.RegisterTypeAs<IModelLoader>(typeof(AdditionalFilesLoader));
			container.RegisterTypeAs<ITableLoader>(typeof(AdditionalFilesLoader));

			return container;
		}
	}
}
