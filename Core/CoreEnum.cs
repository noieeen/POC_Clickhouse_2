using System.ComponentModel;

namespace Core
{
    public enum CoreDatabaseTypeEnum
    {
        Core = 1,
        Config = 2,
        Data = 3
    }

    public enum CoreDatabaseDbTypeEnum
    {
        [Description("azure-sql")]
        AzureSql = 1,
        [Description("azure-dw")]
        AzureDataWarehouse = 2
    }

    public enum AlertLevel
    {
        Info,
        Warning,
        Error
    }

    public enum Lang
    {
        EN,
        TH
    }
}
