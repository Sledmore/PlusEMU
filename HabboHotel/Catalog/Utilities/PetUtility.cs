using System;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.AI;

namespace Plus.HabboHotel.Catalog.Utilities
{
    public static class PetUtility
    {
        public static bool CheckPetName(string PetName)
        {
            if (PetName.Length < 1 || PetName.Length > 16)
                return false;

            if (!PlusEnvironment.IsValidAlphaNumeric(PetName))
                return false;

            return true;
        }

        public static Pet CreatePet(int UserId, string Name, int Type, string Race, string Color)
        {
            Pet pet = new Pet(0, UserId, 0, Name, Type, Race, Color, 0, 100, 100, 0, PlusEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 0, 0, -1, "-1");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO bots (user_id,name, ai_type) VALUES (" + pet.OwnerId + ",@" + pet.PetId + "name, 'pet')");
                dbClient.AddParameter(pet.PetId + "name", pet.Name);
                pet.PetId = Convert.ToInt32(dbClient.InsertQuery());

                dbClient.SetQuery("INSERT INTO bots_petdata (id,type,race,color,experience,energy,createstamp) VALUES (" + pet.PetId + ", " + pet.Type + ",@" + pet.PetId + "race,@" + pet.PetId + "color,0,100,UNIX_TIMESTAMP())");
                dbClient.AddParameter(pet.PetId + "race", pet.Race);
                dbClient.AddParameter(pet.PetId + "color", pet.Color);
                dbClient.RunQuery();
            }
            return pet;
        }
    }
}
