using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.DeployUtils.Model
{
    public class StepImage
    {
        public StepImage(string message, bool isPreImage, Stages stage)
        {
            Message = message;
            Stage = stage;
            RegistrationState = RegistrationState.NotComputed;
            IsPreImage = isPreImage;
            UniversalImageUsedPrefix = 
                        Message != Messages.Associate.ToString()
                     && Message != Messages.Lose.ToString()
                     && Message != Messages.Win.ToString();
        }

        public Guid Id { get; set; }

        public Guid StepId { get; set; }
        public string Message { get; }
        public Stages Stage { get; }

        public bool IsPreImage { get; set; }

        public bool IsUsed => UniversalImageUsedPrefix && ImageUsedPrefix && (AllAttributes || Attributes.Any());

        private bool UniversalImageUsedPrefix;
        private bool ImageUsedPrefix => IsPreImage ? Message != "Create" && Message != "Book" : Stage == Stages.PostOperation && Message != "Delete";
        public bool AllAttributes { get; set; }
        public List<string> Attributes { get; private set; } = new List<string>();
        public string JoinedAttributes => string.Join(",", Attributes);


        public RegistrationState RegistrationState { get; set; }


    }
}
