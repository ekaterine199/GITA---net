namespace RegisterLoginJWT.Models
{
    public class BaseClass
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }
        public int? CreatorId { get; set; }
        public int? Modifier { get; set; }

    }
}
