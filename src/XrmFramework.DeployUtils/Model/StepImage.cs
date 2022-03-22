using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.DeployUtils.Model
{
    public class StepImage
    {
        public StepImage(string message, bool isPreImage)
        {
            Message = message;
            RegistrationState = RegistrationState.NotComputed;
            IsPreImage = isPreImage;
        }

        public Guid Id { get; set; }

        public Guid StepId { get; set; }
        public string Message { get; }
        public bool IsPreImage { get; set; }

        public bool IsUsed => Message != "Create" && Message != "Book" && !(Message == "Delete" && !IsPreImage) && (AllAttributes || Attributes.Any());
        public bool AllAttributes { get; set; }
        public List<string> Attributes { get; private set; } = new List<string>();
        public string JoinedAttributes => string.Join(",", Attributes);


        public RegistrationState RegistrationState { get; set; }


    }
}
