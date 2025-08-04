using System;

namespace time_track_dshp.Models.Dto
{
    public interface ISyncLockEntity
    {
        public Guid? SyncLockedBy { get; set; }
        public Guid? SyncLockedById { get; set; }
        public DateTime? SyncLockedOn { get; set; }
        public string SyncLockedByName { get; set; }
    }
}
