using MyBlog.Abstraction;

namespace MyBlog.Model
{
  public class BlogQuery : IPaggingQuery
  {
    const int maxPageSize = 20;

    private int _page = 1;
    public int Page
    {
      get { return _page; }
      set
      {
        if (value < 1)
          _page = 1;
        else
          _page = value;
      }
    }

    private int _pageSize = 10;
    public int PageSize
    {
      get
      {
        return _pageSize;
      }
      set
      {
        _pageSize = (value > maxPageSize) ? maxPageSize : value;
      }
    }
    
    public string Tags { get; set; }

    public bool OnlyPublished { get; set; } = true;
  }
}
