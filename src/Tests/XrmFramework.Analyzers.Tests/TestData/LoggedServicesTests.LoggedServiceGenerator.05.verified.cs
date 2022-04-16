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

        Guid IService.Create(Entity entity)
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

        void IService.AssociateRecords(EntityReference objectRef, Relationship relationName, params EntityReference[] entityReferences)
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

        void IService.TestEnum(EnumTest value = EnumTest.Null)
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(TestEnum), "Start: value = {0}", value);

            Service.TestEnum(value);

            Log(nameof(TestEnum), "End : duration = {0}", sw.Elapsed);
        }

        void IService.Update(Entity entity)
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
