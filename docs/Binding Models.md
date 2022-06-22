##  Introduction
There are three classes related to a an Entity from a CRM at various levels of abstraction.
  - Entity, stores the data corresponding to an entity record from the CRM.
  - EntityDefinition, lists the various components of an table. Each Entity has one per project.
  Photo d'une définition, ex compte
  - BindingModel, strongly typed representation of an Entity record. There can be as many binding models as there as needs for one Entity in one project.



## Creating a Binding Model
First, you need a Definition corresponding to your Entity. You need to retrieve it from the CRM. You can see how by following [this](QuickStart.md) tutorial

     Photo du tool XRM

Then create a new class in your project, it needs to inherit either IBindingModel or BindingModelBase.

## Retrieving the CRM data

## Updating the CRM data

## JSon Serialization

## JSon Deserialization


