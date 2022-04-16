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

        public int Count => _internalList.Count;

        public bool IsReadOnly => ((ICollection<Step>)_internalList).IsReadOnly;
        #endregion



        public class StepComparer : IEqualityComparer<Step>
        {
            public bool Equals(Step x, Step y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.PluginTypeName == y.PluginTypeName && x.Message == y.Message && x.Stage == y.Stage && x.Mode == y.Mode && x.EntityName == y.EntityName && x.UnsecureConfig == y.UnsecureConfig;
            }

            public int GetHashCode(Step obj)
            {
                unchecked
                {
                    var hashCode = (obj.PluginTypeName != null ? obj.PluginTypeName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Message != null ? obj.Message.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (int)obj.Stage;
                    hashCode = (hashCode * 397) ^ (int)obj.Mode;
                    hashCode = (hashCode * 397) ^ (obj.EntityName != null ? obj.EntityName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.UnsecureConfig != null ? obj.UnsecureConfig.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}