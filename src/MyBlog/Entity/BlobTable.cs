namespace MyBlog.Entity
{
  public class BlobTable
  {
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string URL { get; set; }
    public long FileSize { get; set; }
  }
}
