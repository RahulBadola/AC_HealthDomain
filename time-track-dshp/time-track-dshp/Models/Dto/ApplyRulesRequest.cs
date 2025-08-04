using System.Collections.Generic;

namespace time_track_dshp.Models.Dto
{
    public class ApplyRulesRequest<T>
    {
        public List<ChangeSet<T>> ChangeSet { get; set; }

        public static ApplyRulesRequest<T> NewRequest(ChangeSet<T> changeSet)
        {
            return new ApplyRulesRequest<T>() { ChangeSet = new List<ChangeSet<T>>() {changeSet}};
        }
    }
}
