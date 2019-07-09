using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Persistence;
using MyBlog.Services;

namespace MyBlog.MappingProfiles
{
  public class BlogSlugResolver : 
    IValueResolver<BlogForCreatingDto, BlogTable, string>, 
    IValueResolver<BlogForUpdatingDto, BlogTable, string>
  {
    private readonly IURLHelper URLHelper;

    public BlogSlugResolver()
    {
      this.URLHelper = new URLHelper();
    }
    public string Resolve(BlogForCreatingDto source, BlogTable destination, string destMember, ResolutionContext context)
    {
      return URLHelper.ToFriendlyUrl(source.Title);
    }

    public string Resolve(BlogForUpdatingDto source, BlogTable destination, string destMember, ResolutionContext context)
    {
      return URLHelper.ToFriendlyUrl(source.Title);
    }
  }
}
