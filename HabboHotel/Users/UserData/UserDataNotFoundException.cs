using System;

namespace Plus.HabboHotel.Users.UserData
{
    public class UserDataNotFoundException : Exception
    {
        public UserDataNotFoundException(string reason)
            : base(reason)
        {
        }
    }
}