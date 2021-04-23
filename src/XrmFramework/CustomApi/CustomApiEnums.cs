namespace XrmFramework
{

    public enum CustomApiBindingType
    {
        Global = 0,
        Entity = 1,
        EntityCollection = 2
    }

    public enum AllowedCustomProcessingStep
    {
        None = 0,
        AsyncOnly = 1,
        SyncAndAsync = 2
    }

    public enum CustomApiArgumentType
    {
        Boolean = 0,
        DateTime = 1,
        Decimal = 2,
        Entity = 3,
        EntityCollection = 4,
        EntityReference = 5,
        Float = 6,
        Integer = 7,
        Money = 8,
        Picklist = 9,
        String = 10,
        StringArray = 11,
        Guid = 12
    }
}
