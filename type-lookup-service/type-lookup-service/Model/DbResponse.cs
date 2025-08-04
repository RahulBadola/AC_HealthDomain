namespace type_lookup_service.Models
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