using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.Definitions;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Model
{
    public class StepImage : ISolutionComponent
    {
        public StepImage(string message, bool isPreImage, Stages stage)
        {
            Message = message;
            Stage = stage;
            IsPreImage = isPreImage;
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }
        public string Message { get; }
        public Stages Stage { get; }

        public bool IsPreImage { get; set; }

        public bool IsUsed => UniversalImageUsedPrefix && ImageUsedPrefix && (AllAttributes || Attributes.Any());

        private bool UniversalImageUsedPrefix => Message != Messages.Associate.ToString()
                                              && Message != Messages.Lose.ToString()
                                              && Message != Messages.Win.ToString();

        private bool ImageUsedPrefix => IsPreImage ? Message != "Create" && Message != "Book" : Stage == Stages.PostOperation && Message != "Delete";
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
