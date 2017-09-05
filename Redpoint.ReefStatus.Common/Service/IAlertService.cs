namespace RedPoint.ReefStatus.Common.Service
{
    public interface IAlertService
    {
        void SendStatusEmail(string message);

        void CheckAlarm();
    }
}