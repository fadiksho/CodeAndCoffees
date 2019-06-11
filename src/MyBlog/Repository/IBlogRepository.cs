using MyBlog.DTO;
using MyBlog.Model;
using System.Threading.Tasks;

namespace MyBlog.Repository
{
	public interface IBlogRepository
  {
    Task<Blog> GetBlogAsync(int id);
    Task<Blog> GetBlogAsync(string slug);

    PaggingResult<Blog> GetBlogsPage(BlogQuery opt);

    Task AddBlog(BlogForCreatingDto newBlogDto);
    Task UpdateBlogAsync(int id, BlogForUpdatingDto updatedBlog);
    Task DeleteBlogAsync(int id);

    Task<bool> BlogExistAsync(int blogId, bool onlyPublishedBlog);
  }
}
