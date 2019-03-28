// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Model.Sdk.Queries
{
    internal class Condition : IQueryToString
    {
        public string EntityName { get; }

        public string AttributeName { get; }

        public ConditionOperator Operator { get; }

        public IList<object> Values { get; } = new List<object>();

        private EntityDefinition Definition { get; }

        private AttributeTypeCode AttributeType { get; }

        public string WebApiAttributeName { get; set; }

        public string EntityAlias { get; set; }

        public bool UseAlias { get; private set; }

        public int AliasIndex { get; private set; }

        internal Condition(string entityName, string entityAlias, string attributeName, ConditionOperator conditionOperator)
        {
            EntityName = entityName;
            AttributeName = attributeName;
            Operator = conditionOperator;
            EntityAlias = entityAlias;
            Definition = DefinitionCache.GetEntityDefinition(EntityName);
            AttributeType = Definition?.GetAttributeType(AttributeName) ?? AttributeTypeCode.String;
            WebApiAttributeName = Definition?.GetWebApiAttributeName(AttributeName);
        }

        internal Condition(string entityName, string attributeName, ConditionOperator conditionOperator, object value) : this(entityName, null, attributeName, conditionOperator)
        {
            Values.Add(value);
        }

        internal Condition(string entityName, string entityAlias, string attributeName, ConditionOperator conditionOperator, object value) : this(entityName, entityAlias, attributeName, conditionOperator)
        {
            Values.Add(value);
        }

        internal Condition(string entityName, string attributeName, ConditionOperator conditionOperator, IEnumerable<object> values) : this(entityName, null, attributeName, conditionOperator)
        {
            values.ToList().ForEach(v => Values.Add(v));
        }

        public string ToFetchXmlString()
        {
            var sb = new StringBuilder();
            sb.Append($"<condition attribute=\"{AttributeName}\" ");
            sb.Append($"operator=\"{Operator.GetDescription()}\" ");

            if (!string.IsNullOrEmpty(EntityAlias))
            {
                sb.Append($"entityname=\"{EntityAlias}\" ");
            }

            if (Values.Count == 1)
            {
                string stringValue;
                switch (AttributeType)
                {
                    case AttributeTypeCode.Boolean:
                        stringValue = (bool) Values.First() ? "1" : "0";
                        break;
                    case AttributeTypeCode.DateTime:
                        stringValue = ((DateTime) Values.First()).ToString("o");
                        break;
                    default:
                        stringValue = $"{Values.First()}";
                        break;
                }
                sb.Append($"value=\"{stringValue}\"");
            }

            if (Values.Count <= 1)
            {
                sb.Append(" />");
            }
            else
            {
                sb.Append(" >");
                foreach (var value in Values)
                {
                    sb.Append($"<value>{value}</value>");
                }
                sb.Append("</condition>");
            }

            return sb.ToString();
        }

        public string ToWebApiString(Func<int> aliasIndexer)
        {
            var sb = new StringBuilder();

            if (AttributeType == AttributeTypeCode.String && Values.Count == 1)
            {
                var specificString = SpecificOperator(Values.Single());

                if (!string.IsNullOrEmpty(specificString))
                {
                    return specificString;
                }
            }

            if (Operator == ConditionOperator.In || Operator == ConditionOperator.NotIn)
            {
                UseAlias = true;
                AliasIndex = aliasIndexer();

                var negate = Operator == ConditionOperator.NotIn;
                if (negate)
                {
                    sb.Append("not ");
                }

                sb.Append("(");
                foreach (var value in Values)
                {
                    sb.Append($"@p{AliasIndex} ");
                    sb.Append(negate ? "neq " : "eq ");
                    sb.Append(Query.ToWebApiValue(AttributeType, value));
                    sb.Append(negate ? " and" : " or");
                }

                sb.Remove(sb.Length - (negate ? 4 : 3), negate ? 4 : 3);

                sb.Append(")");
            }
            else
            {
                var value = Values.FirstOrDefault();

                sb.Append(Definition.GetWebApiAttributeName(AttributeName));

                if (Operator == ConditionOperator.Null)
                {
                    sb.Append(" eq null");
                }
                else if (Operator == ConditionOperator.NotNull)
                {
                    sb.Append(" ne null");
                }
                else
                {
                    sb.Append(OperatorString);

                    sb.Append(Query.ToWebApiValue(AttributeType, value));
                }
            }

            return sb.ToString();
        }

        public string OperatorString
        {
            get
            {
                switch (Operator)
                {
                    case ConditionOperator.Equal:
                        return " eq ";
                    case ConditionOperator.NotEqual:
                        return " ne ";
                    case ConditionOperator.GreaterThan:
                        return " gt ";
                    case ConditionOperator.LessThan:
                        return " lt ";
                    case ConditionOperator.GreaterEqual:
                        return " ge ";
                    case ConditionOperator.LessEqual:
                        return " le ";
                }

                return string.Empty;
            }
        }

        public bool IsWebApiFriendly
        {
            get
            {
                if (!string.IsNullOrEmpty(EntityAlias))
                {
                    return false;
                }

                switch (Operator)
                {
                    case ConditionOperator.Yesterday:
                    case ConditionOperator.Today:
                    case ConditionOperator.Tomorrow:
                    case ConditionOperator.Last7Days:
                    case ConditionOperator.Next7Days:
                    case ConditionOperator.LastWeek:
                    case ConditionOperator.ThisWeek:
                    case ConditionOperator.NextWeek:
                    case ConditionOperator.LastMonth:
                    case ConditionOperator.ThisMonth:
                    case ConditionOperator.NextMonth:
                    case ConditionOperator.On:
                    case ConditionOperator.OnOrBefore:
                    case ConditionOperator.OnOrAfter:
                    case ConditionOperator.LastYear:
                    case ConditionOperator.ThisYear:
                    case ConditionOperator.NextYear:
                    case ConditionOperator.LastXHours:
                    case ConditionOperator.NextXHours:
                    case ConditionOperator.LastXDays:
                    case ConditionOperator.NextXDays:
                    case ConditionOperator.LastXWeeks:
                    case ConditionOperator.NextXWeeks:
                    case ConditionOperator.LastXMonths:
                    case ConditionOperator.NextXMonths:
                    case ConditionOperator.LastXYears:
                    case ConditionOperator.NextXYears:
                    case ConditionOperator.EqualUserId:
                    case ConditionOperator.NotEqualUserId:
                    case ConditionOperator.EqualBusinessId:
                    case ConditionOperator.NotEqualBusinessId:
                    case ConditionOperator.OlderThanXMonths:
                    case ConditionOperator.EqualUserTeams:
                    case ConditionOperator.EqualUserOrUserTeams:
                    case ConditionOperator.Under:
                    case ConditionOperator.NotUnder:
                    case ConditionOperator.UnderOrEqual:
                    case ConditionOperator.Above:
                    case ConditionOperator.AboveOrEqual:
                    case ConditionOperator.EqualUserOrUserHierarchy:
                    case ConditionOperator.EqualUserOrUserHierarchyAndTeams:
                    case ConditionOperator.OlderThanXYears:
                    case ConditionOperator.OlderThanXWeeks:
                    case ConditionOperator.OlderThanXDays:
                    case ConditionOperator.OlderThanXHours:
                    case ConditionOperator.OlderThanXMinutes:
                        return false;
                    default:
                        return true;
                }
            }
        }

        public string SpecificOperator(object value)
        {
            var preOperator = string.Empty;
            if (value is string tempValue)
            {
                switch (Operator)
                {
                    case ConditionOperator.NotLike:
                        preOperator = "not ";
                        goto case ConditionOperator.Like;
                    case ConditionOperator.Like:
                        if (tempValue.StartsWith("%") && tempValue.EndsWith("%"))
                        {
                            tempValue = tempValue.Substring(1, tempValue.Length - 2);
                        }
                        else if (tempValue.StartsWith("%"))
                        {
                            tempValue = tempValue.Substring(1);
                            goto case ConditionOperator.EndsWith;
                        }
                        else if (tempValue.EndsWith("%"))
                        {
                            tempValue = tempValue.Substring(0, tempValue.Length - 1);
                            goto case ConditionOperator.BeginsWith;
                        }
                        preOperator += $"contains({WebApiAttributeName}, '{tempValue}')";
                        break;
                    case ConditionOperator.DoesNotBeginWith:
                        preOperator = "not ";
                        goto case ConditionOperator.BeginsWith;
                    case ConditionOperator.BeginsWith:
                        preOperator += $"startswith({WebApiAttributeName}, '{tempValue}')";
                        break;
                    case ConditionOperator.DoesNotEndWith:
                        preOperator = "not ";
                        goto case ConditionOperator.EndsWith;
                    case ConditionOperator.EndsWith:
                        preOperator += $"endswith({WebApiAttributeName}, '{tempValue}')";
                        break;
                }
            }

            return preOperator;
        }
    }


    public enum ConditionOperator
    {
        //
        // Summary:
        //     The values are compared for equality. Value = 0.
        [Description("eq")] Equal = 0,

        //
        // Summary:
        //     The two values are not equal. Value = 1.
        [Description("ne")] NotEqual = 1,

        //
        // Summary:
        //     The value is greater than the compared value. Value = 2.
        [Description("gt")] GreaterThan = 2,

        //
        // Summary:
        //     The value is less than the compared value. Value = 3.
        [Description("lt")] LessThan = 3,

        //
        // Summary:
        //     The value is greater than or equal to the compared value. Value = 4.
        [Description("ge")] GreaterEqual = 4,

        //
        // Summary:
        //     The value is less than or equal to the compared value. Value = 5.
        [Description("le")] LessEqual = 5,

        //
        // Summary:
        //     The character string is matched to the specified pattern. Value = 6.
        [Description("like")] Like = 6,

        //
        // Summary:
        //     The character string does not match the specified pattern. Value = 7.
        [Description("not-like")] NotLike = 7,

        //
        // Summary:
        //     TheThe value exists in a list of values. Value = 8.
        [Description("in")] In = 8,

        //
        // Summary:
        //     The given value is not matched to a value in a subquery or a list. Value = 9.
        [Description("not-in")] NotIn = 9,

        //
        // Summary:
        //     The value is between two values. Value = 10.
        //Between = 10,
        //
        // Summary:
        //     The value is not between two values. Value = 11.
        //NotBetween = 11,
        //
        // Summary:
        //     The value is null. Value = 12.
        [Description("null")] Null = 12,

        //
        // Summary:
        //     The value is not null. Value = 13.
        [Description("not-null")] NotNull = 13,

        //
        // Summary:
        //     The value equals yesterday’s date. Value = 14.
        [Description("yesterday")] Yesterday = 14,

        //
        // Summary:
        //     The value equals today’s date. Value = 15.
        [Description("today")] Today = 15,

        //
        // Summary:
        //     The value equals tomorrow’s date. Value = 16.
        [Description("tomorrow")] Tomorrow = 16,

        //
        // Summary:
        //     The value is within the last seven days including today. Value = 17.
        [Description("last-seven-days")] Last7Days = 17,

        //
        // Summary:
        //     The value is within the next seven days. Value = 18.
        [Description("next-seven-days")] Next7Days = 18,

        //
        // Summary:
        //     The value is within the previous week including Sunday through Saturday. Value
        //     = 19.
        [Description("last-week")] LastWeek = 19,

        //
        // Summary:
        //     The value is within the current week. Value = 20.
        [Description("this-week")] ThisWeek = 20,

        //
        // Summary:
        //     The value is within the next week. Value = 21.
        [Description("next-week")] NextWeek = 21,

        //
        // Summary:
        //     The value is within the last month including first day of the last month and
        //     last day of the last month. Value = 22.
        [Description("last-month")] LastMonth = 22,

        //
        // Summary:
        //     The value is within the current month. Value = 23.
        [Description("this-month")] ThisMonth = 23,

        //
        // Summary:
        //     The value is within the next month. Value = 24.
        [Description("next-month")] NextMonth = 24,

        //
        // Summary:
        //     The value is on a specified date. Value = 25.
        [Description("on")] On = 25,

        //
        // Summary:
        //     The value is on or before a specified date. Value = 26.
        [Description("on-or-before")] OnOrBefore = 26,

        //
        // Summary:
        //     The value is on or after a specified date. Value = 27.
        [Description("on-or-after")] OnOrAfter = 27,

        //
        // Summary:
        //     The value is within the previous year. Value = 28.
        [Description("last-year")] LastYear = 28,

        //
        // Summary:
        //     The value is within the current year. Value = 29.
        [Description("this-year")] ThisYear = 29,

        //
        // Summary:
        //     The value is within the next year. Value = 30.
        [Description("next-year")] NextYear = 30,

        //
        // Summary:
        //     The value is within the last X hours. Value =31.
        [Description("last-x-hours")] LastXHours = 31,

        //
        // Summary:
        //     The value is within the next X (specified value) hours. Value = 32.
        [Description("next-x-hours")] NextXHours = 32,

        //
        // Summary:
        //     The value is within last X days. Value = 33.
        [Description("last-x-days")] LastXDays = 33,

        //
        // Summary:
        //     The value is within the next X (specified value) days. Value = 34.
        [Description("next-x-days")] NextXDays = 34,

        //
        // Summary:
        //     The value is within the last X (specified value) weeks. Value = 35.
        [Description("last-x-weeks")] LastXWeeks = 35,

        //
        // Summary:
        //     The value is within the next X weeks. Value = 36.
        [Description("next-x-weeks")] NextXWeeks = 36,

        //
        // Summary:
        //     The value is within the last X (specified value) months. Value = 37.
        [Description("last-x-months")] LastXMonths = 37,

        //
        // Summary:
        //     The value is within the next X (specified value) months. Value = 38.
        [Description("next-x-months")] NextXMonths = 38,

        //
        // Summary:
        //     The value is within the last X years. Value = 39.
        [Description("last-x-years")] LastXYears = 39,

        //
        // Summary:
        //     The value is within the next X years. Value = 40.
        [Description("next-x-years")] NextXYears = 40,

        //
        // Summary:
        //     The value is equal to the specified user ID. Value = 41.
        [Description("eq-userid")] EqualUserId = 41,

        //
        // Summary:
        //     The value is not equal to the specified user ID. Value = 42.
        [Description("ne-userid")] NotEqualUserId = 42,

        //
        // Summary:
        //     The value is equal to the specified business ID. Value = 43
        [Description("eq-businessid")] EqualBusinessId = 43,

        //
        // Summary:
        //     The value is not equal to the specified business ID. Value = 44.
        [Description("ne-businessid")] NotEqualBusinessId = 44,

        //
        // Summary:
        //     No token name is specified <token xmlns="http://ddue.schemas.microsoft.com/authoring/2003/5">
        //     <?Comment AL: Bug fix 5/30/12 2012-05-30T11:03:00Z Id='2?>internal<?CommentEnd
        //     Id='2' ?></token>.
        //ChildOf = 45,
        //
        // Summary:
        //     The value is found in the specified bit-mask value. Value = 46.
        //Mask = 46,
        //
        // Summary:
        //     The value is not found in the specified bit-mask value. Value = 47.
        //NotMask = 47,
        //
        // Summary:
        //     For internal use only. Value = 48.
        //MasksSelect = 48,
        //
        // Summary:
        //     The string contains another string. Value = 49.
        //     You must use the Contains operator for only those attributes that are enabled
        //     for full-text indexing. Otherwise, you will receive a generic SQL error message
        //     while retrieving data. In a Microsoft Dynamics 365 default installation, only
        //     the attributes of the KBArticle (article) entity are enabled for full-text indexing.
        //Contains = 49,
        //
        // Summary:
        //     The string does not contain another string. Value = 50.
        //DoesNotContain = 50,
        //
        // Summary:
        //     The value is equal to the language for the user. Value = 51.
        //EqualUserLanguage = 51,
        //
        // Summary:
        //     For internal use only.
        //NotOn = 52,
        //
        // Summary:
        //     The value is older than the specified number of months. Value = 53.
        [Description("olderthan-x-months")] OlderThanXMonths = 53,

        //
        // Summary:
        //     The string occurs at the beginning of another string. Value = 54.
        [Description("like")] BeginsWith = 54,

        //
        // Summary:
        //     The string does not begin with another string. Value = 55.
        [Description("not-like")] DoesNotBeginWith = 55,

        //
        // Summary:
        //     The string ends with another string. Value = 56.
        [Description("like")] EndsWith = 56,

        //
        // Summary:
        //     The string does not end with another string. Value = 57.
        [Description("not-like")] DoesNotEndWith = 57,

        //
        // Summary:
        //     The value is within the current fiscal year . Value = 58.
        //ThisFiscalYear = 58,
        //
        // Summary:
        //     The value is within the current fiscal period. Value = 59.
        //ThisFiscalPeriod = 59,
        //
        // Summary:
        //     The value is within the next fiscal year. Value = 60.
        //NextFiscalYear = 60,
        //
        // Summary:
        //     The value is within the next fiscal period. Value = 61.
        //NextFiscalPeriod = 61,
        //
        // Summary:
        //     The value is within the last fiscal year. Value = 62.
        //LastFiscalYear = 62,
        //
        // Summary:
        //     The value is within the last fiscal period. Value = 63.
        //LastFiscalPeriod = 63,
        //
        // Summary:
        //     The value is within the last X (specified value) fiscal periods. Value = 0x40.
        //LastXFiscalYears = 64,
        //
        // Summary:
        //     The value is within the last X (specified value) fiscal periods. Value = 65.
        //LastXFiscalPeriods = 65,
        //
        // Summary:
        //     The value is within the next X (specified value) fiscal years. Value = 66.
        //NextXFiscalYears = 66,
        //
        // Summary:
        //     The value is within the next X (specified value) fiscal period. Value = 67.
        //NextXFiscalPeriods = 67,
        //
        // Summary:
        //     The value is within the specified year. Value = 68.
        //InFiscalYear = 68,
        //
        // Summary:
        //     The value is within the specified fiscal period. Value = 69.
        //InFiscalPeriod = 69,
        //
        // Summary:
        //     The value is within the specified fiscal period and year. Value = 70.
        //InFiscalPeriodAndYear = 70,
        //
        // Summary:
        //     The value is within or before the specified fiscal period and year. Value = 71.
        //InOrBeforeFiscalPeriodAndYear = 71,
        //
        // Summary:
        //     The value is within or after the specified fiscal period and year. Value = 72.
        //InOrAfterFiscalPeriodAndYear = 72,
        //
        // Summary:
        //     The record is owned by teams that the user is a member of. Value = 73.
        [Description("eq-userteams")] EqualUserTeams = 73,

        //
        // Summary:
        //     The record is owned by a user or teams that the user is a member of. Value =
        //     74.
        [Description("eq-useroruserteams")] EqualUserOrUserTeams = 74,

        //
        // Summary:
        //     Returns all child records below the referenced record in the hierarchy. Value
        //     = 76.
        [Description("under")] Under = 75,

        //
        // Summary:
        //     Returns all records not below the referenced record in the hierarchy. Value =
        //     77.
        [Description("not-under")] NotUnder = 76,

        //
        // Summary:
        //     Returns the referenced record and all child records below it in the hierarchy.
        //     Value = 79.
        [Description("under-equal")] UnderOrEqual = 77,

        //
        // Summary:
        //     Returns all records in referenced record's hierarchical ancestry line. Value
        //     = 75..
        [Description("above")]
        Above = 78,

        //
        // Summary:
        //     Returns the referenced record and all records above it in the hierarchy. Value
        //     = 78..
        [Description("above-equal")]
        AboveOrEqual = 79,

        //
        // Summary:
        //     When hierarchical security models are used, Equals current user or their reporting
        //     hierarchy. Value = 80..
        [Description("eq-useroruserhierarchy")]
        EqualUserOrUserHierarchy = 80,

        //
        // Summary:
        //     When hierarchical security models are used, Equals current user and his teams
        //     or their reporting hierarchy and their teams. Value = 81.
        [Description("eq-useroruserhierarchyandteams")]
        EqualUserOrUserHierarchyAndTeams = 81,

        //
        [Description("olderthan-x-years")]
        OlderThanXYears = 82,

        //
        [Description("olderthan-x-weeks")]
        OlderThanXWeeks = 83,

        //
        [Description("olderthan-x-days")]
        OlderThanXDays = 84,

        //
        [Description("olderthan-x-hours")]
        OlderThanXHours = 85,

        //
        [Description("olderthan-x-minutes")]
        OlderThanXMinutes = 86
    }
}

