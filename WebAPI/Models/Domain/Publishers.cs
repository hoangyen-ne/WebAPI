namespace WebAPI.Models.Domain
{
    public class Publishers
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation Properties - One publisher has many books
        public List<Books> Books { get; set; }
    }
}
