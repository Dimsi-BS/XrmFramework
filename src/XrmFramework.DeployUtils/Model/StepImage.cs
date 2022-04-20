using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public class StepImage : ICrmComponent
    {
        public StepImage(Messages message, bool isPreImage, Stages stage)
        {
            Message = message;
            Stage = stage;
            IsPreImage = isPreImage;
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }
        public Messages Message { get; }
        public Stages Stage { get; }

        public bool IsPreImage { get; set; }

        public bool IsUsed => UniversalImageUsedPrefix && ImageUsedPrefix && (AllAttributes || Attributes.Any());

        private bool UniversalImageUsedPrefix => Message != Messages.Associate
                                                 && Message != Messages.Lose
                                                 && Message != Messages.Win;

        private bool ImageUsedPrefix => IsPreImage ? Message != Messages.Create && Message != Messages.Book : Stage == Stages.PostOperation && Message != Messages.Delete;
        public bool AllAttributes { get; set; }
        public List<string> Attributes { get; } = new List<string>();
        public string JoinedAttributes => string.Join(",", Attributes);


        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public string EntityTypeName => SdkMessageProcessingStepImageDefinition.EntityName;
        public string UniqueName => "ISolutionComponent Implementation";

        public Step FatherStep { get; set; }
        public IEnumerable<ICrmComponent> Children => new List<ICrmComponent>();
        public void AddChild(ICrmComponent child) => throw new ArgumentException("StepImage doesn't take children");

        public void RemoveChild(ICrmComponent child)
        {
            throw new NotImplementedException();
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            foreach (var child in Children)
            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any() && child.RegistrationState == state)
                {
                    RemoveChild(child);
                }
            }
        }

        public int Rank => 3;
        public bool DoAddToSolution => false;
        public bool DoFetchTypeCode => false;
    }
}
