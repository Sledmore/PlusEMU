namespace Plus.HabboHotel.Rooms.Chat.Pets.Commands
{
    public class PetCommand
    {
        public int Id;
        public string Input;

        public PetCommand(int CommandId, string CommandInput)
        {
            Id = CommandId;
            Input = CommandInput;
        }
    }
}