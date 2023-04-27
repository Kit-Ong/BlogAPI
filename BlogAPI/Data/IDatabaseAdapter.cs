using BlogAPI.Models;
using AuthorAPI.Models;

namespace BlogAPI.Data
{
    public interface IDatabaseAdapter
    {
        Task<List<Blog>> GetAllBlogs();
        Task<List<Author>> GetAllAuthors();
        Task<Blog> GetBlogById(string id);
        Task<TransactionResult> AddBlog(Blog blog);
        Task<TransactionResult> AddAuthor(Author author);
    }
}
