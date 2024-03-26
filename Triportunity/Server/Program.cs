using System;
using System.Linq;
using Server.DataContext;
using Server.Objects.Domain.ClientModels;

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