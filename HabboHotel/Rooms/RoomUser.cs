using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.AI;

using Plus.HabboHotel.Rooms.Games.Freeze;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Rooms.PathFinding;

namespace Plus.HabboHotel.Rooms
{
    public class RoomUser
    {
        public bool AllowOverride;
        public BotAI BotAI;
        public RoomBot BotData;
        public bool CanWalk;
        public int CarryItemId; //byte
        public int CarryTimer; //byte
        public int ChatSpamCount = 0;
        public int ChatSpamTicks = 16;
        public ItemEffectType CurrentItemEffect;
        public int DanceId;
        public bool FastWalking = false;
        public bool SuperFastWalking = false;
        public int FreezeCounter;
        public int FreezeLives;
        public bool Freezed;
        public bool Frozen;
        public int GateId;

        public int GoalX; //byte
        public int GoalY; //byte
        public int HabboId;
        public int HorseID = 0;
        public int IdleTime; //byte
        public bool InteractingGate;
        public int InternalRoomID;
        public bool IsAsleep;
        public bool IsWalking;
        public int LastBubble = 0;
        public double LastInteraction;
        public Item LastItem = null;
        public int LockedTilesCount;

        public List<Vector2D> Path = new List<Vector2D>();
        public bool PathRecalcNeeded = false;
        public int PathStep = 1;
        public Pet PetData;

        public int PrevTime;
        public bool RidingHorse = false;
        public int RoomId;
        public int RotBody; //byte
        public int RotHead; //byte

        public bool SetStep;
        public int SetX; //byte
        public int SetY; //byte
        public double SetZ;
        public double SignTime;
        public byte SqState;
        private Dictionary<string, string> _statusses;
        public int TeleDelay; //byte
        public bool TeleportEnabled;
        public bool UpdateNeeded;
        public int VirtualId;

        public int X; //byte
        public int Y; //byte
        public double Z;

        public FreezePowerUp banzaiPowerUp;
        public bool isLying = false;
        public bool isSitting = false;
        private GameClient mClient;
        private Room mRoom;
        public bool moonwalkEnabled = false;
        public bool shieldActive;
        public int shieldCounter;
        public Team Team;
        public bool FreezeInteracting;
        public int UserId;
        public bool IsJumping;

        public bool isRolling = false;
        public int rollerDelay = 0;

        public int LLPartner = 0;
        public double TimeInRoom = 0;

        private bool _trading = false;
        private int _tradePartner = 0;
        private int _tradeId = 0;

        public RoomUser(int HabboId, int RoomId, int VirtualId, Room room)
        {
            Freezed = false;
            this.HabboId = HabboId;
            this.RoomId = RoomId;
            this.VirtualId = VirtualId;
            IdleTime = 0;

            X = 0;
            Y = 0;
            Z = 0;
            PrevTime = 0;
            RotHead = 0;
            RotBody = 0;
            UpdateNeeded = true;
            _statusses = new Dictionary<string, string>();

            TeleDelay = -1;
            mRoom = room;

            AllowOverride = false;
            CanWalk = true;


            SqState = 3;

            InternalRoomID = 0;
            CurrentItemEffect = ItemEffectType.None;

            FreezeLives = 0;
            InteractingGate = false;
            GateId = 0;
            LastInteraction = 0;
            LockedTilesCount = 0;

            IsJumping = false;
            TimeInRoom = 0;

            _tradeId = 0;
            _tradePartner = 0;
            _trading = false;
        }


        public Point Coordinate
        {
            get { return new Point(X, Y); }
        }

        public bool IsPet
        {
            get { return (IsBot && BotData.IsPet); }
        }

        public int CurrentEffect
        {
            get { return GetClient().GetHabbo().Effects().CurrentEffect; }
        }


        public bool IsDancing
        {
            get
            {
                if (DanceId >= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsTrading
        {
            get { return _trading; }
            set { _trading = value; }
        }

        public int TradePartner
        {
            get { return _tradePartner; }
            set { _tradePartner = value; }
        }

        public int TradeId
        {
            get { return _tradeId; }
            set { _tradeId = value; }
        }


        public Dictionary<string, string> Statusses
        {
            get { return _statusses; }
            //set { this._statusses = value; }
        }


        public bool NeedsAutokick
        {
            get
            {
                if (IsBot)
                    return false;

                if (GetClient() == null || GetClient().GetHabbo() == null)
                    return true;

                if (GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") || GetRoom().OwnerId == HabboId)
                    return false;

                if (GetRoom().Id == 1649919)
                    return false;

                if (IdleTime >= 7200)
                    return true;

                return false;
            }
        }

        public bool IsBot
        {
            get
            {
                if (BotData != null)
                    return true;

                return false;
            }
        }

        public string GetUsername()
        {
            if (IsBot)
                return string.Empty;

            if (GetClient() != null)
            {
                if (GetClient().GetHabbo() != null)
                {
                    return GetClient().GetHabbo().Username;
                }
                else
                    return PlusEnvironment.GetUsernameById(HabboId);

            }
            else
                return PlusEnvironment.GetUsernameById(HabboId);
        }

        public void UnIdle()
        {
            if (!IsBot)
            {
                if (GetClient() != null && GetClient().GetHabbo() != null)
                    GetClient().GetHabbo().TimeAFK = 0;
            }

            IdleTime = 0;

            if (IsAsleep)
            {
                IsAsleep = false;
                GetRoom().SendPacket(new SleepComposer(this, false));
            }
        }

        public void Dispose()
        {
            Statusses.Clear();
            mRoom = null;
            mClient = null;
        }

        public void Chat(string Message, int colour = 0)
        {
            if (GetRoom() == null)
                return;

            if (!IsBot)
                return;


            if (IsPet)
            {
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetUserList().ToList())
                {
                    if (User == null || User.IsBot)
                        continue;

                    if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                        return;

                    if (!User.GetClient().GetHabbo().AllowPetSpeech)
                        User.GetClient().SendPacket(new ChatComposer(VirtualId, Message, 0, 0));
                }
            }
            else
            {
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetUserList().ToList())
                {
                    if (User == null || User.IsBot)
                        continue;

                    if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                        return;

                    if (!User.GetClient().GetHabbo().AllowBotSpeech)
                        User.GetClient().SendPacket(new ChatComposer(VirtualId, Message, 0, (colour == 0 ? 2 : colour)));
                }
            }
        }

        public void HandleSpamTicks()
        {
            if (ChatSpamTicks >= 0)
            {
                ChatSpamTicks--;

                if (ChatSpamTicks == -1)
                {
                    ChatSpamCount = 0;
                }
            }
        }

        public bool IncrementAndCheckFlood(out int MuteTime)
        {
            MuteTime = 0;

            ChatSpamCount++;
            if (ChatSpamTicks == -1)
                ChatSpamTicks = 8;
            else if (ChatSpamCount >= 6)
            {
                if (GetClient().GetHabbo().GetPermissions().HasRight("events_staff"))
                    MuteTime = 3;
                else if (GetClient().GetHabbo().GetPermissions().HasRight("gold_vip"))
                    MuteTime = 7;
                else if (GetClient().GetHabbo().GetPermissions().HasRight("silver_vip"))
                    MuteTime = 10;
                else
                    MuteTime = 20;

                GetClient().GetHabbo().FloodTime = PlusEnvironment.GetUnixTimestamp() + MuteTime;

                ChatSpamCount = 0;
                return true;
            }
            return false;
        }

        public void OnChat(int Colour, string Message, bool Shout)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null || mRoom == null)
                return;

            if (mRoom.GetWired().TriggerEvent(Items.Wired.WiredBoxType.TriggerUserSays, GetClient().GetHabbo(), Message))
                return;


            GetClient().GetHabbo().HasSpoken = true;

            if (mRoom.WordFilterList.Count > 0 && !GetClient().GetHabbo().GetPermissions().HasRight("word_filter_override"))
            {
                Message = mRoom.GetFilter().CheckMessage(Message);
            }

            ServerPacket Packet = null;
            if (Shout)
                Packet = new ShoutComposer(VirtualId, Message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message), Colour);
            else
                Packet = new ChatComposer(VirtualId, Message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message), Colour);


            if (GetClient().GetHabbo().TentId > 0)
            {
                mRoom.SendToTent(GetClient().GetHabbo().Id, GetClient().GetHabbo().TentId, Packet);

                Packet = new WhisperComposer(VirtualId, "[Tent Chat] " + Message, 0, Colour);

                List<RoomUser> ToNotify = mRoom.GetRoomUserManager().GetRoomUserByRank(2);

                if (ToNotify.Count > 0)
                {
                    foreach (RoomUser user in ToNotify)
                    {
                        if (user == null || user.GetClient() == null || user.GetClient().GetHabbo() == null ||
                            user.GetClient().GetHabbo().TentId == GetClient().GetHabbo().TentId)
                        {
                            continue;
                        }

                        user.GetClient().SendPacket(Packet);
                    }
                }
            }
            else
            {
                foreach (RoomUser User in mRoom.GetRoomUserManager().GetRoomUsers().ToList())
                {
                    if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().GetIgnores().IgnoredUserIds().Contains(mClient.GetHabbo().Id))
                        continue;

                    if (mRoom.ChatDistance > 0 && Gamemap.TileDistance(X, Y, User.X, User.Y) > mRoom.ChatDistance)
                        continue;

                    User.GetClient().SendPacket(Packet);
                }
            }

            #region Pets/Bots responces
            if (Shout)
            {
                foreach (RoomUser User in mRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (!User.IsBot)
                        continue;

                    if (User.IsBot)
                        User.BotAI.OnUserShout(this, Message);
                }
            }
            else
            {
                foreach (RoomUser User in mRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (!User.IsBot)
                        continue;

                    if (User.IsBot)
                        User.BotAI.OnUserSay(this, Message);
                }
            }
            #endregion

        }

        public void ClearMovement(bool Update)
        {
            IsWalking = false;
            Statusses.Remove("mv");
            GoalX = 0;
            GoalY = 0;
            SetStep = false;
            SetX = 0;
            SetY = 0;
            SetZ = 0;

            if (Update)
            {
                UpdateNeeded = true;
            }
        }

        public void MoveTo(Point c)
        {
            MoveTo(c.X, c.Y);
        }

        public void MoveTo(int pX, int pY, bool pOverride)
        {
            if (TeleportEnabled)
            {
                UnIdle();
                GetRoom().SendPacket(GetRoom().GetRoomItemHandler().UpdateUserOnRoller(this, new Point(pX, pY), 0, GetRoom().GetGameMap().SqAbsoluteHeight(GoalX, GoalY)));
                if (Statusses.ContainsKey("sit"))
                    Z -= 0.35;
                UpdateNeeded = true;
                return;
            }

            if ((GetRoom().GetGameMap().SquareHasUsers(pX, pY) && !pOverride) || Frozen)
                return;

            UnIdle();

            GoalX = pX;
            GoalY = pY;
            PathRecalcNeeded = true;
            FreezeInteracting = false;
        }

        public void MoveTo(int pX, int pY)
        {
            MoveTo(pX, pY, false);
        }

        public void UnlockWalking()
        {
            AllowOverride = false;
            CanWalk = true;
        }


        public void SetPos(int pX, int pY, double pZ)
        {
            X = pX;
            Y = pY;
            Z = pZ;
        }

        public void CarryItem(int Item)
        {
            CarryItemId = Item;

            if (Item > 0)
                CarryTimer = 240;
            else
                CarryTimer = 0;

            GetRoom().SendPacket(new CarryObjectComposer(VirtualId, Item));
        }


        public void SetRot(int Rotation, bool HeadOnly)
        {
            if (Statusses.ContainsKey("lay") || IsWalking)
            {
                return;
            }

            int diff = RotBody - Rotation;

            RotHead = RotBody;

            if (Statusses.ContainsKey("sit") || HeadOnly)
            {
                if (RotBody == 2 || RotBody == 4)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
                else if (RotBody == 0 || RotBody == 6)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
            }
            else if (diff <= -2 || diff >= 2)
            {
                RotHead = Rotation;
                RotBody = Rotation;
            }
            else
            {
                RotHead = Rotation;
            }

            UpdateNeeded = true;
        }


        public bool HasStatus(string Key)
        {
            return _statusses.ContainsKey(Key);
        }

        public void RemoveStatus(string Key)
        {
            if (HasStatus(Key))
                _statusses.Remove(Key);
        }

        public void SetStatus(string Key, string Value = "")
        {
            if (_statusses.ContainsKey(Key))
            {
                _statusses[Key] = Value;
            }
            else
            {
                _statusses.Add(Key, Value);
            }
        }


        public void ApplyEffect(int effectID)
        {
            if (IsBot)
            {
                mRoom.SendPacket(new AvatarEffectComposer(VirtualId, effectID));
                return;
            }

            if (IsBot || GetClient() == null || GetClient().GetHabbo() == null || GetClient().GetHabbo().Effects() == null)
                return;

            GetClient().GetHabbo().Effects().ApplyEffect(effectID);
        }

        public Point SquareInFront
        {
            get
            {
                var Sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    Sq.Y--;
                }
                else if (RotBody == 2)
                {
                    Sq.X++;
                }
                else if (RotBody == 4)
                {
                    Sq.Y++;
                }
                else if (RotBody == 6)
                {
                    Sq.X--;
                }

                return Sq;
            }
        }

        public Point SquareBehind
        {
            get
            {
                var Sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    Sq.Y++;
                }
                else if (RotBody == 2)
                {
                    Sq.X--;
                }
                else if (RotBody == 4)
                {
                    Sq.Y--;
                }
                else if (RotBody == 6)
                {
                    Sq.X++;
                }

                return Sq;
            }
        }

        public Point SquareLeft
        {
            get
            {
                var Sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    Sq.X++;
                }
                else if (RotBody == 2)
                {
                    Sq.Y--;
                }
                else if (RotBody == 4)
                {
                    Sq.X--;
                }
                else if (RotBody == 6)
                {
                    Sq.Y++;
                }

                return Sq;
            }
        }

        public Point SquareRight
        {
            get
            {
                var Sq = new Point(X, Y);

                if (RotBody == 0)
                {
                    Sq.X--;
                }
                else if (RotBody == 2)
                {
                    Sq.Y++;
                }
                else if (RotBody == 4)
                {
                    Sq.X++;
                }
                else if (RotBody == 6)
                {
                    Sq.Y--;
                }
                return Sq;
            }
        }


        public GameClient GetClient()
        {
            if (IsBot)
            {
                return null;
            }
            if (mClient == null)
                mClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(HabboId);
            return mClient;
        }

        private Room GetRoom()
        {
            if (mRoom == null)
                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out mRoom))
                    return mRoom;

            return mRoom;
        }
    }
}