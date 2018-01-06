using System.Linq;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Relationships;
using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Users
{
    class GetRelationshipsComposer : ServerPacket
    {
        public GetRelationshipsComposer(Habbo habbo)
            : base(ServerPacketHeader.GetRelationshipsMessageComposer)
        {
            WriteInteger(habbo.Id);
            WriteInteger(habbo.Relationships.Count); // Count
            foreach (Relationship relationship in habbo.Relationships.Values)
            {
                UserCache relation = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(relationship.UserId);
                if (relation == null)
                {
                    WriteInteger(0);
                    WriteInteger(0);
                    WriteInteger(0); // Their ID
                    WriteString("Placeholder");
                    WriteString("hr-115-42.hd-190-1.ch-215-62.lg-285-91.sh-290-62");
                }
                else
                {
                    WriteInteger(relationship.Type);
                    WriteInteger(habbo.Relationships.Count(x => x.Value.Type == relationship.Type));
                    WriteInteger(relationship.UserId); // Their ID
                    WriteString(relation.Username);
                    WriteString(relation.Look);
                }
            }
        }
    }
}