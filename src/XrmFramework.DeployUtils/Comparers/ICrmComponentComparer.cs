using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils;

/// <summary>Compares ICrmComponents to determine equality or update status</summary>
public interface ICrmComponentComparer : IEqualityComparer<ICrmComponent>
{
    /// <summary>Finds the first occurrence of <paramref name="component" /> in the <paramref name="target" /> Collection</summary>
    /// <param name="component"></param>
    /// <param name="target"></param>
    /// <returns>The first occurrence of <paramref name="component" />, null if not found</returns>
    ICrmComponent CorrespondingComponent(ICrmComponent component, IReadOnlyCollection<ICrmComponent> target);

    /// <summary>
    ///     Checks if two components have different non-defining properties
    /// </summary>
    /// <returns>true if they need updating, false if they are exactly the same</returns>
    /// <exception cref="ArgumentException">If the two components were not of the same type or of unknown type</exception>
    bool NeedsUpdate(ICrmComponent x, ICrmComponent y);
}