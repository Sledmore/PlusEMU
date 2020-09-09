namespace Plus.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollContentsComposer : MessageComposer
    {
        public PollContentsComposer()
            : base(3826)
        {

        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(111141);//Room Id
            packet.WriteString("Customer Satisfaction Poll");//Title
            packet.WriteString("Thanks!");//Ending message
            packet.WriteInteger(2);//Questions
            {
                packet.WriteInteger(9299);
                packet.WriteInteger(1);
                packet.WriteInteger(1);
                packet.WriteString("Would you recommend Habbo to a friend?");
                packet.WriteInteger(0);
                packet.WriteInteger(1);
                packet.WriteInteger(5);
                packet.WriteString("1");
                packet.WriteString("Definitely");
                packet.WriteInteger(1);
                packet.WriteString("2");
                packet.WriteString("Maybe");
                packet.WriteInteger(2);
                packet.WriteString("3");
                packet.WriteString("Donâ€™t know");
                packet.WriteInteger(3);
                packet.WriteString("4");
                packet.WriteString("Probably not");
                packet.WriteInteger(3);
                packet.WriteString("5");
                packet.WriteString("Definitely not");
                packet.WriteInteger(3);
                packet.WriteInteger(3);
                packet.WriteInteger(9117);
                packet.WriteInteger(2);
                packet.WriteInteger(1);
                packet.WriteString("What was the primary reason for the score you just gave us?");
                packet.WriteInteger(1);
                packet.WriteInteger(1);
                packet.WriteInteger(6);
                packet.WriteString("1");
                packet.WriteString("Friends");
                packet.WriteInteger(0);
                packet.WriteString("2");
                packet.WriteString("Avatar clothes and looks");
                packet.WriteInteger(0);
                packet.WriteString("3");
                packet.WriteString("Pets");
                packet.WriteInteger(0);
                packet.WriteString("4");
                packet.WriteString("Room building");
                packet.WriteInteger(0);
                packet.WriteString("5");
                packet.WriteString("Room games");
                packet.WriteInteger(0);
                packet.WriteString("6");
                packet.WriteString("Other");
                packet.WriteInteger(0);
                packet.WriteInteger(9342);
                packet.WriteInteger(3);
                packet.WriteInteger(1);
                packet.WriteString("What was the primary reason for the score you just gave us?");
                packet.WriteInteger(2);
                packet.WriteInteger(1);
                packet.WriteInteger(6);
                packet.WriteString("1");
                packet.WriteString("Friends");
                packet.WriteInteger(0);
                packet.WriteString("2");
                packet.WriteString("Avatar clothes and looks");
                packet.WriteInteger(0);
                packet.WriteString("3");
                packet.WriteString("Pets");
                packet.WriteInteger(0);
                packet.WriteString("4");
                packet.WriteString("Room building");
                packet.WriteInteger(0);
                packet.WriteString("5");
                packet.WriteString("Room games");
                packet.WriteInteger(0);
                packet.WriteString("6");
                packet.WriteString("Other");
                packet.WriteInteger(0);
                packet.WriteInteger(9336);
                packet.WriteInteger(4);
                packet.WriteInteger(1);
                packet.WriteString("What was the primary reason for the score you just gave us?");
                packet.WriteInteger(3);
                packet.WriteInteger(1);
                packet.WriteInteger(5);
                packet.WriteString("1");
                packet.WriteString("Moderation");
                packet.WriteInteger(0);
                packet.WriteString("2");
                packet.WriteString("Prices");
                packet.WriteInteger(0);
                packet.WriteString("3");
                packet.WriteString("Bullying");
                packet.WriteInteger(0);
                packet.WriteString("4");
                packet.WriteString("Cybering");
                packet.WriteInteger(0);
                packet.WriteString("5");
                packet.WriteString("Other");
                packet.WriteInteger(0);
                packet.WriteInteger(9120);
                packet.WriteInteger(2);
                packet.WriteInteger(3);
                packet.WriteString(" What is the most important improvement that would make you more likely to recommend us?");
                packet.WriteInteger(0);
                packet.WriteInteger(3);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
            }
            packet.WriteBoolean(true);
        }
    }
}
