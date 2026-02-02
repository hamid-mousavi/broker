using Broker.Data;
using Broker.DTOs.Document;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Broker.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<DocumentDto>> GetUserDocumentsAsync(int userId)
        {
            var documents = await _context.Documents
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return documents.Select(d => new DocumentDto
            {
                Id = d.Id,
                UserId = d.UserId,
                DocumentType = d.DocumentType,
                FileName = d.FileName,
                FilePath = d.FilePath,
                MimeType = d.MimeType,
                FileSize = d.FileSize,
                Description = d.Description,
                IsVerified = d.IsVerified,
                CreatedAt = d.CreatedAt
            }).ToList();
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(int documentId, int userId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId);

            if (document == null) return null;

            return new DocumentDto
            {
                Id = document.Id,
                UserId = document.UserId,
                DocumentType = document.DocumentType,
                FileName = document.FileName,
                FilePath = document.FilePath,
                MimeType = document.MimeType,
                FileSize = document.FileSize,
                Description = document.Description,
                IsVerified = document.IsVerified,
                CreatedAt = document.CreatedAt
            };
        }

        public async Task<DocumentDto?> UploadDocumentAsync(int userId, UploadDocumentDto uploadDto, string webRootPath)
        {
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "documents");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{uploadDto.File.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadDto.File.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", "documents", uniqueFileName).Replace("\\", "/");

            var document = new Models.Document
            {
                UserId = userId,
                DocumentType = uploadDto.DocumentType,
                FileName = uploadDto.File.FileName,
                FilePath = relativePath,
                MimeType = uploadDto.File.ContentType,
                FileSize = uploadDto.File.Length,
                Description = uploadDto.Description,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return new DocumentDto
            {
                Id = document.Id,
                UserId = document.UserId,
                DocumentType = document.DocumentType,
                FileName = document.FileName,
                FilePath = document.FilePath,
                MimeType = document.MimeType,
                FileSize = document.FileSize,
                Description = document.Description,
                IsVerified = document.IsVerified,
                CreatedAt = document.CreatedAt
            };
        }

        public async Task<bool> DeleteDocumentAsync(int documentId, int userId)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.Id == documentId && d.UserId == userId);

            if (document == null) return false;

            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath ?? "", document.FilePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<DocumentTypeDto>> GetDocumentTypesAsync()
        {
            // Return predefined document types
            return new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Type = "License", Description = "مجوز ترخیص" },
                new DocumentTypeDto { Type = "IdentityCard", Description = "کارت ملی" },
                new DocumentTypeDto { Type = "BusinessLicense", Description = "پروانه کسب" },
                new DocumentTypeDto { Type = "TaxCertificate", Description = "گواهی مالیاتی" },
                new DocumentTypeDto { Type = "Other", Description = "سایر" }
            };
        }
    }
}

