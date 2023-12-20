using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Comparers;

/// <summary>
///     Base implementation of <see cref="ICrmComponentComparer" />
/// </summary>
public class CrmComponentComparer : ICrmComponentComparer
{
	private readonly StepComparer _stepComparer = new();

	public ICrmComponent CorrespondingComponent(ICrmComponent component, IReadOnlyCollection<ICrmComponent> target)
	{
		return target.FirstOrDefault(x => Equals(component, x));
	}

	public bool Equals(ICrmComponent x, ICrmComponent y)
	{
		if (x == null || y == null || x.GetType() != y.GetType()) return false;

		return x switch
		{
			Step step => _stepComparer.Equals(step, (Step) y),
			StepImage image => _stepComparer.Equals(image.FatherStep, ((StepImage) y).FatherStep) &&
			                   image.IsPreImage == ((StepImage) y).IsPreImage,
			ICustomApiComponent apiComponent => apiComponent.UniqueName == y.UniqueName
			                                    && apiComponent.Type.Equals(((ICustomApiComponent) y).Type),
			_ => x.UniqueName == y.UniqueName
		};
	}

	public bool NeedsUpdate(ICrmComponent x, ICrmComponent y)
	{
		if (!Equals(x, y)) return false;

		return x switch
		{
			Step step => _stepComparer.NeedsUpdate(step, (Step) y),
			StepImage image => image.JoinedAttributes != ((StepImage) y).JoinedAttributes ||
			                   image.AllAttributes ^ ((StepImage) y).AllAttributes,
			CustomApiRequestParameter request => NeedsUpdate(request, (CustomApiRequestParameter) y),
			CustomApiResponseProperty => false,
			Plugin => false,
			CustomApi api => NeedsUpdate(api, (CustomApi) y),
			AssemblyInfo => true,
			PluginPackage => true,
			_ => throw new ArgumentException("SolutionComponent not recognized")
		};
	}

	public int GetHashCode(ICrmComponent obj)
	{
		throw new NotImplementedException();
	}

    /// <summary>
    ///     NeedsUpdate implementation for two <see cref="CustomApi" />
    /// </summary>
    /// <returns>true if they need updating, false if they are exactly the same</returns>
    private static bool NeedsUpdate(CustomApi x, CustomApi y)
	{
		return !x.BindingType.Equals(y.BindingType)
		       || !string.Equals(x.BoundEntityLogicalName, y.BoundEntityLogicalName)
		       || x.IsFunction ^ y.IsFunction
		       || x.WorkflowSdkStepEnabled ^ y.WorkflowSdkStepEnabled
		       || !x.AllowedCustomProcessingStepType.Equals(y.AllowedCustomProcessingStepType);
	}

    /// <summary>
    ///     NeedsUpdate implementation for two <see cref="CustomApiRequestParameter" />
    /// </summary>
    /// <returns>true if they need updating, false if they are exactly the same</returns>
    private static bool NeedsUpdate(CustomApiRequestParameter x, CustomApiRequestParameter y)
	{
		return !x.IsOptional == y.IsOptional;
	}
}