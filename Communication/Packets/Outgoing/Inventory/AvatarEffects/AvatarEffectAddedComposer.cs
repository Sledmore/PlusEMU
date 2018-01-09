namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectAddedComposer : ServerPacket
    {
        public AvatarEffectAddedComposer(int SpriteId, int Duration)
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
            WriteInteger(SpriteId);
            WriteInteger(0);//Types
            WriteInteger(Duration);
            WriteBoolean(false);//Permanent
        }
    }
}
