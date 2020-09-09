using DotNetty.Buffers;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.Communication.Packets.Outgoing
{
    public abstract class MessageComposer
    {
        protected short Id { get; }

        public MessageComposer(short Id)
        {
            this.Id = Id;
        }

        public ServerPacket WriteMessage(IByteBuffer buf)
        {
            ServerPacket packet = new ServerPacket(Id, buf);
            try
            {
                this.Compose(packet);
            } finally
            {
                this.Dispose();
            }
            return packet;
        }

        public abstract void Compose(ServerPacket packet);

        public void Dispose()
        {

        }
    }
}
