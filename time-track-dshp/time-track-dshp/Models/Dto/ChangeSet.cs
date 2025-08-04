namespace time_track_dshp.Models.Dto
{
    public class ChangeSet<T>
    {
        public string Name { get; set; }
        public string Operation { get; set; }
        public T New { get; set; }
        public T Original { get; set; }
    }
}
