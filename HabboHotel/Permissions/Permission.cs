namespace Plus.HabboHotel.Permissions
{
    class Permission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }

        public Permission(int id, string name, string description)
        {
            Id = id;
            PermissionName = name;
            Description = description;
        }
    }
}
