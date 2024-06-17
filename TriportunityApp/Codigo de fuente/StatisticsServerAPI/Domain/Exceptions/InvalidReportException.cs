namespace StatisticsServerAPI.Domain.Exceptions;

public class InvalidReportException : Exception
{
    public InvalidReportException(string message) : base(message)
    {
    }
}