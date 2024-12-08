namespace Identity.Areas.Admin.Models.User
{
    public class UserVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public List<string> Roles { get; set; }
    }
}
