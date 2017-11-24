using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.Communication.Packets.Incoming;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class AddActorToTeamBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectAddActorToTeam; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public AddActorToTeamBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;

            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Team = Packet.PopInt();

            this.StringData = Team.ToString();
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0 || Instance == null || String.IsNullOrEmpty(this.StringData))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (User == null)
                return false;

            TEAM ToJoin = (int.Parse(this.StringData) == 1 ? TEAM.RED : int.Parse(this.StringData) == 2 ? TEAM.GREEN : int.Parse(this.StringData) == 3 ? TEAM.BLUE : int.Parse(this.StringData) == 4 ? TEAM.YELLOW : TEAM.NONE);

            TeamManager Team = Instance.GetTeamManagerForFreeze();
            if (Team != null)
            {
                if (Team.CanEnterOnTeam(ToJoin))
                {
                    if (User.Team != TEAM.NONE)
                        Team.OnUserLeave(User);

                    User.Team = ToJoin;
                    Team.AddUser(User);

                    if (User.GetClient().GetHabbo().Effects().CurrentEffect != Convert.ToInt32(ToJoin + 39))
                        User.GetClient().GetHabbo().Effects().ApplyEffect(Convert.ToInt32(ToJoin + 39));
                }
            }
            return true;
        }
    }
}