namespace WebAPI.Models.Domain
{
    public class Book_Author
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        //Navigation Properties - One book_author has one book
        public Books Book { get; set; }

        public int AuthorId { get; set; }
        //Navigation Properties - One book_author has one author
        public Authors Author { get; set; }
    }
}
