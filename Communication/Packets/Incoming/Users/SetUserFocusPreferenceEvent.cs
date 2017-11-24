using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Sound;
using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Users
{
    class SetUserFocusPreferenceEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            bool FocusPreference = Packet.PopBoolean();

            Session.GetHabbo().FocusPreference = FocusPreference;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `focus_preference` = @focusPreference WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("focusPreference", PlusEnvironment.BoolToEnum(FocusPreference));
                dbClient.RunQuery();
            }
        }
    }
}
