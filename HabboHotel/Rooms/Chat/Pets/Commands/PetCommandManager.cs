using System;
using System.Data;
using System.Collections.Generic;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Rooms.Chat.Pets.Commands
{
    public class PetCommandManager
    {
        private Dictionary<int, string> _commandRegister;
        private Dictionary<string, string> _commandDatabase;
        private Dictionary<string, PetCommand> _petCommands;

        public PetCommandManager()
        {
            this._petCommands = new Dictionary<string, PetCommand>();
            this._commandRegister = new Dictionary<int, string>();
            this._commandDatabase = new Dictionary<string, string>();

            this.Init();
        }

        public void Init()
        {
            this._petCommands.Clear();
            this._commandRegister.Clear();
            this._commandDatabase.Clear();

            DataTable Table = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `bots_pet_commands`");
                Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow row in Table.Rows)
                    {
                        _commandRegister.Add(Convert.ToInt32(row[0]), row[1].ToString());
                        _commandDatabase.Add(row[1] + ".input", row[2].ToString());
                    }
                }
            }

            foreach (var pair in _commandRegister)
            {
                int commandID = pair.Key;
                string commandStringedID = pair.Value;
                string[] commandInput = this._commandDatabase[commandStringedID + ".input"].Split(',');

                foreach (string command in commandInput)
                {
                    this._petCommands.Add(command, new PetCommand(commandID, command));
                }
            }
        }

        public int TryInvoke(string Input)
        {
            PetCommand Command = null;
            if (this._petCommands.TryGetValue(Input.ToLower(), out Command))
                return Command.Id;
            return 0;
        }
    }
}