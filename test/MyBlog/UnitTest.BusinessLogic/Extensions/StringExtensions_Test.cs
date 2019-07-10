using MyBlog.Extensions;
using Xunit;

namespace UnitTest.BusinessLogic.Extensions
{
  public class StringExtensions_Test
  {
    [Theory]
    [InlineData("<p>first paragraph</p>", "first paragraph")]
    [InlineData("<img src=\"https://www.fakeurl.com/imgaes/test.png\"/><p>first paragraph</p>", "first paragraph")]
    [InlineData("<img src=\"https://www.fakeurl.com/imgaes/test.png\"/><p>first paragraph</p><p>second paragraph</p>", "first paragraph")]
    [InlineData("<img src=\"https://www.fakeurl.com/imgaes/test.png\"/><a href=\"https://www.fakeurl.com\">fake link</a><p>first paragraph</p><p>second paragraph</p>", "first paragraph")]
    [InlineData("<img src=\"https://staging.codeandcoffees.com/uploads/images/12-3-2018_service-worker.png\" alt=\"service worker icon\"><p>first paragraph</p>", "first paragraph")]
    public void Get_First_Paragraph_From_Html_Should_Not_Be_Empty(string html, string expected)
    {
      // Act
      var firstParagraph = html.GetFirstParagraphTextFromHtml();
      //Assert
      Assert.Equal(expected, firstParagraph);
    }

    [Theory]
    [InlineData("")]
    [InlineData("<p class=\"className\">first paragraph</p>")]
    [InlineData("<img src=\"https://www.fakeurl.com/imgaes/test.png\"/>")]
    [InlineData("<p class=\"className\">first <span>test</span> paragraph</p>")]
    public void Get_First_Paragraph_From_Html_Should_Be_Empty(string html)
    {
      // Act
      var firstParagraph = html.GetFirstParagraphTextFromHtml();
      //Assert
      Assert.Empty(firstParagraph);
    }
  }
}
