using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Extensions;
using MyBlog.Model;
using MyBlog.Persistence.Data;
using MyBlog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Persistence
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

    public async Task<BlogTable> AddBlog(BlogForCreatingDto newBlogDto)
    {
      var blogEntity = mapper.Map<BlogForCreatingDto, BlogTable>(newBlogDto);

      await context.Blogs
        .AddAsync(blogEntity);

      return blogEntity;
    }

    public async Task<bool> IsBlogExistBySlugAsync(string slug, bool onlyPublishedBlog)
    {
      var blogEntity = await context.Blogs
        .AsNoTracking()
        .FirstOrDefaultAsync(b => b.Slug == slug
          && b.IsPublished == onlyPublishedBlog);

      return (blogEntity != null);
    }

    public async Task DeleteBlogAsync(int id)
    {
      var blogEntity = await context.Blogs
        .FirstAsync(b => b.Id == id);

      context.Blogs.Remove(blogEntity);
    }

    public async Task<Blog> GetBlogAsync(int id)
    {
      var blogEntity = await context.Blogs
        .Where(b => b.Id == id)
        .FirstAsync(b => b.Id == id);
      var blog = mapper.Map<Blog>(blogEntity);

      return blog;
    }

    public async Task<Blog> GetBlogAsync(string slug)
    {
      var blogEntity = await context.Blogs
        .FirstAsync(b => b.Slug == slug);

      return mapper.Map<BlogTable, Blog>(blogEntity);
    }

    public PaggingResult<Blog> GetBlogsPage(BlogQuery query)
    {
      var blogsTable = context.Blogs
        .AsNoTracking()
        .Where(b => b.IsPublished == query.OnlyPublished);

      // Apply Ordering
      blogsTable = blogsTable.OrderByDescending(b => b.PublishedDate);
      // Convert BlogTable to Blog
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
      // Apply Paging
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
        .FirstAsync(b => b.Id == id);

      mapper.Map<BlogForUpdatingDto, BlogTable>(updatedBlogDto, blogEntity);
    }
  }
}
