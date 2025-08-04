namespace time_track_service.Model
{
    public enum DbResponse
    {
        Error,
        Invalid,
        NotFound,
        Found,
        Inserted,
        Updated,
        Reverted,
        Deleted,
        Conflict,
        Forbidden
    }
}