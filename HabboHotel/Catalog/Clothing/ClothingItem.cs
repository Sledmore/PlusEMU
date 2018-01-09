using System;
using System.Collections.Generic;

namespace Plus.HabboHotel.Catalog.Clothing
{
    public class ClothingItem
    {
        public int Id { get; private set; }
        public string ClothingName { get; private set; }
        public List<int> PartIds { get; private set; }

        public ClothingItem(int id, string name, string partIds)
        {
            Id = id;
            ClothingName = name;

            PartIds = new List<int>();
            if (partIds.Contains(","))
            {
                foreach (string PartId in partIds.Split(','))
                {
                    PartIds.Add(int.Parse(PartId));
                }
            }
            else if (!String.IsNullOrEmpty(partIds) && (int.Parse(partIds)) > 0)
            {
                PartIds.Add(int.Parse(partIds));
            }
        }
    }
}
