using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.DeployUtils.Model
{
    public class StepCollection : ICollection<Step>
    {
        private readonly StepComparer _stepComparer = new StepComparer();

        private readonly List<Step> _internalList = new List<Step>();

        public void Add(Step step)
        {
            var existingStep = _internalList.FirstOrDefault(s => _stepComparer.Equals(s, step));

            if (existingStep != null)
            {
                existingStep.Merge(step);
            }
            else
            {
                _internalList.Add(step);
            }
        }


        #region ICollection Members
        public IEnumerator<Step> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_internalList).GetEnumerator();
        }

        public void Clear()
        {
            _internalList.Clear();
        }

        public bool Contains(Step item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(Step[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(Step item)
        {
            return _internalList.Remove(item);
        }

        //public bool Any(Func<Step, bool> predicate)
        //{ 
        //    return _internalList.Any(predicate);
        //}

        public int Count => _internalList.Count;

        public bool IsReadOnly => ((ICollection<Step>)_internalList).IsReadOnly;
        #endregion

        

        public class StepComparer : IEqualityComparer<Step>
        {
            public bool Equals(Step x, Step y) =>
                x == null && y == null 
                ||  
                x?.PluginTypeFullName == y?.PluginTypeFullName
                && x?.EntityName == y?.EntityName
                && x?.Message == y?.Message 
                && x?.Stage == y?.Stage 
                && x.Mode == y.Mode;

            public int GetHashCode(Step obj)
                => obj.PluginTypeName.GetHashCode() 
                   + obj.EntityName.GetHashCode() 
                   + obj.Message.GetHashCode() 
                   + obj.Stage.GetHashCode() 
                   + obj.Mode.GetHashCode();

            public bool NeedsUpdate(Step x, Step y) =>
                   x?.DoNotFilterAttributes != y?.DoNotFilterAttributes
                || x.FilteringAttributes.Any() != x.FilteringAttributes.Any()
                || string.Join(",", x.FilteringAttributes) != string.Join(",", y.FilteringAttributes)
                || x?.UnsecureConfig != y?.UnsecureConfig
                || x.ImpersonationUsername != y.ImpersonationUsername;
        }
    }
}