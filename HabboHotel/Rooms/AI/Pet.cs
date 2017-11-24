using System;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Pets;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;

namespace Plus.HabboHotel.Rooms.AI
{
    public class Pet
    {
        public int PetId;
        public int OwnerId;
        public int RoomId;

        public int AnyoneCanRide;
        public string Color;
        public double CreationStamp;
        public DatabaseUpdateState DBState;

        public int Energy;
        public int HairDye;
        public int Saddle;
        public string Name;
        public int Nutrition;
        public int PetHair;
        public bool PlacedInRoom;
        public string Race;
        public int Respect;

        public int Type;
        public int VirtualId;
        public int X;
        public int Y;
        public double Z;
        public int experience;

        public int[] experienceLevels = new[] { 100, 200, 400, 600, 1000, 1300, 1800, 2400, 3200, 4300, 7200, 8500, 10100, 13300, 17500, 23000, 51900, 75000, 128000, 150000 };

        public string GnomeClothing;

        public Pet(int PetId, int OwnerId, int RoomId, string Name, int Type, string Race, string Color, int experience, int Energy, int Nutrition, int Respect, double CreationStamp, int X, int Y, double Z, int Saddle, int Anyonecanride, int Dye, int PetHer, string GnomeClothing)
        {
            this.PetId = PetId;
            this.OwnerId = OwnerId;
            this.RoomId = RoomId;
            this.Name = Name;
            this.Type = Type;
            this.Race = Race;
            this.Color = Color;
            this.experience = experience;
            this.Energy = Energy;
            this.Nutrition = Nutrition;
            this.Respect = Respect;
            this.CreationStamp = CreationStamp;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.PlacedInRoom = false;
            this.DBState = DatabaseUpdateState.Updated;
            this.Saddle = Saddle;
            this.AnyoneCanRide = Anyonecanride;
            this.PetHair = PetHer;
            this.HairDye = Dye;
            this.GnomeClothing = GnomeClothing;
        }

        public void OnRespect()
        {
            Respect++;
            Room.SendPacket(new RespectPetNotificationMessageComposer(this));

            if (DBState != DatabaseUpdateState.NeedsInsert)
                DBState = DatabaseUpdateState.NeedsUpdate;

            if (experience <= 150000)
                Addexperience(10);
        }

        public void Addexperience(int Amount)
        {
            experience = experience + Amount;

            if (experience > 150000)
            {
                experience = 150000;

                if (Room != null)
                    Room.SendPacket(new AddExperiencePointsComposer(PetId, VirtualId, Amount));

                return;
            }

            if (DBState != DatabaseUpdateState.NeedsInsert)
                DBState = DatabaseUpdateState.NeedsUpdate;

            if (Room != null)
            {
                Room.SendPacket(new AddExperiencePointsComposer(PetId, VirtualId, Amount));

                if (experience >= experienceGoal)
                    Room.SendPacket(new ChatComposer(VirtualId, "*leveled up to level " + Level + " *", 0, 0));
            }
        }

        public void PetEnergy(bool Add)
        {
            int MaxE;
            if (Add)
            {
                if (Energy == 100) // If Energy is 100, no point.
                    return;

                if (Energy > 85)
                    MaxE = MaxEnergy - Energy;
                else
                    MaxE = 10;

            }
            else
                MaxE = 15; // Remove Max Energy as 15

            if (MaxE <= 4)
                MaxE = 15;

            int r = PlusEnvironment.GetRandomNumber(4, MaxE);

            if (!Add)
            {
                Energy = Energy - r;

                if (Energy < 0)
                {
                    Energy = 1;
                    r = 1;
                }
            }
            else

                Energy = Energy + r;


            if (DBState != DatabaseUpdateState.NeedsInsert)
                DBState = DatabaseUpdateState.NeedsUpdate;
        }

        public Room Room
        {
            get
            {
                if (!IsInRoom)
                    return null;

                Room _room;

                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out _room))
                    return _room;
                else
                    return null;
            
            }
        }

        public bool IsInRoom
        {
            get { return (RoomId > 0); }
        }

        public int Level
        {
            get
            {
                for (int level = 0; level < experienceLevels.Length; ++level)
                {
                    if (experience < experienceLevels[level])
                        return level + 1;
                }
                return experienceLevels.Length;
            }
        }

        public static int MaxLevel
        {
            get { return 20; }
        }

        public int experienceGoal
        {
            get
            {
                //will error index out of range (need to look into this sometime)
                return experienceLevels[Level - 1];
            }
        }

        public static int MaxEnergy
        {
            get { return 100; }
        }

        public static int MaxNutrition
        {
            get { return 150; }
        }

        public int Age
        {
            get { return Convert.ToInt32(Math.Floor((PlusEnvironment.GetUnixTimestamp() - CreationStamp) / 86400)); }
        }

        public string Look
        {
            get { return Type + " " + Race + " " + Color + " " + GnomeClothing; }
        }

        public string OwnerName
        {
            get { return PlusEnvironment.GetGame().GetClientManager().GetNameById(OwnerId); }
        }
    }

    public enum DatabaseUpdateState
    {
        Updated,
        NeedsUpdate,
        NeedsInsert
    }
}