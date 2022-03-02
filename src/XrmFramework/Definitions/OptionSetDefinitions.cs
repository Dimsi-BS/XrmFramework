using System.ComponentModel;
using XrmFramework;
using XrmFramework.Definitions.Internal;

namespace XrmFramework.Definitions
{


    [OptionSetDefinition("componentstate")]
    public enum EtatDuComposant
    {
        [Description("Publié")]
        Publie = 0,
        [Description("Non publié")]
        NonPublie = 1,
        [Description("Supprimé")]
        Supprime = 2,
        [Description("Non publié supprimé")]
        NonPublieSupprime = 3,
    }

    [OptionSetDefinition(CustomapiDefinition.EntityName, CustomapiDefinition.Columns.BindingType)]
    public enum BindingType
    {
        [Description("Global")]
        Global = 0,
        [Description("Entity")]
        Entity = 1,
        [Description("Entity Collection")]
        EntityCollection = 2,
    }

    [OptionSetDefinition(SdkMessageProcessingStepDefinition.EntityName, SdkMessageProcessingStepDefinition.Columns.Mode)]
    [DefinitionManagerIgnore]
    public enum Mode
    {
        [Description("Synchrone")]
        Synchrone = 0,
        [Description("Asynchrone")]
        Asynchrone = 1,
    }

    [OptionSetDefinition(SdkMessageProcessingStepDefinition.EntityName, SdkMessageProcessingStepDefinition.Columns.Stage)]
    public enum Phase
    {
        Null = 0,
        [Description("Validation préalable")]
        ValidationPrealable = 10,
        [Description("Opération antérieure interne avant les plug-ins externes (à usage interne uniquement)")]
        OperationAnterieureInterneAvantLesPlugInsExternesAUsageInterneUniquement = 15,
        [Description("Opération préalable")]
        OperationPrealable = 20,
        [Description("Opération antérieure interne après les plug-ins externes (à usage interne uniquement)")]
        OperationAnterieureInterneApresLesPlugInsExternesAUsageInterneUniquement = 25,
        [Description("Opération principale (à usage interne uniquement)")]
        OperationPrincipaleAUsageInterneUniquement = 30,
        [Description("Opération postérieure interne avant les plug-ins externes (à usage interne uniquement)")]
        OperationPosterieureInterneAvantLesPlugInsExternesAUsageInterneUniquement = 35,
        [Description("Opération postérieure")]
        OperationPosterieure = 40,
        [Description("Opération postérieure interne après les plug-ins externes (à usage interne uniquement)")]
        OperationPosterieureInterneApresLesPlugInsExternesAUsageInterneUniquement = 45,
        [Description("Opération antérieure initiale (à usage interne uniquement)")]
        OperationAnterieureInitialeAUsageInterneUniquement = 5,
        [Description("Opération postérieure (déconseillé)")]
        OperationPosterieureDeconseille = 50,
        [Description("Opération postérieure finale (à usage interne uniquement)")]
        OperationPosterieureFinaleAUsageInterneUniquement = 55,
        [Description("Phase de pré-validation déclenchée avant la validation de la transaction (utilisation interne uniquement)")]
        PhaseDePreValidationDeclencheeAvantLaValidationDeLaTransactionUtilisationInterneUniquement = 80,
        [Description("Phase de post-validation déclenchée après la validation de la transaction (utilisation interne uniquement)")]
        PhaseDePostValidationDeclencheeApresLaValidationDeLaTransactionUtilisationInterneUniquement = 90,
    }

    [OptionSetDefinition(SystemUserDefinition.EntityName, SystemUserDefinition.Columns.AccessMode)]
    public enum ModeDAcces
    {
        [Description("Lecture-écriture")]
        LectureEcriture = 0,
        [Description("Administrateur")]
        Administrateur = 1,
        [Description("Lecture")]
        Lecture = 2,
        [Description("Utilisateur de support")]
        UtilisateurDeSupport = 3,
        [Description("Non interactif")]
        NonInteractif = 4,
        [Description("Administrateur délégué")]
        AdministrateurDelegue = 5,
    }


    [OptionSetDefinition(TransactioncurrencyDefinition.EntityName, TransactioncurrencyDefinition.Columns.StateCode)]
    public enum TransactioncurrencyState
    {
        [Description("Actif")]
        Actif = 0,
        [Description("Inactif")]
        Inactif = 1,
    }

    [OptionSetDefinition(TransactioncurrencyDefinition.EntityName, TransactioncurrencyDefinition.Columns.StatusCode)]
    public enum TransactioncurrencyStatus
    {
        [Description("Actif")]
        Actif = 1,
        [Description("Inactif")]
        Inactif = 2,
    }

    [OptionSetDefinition(WebresourceDefinition.EntityName, WebresourceDefinition.Columns.WebResourceType)]
    [DefinitionManagerIgnore]
    public enum WebResourceType
    {
        Null = 0,
        [Description("Page Web (HTML)")]
        PageWebHTML = 1,
        [Description("Format ICO")]
        FormatICO = 10,
        [Description("Format vectoriel (SVG)")]
        FormatVectorielSVG = 11,
        [Description("Chaîne (RESX)")]
        ChaineRESX = 12,
        [Description("Feuille de style (CSS)")]
        FeuilleDeStyleCSS = 2,
        [Description("Script (JScript)")]
        ScriptJscript = 3,
        [Description("Données (XML)")]
        DonneesXML = 4,
        [Description("Format PNG")]
        FormatPNG = 5,
        [Description("Format JPG")]
        FormatJPG = 6,
        [Description("Format GIF")]
        FormatGIF = 7,
        [Description("Silverlight (XAP)")]
        SilverlightXAP = 8,
        [Description("Feuille de style (XSL)")]
        FeuilleDeStyleXSL = 9,
    }
}
