using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Sound
{
    class SoundSettingsComposer : MessageComposer
    {
        public IEnumerable<int> Volumes { get; }
        public bool ChatPreference { get; }
        public bool InviteStatus { get; }
        public bool FocusPreference { get; }
        public int FriendBarState { get; }
        public SoundSettingsComposer(IEnumerable<int> volumes, bool chatPreference, bool invitesStatus, bool focusPreference, int friendBarState)
            : base(ServerPacketHeader.SoundSettingsMessageComposer)
        {
            this.Volumes = volumes;
            this.ChatPreference = chatPreference;
            this.InviteStatus = invitesStatus;
            this.FocusPreference = focusPreference;
            this.FriendBarState = friendBarState;
        }

        public override void Compose(ServerPacket packet)
        {
            foreach (int volume in Volumes)
            {
                packet.WriteInteger(volume);
            }

            packet.WriteBoolean(ChatPreference);
            packet.WriteBoolean(InviteStatus);
            packet.WriteBoolean(FocusPreference);
            packet.WriteInteger(FriendBarState);
            packet.WriteInteger(0);
            packet.WriteInteger(0);
        }
    }
}