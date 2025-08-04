namespace time_track_dshp.Models.Dto
{
    public class SyncBackRecord
    {
        public string ObjectType { get; set; }
        public string ObjectJson { get; set; }

        public bool IsDefault()
        {
            return ObjectType.Equals(default) &&
                    ObjectJson.Equals(default);
        }
    }
}
