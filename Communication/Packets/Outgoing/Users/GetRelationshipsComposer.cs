using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Relationships;
using Plus.HabboHotel.Cache;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class GetRelationshipsComposer : ServerPacket
    {
        public GetRelationshipsComposer(Habbo Habbo, int Loves, int Likes, int Hates)
            : base(ServerPacketHeader.GetRelationshipsMessageComposer)
        {
            base.WriteInteger(Habbo.Id);
            base.WriteInteger(Habbo.Relationships.Count); // Count
            foreach (Relationship Rel in Habbo.Relationships.Values)
            {
                UserCache HHab = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Rel.UserId);
                if (HHab == null)
                {
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(0); // Their ID
                   base.WriteString("Placeholder");
                   base.WriteString("hr-115-42.hd-190-1.ch-215-62.lg-285-91.sh-290-62");
                }
                else
                {
                    base.WriteInteger(Rel.Type);
                    base.WriteInteger(Rel.Type == 1 ? Loves : Rel.Type == 2 ? Likes : Hates);
                    base.WriteInteger(Rel.UserId); // Their ID
                   base.WriteString(HHab.Username);
                   base.WriteString(HHab.Look);
                }
            }
        }
    }
}