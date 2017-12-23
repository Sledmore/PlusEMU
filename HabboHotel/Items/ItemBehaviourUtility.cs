using System;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items.Data.Toner;

using Plus.Communication.Packets.Outgoing;
using Plus.HabboHotel.Cache.Type;

namespace Plus.HabboHotel.Items
{
    static class ItemBehaviourUtility
    {
        public static void GenerateExtradata(Item item, ServerPacket packet)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                default:
                    packet.WriteInteger(1);
                    packet.WriteInteger(0);
                    packet.WriteString(item.GetBaseItem().InteractionType != InteractionType.FOOTBALL_GATE ? item.ExtraData : string.Empty);
                    break;

                case InteractionType.GNOME_BOX:
                    packet.WriteInteger(0);
                    packet.WriteInteger(0);
                    packet.WriteString("");
                    break;

                case InteractionType.PET_BREEDING_BOX:
                case InteractionType.PURCHASABLE_CLOTHING:
                    packet.WriteInteger(0);
                    packet.WriteInteger(0);
                    packet.WriteString("0");
                    break;

                case InteractionType.STACKTOOL:
                    packet.WriteInteger(0);
                    packet.WriteInteger(0);
                    packet.WriteString("");
                    break;

                case InteractionType.WALLPAPER:
                    packet.WriteInteger(2);
                    packet.WriteInteger(0);
                    packet.WriteString(item.ExtraData);

                    break;
                case InteractionType.FLOOR:
                    packet.WriteInteger(3);
                    packet.WriteInteger(0);
                    packet.WriteString(item.ExtraData);
                    break;

                case InteractionType.LANDSCAPE:
                    packet.WriteInteger(4);
                    packet.WriteInteger(0);
                    packet.WriteString(item.ExtraData);
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                case InteractionType.GUILD_FORUM:
                    Group Group = null;
                    if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(item.GroupId, out Group))
                    {
                        packet.WriteInteger(1);
                        packet.WriteInteger(0);
                        packet.WriteString(item.ExtraData);
                    }
                    else
                    {
                        packet.WriteInteger(0);
                        packet.WriteInteger(2);
                        packet.WriteInteger(5);
                        packet.WriteString(item.ExtraData);
                        packet.WriteString(Group.Id.ToString());
                        packet.WriteString(Group.Badge);
                        packet.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour1, true));
                        packet.WriteString(PlusEnvironment.GetGame().GetGroupManager().GetColourCode(Group.Colour2, false));
                    }
                    break;

                case InteractionType.BACKGROUND:
                    packet.WriteInteger(0);
                    packet.WriteInteger(1);
                    if (!String.IsNullOrEmpty(item.ExtraData))
                    {
                        packet.WriteInteger(item.ExtraData.Split(Convert.ToChar(9)).Length / 2);

                        for (int i = 0; i <= item.ExtraData.Split(Convert.ToChar(9)).Length - 1; i++)
                        {
                            packet.WriteString(item.ExtraData.Split(Convert.ToChar(9))[i]);
                        }
                    }
                    else
                    {
                        packet.WriteInteger(0);
                    }
                    break;

                case InteractionType.GIFT:
                    {
                        string[] ExtraData = item.ExtraData.Split(Convert.ToChar(5));
                        if (ExtraData.Length != 7)
                        {
                            packet.WriteInteger(0);
                            packet.WriteInteger(0);
                            packet.WriteString(item.ExtraData);
                        }
                        else
                        {
                            int Style = int.Parse(ExtraData[6]) * 1000 + int.Parse(ExtraData[6]);

                            UserCache Purchaser = PlusEnvironment.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(ExtraData[2]));
                            if (Purchaser == null)
                            {
                                packet.WriteInteger(0);
                                packet.WriteInteger(0);
                                packet.WriteString(item.ExtraData);
                            }
                            else
                            {
                                packet.WriteInteger(Style);
                                packet.WriteInteger(1);
                                packet.WriteInteger(6);
                                packet.WriteString("EXTRA_PARAM");
                                packet.WriteString("");
                                packet.WriteString("MESSAGE");
                                packet.WriteString(ExtraData[1]);
                                packet.WriteString("PURCHASER_NAME");
                                packet.WriteString(Purchaser.Username);
                                packet.WriteString("PURCHASER_FIGURE");
                                packet.WriteString(Purchaser.Look);
                                packet.WriteString("PRODUCT_CODE");
                                packet.WriteString("A1 KUMIANKKA");
                                packet.WriteString("state");
                                packet.WriteString(item.MagicRemove == true ? "1" : "0");
                            }
                        }
                    }
                    break;

                case InteractionType.MANNEQUIN:
                    packet.WriteInteger(0);
                    packet.WriteInteger(1);
                    packet.WriteInteger(3);
                    if (item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                    {
                        string[] Stuff = item.ExtraData.Split(Convert.ToChar(5));
                        packet.WriteString("GENDER");
                        packet.WriteString(Stuff[0]);
                        packet.WriteString("FIGURE");
                        packet.WriteString(Stuff[1]);
                        packet.WriteString("OUTFIT_NAME");
                        packet.WriteString(Stuff[2]);
                    }
                    else
                    {
                        packet.WriteString("GENDER");
                        packet.WriteString("");
                        packet.WriteString("FIGURE");
                        packet.WriteString("");
                        packet.WriteString("OUTFIT_NAME");
                        packet.WriteString("");
                    }
                    break;

                case InteractionType.TONER:
                    if (item.RoomId != 0)
                    {
                        if (item.GetRoom().TonerData == null)
                            item.GetRoom().TonerData = new TonerData(item.Id);

                        packet.WriteInteger(0);
                        packet.WriteInteger(5);
                        packet.WriteInteger(4);
                        packet.WriteInteger(item.GetRoom().TonerData.Enabled);
                        packet.WriteInteger(item.GetRoom().TonerData.Hue);
                        packet.WriteInteger(item.GetRoom().TonerData.Saturation);
                        packet.WriteInteger(item.GetRoom().TonerData.Lightness);
                    }
                    else
                    {
                        packet.WriteInteger(0);
                        packet.WriteInteger(0);
                        packet.WriteString(string.Empty);
                    }
                    break;

                case InteractionType.BADGE_DISPLAY:
                    packet.WriteInteger(0);
                    packet.WriteInteger(2);
                    packet.WriteInteger(4);

                    string[] BadgeData = item.ExtraData.Split(Convert.ToChar(9));
                    if (item.ExtraData.Contains(Convert.ToChar(9).ToString()))
                    {
                        packet.WriteString("0");//No idea
                        packet.WriteString(BadgeData[0]);//Badge name
                        packet.WriteString(BadgeData[1]);//Owner
                        packet.WriteString(BadgeData[2]);//Date
                    }
                    else
                    {
                        packet.WriteString("0");//No idea
                        packet.WriteString("DEV");//Badge name
                        packet.WriteString("Sledmore");//Owner
                        packet.WriteString("13-13-1337");//Date
                    }
                    break;

                case InteractionType.TELEVISION:
                    packet.WriteInteger(0);
                    packet.WriteInteger(1);
                    packet.WriteInteger(1);

                    packet.WriteString("THUMBNAIL_URL");
                    //Message.WriteString("http://img.youtube.com/vi/" + PlusEnvironment.GetGame().GetTelevisionManager().TelevisionList.OrderBy(x => Guid.NewGuid()).FirstOrDefault().YouTubeId + "/3.jpg");
                    packet.WriteString("");
                    break;

                case InteractionType.LOVELOCK:
                    if (item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                    {
                        var EData = item.ExtraData.Split((char)5);
                        int I = 0;
                        packet.WriteInteger(0);
                        packet.WriteInteger(2);
                        packet.WriteInteger(EData.Length);
                        while (I < EData.Length)
                        {
                            packet.WriteString(EData[I]);
                            I++;
                        }
                    }
                    else
                    {
                        packet.WriteInteger(0);
                        packet.WriteInteger(0);
                        packet.WriteString("0");
                    }
                    break;

                case InteractionType.MONSTERPLANT_SEED:
                    packet.WriteInteger(0);
                    packet.WriteInteger(1);
                    packet.WriteInteger(1);

                    packet.WriteString("rarity");
                    packet.WriteString("1");//Leve should be dynamic.
                    break;
            }
        }

        public static void GenerateWallExtradata(Item Item, ServerPacket Message)
        {
            switch (Item.GetBaseItem().InteractionType)
            {
                default:
                    Message.WriteString(Item.ExtraData);
                    break;

                case InteractionType.POSTIT:
                    Message.WriteString(Item.ExtraData.Split(' ')[0]);
                    break;
            }
        }
    }
}