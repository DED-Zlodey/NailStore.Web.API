namespace NailStore.Core.Models
{
    public class ResponseBodyCore
    {
        public string? Token { get; set; }
        public string? Message { get; set; }
        public UserIdentityCoreModel? User { get; set; }
        public DateTimeOffset? LockedOutTime { get; set; }
    }
}