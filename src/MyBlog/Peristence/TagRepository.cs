using Microsoft.EntityFrameworkCore;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Peristence.Data;
using MyBlog.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Peristence
{
	public class TagRepository : ITagRepository
  {
    private readonly BlogContext context;

    public TagRepository(BlogContext context)
    {
      this.context = context;
    }

    public async Task AddTagAsync(TagDto tagDto)
    {
      await context.AddAsync(new TagTable { Name = tagDto.Name });
    }

    public async Task DeleteTagAsync(string tagName)
    {
      var tagEntity = await context.Tags
        .Where(t => t.Name == tagName)
        .FirstOrDefaultAsync();

      context.Tags.Remove(tagEntity);
    }

    public async Task<IEnumerable<string>> GetTagsAsync()
    {
      var tags = await context.Tags
        .Select(t => t.Name).ToListAsync();

      return tags;
    }

    public async Task<bool> TagExistAsync(string tagName)
    {
      var tagEntity = await context.Tags
        .AsNoTracking()
        .Where(t => t.Name.ToLower() == tagName.ToLower())
        .FirstOrDefaultAsync();

      return tagEntity != null;
    }

    public async Task UpdateTagAsync(string tag, TagDto updatedTagDto)
    {
      var tagEntity = await context.Tags
        .Where(t => t.Name == tag)
        .FirstOrDefaultAsync();

      tagEntity.Name = updatedTagDto.Name;
    }
  }
}
