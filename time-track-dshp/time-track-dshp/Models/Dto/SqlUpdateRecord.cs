namespace time_track_dshp.Models.Dto
{
    public class SqlUpdateRecord
    {
        public string TransactionStartTime { get; set; }
        public string TransactionEndTime { get; set; }
        public string Table { get; set; }
        public string PrimaryKey { get; set; }
        public string TransactionType { get; set; }
        public string StartLns { get; set; }

    }
}
