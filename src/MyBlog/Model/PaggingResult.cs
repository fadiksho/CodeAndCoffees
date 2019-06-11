using MyBlog.Abstraction;
using System.Collections.Generic;

namespace MyBlog.Model
{
  public class PaggingResult<T> : PaggingData
  {
    public IEnumerable<T> TResult { get; set; }
  }
}
