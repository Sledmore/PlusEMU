namespace Plus.Communication.Packets.Outgoing.Rooms.Polls
{
    class PollContentsComposer : ServerPacket
    {
        public PollContentsComposer()
            : base(3826)
        {
            WriteInteger(111141);//Room Id
           WriteString("Customer Satisfaction Poll");//Title
           WriteString("Thanks!");//Ending message
            WriteInteger(2);//Questions
            {
                WriteInteger(9299);
                WriteInteger(1);
                WriteInteger(1);
               WriteString("Would you recommend Habbo to a friend?");
                WriteInteger(0);
                WriteInteger(1);
                WriteInteger(5);
               WriteString("1");
               WriteString("Definitely");
                WriteInteger(1);
               WriteString("2");
               WriteString("Maybe");
                WriteInteger(2);
               WriteString("3");
               WriteString("Donâ€™t know");
                WriteInteger(3);
               WriteString("4");
               WriteString("Probably not");
                WriteInteger(3);
               WriteString("5");
               WriteString("Definitely not");
                WriteInteger(3);
                WriteInteger(3);
                WriteInteger(9117);
                WriteInteger(2);
                WriteInteger(1);
               WriteString("What was the primary reason for the score you just gave us?");
                WriteInteger(1);
                WriteInteger(1);
                WriteInteger(6);
               WriteString("1");
               WriteString("Friends");
                WriteInteger(0);
               WriteString("2");
               WriteString("Avatar clothes and looks");
                WriteInteger(0);
               WriteString("3");
               WriteString("Pets");
                WriteInteger(0);
               WriteString("4");
               WriteString("Room building");
                WriteInteger(0);
               WriteString("5");
               WriteString("Room games");
                WriteInteger(0);
               WriteString("6");
               WriteString("Other");
                WriteInteger(0);
                WriteInteger(9342);
                WriteInteger(3);
                WriteInteger(1);
               WriteString("What was the primary reason for the score you just gave us?");
                WriteInteger(2);
                WriteInteger(1);
                WriteInteger(6);
               WriteString("1");
               WriteString("Friends");
                WriteInteger(0);
               WriteString("2");
               WriteString("Avatar clothes and looks");
                WriteInteger(0);
               WriteString("3");
               WriteString("Pets");
                WriteInteger(0);
               WriteString("4");
               WriteString("Room building");
                WriteInteger(0);
               WriteString("5");
               WriteString("Room games");
                WriteInteger(0);
               WriteString("6");
               WriteString("Other");
                WriteInteger(0);
                WriteInteger(9336);
                WriteInteger(4);
                WriteInteger(1);
               WriteString("What was the primary reason for the score you just gave us?");
                WriteInteger(3);
                WriteInteger(1);
                WriteInteger(5);
               WriteString("1");
               WriteString("Moderation");
                WriteInteger(0);
               WriteString("2");
               WriteString("Prices");
                WriteInteger(0);
               WriteString("3");
               WriteString("Bullying");
                WriteInteger(0);
               WriteString("4");
               WriteString("Cybering");
                WriteInteger(0);
               WriteString("5");
               WriteString("Other");
                WriteInteger(0);
                WriteInteger(9120);
                WriteInteger(2);
                WriteInteger(3);
               WriteString(" What is the most important improvement that would make you more likely to recommend us?");
                WriteInteger(0);
                WriteInteger(3);
                WriteInteger(0);
                WriteInteger(0);
            }
            WriteBoolean(true);

        }
    }
}
