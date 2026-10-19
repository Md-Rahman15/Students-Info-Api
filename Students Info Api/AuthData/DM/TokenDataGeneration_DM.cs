namespace Students_Info_Api.AuthData.DM
{
    public class TokenDataGeneration_DM
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string TokenHash { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

    }
}
