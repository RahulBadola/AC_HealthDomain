using System;

namespace time_track_dshp.Models.Dto
{
    public class HydrationRecord
    {
        public string DomainName { get; set; }
        public string PrimaryKey { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLastItem { get; set; }
        public bool IsHydrationItem { get; set; }
                
        public bool IsDefault()
        {
            return DomainName.Equals(default) &&
                PrimaryKey.Equals(default) &&
                LastUpdated.Equals(default) &&
                IsLastItem.Equals(default) &&
                IsHydrationItem.Equals(default);
        }
    }
}
