using time_track_service.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace time_track_service.Model.Dto
{
    public class VoidModel
    {
        [Required]
        [GuidString]
        public string EntityId { get; set; }

        [Required]
        public string ObjectType { get; set; }

        [Required]
        [GuidString]
        public string VoidReasonId { get; set; }
    }
}