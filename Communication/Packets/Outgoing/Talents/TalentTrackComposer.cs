using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Talents;

namespace Plus.Communication.Packets.Outgoing.Talents
{
    class TalentTrackComposer : ServerPacket
    {
        public TalentTrackComposer(ICollection<TalentTrackLevel> levels, string type)
            : base(ServerPacketHeader.TalentTrackMessageComposer)
        {
            base.WriteString(type);
            base.WriteInteger(levels.Count);

            foreach (TalentTrackLevel level in levels.ToList())
            {
                base.WriteInteger(level.Level);//First level
                base.WriteInteger(0);//Progress, 0 = nothing, 1 = started, 2 = done

                base.WriteInteger(level.GetSubLevels().Count);
                foreach (TalentTrackSubLevel sub in level.GetSubLevels())
                {
                    base.WriteInteger(0);//Achievement Id
                    base.WriteInteger(0);//Achievement level
                    base.WriteString(sub.Badge);//Achievement name
                    base.WriteInteger(0);//Progress, 0 = nothing, 1 = started, 2 = done
                    base.WriteInteger(0);//My actual progress
                    base.WriteInteger(sub.RequiredProgress);
                }

                base.WriteInteger(level.Actions.Count);
                foreach (string action in level.Actions.ToList())
                {
                    base.WriteString(action);
                }

                base.WriteInteger(level.Gifts.Count);
                foreach (string gift in level.Gifts.ToList())
                {
                    base.WriteString(gift);
                    base.WriteInteger(0);
                }
            }

            /*base.WriteString("citizenship");
            base.WriteInteger(5);
            {
                {
                    base.WriteInteger(0);//First level
                    base.WriteInteger(2);//Progress, 0 = nothing, 1 = started, 2 = done
                    base.WriteInteger(1);
                    {
                        base.WriteInteger(125);
                        base.WriteInteger(1);
                        base.WriteString("ACH_SafetyQuizGraduate1");
                        base.WriteInteger(2);//Progress, 0 = nothing, 1 = started, 2 = done
                        base.WriteInteger(0);
                        base.WriteInteger(1);
                    }

                    base.WriteInteger(0);//umm??
                    {

                    }

                    base.WriteInteger(1);//Gift?
                    {
                        base.WriteString("A1 KUMIANKKA");
                        base.WriteInteger(0);
                    }
                }

                {
                    base.WriteInteger(1);//Second level
                    base.WriteInteger(1);
                    base.WriteInteger(4);//4 loops
                                         //1
                    {
                        base.WriteInteger(6);
                        base.WriteInteger(1);
                        base.WriteString("ACH_AvatarLooks1");
                        base.WriteInteger(1);
                        base.WriteInteger(0);
                        base.WriteInteger(1);
                    }
                    //2
                    {
                        base.WriteInteger(18);
                        base.WriteInteger(1);
                        base.WriteString("ACH_RespectGiven1");
                        base.WriteInteger(1);
                        base.WriteInteger(0);
                        base.WriteInteger(2);
                    }
                    //3
                    {
                        base.WriteInteger(19);
                        base.WriteInteger(1);
                        base.WriteString("ACH_AllTimeHotelPresence1");//badge name
                        base.WriteInteger(1);//Progress
                        base.WriteInteger(50);//Current progress?
                        base.WriteInteger(60);//Required progress
                    }
                    //4
                    {
                        base.WriteInteger(8);
                        base.WriteInteger(1);
                        base.WriteString("ACH_RoomEntry1");
                        base.WriteInteger(0);
                        base.WriteInteger(0);
                        base.WriteInteger(5);
                    }

                    base.WriteInteger(0);//count
                    {

                    }

                    base.WriteInteger(1);//1 loop
                    {
                        base.WriteString("A1 KUMIANKKA");
                        base.WriteInteger(0);
                    }

                    base.WriteInteger(2);
                    base.WriteInteger(0);
                }

                {
                    base.WriteInteger(3);//Third
                    base.WriteInteger(11);
                    base.WriteInteger(1);
                    base.WriteString("ACH_RegistrationDuration1");
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(1);
                    base.WriteInteger(19);
                    base.WriteInteger(2);
                    base.WriteString("ACH_AllTimeHotelPresence2");
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(60);
                    base.WriteInteger(8);
                    base.WriteInteger(2);
                    base.WriteString("ACH_RoomEntry2");
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(20);
                    base.WriteInteger(0);
                    base.WriteInteger(1);
                    base.WriteString("A1 KUMIANKKA");
                    base.WriteInteger(0);
                    base.WriteInteger(3);
                    base.WriteInteger(0);
                }

                {
                    base.WriteInteger(4);//Forth
                    base.WriteInteger(11);
                    base.WriteInteger(2);
                    base.WriteString("ACH_RegistrationDuration2");
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(3);
                    base.WriteInteger(94);
                    base.WriteInteger(1);
                    base.WriteString("ACH_HabboWayGraduate1");
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(1);
                    base.WriteInteger(19);
                    base.WriteInteger(3);
                    base.WriteString("ACH_AllTimeHotelPresence3");
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(120);
                    base.WriteInteger(381);
                    base.WriteInteger(1);
                    base.WriteString("ACH_FriendListSize1");
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(2);
                    base.WriteInteger(1);
                    base.WriteString("TRADE");
                    base.WriteInteger(1);
                    base.WriteString("A1 KUMIANKKA");
                    base.WriteInteger(0);
                }

                {
                    base.WriteInteger(4);//Final
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteInteger(1);
                    base.WriteString("CITIZEN");
                    base.WriteInteger(2);
                    base.WriteString("A1 KUMIANKKA");
                    base.WriteInteger(0);
                    base.WriteString("pixel_walldeco");
                    base.WriteInteger(0);
                }
            }*/
        }
    }
}