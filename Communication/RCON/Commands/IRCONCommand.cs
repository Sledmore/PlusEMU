namespace Plus.Communication.Rcon.Commands
{
    public interface IRconCommand
    {
        string Parameters { get; }
        string Description { get; }
        bool TryExecute(string[] parameters);
    }
}