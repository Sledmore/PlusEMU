namespace Plus.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollContentsComposer : ServerPacket
    {
        public PollContentsComposer()
            : base(3826)
        {
            base.WriteInteger(111141);//Room Id
           base.WriteString("Customer Satisfaction Poll");//Title
           base.WriteString("Thanks!");//Ending message
            base.WriteInteger(2);//Questions
            {
                base.WriteInteger(9299);
                base.WriteInteger(1);
                base.WriteInteger(1);
               base.WriteString("Would you recommend Habbo to a friend?");
                base.WriteInteger(0);
                base.WriteInteger(1);
                base.WriteInteger(5);
               base.WriteString("1");
               base.WriteString("Definitely");
                base.WriteInteger(1);
               base.WriteString("2");
               base.WriteString("Maybe");
                base.WriteInteger(2);
               base.WriteString("3");
               base.WriteString("Donâ€™t know");
                base.WriteInteger(3);
               base.WriteString("4");
               base.WriteString("Probably not");
                base.WriteInteger(3);
               base.WriteString("5");
               base.WriteString("Definitely not");
                base.WriteInteger(3);
                base.WriteInteger(3);
                base.WriteInteger(9117);
                base.WriteInteger(2);
                base.WriteInteger(1);
               base.WriteString("What was the primary reason for the score you just gave us?");
                base.WriteInteger(1);
                base.WriteInteger(1);
                base.WriteInteger(6);
               base.WriteString("1");
               base.WriteString("Friends");
                base.WriteInteger(0);
               base.WriteString("2");
               base.WriteString("Avatar clothes and looks");
                base.WriteInteger(0);
               base.WriteString("3");
               base.WriteString("Pets");
                base.WriteInteger(0);
               base.WriteString("4");
               base.WriteString("Room building");
                base.WriteInteger(0);
               base.WriteString("5");
               base.WriteString("Room games");
                base.WriteInteger(0);
               base.WriteString("6");
               base.WriteString("Other");
                base.WriteInteger(0);
                base.WriteInteger(9342);
                base.WriteInteger(3);
                base.WriteInteger(1);
               base.WriteString("What was the primary reason for the score you just gave us?");
                base.WriteInteger(2);
                base.WriteInteger(1);
                base.WriteInteger(6);
               base.WriteString("1");
               base.WriteString("Friends");
                base.WriteInteger(0);
               base.WriteString("2");
               base.WriteString("Avatar clothes and looks");
                base.WriteInteger(0);
               base.WriteString("3");
               base.WriteString("Pets");
                base.WriteInteger(0);
               base.WriteString("4");
               base.WriteString("Room building");
                base.WriteInteger(0);
               base.WriteString("5");
               base.WriteString("Room games");
                base.WriteInteger(0);
               base.WriteString("6");
               base.WriteString("Other");
                base.WriteInteger(0);
                base.WriteInteger(9336);
                base.WriteInteger(4);
                base.WriteInteger(1);
               base.WriteString("What was the primary reason for the score you just gave us?");
                base.WriteInteger(3);
                base.WriteInteger(1);
                base.WriteInteger(5);
               base.WriteString("1");
               base.WriteString("Moderation");
                base.WriteInteger(0);
               base.WriteString("2");
               base.WriteString("Prices");
                base.WriteInteger(0);
               base.WriteString("3");
               base.WriteString("Bullying");
                base.WriteInteger(0);
               base.WriteString("4");
               base.WriteString("Cybering");
                base.WriteInteger(0);
               base.WriteString("5");
               base.WriteString("Other");
                base.WriteInteger(0);
                base.WriteInteger(9120);
                base.WriteInteger(2);
                base.WriteInteger(3);
               base.WriteString(" What is the most important improvement that would make you more likely to recommend us?");
                base.WriteInteger(0);
                base.WriteInteger(3);
                base.WriteInteger(0);
                base.WriteInteger(0);
            }
            base.WriteBoolean(true);

        }
    }
}
