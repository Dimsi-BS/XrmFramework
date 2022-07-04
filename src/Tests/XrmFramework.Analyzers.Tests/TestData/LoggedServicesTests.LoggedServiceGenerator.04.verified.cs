//HintName: XrmFramework.IService.logged.cs
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using XrmFramework;

namespace XrmFramework
{
    [DebuggerStepThrough, CompilerGenerated]
    public class LoggedIService : LoggedServiceBase, IService
    {

        #region .ctor
        public LoggedIService(IServiceContext context, IService service) : base(context, service)
        {
        }
        #endregion

        public global::System.Guid Create(global::Microsoft.Xrm.Sdk.Entity entity)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Create), "Start: entity = {0}", entity);

            var returnValue = Service.Create(entity);

            Log(nameof(Create), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public global::System.Guid Create2(global::Microsoft.Xrm.Sdk.Entity entity)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Create2), "Start: entity = {0}", entity);

            var returnValue = Service.Create2(entity);

            Log(nameof(Create2), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public void AssociateRecords(global::Microsoft.Xrm.Sdk.EntityReference objectRef, global::Microsoft.Xrm.Sdk.Relationship relationName, params global::Microsoft.Xrm.Sdk.EntityReference[] entityReferences)
        {
            #region Parameters check
            if (objectRef == default)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (relationName == default)
            {
                throw new ArgumentNullException(nameof(relationName));
            }
            if (entityReferences == default)
            {
                throw new ArgumentNullException(nameof(entityReferences));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AssociateRecords), "Start: objectRef = {0}, relationName = {1}, entityReferences = {2}", objectRef, relationName, entityReferences);

            Service.AssociateRecords(objectRef, relationName, entityReferences);

            Log(nameof(AssociateRecords), "End : duration = {0}", sw.Elapsed);
        }

        public void AssociateRecords(global::Microsoft.Xrm.Sdk.EntityReference objectRef, global::Microsoft.Xrm.Sdk.Relationship relationName, bool bypassCustomPluginExecution, params global::Microsoft.Xrm.Sdk.EntityReference[] entityReferences)
        {
            #region Parameters check
            if (objectRef == default)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (relationName == default)
            {
                throw new ArgumentNullException(nameof(relationName));
            }
            if (entityReferences == default)
            {
                throw new ArgumentNullException(nameof(entityReferences));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AssociateRecords), "Start: objectRef = {0}, relationName = {1}, bypassCustomPluginExecution = {2}, entityReferences = {3}", objectRef, relationName, bypassCustomPluginExecution, entityReferences);

            Service.AssociateRecords(objectRef, relationName, bypassCustomPluginExecution, entityReferences);

            Log(nameof(AssociateRecords), "End : duration = {0}", sw.Elapsed);
        }

        public void TestEnum(global::XrmFramework.EnumTest value)
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(TestEnum), "Start: value = {0}", value);

            Service.TestEnum(value);

            Log(nameof(TestEnum), "End : duration = {0}", sw.Elapsed);
        }

        public byte[] GetByteContent()
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetByteContent), "Start");

            var returnValue = Service.GetByteContent();

            Log(nameof(GetByteContent), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public int[] GetIntArray(byte[] content)
        {
            #region Parameters check
            if (content == default)
            {
                throw new ArgumentNullException(nameof(content));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetIntArray), "Start: content = {0}", content);

            var returnValue = Service.GetIntArray(content);

            Log(nameof(GetIntArray), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public void Update(global::Microsoft.Xrm.Sdk.Entity entity)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Update), "Start: entity = {0}", entity);

            Service.Update(entity);

            Log(nameof(Update), "End : duration = {0}", sw.Elapsed);
        }
    }
}
