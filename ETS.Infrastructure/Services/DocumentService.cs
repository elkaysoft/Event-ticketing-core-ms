using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ETS.Domain.Contracts;
using Microsoft.AspNetCore.Http;

namespace ETS.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly Cloudinary _cloudinary;

        public DocumentService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }


        public string UploadDocument(IFormFile doc)
        {
            return UploadResult(doc);
        }

        public List<string> UploadDocuments(List<IFormFile> documents)
        {
            var uploadedResults = new List<string>();
            foreach (var doc in documents)
            {
                uploadedResults.Add(UploadResult(doc));
            }

            return uploadedResults;
        }

        private string UploadResult(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            using var stream = file.OpenReadStream();
            string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}{file.FileName}";
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, stream),
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };
            var uploadResult = _cloudinary.Upload(uploadParams);
            return uploadResult?.SecureUrl.ToString() ?? "";
        }


    }
}
