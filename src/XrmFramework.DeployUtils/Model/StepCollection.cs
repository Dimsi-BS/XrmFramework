using System.Collections;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Model
{
    public class StepCollection : ICollection<Step>
    {
        private readonly StepComparer _stepComparer = new();

        // Do not make this field readonly as AutoMapper wouldn't be able to map it anymore
        // Same comment for the CustomApi arguments ^^
        private List<Step> _internalList = new List<Step>();

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

        public int Count => _internalList.Count;

        public bool IsReadOnly => ((ICollection<Step>)_internalList).IsReadOnly;
        #endregion

    }
}