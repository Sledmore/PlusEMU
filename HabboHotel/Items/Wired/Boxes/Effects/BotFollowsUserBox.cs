using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotFollowsUserBox: IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectBotFollowsUserBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotFollowsUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int FollowMode = Packet.PopInt();//1 = follow, 0 = don't.
            string BotConfiguration = Packet.PopString();
       
            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            this.StringData = FollowMode + ";" +BotConfiguration;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(this.StringData))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            RoomUser Human = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
            if (Human == null)
                return false;

            string[] Stuff = this.StringData.Split(';');
            if (Stuff.Length != 2)
                return false;//This is important, incase a cunt scripts.

            string Username = Stuff[1];

            RoomUser User = this.Instance.GetRoomUserManager().GetBotByName(Username);
            if (User == null)
                return false;

            int FollowMode = 0;
            if (!int.TryParse(Stuff[0], out FollowMode))
                return false;

            if (FollowMode == 0)
            {
                User.BotData.ForcedUserTargetMovement = 0;

                if (User.IsWalking)
                    User.ClearMovement(true);
            }
            else if (FollowMode == 1)
            {
                User.BotData.ForcedUserTargetMovement = Player.Id;

                if (User.IsWalking)
                    User.ClearMovement(true);
                User.MoveTo(Human.X, Human.Y);
            }

            return true;
        }
    }
}