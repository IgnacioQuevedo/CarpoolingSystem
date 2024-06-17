namespace Common;

public class MomConstraints
{
    public static string userQueueName = "users_queue_statistics";
    public static string exchangeName = "statisticsLogs";
    public static string userQueueRoutingKey = "users";
    
    public static string rideQueueName = "ride_queue_statistics";
    public static string rideQueueRoutingKey = "rides";
}