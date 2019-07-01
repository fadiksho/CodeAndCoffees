using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Extensions;
using MyBlog.Model;
using MyBlog.Peristence.Data;
using MyBlog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Peristence
{
  public class BlogRepository : IBlogRepository
  {
    private readonly BlogContext context;
    private readonly IMapper mapper;
    public BlogRepository(
      BlogContext context,
      IMapper mapper)
    {
      this.context = context;
      this.mapper = mapper;
    }

    public async Task AddBlog(BlogForCreatingDto newBlogDto)
    {
      var blogEnitity = mapper.Map<BlogForCreatingDto, BlogTable>(newBlogDto);

      await context.Blogs
        .AddAsync(blogEnitity);
    }

    public async Task<bool> BlogExistAsync(int blogId, bool onlyPublishedBlog)
    {
      var blogEntity = await context.Blogs
        .AsNoTracking()
        .Where(b => b.Id == blogId
          && b.IsPublished == onlyPublishedBlog)
        .FirstOrDefaultAsync();

      return (blogEntity != null);
    }

    public async Task DeleteBlogAsync(int id)
    {
      var blogEntity = await context.Blogs
        .Where(b => b.Id == id)
        .FirstOrDefaultAsync();

      context.Blogs.Remove(blogEntity);
    }

    public async Task<Blog> GetBlogAsync(int id)
    {
      var blogEntity = await context.Blogs
        .Where(b => b.Id == id)
        .FirstOrDefaultAsync();

      var blog = mapper.Map<BlogTable, Blog>(blogEntity);
      return blog;
    }

    public async Task<Blog> GetBlogAsync(string slug)
    {
      var blogEntity = await context.Blogs
        .Where(b => b.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
        .FirstOrDefaultAsync();

      var blog = mapper.Map<BlogTable, Blog>(blogEntity);
      return blog;
    }

    public PaggingResult<Blog> GetBlogsPage(BlogQuery query)
    {
      var blogsTable = context.Blogs
        .Where(b => b.IsPublished == query.OnlyPublished);
      var blogs = mapper.Map<IEnumerable<Blog>>(blogsTable);
      // Tag Filter
      if (query.Tags != null && query.Tags.Any())
      {
        blogs = blogs.Where(
        b => b.Tags.Any(s => query.Tags
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Contains(s, StringComparer.OrdinalIgnoreCase)));
      }
      var totalItems = blogs.Count();
      var blogsAfterPagging = blogs.ApplayPaging(query);

      var paggingResult = new PaggingResult<Blog>
      {
        CurrentPage = query.Page,
        PageSize = query.PageSize,
        TotalItems = totalItems,
        TResult = blogsAfterPagging
      };
      return paggingResult;
    }

    public async Task UpdateBlogAsync(int id, BlogForUpdatingDto updatedBlogDto)
    {
      var blogEntity = await context.Blogs
        .Where(b => b.Id == id)
        .FirstOrDefaultAsync();

      mapper.Map<BlogForUpdatingDto, BlogTable>(updatedBlogDto, blogEntity);
    }
  }
}
