using MyBlog.Persistence;
using Xunit;

namespace UnitTest.BusinessLogic
{
  public class URLHelper_Test
  {
    private readonly URLHelper URLHelper;
    public URLHelper_Test()
    {
      URLHelper = new URLHelper(); 
    }

    [Theory]
    [InlineData(@"new.", "new")]
    [InlineData(@" new Blog Post", "new-blog-post")]
    [InlineData(@"new:Blog Post", "new-blog-post")]
    [InlineData(@"new: Blog Post", "new-blog-post")]
    [InlineData(@"new::Blog&Post\new blog", "new-blog-and-post-new-blog")]
    [InlineData(@"new:&:Blog& Post\new blog", "new-and-blog-and-post-new-blog")]
    [InlineData(@"new,Blog\,Post.", "new-blog-post")]
    public void ToFriendlyUrl_Test(string url, string expected)
    {
      // Arrange && Act
      var friendlyUrl = URLHelper.ToFriendlyUrl(url);
      // Assert
      Assert.Equal(expected, friendlyUrl);
    }
  }
}
