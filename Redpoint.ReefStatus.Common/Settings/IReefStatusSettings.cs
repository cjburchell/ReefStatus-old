namespace RedPoint.ReefStatus.Common.Settings
{
    public interface IReefStatusSettings
    {
        ConnectionSettings Connection { get; }

        LoggingSettings Logging { get; }

        MailSettings Mail { get; }
    }
}