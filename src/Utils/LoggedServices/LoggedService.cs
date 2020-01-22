using System;
using Model;
using Microsoft.Crm.Sdk.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Diagnostics;

namespace Plugins
{
	public class LoggedService : ILoggedService, IService
	{
		private IService Service { get; set; }

		protected Logger Log { get; set; }

		#region .ctor
		public LoggedService(IServiceContext context, IService service)
		{
			Service = service;

			Log = context.Logger;
		}
		#endregion

		public Guid Create(Entity  entity, Boolean  useAdmin = false)
		{
			#region Parameters check
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Create", "Start: entity = {0}, useAdmin = {1}", entity, useAdmin);

			var returnValue = Service.Create( entity,  useAdmin);

			Log("Create", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public UpsertResponse Upsert(Entity  entity, Boolean  useAdmin = false)
		{
			#region Parameters check
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Upsert", "Start: entity = {0}, useAdmin = {1}", entity, useAdmin);

			var returnValue = Service.Upsert( entity,  useAdmin);

			Log("Upsert", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public void Update(Entity  entity, Boolean  useAdmin = false)
		{
			#region Parameters check
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Update", "Start: entity = {0}, useAdmin = {1}", entity, useAdmin);

			Service.Update( entity,  useAdmin);

			Log("Update", "End : duration = {0}", sw.Elapsed);
		}

		public void Delete(String  logicalName, Guid  id, Boolean  useAdmin = false)
		{
			#region Parameters check
			if (string.IsNullOrEmpty(logicalName))
			{
				throw new ArgumentNullException(nameof(logicalName));
			}
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(id));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Delete", "Start: logicalName = {0}, id = {1}, useAdmin = {2}", logicalName, id, useAdmin);

			Service.Delete( logicalName,  id,  useAdmin);

			Log("Delete", "End : duration = {0}", sw.Elapsed);
		}

		public Guid Create(Entity  entity, Guid  callerId)
		{
			#region Parameters check
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			if (callerId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(callerId));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Create", "Start: entity = {0}, callerId = {1}", entity, callerId);

			var returnValue = Service.Create( entity,  callerId);

			Log("Create", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public UpsertResponse Upsert(Entity  entity, Guid  callerId)
		{
			#region Parameters check
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			if (callerId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(callerId));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Upsert", "Start: entity = {0}, callerId = {1}", entity, callerId);

			var returnValue = Service.Upsert( entity,  callerId);

			Log("Upsert", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public void Update(Entity  entity, Guid  callerId)
		{
			#region Parameters check
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}
			if (callerId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(callerId));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Update", "Start: entity = {0}, callerId = {1}", entity, callerId);

			Service.Update( entity,  callerId);

			Log("Update", "End : duration = {0}", sw.Elapsed);
		}

		public void Delete(String  logicalName, Guid  id, Guid  callerId)
		{
			#region Parameters check
			if (string.IsNullOrEmpty(logicalName))
			{
				throw new ArgumentNullException(nameof(logicalName));
			}
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(id));
			}
			if (callerId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(callerId));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Delete", "Start: logicalName = {0}, id = {1}, callerId = {2}", logicalName, id, callerId);

			Service.Delete( logicalName,  id,  callerId);

			Log("Delete", "End : duration = {0}", sw.Elapsed);
		}

		public void Delete(EntityReference  objectReference, Boolean  useAdmin = false)
		{
			#region Parameters check
			if (objectReference == null)
			{
				throw new ArgumentNullException(nameof(objectReference));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Delete", "Start: objectReference = {0}, useAdmin = {1}", objectReference, useAdmin);

			Service.Delete( objectReference,  useAdmin);

			Log("Delete", "End : duration = {0}", sw.Elapsed);
		}

		public void Delete(EntityReference  objectReference, Guid  callerId)
		{
			#region Parameters check
			if (objectReference == null)
			{
				throw new ArgumentNullException(nameof(objectReference));
			}
			if (callerId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(callerId));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Delete", "Start: objectReference = {0}, callerId = {1}", objectReference, callerId);

			Service.Delete( objectReference,  callerId);

			Log("Delete", "End : duration = {0}", sw.Elapsed);
		}

		public void AssignEntity(EntityReference  objectReference, EntityReference  ownerRef)
		{
			#region Parameters check
			if (objectReference == null)
			{
				throw new ArgumentNullException(nameof(objectReference));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("AssignEntity", "Start: objectReference = {0}, ownerRef = {1}", objectReference, ownerRef);

			Service.AssignEntity( objectReference,  ownerRef);

			Log("AssignEntity", "End : duration = {0}", sw.Elapsed);
		}

		public void AddUsersToTeam(EntityReference  teamRef, EntityReference[]  userRefs)
		{
			#region Parameters check
			if (teamRef == null)
			{
				throw new ArgumentNullException(nameof(teamRef));
			}
			if (userRefs == null)
			{
				throw new ArgumentNullException(nameof(userRefs));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("AddUsersToTeam", "Start: teamRef = {0}, userRefs = {1}", teamRef, userRefs);

			Service.AddUsersToTeam( teamRef,  userRefs);

			Log("AddUsersToTeam", "End : duration = {0}", sw.Elapsed);
		}

		public void RemoveUsersFromTeam(EntityReference  teamRef, EntityReference[]  userRefs)
		{
			#region Parameters check
			if (teamRef == null)
			{
				throw new ArgumentNullException(nameof(teamRef));
			}
			if (userRefs == null)
			{
				throw new ArgumentNullException(nameof(userRefs));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("RemoveUsersFromTeam", "Start: teamRef = {0}, userRefs = {1}", teamRef, userRefs);

			Service.RemoveUsersFromTeam( teamRef,  userRefs);

			Log("RemoveUsersFromTeam", "End : duration = {0}", sw.Elapsed);
		}

		public void AddToQueue(Guid  queueId, EntityReference  target)
		{
			#region Parameters check
			if (queueId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(queueId));
			}
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("AddToQueue", "Start: queueId = {0}, target = {1}", queueId, target);

			Service.AddToQueue( queueId,  target);

			Log("AddToQueue", "End : duration = {0}", sw.Elapsed);
		}

		public void Merge(EntityReference  target, Guid  subordonate, Entity  content)
		{
			#region Parameters check
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			if (subordonate == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(subordonate));
			}
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Merge", "Start: target = {0}, subordonate = {1}, content = {2}", target, subordonate, content);

			Service.Merge( target,  subordonate,  content);

			Log("Merge", "End : duration = {0}", sw.Elapsed);
		}

		public void SetState(EntityReference  objectRef, Int32  stateCode, Int32  statusCode, Boolean  useAdmin = false)
		{
			#region Parameters check
			if (objectRef == null)
			{
				throw new ArgumentNullException(nameof(objectRef));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("SetState", "Start: objectRef = {0}, stateCode = {1}, statusCode = {2}, useAdmin = {3}", objectRef, stateCode, statusCode, useAdmin);

			Service.SetState( objectRef,  stateCode,  statusCode,  useAdmin);

			Log("SetState", "End : duration = {0}", sw.Elapsed);
		}

		public void Share(EntityReference  objectRef, EntityReference  assignee, AccessRights  accessRights)
		{
			#region Parameters check
			if (objectRef == null)
			{
				throw new ArgumentNullException(nameof(objectRef));
			}
			if (assignee == null)
			{
				throw new ArgumentNullException(nameof(assignee));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Share", "Start: objectRef = {0}, assignee = {1}, accessRights = {2}", objectRef, assignee, accessRights);

			Service.Share( objectRef,  assignee,  accessRights);

			Log("Share", "End : duration = {0}", sw.Elapsed);
		}

		public void UnShare(EntityReference  objectRef, EntityReference  revokee, EntityReference  callerRef = null)
		{
			#region Parameters check
			if (objectRef == null)
			{
				throw new ArgumentNullException(nameof(objectRef));
			}
			if (revokee == null)
			{
				throw new ArgumentNullException(nameof(revokee));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("UnShare", "Start: objectRef = {0}, revokee = {1}, callerRef = {2}", objectRef, revokee, callerRef);

			Service.UnShare( objectRef,  revokee,  callerRef);

			Log("UnShare", "End : duration = {0}", sw.Elapsed);
		}

		public Entity Retrieve(String  entityName, Guid  id, String[]  columns)
		{
			#region Parameters check
			if (string.IsNullOrEmpty(entityName))
			{
				throw new ArgumentNullException(nameof(entityName));
			}
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(id));
			}
			if (columns == null)
			{
				throw new ArgumentNullException(nameof(columns));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Retrieve", "Start: entityName = {0}, id = {1}, columns = {2}", entityName, id, columns);

			var returnValue = Service.Retrieve( entityName,  id,  columns);

			Log("Retrieve", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public Entity Retrieve(String  entityName, Guid  id, Boolean  allColumns)
		{
			#region Parameters check
			if (string.IsNullOrEmpty(entityName))
			{
				throw new ArgumentNullException(nameof(entityName));
			}
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(id));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Retrieve", "Start: entityName = {0}, id = {1}, allColumns = {2}", entityName, id, allColumns);

			var returnValue = Service.Retrieve( entityName,  id,  allColumns);

			Log("Retrieve", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public Entity Retrieve(EntityReference  objectRef, String[]  columns)
		{
			#region Parameters check
			if (objectRef == null)
			{
				throw new ArgumentNullException(nameof(objectRef));
			}
			if (columns == null)
			{
				throw new ArgumentNullException(nameof(columns));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Retrieve", "Start: objectRef = {0}, columns = {1}", objectRef, columns);

			var returnValue = Service.Retrieve( objectRef,  columns);

			Log("Retrieve", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public Entity Retrieve(EntityReference  objectRef, Boolean  allColumns)
		{
			#region Parameters check
			if (objectRef == null)
			{
				throw new ArgumentNullException(nameof(objectRef));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Retrieve", "Start: objectRef = {0}, allColumns = {1}", objectRef, allColumns);

			var returnValue = Service.Retrieve( objectRef,  allColumns);

			Log("Retrieve", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public String GetOptionSetNameFromValue(String  optionsetName, Int32  optionsetValue)
		{
			#region Parameters check
			if (string.IsNullOrEmpty(optionsetName))
			{
				throw new ArgumentNullException(nameof(optionsetName));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("GetOptionSetNameFromValue", "Start: optionsetName = {0}, optionsetValue = {1}", optionsetName, optionsetValue);

			var returnValue = Service.GetOptionSetNameFromValue( optionsetName,  optionsetValue);

			Log("GetOptionSetNameFromValue", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public String GetOptionSetNameFromValue<T>(Int32  optionsetValue)
		{
			#region Parameters check
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("GetOptionSetNameFromValue", "Start: optionsetValue = {0}", optionsetValue);

			var returnValue = Service.GetOptionSetNameFromValue<T>( optionsetValue);

			Log("GetOptionSetNameFromValue", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public T GetById<T>(Guid  id) where T : IBindingModel, new()
		{
			#region Parameters check
			if (id == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(id));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("GetById", "Start: id = {0}", id);

			var returnValue = Service.GetById<T>( id);

			Log("GetById", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public T GetById<T>(EntityReference  entityReference) where T : IBindingModel, new()
		{
			#region Parameters check
			if (entityReference == null)
			{
				throw new ArgumentNullException(nameof(entityReference));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("GetById", "Start: entityReference = {0}", entityReference);

			var returnValue = Service.GetById<T>( entityReference);

			Log("GetById", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public T Upsert<T>(T  model, Boolean  isAdmin = false) where T : IBindingModel, new()
		{
			#region Parameters check
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("Upsert", "Start: model = {0}, isAdmin = {1}", model, isAdmin);

			var returnValue = Service.Upsert<T>( model,  isAdmin);

			Log("Upsert", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public Boolean UserHasRole(Guid  userId, Guid  parentRoleId)
		{
			#region Parameters check
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			if (parentRoleId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(parentRoleId));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("UserHasRole", "Start: userId = {0}, parentRoleId = {1}", userId, parentRoleId);

			var returnValue = Service.UserHasRole( userId,  parentRoleId);

			Log("UserHasRole", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public Boolean UserHasOneRoleOf(Guid  userId, Guid[]  parentRoleIds)
		{
			#region Parameters check
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			if (parentRoleIds == null)
			{
				throw new ArgumentNullException(nameof(parentRoleIds));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("UserHasOneRoleOf", "Start: userId = {0}, parentRoleIds = {1}", userId, parentRoleIds);

			var returnValue = Service.UserHasOneRoleOf( userId,  parentRoleIds);

			Log("UserHasOneRoleOf", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public Boolean UserHasOneRoleOf(Guid  userId, String[]  parentRoleIds)
		{
			#region Parameters check
			if (userId == Guid.Empty)
			{
				throw new ArgumentNullException(nameof(userId));
			}
			if (parentRoleIds == null)
			{
				throw new ArgumentNullException(nameof(parentRoleIds));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("UserHasOneRoleOf", "Start: userId = {0}, parentRoleIds = {1}", userId, parentRoleIds);

			var returnValue = Service.UserHasOneRoleOf( userId,  parentRoleIds);

			Log("UserHasOneRoleOf", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public ICollection<Guid> GetUserRoleIds(EntityReference  userRef)
		{
			#region Parameters check
			if (userRef == null)
			{
				throw new ArgumentNullException(nameof(userRef));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("GetUserRoleIds", "Start: userRef = {0}", userRef);

			var returnValue = Service.GetUserRoleIds( userRef);

			Log("GetUserRoleIds", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public Entity ToEntity<T>(T  model) where T : IBindingModel
		{
			#region Parameters check
			if (model == null)
			{
				throw new ArgumentNullException(nameof(model));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("ToEntity", "Start: model = {0}", model);

			var returnValue = Service.ToEntity<T>( model);

			Log("ToEntity", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public ICollection<EntityReference> GetTeamMemberRefs(EntityReference  teamRef)
		{
			#region Parameters check
			if (teamRef == null)
			{
				throw new ArgumentNullException(nameof(teamRef));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("GetTeamMemberRefs", "Start: teamRef = {0}", teamRef);

			var returnValue = Service.GetTeamMemberRefs( teamRef);

			Log("GetTeamMemberRefs", "End : duration = {0}, returnValue = {1}", sw.Elapsed, returnValue);
			return returnValue;
		}

		public void AssociateRecords(EntityReference  objectRef, Relationship  relationName, EntityReference[]  entityReferences)
		{
			#region Parameters check
			if (objectRef == null)
			{
				throw new ArgumentNullException(nameof(objectRef));
			}
			if (relationName == null)
			{
				throw new ArgumentNullException(nameof(relationName));
			}
			if (entityReferences == null)
			{
				throw new ArgumentNullException(nameof(entityReferences));
			}
			#endregion


			var sw = new Stopwatch();
			sw.Start();

			Log("AssociateRecords", "Start: objectRef = {0}, relationName = {1}, entityReferences = {2}", objectRef, relationName, entityReferences);

			Service.AssociateRecords( objectRef,  relationName,  entityReferences);

			Log("AssociateRecords", "End : duration = {0}", sw.Elapsed);
		}
	}
}
