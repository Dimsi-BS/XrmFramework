using System.Linq;
using NUnit.Framework;

namespace XrmFramework.Core.Tests;

/// <summary>
/// The rule the generated definitions rest on: a relationship becomes a constant when a selected
/// lookup column stands behind it, on whichever side of the relationship that column lives.
/// </summary>
[TestFixture]
public class RelationSelectorTests
{
    private TableCollection _tables = null!;

    private Table _account = null!;
    private Table _contact = null!;

    [SetUp]
    public void InitTests()
    {
        _account = new Table { LogicalName = "account", Name = "Account", CollectionName = "accounts" };
        _account.Columns.Add(new Column
        {
            LogicalName = "accountid", Name = "Id", PrimaryType = PrimaryType.Id, Selected = true
        });

        _contact = new Table { LogicalName = "contact", Name = "Contact", CollectionName = "contacts" };
        _contact.Columns.Add(new Column
        {
            LogicalName = "contactid", Name = "Id", PrimaryType = PrimaryType.Id, Selected = true
        });
        _contact.Columns.Add(new Column
        {
            LogicalName = "accountid", Name = "AccountId", Type = AttributeTypeCode.Lookup, Selected = true
        });
        _contact.Columns.Add(new Column
        {
            LogicalName = "parentcustomerid", Name = "ParentCustomerId", Type = AttributeTypeCode.Lookup
        });

        // The two ends of each relationship, as the CRM reports them on either table.
        _contact.ManyToOneRelationships.Add(new Relation
        {
            Name = "contact_customer_accounts", EntityName = "account", LookupFieldName = "accountid"
        });
        _contact.ManyToOneRelationships.Add(new Relation
        {
            Name = "account_primary_contact", EntityName = "account", LookupFieldName = "parentcustomerid"
        });

        _account.OneToManyRelationships.Add(new Relation
        {
            Name = "contact_customer_accounts", EntityName = "contact",
            Role = EntityRole.Referenced, LookupFieldName = "accountid"
        });
        _account.OneToManyRelationships.Add(new Relation
        {
            Name = "account_primary_contact", EntityName = "contact",
            Role = EntityRole.Referenced, LookupFieldName = "parentcustomerid"
        });

        _tables = new TableCollection { _account, _contact };
    }

    [Test]
    public void ManyToOne_KeepsTheRelationshipOfASelectedLookup()
    {
        var relations = RelationSelector.ManyToOne(_contact);

        Assert.AreEqual(1, relations.Count);
        Assert.AreEqual("contact_customer_accounts", relations[0].Name);
    }

    [Test]
    public void OneToMany_KeepsTheRelationshipTheOtherEndSelects()
    {
        // Nothing is selected on the account: it is contact.accountid that answers for both ends.
        var relations = RelationSelector.OneToMany(_tables, _account);

        Assert.AreEqual(1, relations.Count);
        Assert.AreEqual("contact_customer_accounts", relations[0].Name);
    }

    [Test]
    public void OneToMany_DropsTheRelationshipOfAReferencingTableOutOfTheCompilation()
    {
        _tables.Remove(_contact);

        Assert.IsEmpty(RelationSelector.OneToMany(_tables, _account));
    }

    [Test]
    public void Selection_FollowsTheLookupBeingSelected()
    {
        _contact.Columns.Single(c => c.LogicalName == "parentcustomerid").Selected = true;

        Assert.AreEqual(2, RelationSelector.ManyToOne(_contact).Count);
        Assert.AreEqual(2, RelationSelector.OneToMany(_tables, _account).Count);
    }

    [Test]
    public void ManyToOne_KeepsTheRelationshipEvenWhenTheReferencedTableIsAbsent()
    {
        // The lookup column is generated with a [CrmLookup(..., RelationshipName = ...)] attribute
        // naming this very constant: dropping it would leave the attribute pointing at nothing.
        _tables.Remove(_account);

        var relations = RelationSelector.ManyToOne(_contact);

        Assert.AreEqual(1, relations.Count);
        Assert.AreEqual("contact_customer_accounts", relations[0].Name);
    }

    [Test]
    public void ManyToMany_KeepsTheRelationshipsWhoseOtherEndIsPartOfTheCompilation()
    {
        // A N:N rests on an intersect table nobody declares: no lookup to select, so presence is all
        // there is to go by.
        _account.ManyToManyRelationships.Add(new Relation { Name = "accountleads_association", EntityName = "lead" });
        _account.ManyToManyRelationships.Add(new Relation
        {
            Name = "contactaccount_association", EntityName = "contact", LookupFieldName = "accountid"
        });

        var relations = RelationSelector.ManyToMany(_tables, _account);

        Assert.AreEqual(1, relations.Count);
        Assert.AreEqual("contactaccount_association", relations[0].Name);
    }

    [Test]
    public void Selection_OfANullTable_IsEmpty()
    {
        Assert.IsEmpty(RelationSelector.ManyToOne(null));
        Assert.IsEmpty(RelationSelector.OneToMany(_tables, null));
        Assert.IsEmpty(RelationSelector.ManyToMany(null, _account));
    }
}
