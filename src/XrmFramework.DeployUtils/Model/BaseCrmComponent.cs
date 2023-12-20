using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Model
{
    public abstract class BaseCrmComponent : ICrmComponent
    {
        public abstract string UniqueName { get; set; }
        public abstract int Rank { get; }
        public abstract bool DoAddToSolution { get; }
        public abstract bool DoFetchTypeCode { get; }
        public abstract string EntityTypeName { get; }
        public abstract IEnumerable<ICrmComponent> Children { get; }
        public virtual void AddChild(ICrmComponent child) { child.ParentId = Id; }
        protected abstract void RemoveChild(ICrmComponent child);

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        
        private Guid _id = Guid.NewGuid();
        public Guid ParentId { get; set; } = Guid.NewGuid();

        public Guid Id
        {
            get => _id;
            set
            {
                foreach (var child in Children)
                {
                    child.ParentId = value;
                }
                _id = value;
            }
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            var childrenWithStateSafe = Children
                .Where(c => c.RegistrationState == state)
                .ToList();
            foreach (var child in childrenWithStateSafe)
            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any())
                {
                    RemoveChild(child);
                }
            }
        }
    }
}
