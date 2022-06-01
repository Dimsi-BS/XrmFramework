using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Metadata of a StepImage, allows the user to ask for more data concerning the context when triggered
    /// </summary>
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICrmComponent" />
    public class StepImage : BaseCrmComponent
    {
        public StepImage(Messages message, bool isPreImage, Stages stage)
        {
            Message = message;
            Stage = stage;
            IsPreImage = isPreImage;
        }

        /// <summary>
        /// Determines the type of <see cref="StepImage"/>
        /// </summary>
        public bool IsPreImage { get; set; }

        /// <summary>
        /// Determines whether the <see cref="StepImage"/> should appear as a child of its <see cref="Step"/>
        /// </summary>
        /// <remarks>
        /// This workaround is necessary as a StepImage is always instantiated<br/>
        /// This is a way of hiding it without actually disposing of it
        /// </remarks>
        /// TODO The way things are now I think the ToDelete part has become unnecessary, but hey it works I'll check later
        public bool ShouldAppearAsChild => IsUsed && RegistrationState != RegistrationState.Computed ||
                                           RegistrationState == RegistrationState.ToDelete;

        /// <summary>Indicates whether the <see cref="StepImage"/> is relevant to the Crm</summary>
        public bool IsUsed => UniversalImageUsedPrefix && ImageUsedPrefix && (AllAttributes || Attributes.Any());

        /// <summary>This predicate is valid for any kind of StepImage</summary>
        private bool UniversalImageUsedPrefix => Message != Messages.Associate
                                                 && Message != Messages.Lose
                                                 && Message != Messages.Win;

        /// <summary>Redirect to the appropriate Prefix</summary>
        private bool ImageUsedPrefix => IsPreImage ? PreImagePrefix : PostImagePrefix;

        /// <summary>Prefix to apply to PreImages</summary>
        private bool PreImagePrefix => Message != Messages.Create && Message != Messages.Book;

        /// <summary>Prefix to apply to PostImages</summary>
        private bool PostImagePrefix => Stage == Stages.PostOperation && Message != Messages.Delete;

        /// <summary><inheritdoc cref="Step.Message"/></summary>
        public Messages Message { get; }

        /// <summary><inheritdoc cref="Step.Stage"/></summary>
        public Stages Stage { get; }

        /// <summary>Is true if all attributes must appear in the StepImage</summary>
        public bool AllAttributes { get; set; }

        /// <summary>List of the attributes that must appear in the StepImage</summary>
        public HashSet<string> Attributes { get; } = new HashSet<string>();

        /// <summary>Joined string of the <see cref="Attributes"/></summary>
        public string JoinedAttributes => string.Join(",", Attributes);
        public void Merge(StepImage other)
        {
            AllAttributes |= other.AllAttributes;

            if (AllAttributes)
            {
                Attributes.Clear();
            }
            else
            {
                Attributes.UnionWith(other.Attributes);
            }
        }

        /// <summary><see cref="Step"/> this StepImage is attached to</summary>
        /// <remarks>
        /// This is used mainly to be able to differentiate two StepImages on the fly.<br/>
        /// See an implementation of <see cref="Utils.ICrmComponentComparer"/> for better understanding
        /// </remarks>
        public Step FatherStep { get; set; }

        #region BaseCrmComponent overrides
        public override string EntityTypeName => SdkMessageProcessingStepImageDefinition.EntityName;
        public override int Rank => 3;
        public override bool DoAddToSolution => false;
        public override bool DoFetchTypeCode => false;
        #endregion

        #region ICrmComponent dummy implementation
        public override string UniqueName
        {
            get => "ISolutionComponent Implementation";
            set => _ = value;
        }
        public override IEnumerable<ICrmComponent> Children => Enumerable.Empty<ICrmComponent>();
        public override void AddChild(ICrmComponent child) => throw new ArgumentException("StepImage doesn't take children");
        protected override void RemoveChild(ICrmComponent child) { }

        #endregion
    }
}
