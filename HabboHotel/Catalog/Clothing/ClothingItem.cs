using System;
using System.Collections.Generic;

namespace Plus.HabboHotel.Catalog.Clothing
{
    public class ClothingItem
    {
        public int Id { get; set; }
        public string ClothingName { get; set; }
        public List<int> PartIds { get; private set; }

        public ClothingItem(int Id, string ClothingName, string PartIds)
        {
            this.Id = Id;
            this.ClothingName = ClothingName;

            this.PartIds = new List<int>();
            if (PartIds.Contains(","))
            {
                foreach (string PartId in PartIds.Split(','))
                {
                    this.PartIds.Add(int.Parse(PartId));
                }
            }
            else if (!String.IsNullOrEmpty(PartIds) && (int.Parse(PartIds)) > 0)
                this.PartIds.Add(int.Parse(PartIds));
        }
    }
}
