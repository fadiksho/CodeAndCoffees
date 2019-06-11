namespace MyBlog.Abstraction
{
  public class PaggingData : IPaggingData
  {
    public int TotalItems { get; set; }
    public int TotalPages
    {
      get
      {
        return TotalItems / PageSize + (TotalItems % PageSize != 0 ? 1 : 0);
      }
    }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }

    public bool HasPrevious
    {
      get
      {
        return (CurrentPage > 1);
      }
    }
    public bool HasNext
    {
      get
      {
        return (CurrentPage < TotalPages);
      }
    }
  }
}
