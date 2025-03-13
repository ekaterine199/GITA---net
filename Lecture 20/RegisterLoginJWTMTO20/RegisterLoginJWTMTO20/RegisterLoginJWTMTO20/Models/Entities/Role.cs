namespace RegisterLoginJWTMTO20.Models.Entities
{
    public class Role : BaseClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> users { get; set; }
    }
}
