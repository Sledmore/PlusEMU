using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using System;
using System.Collections.Concurrent;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class BotGivesHandItemBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectBotGivesHanditemBox; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public BotGivesHandItemBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int DrinkID = Packet.PopInt();
            string BotName = Packet.PopString();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            this.StringData = BotName.ToString() + ";" + DrinkID.ToString();
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(this.StringData))
                return false;

            Habbo Player = (Habbo)Params[0];

            if(Player == null)
                return false;

            RoomUser Actor = this.Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);

            if(Actor == null)
                return false;

            RoomUser User = this.Instance.GetRoomUserManager().GetBotByName(this.StringData.Split(';')[0]);

            if (User == null)
                return false;

            if (User.BotData.TargetUser == 0)
            {
                if (!Instance.GetGameMap().CanWalk(Actor.SquareBehind.X, Actor.SquareBehind.Y, false))
                    return false;

                string[] Data = this.StringData.Split(';');

                int DrinkId;

                if (!int.TryParse(Data[1], out DrinkId))
                    return false;

                User.CarryItem(DrinkId);
                User.BotData.TargetUser = Actor.HabboId;

                User.MoveTo(Actor.SquareBehind.X, Actor.SquareBehind.Y);
            }

            return true;
        }
    }
}
