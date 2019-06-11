namespace MyBlog.Abstraction
{
  public interface IPaggingQuery
  {
    int Page { get; set; }
    int PageSize { get; set; }
  }
}
