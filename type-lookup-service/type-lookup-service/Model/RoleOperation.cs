namespace type_lookup_service.Model
{
    public class RoleOperation
    {
        /// <summary>
        /// Name of object related to operation
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// Name of operation
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Security Role Id
        /// </summary>
        public string SecurityRoleId { get; set; }
    }
}
