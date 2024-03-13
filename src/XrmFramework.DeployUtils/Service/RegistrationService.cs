using Deploy;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
#if NET6_0_OR_GREATER
using Microsoft.PowerPlatform.Dataverse.Client;
#else
using Microsoft.Xrm.Tooling.Connector;
#endif
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils.Model;
using CustomApi = Deploy.CustomApi;
using CustomApiRequestParameter = Deploy.CustomApiRequestParameter;
using CustomApiResponseProperty = Deploy.CustomApiResponseProperty;
using PluginPackage = XrmFramework.DeployUtils.Model.PluginPackage;

namespace XrmFramework.DeployUtils.Service;

/// <summary>
///     Base implementation of <see cref="IRegistrationService" />
/// </summary>
public class RegistrationService : IRegistrationService
{
	private readonly IOrganizationService _client;

	public RegistrationService(IOrganizationService client)
	{
		_client = client;
	}

	public RegistrationService(string connectionString)
	{

#if NET6_0_OR_GREATER
        _client = new ServiceClient(connectionString);
#else
				_client = new CrmServiceClient(connectionString);
#endif	
    }

    public IEnumerable<PluginAssembly> GetAssemblies()
	{
		var query = new QueryExpression(PluginAssemblyDefinition.EntityName);
		query.ColumnSet.AddColumns(PluginAssemblyDefinition.Columns.Id, PluginAssemblyDefinition.Columns.Name);
		query.Distinct                = true;
		query.Criteria.FilterOperator = LogicalOperator.And;
		query.Criteria.AddCondition(PluginAssemblyDefinition.Columns.Name, ConditionOperator.NotLike,
		                            "CompiledWorkflow%");
		var filter = query.Criteria.AddFilter(LogicalOperator.Or);
		filter.AddCondition(PluginAssemblyDefinition.Columns.CustomizationLevel, ConditionOperator.Null);
		filter.AddCondition(PluginAssemblyDefinition.Columns.CustomizationLevel, ConditionOperator.NotEqual, 0);
		filter.AddCondition(PluginAssemblyDefinition.Columns.Name, ConditionOperator.In, "Microsoft.Crm.ObjectModel",
		                    "Microsoft.Crm.ServiceBus");

		var result = RetrieveAll(query);

		var list = new List<PluginAssembly>();
		foreach (var assembly in result)
		{
			list.Add(assembly.ToEntity<PluginAssembly>());
		}

		return list;
	}

	public PluginAssembly GetAssemblyByName(string assemblyName)
	{
		var query = new QueryExpression(PluginAssemblyDefinition.EntityName);
		query.ColumnSet.AddColumns(PluginAssemblyDefinition.Columns.Id, PluginAssemblyDefinition.Columns.Name);
		query.Distinct                = true;
		query.Criteria.FilterOperator = LogicalOperator.And;
		query.Criteria.AddCondition(PluginAssemblyDefinition.Columns.Name, ConditionOperator.Equal, assemblyName);

		return RetrieveAll(query).FirstOrDefault()?.ToEntity<PluginAssembly>();
	}

	public AssemblyInfo GetAssemblyInfoByName(string assemblyName)
	{
		var query = BindingModelHelper.GetRetrieveAllQuery<AssemblyInfo>();

		query.Criteria.AddCondition(PluginAssemblyDefinition.Columns.Name, ConditionOperator.Equal, assemblyName);

		return RetrieveAll(query).FirstOrDefault()?.ToBindingModel<AssemblyInfo>();
	}


	public ICollection<CustomApiRequestParameter> GetRegisteredCustomApiRequestParameters(Guid assemblyId)
	{
		var list = new List<CustomApiRequestParameter>();

		var query = new QueryExpression(CustomApiRequestParameterDefinition.EntityName);
		query.ColumnSet.AllColumns = true;
		query.Criteria.AddCondition(CustomApiRequestParameterDefinition.Columns.IsManaged, ConditionOperator.Equal,
		                            false);

		var linkCustomApi = query.AddLink(CustomApiDefinition.EntityName,
		                                  CustomApiRequestParameterDefinition.Columns.CustomAPIId,
		                                  CustomApiDefinition.Columns.Id);
		var linkPluginType = linkCustomApi.AddLink(PluginTypeDefinition.EntityName,
		                                           CustomApiDefinition.Columns.PluginTypeId,
		                                           PluginTypeDefinition.Columns.Id);
		linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal,
		                                         assemblyId);

		var result = RetrieveAll(query);
		foreach (var type in result) list.Add(type.ToEntity<CustomApiRequestParameter>());

		return list;
	}

	public ICollection<CustomApiResponseProperty> GetRegisteredCustomApiResponseProperties(Guid assemblyId)
	{
		var list = new List<CustomApiResponseProperty>();

		var query = new QueryExpression(CustomApiResponsePropertyDefinition.EntityName);
		query.ColumnSet.AllColumns = true;
		query.Criteria.AddCondition(CustomApiResponsePropertyDefinition.Columns.IsManaged, ConditionOperator.Equal,
		                            false);

		var linkCustomApi = query.AddLink(CustomApiDefinition.EntityName,
		                                  CustomApiResponsePropertyDefinition.Columns.CustomAPIId,
		                                  CustomApiDefinition.Columns.Id);
		var linkPluginType = linkCustomApi.AddLink(PluginTypeDefinition.EntityName,
		                                           CustomApiDefinition.Columns.PluginTypeId,
		                                           PluginTypeDefinition.Columns.Id);
		linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal,
		                                         assemblyId);

		var result = RetrieveAll(query);
		foreach (var type in result) list.Add(type.ToEntity<CustomApiResponseProperty>());

		return list;
	}

	public ICollection<CustomApi> GetRegisteredCustomApis(Guid assemblyId)
	{
		var list = new List<CustomApi>();

		var query = new QueryExpression(CustomApiDefinition.EntityName);
		query.ColumnSet.AllColumns = true;
		query.Criteria.AddCondition(CustomApiDefinition.Columns.IsManaged, ConditionOperator.Equal, false);
		var linkPluginType = query.AddLink(PluginTypeDefinition.EntityName,
		                                   CustomApiDefinition.Columns.PluginTypeId,
		                                   PluginTypeDefinition.Columns.Id);
		linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal,
		                                         assemblyId);

		var result = RetrieveAll(query);
		foreach (var type in result) list.Add(type.ToEntity<CustomApi>());

		return list;
	}

	public ICollection<SdkMessageProcessingStepImage> GetRegisteredImages(Guid assemblyId)
	{
		var list = new List<SdkMessageProcessingStepImage>();

		var query = new QueryExpression(SdkMessageProcessingStepImageDefinition.EntityName);
		query.ColumnSet.AllColumns = true;
		var stepLink = query.AddLink(SdkMessageProcessingStepDefinition.EntityName,
		                             SdkMessageProcessingStepImageDefinition.Columns.SdkMessageProcessingStepId,
		                             SdkMessageProcessingStepDefinition.Columns.Id);
		var linkPluginType = stepLink.AddLink(PluginTypeDefinition.EntityName,
		                                      SdkMessageProcessingStepDefinition.Columns.EventHandler,
		                                      PluginTypeDefinition.Columns.Id);
		linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal,
		                                         assemblyId);

		var result = RetrieveAll(query);
		foreach (var type in result) list.Add(type.ToEntity<SdkMessageProcessingStepImage>());

		return list;
	}

	public ICollection<PluginType> GetRegisteredPluginTypes(Guid assemblyId)
	{
		var list = new List<PluginType>();

		var query = new QueryExpression(PluginTypeDefinition.EntityName);
		query.ColumnSet.AllColumns = true;
		query.Criteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal, assemblyId);

		var result = RetrieveAll(query);
		foreach (var type in result) list.Add(type.ToEntity<PluginType>());

		return list;
	}

	public ICollection<SdkMessageProcessingStep> GetRegisteredSteps(Guid assemblyId)
	{
		var list = new List<SdkMessageProcessingStep>();

		var query = new QueryExpression(SdkMessageProcessingStepDefinition.EntityName);
		query.ColumnSet.AllColumns = true;
		query.Criteria.AddCondition(SdkMessageProcessingStepDefinition.Columns.Stage, ConditionOperator.NotEqual, 30);

		var linkPluginType = query.AddLink(PluginTypeDefinition.EntityName,
		                                   SdkMessageProcessingStepDefinition.Columns.EventHandler,
		                                   PluginTypeDefinition.Columns.Id);
		linkPluginType.LinkCriteria.AddCondition(PluginTypeDefinition.Columns.PluginAssemblyId, ConditionOperator.Equal,
		                                         assemblyId);

		var filterLink = query.AddLink(SdkMessageFilterDefinition.EntityName,
		                               SdkMessageProcessingStepDefinition.Columns.SdkMessageFilterId,
		                               SdkMessageFilterDefinition.Columns.Id,
		                               JoinOperator.LeftOuter);

		filterLink.EntityAlias = "filter";
		filterLink.Columns.AddColumn(SdkMessageFilterDefinition.PrimaryObjectTypeCode);


		var result = RetrieveAll(query);
		foreach (var type in result) list.Add(type.ToEntity<SdkMessageProcessingStep>());

		return list;
	}

	public PluginPackage GetRegisteredPackage(Guid assemblyId)
		=> this.GetById<PluginPackage>(assemblyId);

	public IList<Entity> RetrieveAll(QueryExpression query, bool cleanLinks = true)
	{
		if (!query.TopCount.HasValue) query.PageInfo = new PagingInfo {Count = 5000, PageNumber = 1};

		var result = new List<Entity>();

		EntityCollection ec;

		do
		{
			ec = _client.RetrieveMultiple(query);

			result.AddRange(ec.Entities);

			if (query.PageInfo != null)
			{
				query.PageInfo.PageNumber++;
				query.PageInfo.PagingCookie = ec.PagingCookie;
			}
		} while (ec.MoreRecords);

		return result;
	}

	public int GetIntEntityTypeCode(string logicalName)
	{
		var entityRequest = new RetrieveEntityRequest {LogicalName = logicalName};

		var entityResponse = (RetrieveEntityResponse) _client.Execute(entityRequest);

		return entityResponse.EntityMetadata.ObjectTypeCode.GetValueOrDefault();
	}

#region IOrganizationService proxy implementation

	public Guid Create(Entity entity)
	{
		return _client.Create(entity);
	}

	public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
	{
		return _client.Retrieve(entityName, id, columnSet);
	}

	public void Update(Entity entity)
	{
		_client.Update(entity);
	}

	public void Delete(string entityName, Guid id)
	{
		_client.Delete(entityName, id);
	}

	public OrganizationResponse Execute(OrganizationRequest request)
	{
		return _client.Execute(request);
	}

	public void Associate(string entityName, Guid entityId, Microsoft.Xrm.Sdk.Relationship relationship,
	                      EntityReferenceCollection relatedEntities)
	{
		_client.Associate(entityName, entityId, relationship, relatedEntities);
	}

	public void Disassociate(string entityName, Guid entityId, Microsoft.Xrm.Sdk.Relationship relationship,
	                         EntityReferenceCollection relatedEntities)
	{
		_client.Disassociate(entityName, entityId, relationship, relatedEntities);
	}

	public EntityCollection RetrieveMultiple(QueryBase query)
	{
		return _client.RetrieveMultiple(query);
	}

#endregion
}
