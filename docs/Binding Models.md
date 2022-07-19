##  Introduction
There are three classes related to a an Entity from a CRM at various levels of abstraction.
  - Entity, stores the data corresponding to an entity record from the CRM.
  - EntityDefinition, lists the various components of an table. Each Entity has one per project.
  Photo d'une définition, ex compte
  - BindingModel, strongly typed representation of an Entity record. There can be as many binding models as there as needs for one Entity in one project.



## Creating a Binding Model
First, you need a Definition corresponding to your Entity. You need to retrieve it from the CRM. You can see how by following [this](QuickStart.md) tutorial

Then create a new class in your project, it needs to inherit either IBindingModel or BindingModelBase the difference between the two is explained in [this](##updating-the_crm-data) part.
You can add a property for each column you want to see in the object.
To do so, use the CrmMapping attribute and the corresponding EntityDefinition. The type of the property has to make sense for the data you want to retrieve. For example, a property corresponding to the name field of an object will have to be of type string. The various types to use are detailed in [this](##types-to-use-for-crm-data) section.

- photo de public class model : IBindingModel
- photo d'un field avec une explication des différentes parties

## Retrieving the CRM data
In order to retrieve entity records as BindingModel, the framework uses custom AdminOrganizationService functions : 
```cs
query = BindingModelHelper.GetRetrieveAllQuery<BindingModel>();
AdminOrganizationService.RetrieveAll<BindingModel>(query); // Returns all records corresponding to the Entity present on the CRM as BindingModels.

AdminOrganizationService.GetById<BindingModel>(ID); // Returns the Entity record corresponding to the ID as a BindingModel

```

## Updating the CRM data

```cs
var existingAccount = service.getById<AccountModel>(accountID);
var newAccountModel = new AccountModel {Name = "Titi"};
var diffAccount = newAccountModel.GetDiffGeneric(existingAccount);
if(diffAccount.InitializedProperties.Any())
{
	service.Upsert(diffAccount);
}
```

When modifying a record on a CRM using an instance of the Entity class. You can simply use the OrganisationService.Update() function. To update a record using a BindingModel, the procedure is slightly different.
First your BindingModel, has to inherit BindingModelBase. A model that inherits IBindingModel can only be used as a way to view data from the CRM, not to update it.
Any property that needs to be updated must call OnPropertyChanged inside of its set property.

## JSon Serialization
Any BindingModel instance can be serialized, you just need to do this.

## JSon Deserialization

## Types to use for CRM data
  
  | CRM Attribute type      | C# equivalents |
| ----------- | ----------- |
  | Boolean   | System.Boolean       |
  |    | System.Int32       |
|    | System.String       |
  | Integer   | System.Int32       |
  | DateTime   | System.DateTime       |
  |    | System.String       |
  | Decimal   | System.Decimal       |
  | Double   | System.Double       |
  | Lookup   | To be explained further       |
  | Memo   | System.String       |
  | PartyList   | To be explained further       |
  | PickList   | System.Int32       |
  | PickList   | Corresponding OptionSetEnum       |
  | State   | Corresponding OptionSetEnum       |
  | State   | System.Int32       |
  | Status   | Corresponding OptionSetEnum       |
  | Status   | System.Int32       |
  | String   | System.String       |
  | UniqueIdentifier   | System.Guid       |
  | BigInt   | System.Int64       |
  | EntityName   | System.String       |




  
  
  
  
  - lookup
  
  


