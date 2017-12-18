using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

using Plus.Core;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.Games;
using Plus.Communication.Interfaces;
using Plus.Communication.Packets.Outgoing;


using Plus.HabboHotel.Rooms.Instance;

using Plus.HabboHotel.Items.Data.Toner;
using Plus.HabboHotel.Rooms.Games.Freeze;
using Plus.HabboHotel.Items.Data.Moodlight;

using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Session;


using Plus.HabboHotel.Rooms.Games.Football;
using Plus.HabboHotel.Rooms.Games.Banzai;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms
{
    public class Room : RoomData
    {
        public bool isCrashed;
        public bool mDisposed;
        public bool RoomMuted;
        public DateTime lastTimerReset;
        public DateTime lastRegeneration;

        public Task ProcessTask;
        public ArrayList ActiveTrades;

        public TonerData TonerData;
        public MoodlightData MoodlightData;

        public Dictionary<int, double> MutedUsers;

        private Dictionary<int, List<RoomUser>> Tents;

        public List<int> UsersWithRights;
        private GameManager _gameManager;
        private Freeze _freeze;
        private Soccer _soccer;
        private BattleBanzai _banzai;

        private Gamemap _gamemap;
        private GameItemHandler _gameItemHandler;
        
        public TeamManager teambanzai;
        public TeamManager teamfreeze;

        private RoomUserManager _roomUserManager;
        private RoomItemHandling _roomItemHandling;

        private List<string> _wordFilterList;

        private FilterComponent _filterComponent = null;
        private WiredComponent _wiredComponent = null;
        private BansComponent _bansComponent = null;
        private TradingComponent _tradingComponent = null;

        public int IsLagging { get; set; }
        public bool Unloaded { get; set; }
        public int IdleTime { get; set; }

        public Room(RoomData data)
         : base(data)
        {
            IsLagging = 0;
            Unloaded = false;
            IdleTime = 0;

            RoomMuted = false;

            MutedUsers = new Dictionary<int, double>();
            Tents = new Dictionary<int, List<RoomUser>>();

            _gamemap = new Gamemap(this, data.Model);
            _roomItemHandling = new RoomItemHandling(this);

            _roomUserManager = new RoomUserManager(this);
            _filterComponent = new FilterComponent(this);
            _wiredComponent = new WiredComponent(this);
            _bansComponent = new BansComponent(this);
            _tradingComponent = new TradingComponent(this);

            GetRoomItemHandler().LoadFurniture();
            GetGameMap().GenerateMaps();

            LoadPromotions();
            LoadRights();
            LoadFilter();
            InitBots();
            InitPets();

            lastRegeneration = DateTime.Now;
        }

        public List<string> WordFilterList
        {
            get { return this._wordFilterList; }
            set { this._wordFilterList = value; }
        }

        public int UserCount
        {
            get { return _roomUserManager.GetRoomUsers().Count; }
        }

        public int RoomId
        {
            get { return Id; }
        }

        public bool CanTradeInRoom
        {
            get { return true; }
        }

        public Gamemap GetGameMap()
        {
            return _gamemap;
        }

        public RoomItemHandling GetRoomItemHandler()
        {
            if (_roomItemHandling == null)
            {
                _roomItemHandling = new RoomItemHandling(this);
            }
            return _roomItemHandling;
        }

        public RoomUserManager GetRoomUserManager()
        {
            return _roomUserManager;
        }

        public Soccer GetSoccer()
        {
            if (_soccer == null)
                _soccer = new Soccer(this);

            return _soccer;
        }

        public TeamManager GetTeamManagerForBanzai()
        {
            if (teambanzai == null)
                teambanzai = TeamManager.createTeamforGame("banzai");
            return teambanzai;
        }

        public TeamManager GetTeamManagerForFreeze()
        {
            if (teamfreeze == null)
                teamfreeze = TeamManager.createTeamforGame("freeze");
            return teamfreeze;
        }

        public BattleBanzai GetBanzai()
        {
            if (_banzai == null)
                _banzai = new BattleBanzai(this);
            return _banzai;
        }

        public Freeze GetFreeze()
        {
            if (_freeze == null)
                _freeze = new Freeze(this);
            return _freeze;
        }

        public GameManager GetGameManager()
        {
            if (_gameManager == null)
                _gameManager = new GameManager(this);
            return _gameManager;
        }

        public GameItemHandler GetGameItemHandler()
        {
            if (_gameItemHandler == null)
                _gameItemHandler = new GameItemHandler(this);
            return _gameItemHandler;
        }

        public bool GotSoccer()
        {
            return (_soccer != null);
        }

        public bool GotBanzai()
        {
            return (_banzai != null);
        }

        public bool GotFreeze()
        {
            return (_freeze != null);
        }

        public void ClearTags()
        {
            Tags.Clear();
        }

        public void AddTagRange(List<string> tags)
        {
            Tags.AddRange(tags);
        }

        public void InitBots()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`room_id`,`name`,`motto`,`look`,`x`,`y`,`z`,`rotation`,`gender`,`user_id`,`ai_type`,`walk_mode`,`automatic_chat`,`speaking_interval`,`mix_sentences`,`chat_bubble` FROM `bots` WHERE `room_id` = '" + RoomId + "' AND `ai_type` != 'pet'");
                DataTable Data = dbClient.GetTable();
                if (Data == null)
                    return;

                foreach (DataRow Bot in Data.Rows)
                {
                    dbClient.SetQuery("SELECT `text` FROM `bots_speech` WHERE `bot_id` = '" + Convert.ToInt32(Bot["id"]) + "'");
                    DataTable BotSpeech = dbClient.GetTable();

                    List<RandomSpeech> Speeches = new List<RandomSpeech>();

                    foreach (DataRow Speech in BotSpeech.Rows)
                    {
                        Speeches.Add(new RandomSpeech(Convert.ToString(Speech["text"]), Convert.ToInt32(Bot["id"])));
                    }

                    _roomUserManager.DeployBot(new RoomBot(Convert.ToInt32(Bot["id"]), Convert.ToInt32(Bot["room_id"]), Convert.ToString(Bot["ai_type"]), Convert.ToString(Bot["walk_mode"]), Convert.ToString(Bot["name"]), Convert.ToString(Bot["motto"]), Convert.ToString(Bot["look"]), int.Parse(Bot["x"].ToString()), int.Parse(Bot["y"].ToString()), int.Parse(Bot["z"].ToString()), int.Parse(Bot["rotation"].ToString()), 0, 0, 0, 0, ref Speeches, "M", 0, Convert.ToInt32(Bot["user_id"].ToString()), Convert.ToBoolean(Bot["automatic_chat"]), Convert.ToInt32(Bot["speaking_interval"]), PlusEnvironment.EnumToBool(Bot["mix_sentences"].ToString()), Convert.ToInt32(Bot["chat_bubble"])), null);
                }
            }
        }

        public void InitPets()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`user_id`,`room_id`,`name`,`x`,`y`,`z` FROM `bots` WHERE `room_id` = '" + RoomId + "' AND `ai_type` = 'pet'");
                DataTable Data = dbClient.GetTable();

                if (Data == null)
                    return;

                foreach (DataRow Row in Data.Rows)
                {
                    dbClient.SetQuery("SELECT `type`,`race`,`color`,`experience`,`energy`,`nutrition`,`respect`,`createstamp`,`have_saddle`,`anyone_ride`,`hairdye`,`pethair`,`gnome_clothing` FROM `bots_petdata` WHERE `id` = '" + Row[0] + "' LIMIT 1");
                    DataRow mRow = dbClient.GetRow();
                    if (mRow == null)
                        continue;

                    Pet Pet = new Pet(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), Convert.ToString(Row["name"]), Convert.ToInt32(mRow["type"]), Convert.ToString(mRow["race"]),
                        Convert.ToString(mRow["color"]), Convert.ToInt32(mRow["experience"]), Convert.ToInt32(mRow["energy"]), Convert.ToInt32(mRow["nutrition"]), Convert.ToInt32(mRow["respect"]), Convert.ToDouble(mRow["createstamp"]), Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]),
                        Convert.ToDouble(Row["z"]), Convert.ToInt32(mRow["have_saddle"]), Convert.ToInt32(mRow["anyone_ride"]), Convert.ToInt32(mRow["hairdye"]), Convert.ToInt32(mRow["pethair"]), Convert.ToString(mRow["gnome_clothing"]));

                    var RndSpeechList = new List<RandomSpeech>();

                    _roomUserManager.DeployBot(new RoomBot(Pet.PetId, RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, Pet.X, Pet.Y, Convert.ToInt32(Pet.Z), 0, 0, 0, 0, 0, ref RndSpeechList, "", 0, Pet.OwnerId, false, 0, false, 0), Pet);
                }
            }
        }

        public FilterComponent GetFilter()
        {
            return this._filterComponent;
        }

        public WiredComponent GetWired()
        {
            return this._wiredComponent;
        }

        public BansComponent GetBans()
        {
            return this._bansComponent;
        }

        public TradingComponent GetTrading()
        {
            return this._tradingComponent;
        }

        public void LoadRights()
        {
            UsersWithRights = new List<int>();
            if (Group != null)
                return;

            DataTable Data = null;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT room_rights.user_id FROM room_rights WHERE room_id = @roomid");
                dbClient.AddParameter("roomid", Id);
                Data = dbClient.GetTable();
            }

            if (Data != null)
            {
                foreach (DataRow Row in Data.Rows)
                {
                    UsersWithRights.Add(Convert.ToInt32(Row["user_id"]));
                }
            }
        }

        private void LoadFilter()
        {
            this._wordFilterList = new List<string>();

            DataTable Data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_filter` WHERE `room_id` = @roomid;");
                dbClient.AddParameter("roomid", Id);
                Data = dbClient.GetTable();
            }

            if (Data == null)
                return;

            foreach (DataRow Row in Data.Rows)
            {
                this._wordFilterList.Add(Convert.ToString(Row["word"]));
            }
        }

        public bool CheckRights(GameClient Session)
        {
            return CheckRights(Session, false);
        }

        public bool CheckRights(GameClient Session, bool RequireOwnership, bool CheckForGroups = false)
        {
            try
            {
                if (Session == null || Session.GetHabbo() == null)
                    return false;

                if (Session.GetHabbo().Username == OwnerName && Type == "private")
                    return true;

                if (Session.GetHabbo().GetPermissions().HasRight("room_any_owner"))
                    return true;

                if (!RequireOwnership && Type == "private")
                {
                    if (Session.GetHabbo().GetPermissions().HasRight("room_any_rights"))
                        return true;

                    if (UsersWithRights.Contains(Session.GetHabbo().Id))
                        return true;
                }

                if (CheckForGroups && Type == "private")
                {
                    if (Group == null)
                        return false;

                    if (Group.IsAdmin(Session.GetHabbo().Id))
                        return true;

                    if (Group.AdminOnlyDeco == 0)
                    {
                        if (Group.IsAdmin(Session.GetHabbo().Id))
                            return true;
                    }
                }
            }
            catch (Exception e) { ExceptionLogger.LogException(e); }
            return false;
        }

        public void OnUserShoot(RoomUser User, Item Ball)
        {
            Func<Item, bool> predicate = null;
            string Key = null;
            foreach (Item item in this.GetRoomItemHandler().GetFurniObjects(Ball.GetX, Ball.GetY).ToList())
            {
                if (item.GetBaseItem().ItemName.StartsWith("fball_goal_"))
                {
                    Key = item.GetBaseItem().ItemName.Split(new char[] { '_' })[2];
                    User.UnIdle();
                    User.DanceId = 0;


                    PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_FootballGoalScored", 1);

                    SendPacket(new ActionComposer(User.VirtualId, 1));
                }
            }

            if (Key != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.GetBaseItem().ItemName == ("fball_score_" + Key);
                }

                foreach (Item item2 in this.GetRoomItemHandler().GetFloor.Where<Item>(predicate).ToList())
                {
                    if (item2.GetBaseItem().ItemName == ("fball_score_" + Key))
                    {
                        if (!String.IsNullOrEmpty(item2.ExtraData))
                            item2.ExtraData = (Convert.ToInt32(item2.ExtraData) + 1).ToString();
                        else
                            item2.ExtraData = "1";
                        item2.UpdateState();
                    }
                }
            }
        }

        public void ProcessRoom()
        {
            if (isCrashed || mDisposed)
                return;

            try
            {
                if (this.GetRoomUserManager().GetRoomUsers().Count == 0)
                    this.IdleTime++;
                else if (this.IdleTime > 0)
                    this.IdleTime = 0;

                if (this.HasActivePromotion && this.Promotion.HasExpired)
                {
                    this.EndPromotion();
                }

                if (this.IdleTime >= 60 && !this.HasActivePromotion)
                {
                    PlusEnvironment.GetGame().GetRoomManager().UnloadRoom(Id);
                    return;
                }

                try { GetRoomItemHandler().OnCycle(); }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e);
                }

                try { GetRoomUserManager().OnCycle(); }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e);
                }

                #region Status Updates
                try
                {
                    GetRoomUserManager().SerializeStatusUpdates();
                }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e);
                }
                #endregion

                #region Game Item Cycle
                try
                {
                    if (_gameItemHandler != null)
                        _gameItemHandler.OnCycle();
                }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e);
                }
                #endregion

                try { GetWired().OnCycle(); }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e);
                }

            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
                OnRoomCrash(e);
            }
        }

        private void OnRoomCrash(Exception e)
        {
            try
            {
                foreach (RoomUser user in _roomUserManager.GetRoomUsers().ToList())
                {
                    if (user == null || user.GetClient() == null)
                        continue;

                    user.GetClient().SendNotification("Sorry, it appears that room has crashed!");//Unhandled exception in room: " + e);

                    try
                    {
                        GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, false);
                    }
                    catch (Exception e2)
                    {
                        ExceptionLogger.LogException(e2); }
                }
            }
            catch (Exception e3)
            {
                ExceptionLogger.LogException(e3);
            }

            isCrashed = true;
            PlusEnvironment.GetGame().GetRoomManager().UnloadRoom(Id);
        }


        public bool CheckMute(GameClient Session)
        {
            if (MutedUsers.ContainsKey(Session.GetHabbo().Id))
            {
                if (MutedUsers[Session.GetHabbo().Id] < PlusEnvironment.GetUnixTimestamp())
                {
                    MutedUsers.Remove(Session.GetHabbo().Id);
                }
                else
                {
                    return true;
                }
            }

            if (Session.GetHabbo().TimeMuted > 0 || (RoomMuted && Session.GetHabbo().Username != OwnerName))
                return true;

            return false;
        }

        public void SendObjects(GameClient Session)
        {
            Room Room = Session.GetHabbo().CurrentRoom;

            Session.SendPacket(new HeightMapComposer(Room.GetGameMap().Model.Heightmap));
            Session.SendPacket(new FloorHeightMapComposer(Room.GetGameMap().Model.GetRelativeHeightmap(), Room.GetGameMap().StaticModel.WallHeight));

            foreach (RoomUser RoomUser in _roomUserManager.GetUserList().ToList())
            {
                if (RoomUser == null)
                    continue;

                Session.SendPacket(new UsersComposer(RoomUser));

                if (RoomUser.IsBot && RoomUser.BotData.DanceId > 0)
                    Session.SendPacket(new DanceComposer(RoomUser, RoomUser.BotData.DanceId));
                else if (!RoomUser.IsBot && !RoomUser.IsPet && RoomUser.IsDancing)
                    Session.SendPacket(new DanceComposer(RoomUser, RoomUser.DanceId));

                if (RoomUser.IsAsleep)
                    Session.SendPacket(new SleepComposer(RoomUser, true));

                if (RoomUser.CarryItemID > 0 && RoomUser.CarryTimer > 0)
                    Session.SendPacket(new CarryObjectComposer(RoomUser.VirtualId, RoomUser.CarryItemID));

                if (!RoomUser.IsBot && !RoomUser.IsPet && RoomUser.CurrentEffect > 0)
                    Session.SendPacket(new AvatarEffectComposer(RoomUser.VirtualId, RoomUser.CurrentEffect));
            }

            Session.SendPacket(new UserUpdateComposer(_roomUserManager.GetUserList().ToList()));
            Session.SendPacket(new ObjectsComposer(Room.GetRoomItemHandler().GetFloor.ToArray(), this));
            Session.SendPacket(new ItemsComposer(Room.GetRoomItemHandler().GetWall.ToArray(), this));
        }

        #region Tents
        public void AddTent(int TentId)
        {
            if (Tents.ContainsKey(TentId))
                Tents.Remove(TentId);

            Tents.Add(TentId, new List<RoomUser>());
        }

        public void RemoveTent(int TentId)
        {
            if (!Tents.ContainsKey(TentId))
                return;

            List<RoomUser> Users = Tents[TentId];
            foreach (RoomUser User in Users.ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    continue;

                User.GetClient().GetHabbo().TentId = 0;
            }

            if (Tents.ContainsKey(TentId))
                Tents.Remove(TentId);
        }

        public void AddUserToTent(int TentId, RoomUser User)
        {
            if (User != null && User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                if (!Tents.ContainsKey(TentId))
                    Tents.Add(TentId, new List<RoomUser>());

                if (!Tents[TentId].Contains(User))
                    Tents[TentId].Add(User);
                User.GetClient().GetHabbo().TentId = TentId;
            }
        }

        public void RemoveUserFromTent(int TentId, RoomUser User)
        {
            if (User != null && User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                if (!Tents.ContainsKey(TentId))
                    Tents.Add(TentId, new List<RoomUser>());

                if (Tents[TentId].Contains(User))
                    Tents[TentId].Remove(User);

                User.GetClient().GetHabbo().TentId = 0;
            }
        }

        public void SendToTent(int Id, int TentId, IServerPacket Packet)
        {
            if (!Tents.ContainsKey(TentId))
                return;

            foreach (RoomUser User in Tents[TentId].ToList())
            {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().GetIgnores().IgnoredUserIds().Contains(Id) || User.GetClient().GetHabbo().TentId != TentId)
                    continue;

                User.GetClient().SendPacket(Packet);
            }
        }
        #endregion

        #region Communication (Packets)
        public void SendPacket(IServerPacket packet, bool withRightsOnly = false)
        {
            if (packet == null)
                return;

            try
            {

                List<RoomUser> users = this._roomUserManager.GetUserList().ToList();

                if (this._roomUserManager == null || users == null)
                    return;

                foreach (RoomUser user in users)
                {
                    if (user == null || user.IsBot)
                        continue;

                    if (user.GetClient() == null || user.GetClient().GetConnection() == null)
                        continue;

                    if (withRightsOnly && !this.CheckRights(user.GetClient()))
                        continue;

                    user.GetClient().SendPacket(packet);
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }

        public void BroadcastPacket(byte[] packet)
        {
            foreach (RoomUser user in this._roomUserManager.GetUserList().ToList())
            {
                if (user == null || user.IsBot)
                    continue;

                if (user.GetClient() == null || user.GetClient().GetConnection() == null)
                    continue;

                user.GetClient().GetConnection().SendData(packet);
            }
        }

        public void SendPacket(List<ServerPacket> packets)
        {
            if (packets.Count == 0)
                return;

            try
            {
                byte[] TotalBytes = new byte[0];
                int Current = 0;

                foreach (ServerPacket packet in packets.ToList())
                {
                    byte[] ToAdd = packet.GetBytes();
                    int NewLen = TotalBytes.Length + ToAdd.Length;

                    Array.Resize(ref TotalBytes, NewLen);

                    for (int i = 0; i < ToAdd.Length; i++)
                    {
                        TotalBytes[Current] = ToAdd[i];
                        Current++;
                    }
                }

                this.BroadcastPacket(TotalBytes);
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }
        }
        #endregion

        public void Dispose()
        {
            SendPacket(new CloseConnectionComposer());

            if (!mDisposed)
            {
                isCrashed = false;
                mDisposed = true;

                /* TODO: Needs reviewing */
                try
                {
                    if (ProcessTask != null && ProcessTask.IsCompleted)
                        ProcessTask.Dispose();
                }
                catch { }

                if (this.ActiveTrades.Count > 0)
                    this.ActiveTrades.Clear();

                this.TonerData = null;
                this.MoodlightData = null;

                if (this.MutedUsers.Count > 0)
                    this.MutedUsers.Clear();

                if (this.Tents.Count > 0)
                    this.Tents.Clear();

                if (this.UsersWithRights.Count > 0)
                    this.UsersWithRights.Clear();

                if (this._gameManager != null)
                {
                    this._gameManager.Dispose();
                    this._gameManager = null;
                }

                if (this._freeze != null)
                {
                    this._freeze.Dispose();
                    this._freeze = null;
                }

                if (this._soccer != null)
                {
                    this._soccer.Dispose();
                    this._soccer = null;
                }

                if (this._banzai != null)
                {
                    this._banzai.Dispose();
                    this._banzai = null;
                }

                if (this._gamemap != null)
                {
                    this._gamemap.Dispose();
                    this._gamemap = null;
                }

                if (this._gameItemHandler != null)
                {
                    this._gameItemHandler.Dispose();
                    this._gameItemHandler = null;
                }

                // Room Data?

                if (this.teambanzai != null)
                {
                    this.teambanzai.Dispose();
                    this.teambanzai = null;
                }

                if (this.teamfreeze != null)
                {
                    this.teamfreeze.Dispose();
                    this.teamfreeze = null;
                }

                if (this._roomUserManager != null)
                {
                    this._roomUserManager.Dispose();
                    this._roomUserManager = null;
                }

                if (this._roomItemHandling != null)
                {
                    this._roomItemHandling.Dispose();
                    this._roomItemHandling = null;
                }

                if (this._wordFilterList.Count > 0)
                    this._wordFilterList.Clear();

                if (this._filterComponent != null)
                    this._filterComponent.Cleanup();

                if (this._wiredComponent != null)
                    this._wiredComponent.Cleanup();

                if (this._bansComponent != null)
                    this._bansComponent.Cleanup();

                if (this._tradingComponent != null)
                    this._tradingComponent.Cleanup();
            }
        }
    }
}