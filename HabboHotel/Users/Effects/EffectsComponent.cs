using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Users.Effects
{
    public sealed class EffectsComponent
    {
        private Habbo _habbo;
        private int _currentEffect;

        /// <summary>
        /// Effects stored by ID > Effect.
        /// </summary>
        private readonly ConcurrentDictionary<int, AvatarEffect> _effects = new ConcurrentDictionary<int, AvatarEffect>();

        public EffectsComponent()
        {
        }

        /// <summary>
        /// Initializes the EffectsComponent.
        /// </summary>
        public bool Init(Habbo habbo)
        {
            if (_effects.Count > 0)
                return false;
            
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_effects` WHERE `user_id` = @id;");
                dbClient.AddParameter("id", habbo.Id);
                DataTable getEffects = dbClient.GetTable();

                if (getEffects != null)
                {
                    foreach (DataRow Row in getEffects.Rows)
                    {
                        if (_effects.TryAdd(Convert.ToInt32(Row["id"]), new AvatarEffect(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["effect_id"]), Convert.ToDouble(Row["total_duration"]), PlusEnvironment.EnumToBool(Row["is_activated"].ToString()), Convert.ToDouble(Row["activated_stamp"]), Convert.ToInt32(Row["quantity"]))))
                        {
                            //umm?
                        }
                    }
                }
            }

            _habbo = habbo;
            _currentEffect = 0;
            return true;
        }

        public bool TryAdd(AvatarEffect Effect)
        {
            return _effects.TryAdd(Effect.Id, Effect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpriteId"></param>
        /// <param name="ActivatedOnly"></param>
        /// <param name="UnactivatedOnly"></param>
        /// <returns></returns>
        public bool HasEffect(int SpriteId, bool ActivatedOnly = false, bool UnactivatedOnly = false)
        {
            return (GetEffectNullable(SpriteId, ActivatedOnly, UnactivatedOnly) != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpriteId"></param>
        /// <param name="ActivatedOnly"></param>
        /// <param name="UnactivatedOnly"></param>
        /// <returns></returns>
        public AvatarEffect GetEffectNullable(int SpriteId, bool ActivatedOnly = false, bool UnactivatedOnly = false)
        {
            foreach (AvatarEffect Effect in _effects.Values.ToList())
            {
                if (!Effect.HasExpired && Effect.SpriteId == SpriteId && (!ActivatedOnly || Effect.Activated) && (!UnactivatedOnly || !Effect.Activated))
                {
                    return Effect;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Habbo"></param>
        public void CheckEffectExpiry(Habbo Habbo)
        {
            foreach (AvatarEffect Effect in _effects.Values.ToList())
            {
                if (Effect.HasExpired)
                {
                    Effect.HandleExpiration(Habbo);
                }
            }
        }

        public void ApplyEffect(int EffectId)
        {
            if (_habbo == null || _habbo.CurrentRoom == null)
                return;

            RoomUser User = _habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(_habbo.Id);
            if (User == null)
                return;

            _currentEffect = EffectId;

            if (User.IsDancing)
                _habbo.CurrentRoom.SendPacket(new DanceComposer(User.VirtualId, 0));
            _habbo.CurrentRoom.SendPacket(new AvatarEffectComposer(User.VirtualId, EffectId));
        }

        public ICollection<AvatarEffect> GetAllEffects
        {
            get { return _effects.Values; }
        }

        public int CurrentEffect
        {
            get { return _currentEffect; }
            set { _currentEffect = value; }
        }

        /// <summary>
        /// Disposes the EffectsComponent.
        /// </summary>
        public void Dispose()
        {
            _effects.Clear();
        }
    }
}
