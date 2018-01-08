using System.Collections;
using System.Collections.Concurrent;

using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    class KickUserBox : IWiredItem, IWiredCycle
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectKickUser; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public int TickCount { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public int Delay { get; set; }
        public string ItemsData { get; set; }
        private Queue _toKick;

        public KickUserBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
            this.TickCount = Delay;
            this._toKick = new Queue();

            if (this.SetItems.Count > 0)
                this.SetItems.Clear();
        }

        public void HandleSave(ClientPacket Packet)
        {
            if (this.SetItems.Count > 0)
                this.SetItems.Clear();

            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();

            this.StringData = Message;
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length != 1)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            if (this.TickCount <= 0)
                this.TickCount = 3;

            if (!this._toKick.Contains(Player))
            {
                RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(Player.Id);
                if (User == null)
                    return false;

                if (Player.GetPermissions().HasRight("mod_tool")  || this.Instance.OwnerId == Player.Id)
                {
                    Player.GetClient().SendPacket(new WhisperComposer(User.VirtualId, "Wired Kick Exception: Unkickable Player", 0, 0));
                    return false;
                }

                this._toKick.Enqueue(Player);
                Player.GetClient().SendPacket(new WhisperComposer(User.VirtualId, this.StringData, 0, 0));
            }
            return true;
        }

        public bool OnCycle()
        {
            if (Instance == null)
                return false;

            if (this._toKick.Count == 0)
            {
                this.TickCount = 3;
                return true;
            }

            lock (this._toKick.SyncRoot)
            {
                while (this._toKick.Count > 0)
                {
                    Habbo Player = (Habbo)this._toKick.Dequeue();
                    if (Player == null || !Player.InRoom || Player.CurrentRoom != Instance)
                        continue;

                    Instance.GetRoomUserManager().RemoveUserFromRoom(Player.GetClient(), true, false);
                }
            }
            this.TickCount = 3;
            return true;
        }
    }
}