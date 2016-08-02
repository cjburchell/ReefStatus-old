namespace RedPoint.ReefStatus.Common.Settings
{
    using RedPoint.ReefStatus.Common.ProfiLux;

    public interface IReefStatusSettings
    {
        ConnectionSettings Connection { get; }

        LoggingSettings Logging { get; }

        WebInterfaceSettings Web { get; }

        MailSettings Mail { get; }
    }
}