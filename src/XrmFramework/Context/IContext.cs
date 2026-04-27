// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public interface IContext
    {
        EntityReference UserRef { get; }
        EntityReference BusinessUnitRef { get; }

        /// <summary>
        /// Fournisseur de date/heure. À utiliser à la place de <see cref="System.DateTime.UtcNow"/>
        /// et <see cref="System.DateTime.Now"/> pour garantir la reproductibilité lors du rejouage
        /// de sessions de test.
        /// </summary>
        IDateTimeProvider Clock { get; }

        T GetInputParameter<T>(InputParameters parameterName);
        void SetInputParameter<T>(InputParameters parameterName, T parameterValue);

        T GetOutputParameter<T>(OutputParameters parameterName);
        void SetOutputParameter<T>(OutputParameters parameterName, T parameterValue);

        bool HasPostImage(string imageName);
        Entity GetPostImage(string imageName);

        bool HasPreImage(string imageName);
        Entity GetPreImage(string imageName);

        bool HasSharedVariable(string variableName);
        T GetSharedVariable<T>(string variableName);
        void SetSharedVariable<T>(string variableName, T value);

        bool IsSynchronous();
        bool IsAsynchronous();
        
        bool IsCreate();
        bool IsUpdate();
        bool IsMessage(Messages message);

        void Log(string message, params object[] formatArgs);

        void LogFields(Entity entity, params string[] fieldNames);
    }
}
