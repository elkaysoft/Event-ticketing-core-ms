using Microsoft.AspNetCore.Http;

namespace ETS.Domain.Contracts
{
    public interface IDocumentService
    {
        string UploadDocument(IFormFile doc);
        List<string> UploadDocuments(List<IFormFile> files);
    }
}
