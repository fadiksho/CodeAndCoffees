using MyBlog.Abstraction;
using MyBlog.Entity;
using MyBlog.Model;
using System.Threading.Tasks;

namespace MyBlog.Repository
{
	public interface IBlobRepository
  {
    Task AddBlobAsync(BlobTable newBlob);
    Task DeleteBlobAsync(int blobId);

    Task<bool> BlobExistAsync(int blobId);
    Task<Blob> GetBlobAsync(int blobId);
    Task<string> GetBlobPathAsync(int blobId);
    PaggingResult<Blob> GetBlobPage(IPaggingQuery query);
  }
}
