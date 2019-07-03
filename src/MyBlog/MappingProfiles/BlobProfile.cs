using AutoMapper;
using MyBlog.Entity;
using MyBlog.Model;

namespace MyBlog.MappingProfiles
{
  public class BlobProfile : Profile
  {
    public BlobProfile()
    {
      CreateMap<BlogTable, Blob>();
    }
  }
}
