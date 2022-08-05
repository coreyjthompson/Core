namespace MEI.Logging
{
    public interface ICorrelationProvider
    {
        string GetCorrelationId();
    }
}
