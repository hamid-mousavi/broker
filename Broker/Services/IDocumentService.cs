using Broker.DTOs.Document;

namespace Broker.Services
{
    public interface IDocumentService
    {
        Task<List<DocumentDto>> GetUserDocumentsAsync(int userId);
        Task<DocumentDto?> GetDocumentByIdAsync(int documentId, int userId);
        Task<DocumentDto?> UploadDocumentAsync(int userId, UploadDocumentDto uploadDto, string webRootPath);
        Task<bool> DeleteDocumentAsync(int documentId, int userId);
        Task<List<DocumentTypeDto>> GetDocumentTypesAsync();
    }
}

