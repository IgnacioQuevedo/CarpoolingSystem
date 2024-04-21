using Server.DataContext;
using Server.Objects.Domain.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Objects.Domain.UserModels;

namespace Server.Repositories
{
    public class UserRepository
    {
        public void CreateUser(User userToAdd)
        {
            LockManager.StartWriting();
            MemoryDatabase.GetInstance().Users.Add(userToAdd);
            LockManager.StopWriting();
        }

       
    }
}
