using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    public class StepImage : ISolutionComponent
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
        public List<string> Attributes { get; private set; } = new List<string>();
        public string JoinedAttributes => string.Join(",", Attributes);


        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

        public string EntityTypeName => SdkMessageProcessingStepImageDefinition.EntityName;
        public string UniqueName => "ISolutionComponent Implementation";

        public IEnumerable<ISolutionComponent> Children => new List<ISolutionComponent>();
        public void AddChild(ISolutionComponent child) => throw new ArgumentException("StepImage doesn't take children");
    }
}
