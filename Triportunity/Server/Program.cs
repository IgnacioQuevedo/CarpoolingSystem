using Server.DataContext;


namespace Server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //For bringing up the db just use this method:
            MemoryDatabase database = MemoryDatabase.GetInstance();
        }
    }
}