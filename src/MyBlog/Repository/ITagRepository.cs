using MyBlog.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyBlog.Repository
{
	public interface ITagRepository
  {
    Task<bool> TagExistAsync(string tagName);
    Task<IEnumerable<string>> GetTagsAsync();

    Task AddTagAsync(TagDto tagDto);
    Task UpdateTagAsync(string tag, TagDto updatedTagDto);
    Task DeleteTagAsync(string tag);
  }
}
