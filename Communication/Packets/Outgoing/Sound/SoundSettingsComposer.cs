using System;
using System.Collections.Generic;

namespace Plus.Communication.Packets.Outgoing.Sound
{
    class SoundSettingsComposer : ServerPacket
    {
        public SoundSettingsComposer(ICollection<int> volumes, bool chatPreference, bool invitesStatus, bool focusPreference, int friendBarState)
            : base(ServerPacketHeader.SoundSettingsMessageComposer)
        {
            foreach (int volume in volumes)
            {
                base.WriteInteger(volume);
            }

            base.WriteBoolean(chatPreference);
            base.WriteBoolean(invitesStatus);
            base.WriteBoolean(focusPreference);
            base.WriteInteger(friendBarState);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}