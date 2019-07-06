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
    private readonly URLHelper URLHelper;

    public BlogSlugResolver()
    {
      URLHelper = new URLHelper();
    }
    public string Resolve(BlogForCreatingDto source, BlogTable destination, string destMember, ResolutionContext context)
    {
      return URLHelper.BuildBlogUrl(source.Title, source.PublishedDate);
    }

    public string Resolve(BlogForUpdatingDto source, BlogTable destination, string destMember, ResolutionContext context)
    {
      return URLHelper.BuildBlogUrl(source.Title, source.PublishedDate);
    }
  }
}
