using System;

namespace Plus.HabboHotel.Rooms.Chat.Pets.Commands
{
    public class PetCommand
    {
        public int Id;
        public string Input;

        public PetCommand(int CommandId, string CommandInput)
        {
            this.Id = CommandId;
            this.Input = CommandInput;
        }
    }
}