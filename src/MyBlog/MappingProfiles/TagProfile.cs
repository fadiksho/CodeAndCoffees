using AutoMapper;
using MyBlog.DTO;
using MyBlog.Entity;

namespace MyBlog.MappingProfiles
{
  public class TagProfile : Profile
  {
    public TagProfile()
    {
      // Dto to Entity
      CreateMap<TagDto, TagTable>();
    }
  }
}
