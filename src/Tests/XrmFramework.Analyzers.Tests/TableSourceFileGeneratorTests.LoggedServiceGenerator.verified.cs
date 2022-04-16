//HintName: XrmFramework.IService.logged.cs
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
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

        public  global::System.Guid Create(global::Microsoft.Xrm.Sdk.Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Create), "Start: entity = {0}, useAdmin = {1}, bypassCustomPluginExecution = {2}", entity, useAdmin, bypassCustomPluginExecution);

            var returnValue = Service.Create(entity, useAdmin, bypassCustomPluginExecution);

            Log(nameof(Create), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::Microsoft.Xrm.Sdk.Messages.UpsertResponse Upsert(global::Microsoft.Xrm.Sdk.Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Upsert), "Start: entity = {0}, useAdmin = {1}, bypassCustomPluginExecution = {2}", entity, useAdmin, bypassCustomPluginExecution);

            var returnValue = Service.Upsert(entity, useAdmin, bypassCustomPluginExecution);

            Log(nameof(Upsert), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  void Update(global::Microsoft.Xrm.Sdk.Entity entity, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Update), "Start: entity = {0}, useAdmin = {1}, bypassCustomPluginExecution = {2}", entity, useAdmin, bypassCustomPluginExecution);

            Service.Update(entity, useAdmin, bypassCustomPluginExecution);

            Log(nameof(Update), "End : duration = {0}", sw.Elapsed);
        }

        public  void Delete(string logicalName, global::System.Guid id, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Delete), "Start: logicalName = {0}, id = {1}, useAdmin = {2}, bypassCustomPluginExecution = {3}", logicalName, id, useAdmin, bypassCustomPluginExecution);

            Service.Delete(logicalName, id, useAdmin, bypassCustomPluginExecution);

            Log(nameof(Delete), "End : duration = {0}", sw.Elapsed);
        }

        public  global::System.Guid Create(global::Microsoft.Xrm.Sdk.Entity entity, global::System.Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (callerId == default)
            {
                throw new ArgumentNullException(nameof(callerId));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Create), "Start: entity = {0}, callerId = {1}, bypassCustomPluginExecution = {2}", entity, callerId, bypassCustomPluginExecution);

            var returnValue = Service.Create(entity, callerId, bypassCustomPluginExecution);

            Log(nameof(Create), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::Microsoft.Xrm.Sdk.Messages.UpsertResponse Upsert(global::Microsoft.Xrm.Sdk.Entity entity, global::System.Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (callerId == default)
            {
                throw new ArgumentNullException(nameof(callerId));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Upsert), "Start: entity = {0}, callerId = {1}, bypassCustomPluginExecution = {2}", entity, callerId, bypassCustomPluginExecution);

            var returnValue = Service.Upsert(entity, callerId, bypassCustomPluginExecution);

            Log(nameof(Upsert), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  void Update(global::Microsoft.Xrm.Sdk.Entity entity, global::System.Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (entity == default)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (callerId == default)
            {
                throw new ArgumentNullException(nameof(callerId));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Update), "Start: entity = {0}, callerId = {1}, bypassCustomPluginExecution = {2}", entity, callerId, bypassCustomPluginExecution);

            Service.Update(entity, callerId, bypassCustomPluginExecution);

            Log(nameof(Update), "End : duration = {0}", sw.Elapsed);
        }

        public  void Delete(string logicalName, global::System.Guid id, global::System.Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (callerId == default)
            {
                throw new ArgumentNullException(nameof(callerId));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Delete), "Start: logicalName = {0}, id = {1}, callerId = {2}, bypassCustomPluginExecution = {3}", logicalName, id, callerId, bypassCustomPluginExecution);

            Service.Delete(logicalName, id, callerId, bypassCustomPluginExecution);

            Log(nameof(Delete), "End : duration = {0}", sw.Elapsed);
        }

        public  void Delete(global::Microsoft.Xrm.Sdk.EntityReference objectReference, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectReference == default)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Delete), "Start: objectReference = {0}, useAdmin = {1}, bypassCustomPluginExecution = {2}", objectReference, useAdmin, bypassCustomPluginExecution);

            Service.Delete(objectReference, useAdmin, bypassCustomPluginExecution);

            Log(nameof(Delete), "End : duration = {0}", sw.Elapsed);
        }

        public  void Delete(global::Microsoft.Xrm.Sdk.EntityReference objectReference, global::System.Guid callerId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectReference == default)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            if (callerId == default)
            {
                throw new ArgumentNullException(nameof(callerId));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Delete), "Start: objectReference = {0}, callerId = {1}, bypassCustomPluginExecution = {2}", objectReference, callerId, bypassCustomPluginExecution);

            Service.Delete(objectReference, callerId, bypassCustomPluginExecution);

            Log(nameof(Delete), "End : duration = {0}", sw.Elapsed);
        }

        public  void AssignEntity(global::Microsoft.Xrm.Sdk.EntityReference objectReference, global::Microsoft.Xrm.Sdk.EntityReference ownerRef, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectReference == default)
            {
                throw new ArgumentNullException(nameof(objectReference));
            }
            if (ownerRef == default)
            {
                throw new ArgumentNullException(nameof(ownerRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AssignEntity), "Start: objectReference = {0}, ownerRef = {1}, bypassCustomPluginExecution = {2}", objectReference, ownerRef, bypassCustomPluginExecution);

            Service.AssignEntity(objectReference, ownerRef, bypassCustomPluginExecution);

            Log(nameof(AssignEntity), "End : duration = {0}", sw.Elapsed);
        }

        public  void AddUsersToTeam(global::Microsoft.Xrm.Sdk.EntityReference teamRef, params global::Microsoft.Xrm.Sdk.EntityReference[] userRefs)
        {
            #region Parameters check
            if (teamRef == default)
            {
                throw new ArgumentNullException(nameof(teamRef));
            }
            if (userRefs == default)
            {
                throw new ArgumentNullException(nameof(userRefs));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AddUsersToTeam), "Start: teamRef = {0}, userRefs = {1}", teamRef, userRefs);

            Service.AddUsersToTeam(teamRef, userRefs);

            Log(nameof(AddUsersToTeam), "End : duration = {0}", sw.Elapsed);
        }

        public  void AddUsersToTeam(global::Microsoft.Xrm.Sdk.EntityReference teamRef, bool bypassCustomPluginExecution, params global::Microsoft.Xrm.Sdk.EntityReference[] userRefs)
        {
            #region Parameters check
            if (teamRef == default)
            {
                throw new ArgumentNullException(nameof(teamRef));
            }
            if (userRefs == default)
            {
                throw new ArgumentNullException(nameof(userRefs));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AddUsersToTeam), "Start: teamRef = {0}, bypassCustomPluginExecution = {1}, userRefs = {2}", teamRef, bypassCustomPluginExecution, userRefs);

            Service.AddUsersToTeam(teamRef, bypassCustomPluginExecution, userRefs);

            Log(nameof(AddUsersToTeam), "End : duration = {0}", sw.Elapsed);
        }

        public  void RemoveUsersFromTeam(global::Microsoft.Xrm.Sdk.EntityReference teamRef, params global::Microsoft.Xrm.Sdk.EntityReference[] userRefs)
        {
            #region Parameters check
            if (teamRef == default)
            {
                throw new ArgumentNullException(nameof(teamRef));
            }
            if (userRefs == default)
            {
                throw new ArgumentNullException(nameof(userRefs));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(RemoveUsersFromTeam), "Start: teamRef = {0}, userRefs = {1}", teamRef, userRefs);

            Service.RemoveUsersFromTeam(teamRef, userRefs);

            Log(nameof(RemoveUsersFromTeam), "End : duration = {0}", sw.Elapsed);
        }

        public  void RemoveUsersFromTeam(global::Microsoft.Xrm.Sdk.EntityReference teamRef, bool bypassCustomPluginExecution, params global::Microsoft.Xrm.Sdk.EntityReference[] userRefs)
        {
            #region Parameters check
            if (teamRef == default)
            {
                throw new ArgumentNullException(nameof(teamRef));
            }
            if (userRefs == default)
            {
                throw new ArgumentNullException(nameof(userRefs));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(RemoveUsersFromTeam), "Start: teamRef = {0}, bypassCustomPluginExecution = {1}, userRefs = {2}", teamRef, bypassCustomPluginExecution, userRefs);

            Service.RemoveUsersFromTeam(teamRef, bypassCustomPluginExecution, userRefs);

            Log(nameof(RemoveUsersFromTeam), "End : duration = {0}", sw.Elapsed);
        }

        public  void AddToQueue(global::System.Guid queueId, global::Microsoft.Xrm.Sdk.EntityReference target, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (queueId == default)
            {
                throw new ArgumentNullException(nameof(queueId));
            }
            if (target == default)
            {
                throw new ArgumentNullException(nameof(target));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AddToQueue), "Start: queueId = {0}, target = {1}, bypassCustomPluginExecution = {2}", queueId, target, bypassCustomPluginExecution);

            Service.AddToQueue(queueId, target, bypassCustomPluginExecution);

            Log(nameof(AddToQueue), "End : duration = {0}", sw.Elapsed);
        }

        public  void Merge(global::Microsoft.Xrm.Sdk.EntityReference target, global::System.Guid subordonate, global::Microsoft.Xrm.Sdk.Entity content, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (target == default)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (subordonate == default)
            {
                throw new ArgumentNullException(nameof(subordonate));
            }
            if (content == default)
            {
                throw new ArgumentNullException(nameof(content));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Merge), "Start: target = {0}, subordonate = {1}, content = {2}, bypassCustomPluginExecution = {3}", target, subordonate, content, bypassCustomPluginExecution);

            Service.Merge(target, subordonate, content, bypassCustomPluginExecution);

            Log(nameof(Merge), "End : duration = {0}", sw.Elapsed);
        }

        public  void SetState(global::Microsoft.Xrm.Sdk.EntityReference objectRef, int stateCode, int statusCode, bool useAdmin = false, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectRef == default)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(SetState), "Start: objectRef = {0}, stateCode = {1}, statusCode = {2}, useAdmin = {3}, bypassCustomPluginExecution = {4}", objectRef, stateCode, statusCode, useAdmin, bypassCustomPluginExecution);

            Service.SetState(objectRef, stateCode, statusCode, useAdmin, bypassCustomPluginExecution);

            Log(nameof(SetState), "End : duration = {0}", sw.Elapsed);
        }

        public  void Share(global::Microsoft.Xrm.Sdk.EntityReference objectRef, global::Microsoft.Xrm.Sdk.EntityReference assignee, AccessRights accessRights, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectRef == default)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (assignee == default)
            {
                throw new ArgumentNullException(nameof(assignee));
            }
            if (accessRights == default)
            {
                throw new ArgumentNullException(nameof(accessRights));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Share), "Start: objectRef = {0}, assignee = {1}, accessRights = {2}, bypassCustomPluginExecution = {3}", objectRef, assignee, accessRights, bypassCustomPluginExecution);

            Service.Share(objectRef, assignee, accessRights, bypassCustomPluginExecution);

            Log(nameof(Share), "End : duration = {0}", sw.Elapsed);
        }

        public  void UnShare(global::Microsoft.Xrm.Sdk.EntityReference objectRef, global::Microsoft.Xrm.Sdk.EntityReference revokee, global::Microsoft.Xrm.Sdk.EntityReference callerRef = null, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (objectRef == default)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (revokee == default)
            {
                throw new ArgumentNullException(nameof(revokee));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(UnShare), "Start: objectRef = {0}, revokee = {1}, callerRef = {2}, bypassCustomPluginExecution = {3}", objectRef, revokee, callerRef, bypassCustomPluginExecution);

            Service.UnShare(objectRef, revokee, callerRef, bypassCustomPluginExecution);

            Log(nameof(UnShare), "End : duration = {0}", sw.Elapsed);
        }

        public  global::Microsoft.Xrm.Sdk.Entity Retrieve(string entityName, global::System.Guid id, params string[] columns)
        {
            #region Parameters check
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (columns == default)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Retrieve), "Start: entityName = {0}, id = {1}, columns = {2}", entityName, id, columns);

            var returnValue = Service.Retrieve(entityName, id, columns);

            Log(nameof(Retrieve), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::Microsoft.Xrm.Sdk.Entity Retrieve(string entityName, global::System.Guid id, bool allColumns)
        {
            #region Parameters check
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Retrieve), "Start: entityName = {0}, id = {1}, allColumns = {2}", entityName, id, allColumns);

            var returnValue = Service.Retrieve(entityName, id, allColumns);

            Log(nameof(Retrieve), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::Microsoft.Xrm.Sdk.Entity Retrieve(global::Microsoft.Xrm.Sdk.EntityReference objectRef, params string[] columns)
        {
            #region Parameters check
            if (objectRef == default)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            if (columns == default)
            {
                throw new ArgumentNullException(nameof(columns));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Retrieve), "Start: objectRef = {0}, columns = {1}", objectRef, columns);

            var returnValue = Service.Retrieve(objectRef, columns);

            Log(nameof(Retrieve), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::Microsoft.Xrm.Sdk.Entity Retrieve(global::Microsoft.Xrm.Sdk.EntityReference objectRef, bool allColumns)
        {
            #region Parameters check
            if (objectRef == default)
            {
                throw new ArgumentNullException(nameof(objectRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Retrieve), "Start: objectRef = {0}, allColumns = {1}", objectRef, allColumns);

            var returnValue = Service.Retrieve(objectRef, allColumns);

            Log(nameof(Retrieve), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  string GetOptionSetNameFromValue(string optionsetName, int optionsetValue)
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetOptionSetNameFromValue), "Start: optionsetName = {0}, optionsetValue = {1}", optionsetName, optionsetValue);

            var returnValue = Service.GetOptionSetNameFromValue(optionsetName, optionsetValue);

            Log(nameof(GetOptionSetNameFromValue), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  string GetOptionSetNameFromValue<T>(int optionsetValue)
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetOptionSetNameFromValue), "Start: optionsetValue = {0}", optionsetValue);

            var returnValue = Service.GetOptionSetNameFromValue<T>(optionsetValue);

            Log(nameof(GetOptionSetNameFromValue), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  T GetById<T>(global::System.Guid id) where T : IBindingModel, new()
        {
            #region Parameters check
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetById), "Start: id = {0}", id);

            var returnValue = Service.GetById<T>(id);

            Log(nameof(GetById), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  T GetById<T>(global::Microsoft.Xrm.Sdk.EntityReference entityReference) where T : IBindingModel, new()
        {
            #region Parameters check
            if (entityReference == default)
            {
                throw new ArgumentNullException(nameof(entityReference));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetById), "Start: entityReference = {0}", entityReference);

            var returnValue = Service.GetById<T>(entityReference);

            Log(nameof(GetById), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  T Upsert<T>(T model, bool isAdmin = false, bool bypassCustomPluginExecution = false) where T : IBindingModel, new()
        {
            #region Parameters check
            if (model == default)
            {
                throw new ArgumentNullException(nameof(model));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(Upsert), "Start: model = {0}, isAdmin = {1}, bypassCustomPluginExecution = {2}", model, isAdmin, bypassCustomPluginExecution);

            var returnValue = Service.Upsert<T>(model, isAdmin, bypassCustomPluginExecution);

            Log(nameof(Upsert), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  bool UserHasRole(global::System.Guid userId, global::System.Guid parentRoleId)
        {
            #region Parameters check
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (parentRoleId == default)
            {
                throw new ArgumentNullException(nameof(parentRoleId));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(UserHasRole), "Start: userId = {0}, parentRoleId = {1}", userId, parentRoleId);

            var returnValue = Service.UserHasRole(userId, parentRoleId);

            Log(nameof(UserHasRole), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  bool UserHasOneRoleOf(global::System.Guid userId, params global::System.Guid[] parentRoleIds)
        {
            #region Parameters check
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (parentRoleIds == default)
            {
                throw new ArgumentNullException(nameof(parentRoleIds));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(UserHasOneRoleOf), "Start: userId = {0}, parentRoleIds = {1}", userId, parentRoleIds);

            var returnValue = Service.UserHasOneRoleOf(userId, parentRoleIds);

            Log(nameof(UserHasOneRoleOf), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  bool UserHasOneRoleOf(global::System.Guid userId, params string[] parentRoleIds)
        {
            #region Parameters check
            if (userId == default)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            if (parentRoleIds == default)
            {
                throw new ArgumentNullException(nameof(parentRoleIds));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(UserHasOneRoleOf), "Start: userId = {0}, parentRoleIds = {1}", userId, parentRoleIds);

            var returnValue = Service.UserHasOneRoleOf(userId, parentRoleIds);

            Log(nameof(UserHasOneRoleOf), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::System.Collections.Generic.ICollection<global::System.Guid> GetUserRoleIds(global::Microsoft.Xrm.Sdk.EntityReference userRef)
        {
            #region Parameters check
            if (userRef == default)
            {
                throw new ArgumentNullException(nameof(userRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetUserRoleIds), "Start: userRef = {0}", userRef);

            var returnValue = Service.GetUserRoleIds(userRef);

            Log(nameof(GetUserRoleIds), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::Microsoft.Xrm.Sdk.Entity ToEntity<T>(T model) where T : IBindingModel
        {
            #region Parameters check
            if (model == default)
            {
                throw new ArgumentNullException(nameof(model));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(ToEntity), "Start: model = {0}", model);

            var returnValue = Service.ToEntity<T>(model);

            Log(nameof(ToEntity), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  global::System.Collections.Generic.ICollection<global::Microsoft.Xrm.Sdk.EntityReference> GetTeamMemberRefs(global::Microsoft.Xrm.Sdk.EntityReference teamRef)
        {
            #region Parameters check
            if (teamRef == default)
            {
                throw new ArgumentNullException(nameof(teamRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetTeamMemberRefs), "Start: teamRef = {0}", teamRef);

            var returnValue = Service.GetTeamMemberRefs(teamRef);

            Log(nameof(GetTeamMemberRefs), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  void AssociateRecords(global::Microsoft.Xrm.Sdk.EntityReference objectRef, global::Microsoft.Xrm.Sdk.Relationship relationName, params global::Microsoft.Xrm.Sdk.EntityReference[] entityReferences)
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

        public  void AssociateRecords(global::Microsoft.Xrm.Sdk.EntityReference objectRef, global::Microsoft.Xrm.Sdk.Relationship relationName, bool bypassCustomPluginExecution, params global::Microsoft.Xrm.Sdk.EntityReference[] entityReferences)
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

        public  TVariable GetEnvironmentVariable<TVariable>(string schemaName)
        {
            #region Parameters check
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(GetEnvironmentVariable), "Start: schemaName = {0}", schemaName);

            var returnValue = Service.GetEnvironmentVariable<TVariable>(schemaName);

            Log(nameof(GetEnvironmentVariable), "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);

            return returnValue;
        }

        public  void AddRoleToUserOrTeam(global::Microsoft.Xrm.Sdk.EntityReference userOrTeamRef, string parentRootRoleIdOrTemplateId, bool bypassCustomPluginExecution = false)
        {
            #region Parameters check
            if (userOrTeamRef == default)
            {
                throw new ArgumentNullException(nameof(userOrTeamRef));
            }
            #endregion

            var sw = new Stopwatch();
            sw.Start();

            Log(nameof(AddRoleToUserOrTeam), "Start: userOrTeamRef = {0}, parentRootRoleIdOrTemplateId = {1}, bypassCustomPluginExecution = {2}", userOrTeamRef, parentRootRoleIdOrTemplateId, bypassCustomPluginExecution);

            Service.AddRoleToUserOrTeam(userOrTeamRef, parentRootRoleIdOrTemplateId, bypassCustomPluginExecution);

            Log(nameof(AddRoleToUserOrTeam), "End : duration = {0}", sw.Elapsed);
        }
    }
}
