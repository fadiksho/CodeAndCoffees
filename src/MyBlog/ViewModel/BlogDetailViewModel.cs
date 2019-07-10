using MyBlog.Model;

namespace MyBlog.ViewModel
{
  public class BlogDetailViewModel
  {
    public Blog Blog { get; set; }

    public string PageDescription { get; set; }
    public DisqusViewModel DisqusViewModel { get; set; }
  }
}
