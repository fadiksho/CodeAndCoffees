using MyBlog.Abstraction;
using MyBlog.Entity;
using MyBlog.Model;
using System.Threading.Tasks;

namespace MyBlog.Repository
{
	public interface IBlobRepository
  {
    Task AddBlobAsync(BlobTable newBlob);
    Task DeleteBlob(int blobId);

    Task<bool> BlobExistAsync(int blobId);
    Task<Blob> GetBlobAsync(int blobId);
    PaggingResult<Blob> GetBlobPageAsync(IPaggingQuery query);
  }
}
