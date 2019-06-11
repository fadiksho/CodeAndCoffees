using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Extensions;
using MyBlog.Model;
using System;
using System.Linq;

namespace MyBlog.MappingProfiles
{
  public class BlogProfile : Profile
  {
    public BlogProfile()
    {
      // Dto to Entity
      CreateMap<BlogForCreatingDto, BlogTable>()
        .ForMember(bt => bt.Tags, bdto => 
          bdto.MapFrom(dto => string.Join(",", dto.Tags)))
        .ForMember(a => a.Slug, b => b.MapFrom(
          c => URLHelper.BuildBlogUrl(c.Title, c.PublishedDate)));

      CreateMap<BlogForUpdatingDto, BlogTable>()
        .ForMember(bt => bt.Tags, bdto => bdto.MapFrom(
          dto => string.Join(",", dto.Tags)))
        .ForMember(a => a.Slug, b => b.MapFrom(
          c => URLHelper.BuildBlogUrl(c.Title, c.PublishedDate)));

      // Entity to Model
      CreateMap<BlogTable, Blog>()
        .ForMember(a => a.Tags, b => b.MapFrom(
          c => c.Tags.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)))
        .ForMember(a => a.Slug, b => b.MapFrom(
          c => URLHelper.BuildBlogUrl(c.Title, c.PublishedDate)));
      CreateMap<IQueryable<BlogTable>, IQueryable<Blog>>();
      CreateMap<BlobTable, Blob>();

      // Model to Dto
      CreateMap<Blog, BlogForUpdatingDto>();
    }
  }
}
