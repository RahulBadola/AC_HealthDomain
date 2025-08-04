using System;

namespace time_track_dshp.Models.Dto
{
    public class SecurityUser : MedCompassBase
    {
        public Guid SecurityUserId { get; set; }
        public string SystemIdentifier { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Salutation { get; set; }
        public string Suffix { get; set; }
        public string PreferredName { get; set; }
    }
}
