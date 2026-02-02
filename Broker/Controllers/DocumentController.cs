using Broker.DTOs.Common;
using Broker.DTOs.Document;
using Broker.Helpers;
using Broker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IWebHostEnvironment _environment;

        public DocumentController(IDocumentService documentService, IWebHostEnvironment environment)
        {
            _documentService = documentService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<DocumentDto>>>> GetDocuments()
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<List<DocumentDto>>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _documentService.GetUserDocumentsAsync(userId);
            return Ok(ApiResponse<List<DocumentDto>>.SuccessResponse(result));
        }

        [HttpPost("upload")]
        public async Task<ActionResult<ApiResponse<DocumentDto>>> UploadDocument([FromForm] UploadDocumentDto uploadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<DocumentDto>.ErrorResponse("اطلاعات ارسالی معتبر نیست",
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<DocumentDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var webRootPath = _environment.WebRootPath ?? _environment.ContentRootPath;
            var result = await _documentService.UploadDocumentAsync(userId, uploadDto, webRootPath);

            if (result == null)
            {
                return BadRequest(ApiResponse<DocumentDto>.ErrorResponse("آپلود مدرک انجام نشد"));
            }

            return Ok(ApiResponse<DocumentDto>.SuccessResponse(result, "مدرک با موفقیت آپلود شد"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DocumentDto>>> GetDocument(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<DocumentDto>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _documentService.GetDocumentByIdAsync(id, userId);
            if (result == null)
            {
                return NotFound(ApiResponse<DocumentDto>.ErrorResponse("مدرک یافت نشد"));
            }

            return Ok(ApiResponse<DocumentDto>.SuccessResponse(result));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDocument(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("کاربر احراز هویت نشده است"));
            }

            var result = await _documentService.DeleteDocumentAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("مدرک یافت نشد یا دسترسی ندارید"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "مدرک با موفقیت حذف شد"));
        }

        [HttpGet("types")]
        public async Task<ActionResult<ApiResponse<List<DocumentTypeDto>>>> GetDocumentTypes()
        {
            var result = await _documentService.GetDocumentTypesAsync();
            return Ok(ApiResponse<List<DocumentTypeDto>>.SuccessResponse(result));
        }
    }
}

