namespace MyBlog.Abstraction
{
  public interface IPaggingData
  {
    int TotalPages { get; }
    int TotalItems { get; set; }
    int CurrentPage { get; set; }
    int PageSize { get; set; }

    bool HasPrevious { get; }
    bool HasNext { get; }
  }
}
