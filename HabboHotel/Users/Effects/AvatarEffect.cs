using Plus.Utilities;


using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Users.Effects
{
    public sealed class AvatarEffect
    {
        private int _id;
        private int _userId;
        private int _spriteId;
        private double _duration;
        private bool _activated;
        private double _timestampActivated;
        private int _quantity;

        public AvatarEffect(int Id, int UserId, int SpriteId, double Duration, bool Activated, double TimestampActivated, int Quantity)
        {
            this.Id = Id;
            this.UserId = UserId;
            this.SpriteId = SpriteId;
            this.Duration = Duration;
            this.Activated = Activated;
            this.TimestampActivated = TimestampActivated;
            this.Quantity = Quantity;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public int SpriteId
        {
            get { return _spriteId; }
            set { _spriteId = value; }
        }

        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public bool Activated
        {
            get { return _activated; }
            set { _activated = value; }
        }

        public double TimestampActivated
        {
            get { return _timestampActivated; }
            set { _timestampActivated = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public double TimeUsed
        {
            get
            {
                return (UnixTimestamp.GetNow() - _timestampActivated);
            }
        }

        public double TimeLeft
        {
            get
            {
                double tl = (_activated ? _duration - TimeUsed : _duration);

                if (tl < 0)
                {
                    tl = 0;
                }

                return tl;
            }
        }

        public bool HasExpired
        {
            get
            {
                return (_activated && TimeLeft <= 0);
            }
        }

        /// <summary>
        /// Activates the AvatarEffect
        /// </summary>
        public bool Activate()
        {
            double TsNow = UnixTimestamp.GetNow();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_effects` SET `is_activated` = '1', `activated_stamp` = @ts WHERE `id` = @id");
                dbClient.AddParameter("ts", TsNow);
                dbClient.AddParameter("id", Id);
                dbClient.RunQuery();

                _activated = true;
                _timestampActivated = TsNow;
                return true;
            }
        }

        public void HandleExpiration(Habbo Habbo)
        {
            _quantity--;

            _activated = false;
            _timestampActivated = 0;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (_quantity < 1)
                {
                    dbClient.SetQuery("DELETE FROM `user_effects` WHERE `id` = @id");
                    dbClient.AddParameter("id", Id);
                    dbClient.RunQuery();
                }
                else
                {
                    dbClient.SetQuery("UPDATE `user_effects` SET `quantity` = @qt, `is_activated` = '0', `activated_stamp` = 0 WHERE `id` = @id");
                    dbClient.AddParameter("qt", Quantity);
                    dbClient.AddParameter("id", Id);
                    dbClient.RunQuery();
                }
            }

            Habbo.GetClient().SendPacket(new AvatarEffectExpiredComposer(this));
            // reset fx if in room?
        }

        public void AddToQuantity()
        {
            _quantity++;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_effects` SET `quantity` = @qt WHERE `id` = @id");
                dbClient.AddParameter("qt", Quantity);
                dbClient.AddParameter("id", Id);
                dbClient.RunQuery();
            }
        }
    }
}