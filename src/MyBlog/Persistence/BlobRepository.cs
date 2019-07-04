using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyBlog.Abstraction;
using MyBlog.Entity;
using MyBlog.Extensions;
using MyBlog.Model;
using MyBlog.Persistence.Data;
using MyBlog.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Persistence
{
  public class BlobRepository : IBlobRepository
  {
    private readonly BlogContext context;
    private readonly IMapper mapper;
    public BlobRepository(
      BlogContext context,
      IMapper mapper)
    {
      this.context = context;
      this.mapper = mapper;
    }

    public async Task AddBlobAsync(BlobTable newBlob)
    {
      await this.context.Blobs
        .AddAsync(newBlob);
    }

    public async Task<bool> BlobExistAsync(int blobId)
    {
      return await context.Blobs
        .AsNoTracking()
        .FirstOrDefaultAsync(b => b.Id == blobId) != null;
    }

    public async Task<Blob> GetBlobAsync(int blobId)
    {
      var blobEntity = await context.Blobs
        .AsNoTracking()
        .FirstAsync(b => b.Id == blobId);

      return mapper.Map<Blob>(blobEntity);
    }

    public PaggingResult<Blob> GetBlobPage(IPaggingQuery query)
    {
      var blobsTable = context.Blobs
        .AsNoTracking();

       // Apply Ordering
      blobsTable = blobsTable.OrderByDescending(b => b.CreatedDate);
      // Convert BlobTable to Blob
      var blobs = mapper.Map<IEnumerable<Blob>>(blobsTable);

      var totalItems = blobs.Count();
      var blobsAfterPagging = blobs.ApplayPaging(query);
      var paggingResult = new PaggingResult<Blob>
      {
        CurrentPage = query.Page,
        PageSize = query.PageSize,
        TotalItems = totalItems,
        TResult = blobsAfterPagging
      };

      return paggingResult;
    }

    public async Task DeleteBlobAsync(int blobId)
    {
      var blobEntity = await context.Blobs
        .FirstAsync(b => b.Id == blobId);

      context.Blobs.Remove(blobEntity);
    }

    public async Task<string> GetBlobPathAsync(int blobId)
    {
      var blobEntity = await context.Blobs
        .AsNoTracking()
        .FirstAsync(b => b.Id == blobId);

      return blobEntity.FilePath;
    }
  }
}
