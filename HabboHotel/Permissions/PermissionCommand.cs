namespace Plus.HabboHotel.Permissions
{
    class PermissionCommand
    {
        public string Command { get; private set; }
        public int GroupId { get; private set; }
        public int SubscriptionId { get; private set; }

        public PermissionCommand(string command, int groupId, int subscriptionId)
        {
            Command = command;
            GroupId = groupId;
            SubscriptionId = subscriptionId;
        }
    }
}