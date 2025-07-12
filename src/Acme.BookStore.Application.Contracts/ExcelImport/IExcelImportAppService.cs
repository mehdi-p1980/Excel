using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.ExcelImport
{
    public interface IExcelImportAppService : IApplicationService
    {
        Task<ExcelPreviewDto> UploadAsync(byte[] fileBytes);
        Task ImportAsync(ExcelPreviewDto previewDto);
    }
}
