using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;
using MyBlog.Model;
using MyBlog.Services;
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
        .ForMember(a => a.Slug, exp => exp.MapFrom(new BlogSlugResolver()));

      CreateMap<BlogForUpdatingDto, BlogTable>()
        .ForMember(bt => bt.Tags, bdto => bdto.MapFrom(
          dto => string.Join(",", dto.Tags)))
        .ForMember(a => a.Slug, exp => exp.MapFrom(new BlogSlugResolver()));

      // Entity to Model
      CreateMap<BlogTable, Blog>()
        .ForMember(a => a.Tags, b => b.MapFrom(
          c => c.Tags.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
      CreateMap<BlobTable, Blob>();

      // Model to Dto
      CreateMap<Blog, BlogForUpdatingDto>();
    }
  }
}
