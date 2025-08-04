namespace time_track_service.Model
{
    public class ServicesSettings
    {
        public string SyncBackUri { get; set; }
        public string MemberProgramUri { get; set; }
        public string TypeLookUpGenericUri { get; set; }
        public string TaskServiceUri { get; set; }
        public string MemberServiceUri { get; set; }
        
        public string HydrationSecurityKey { get; set; }
        public bool AllowLoadTestActions { get; set; }
        public string UserServiceUri { get; set; }
    }
}