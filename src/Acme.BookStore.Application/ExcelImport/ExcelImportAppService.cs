using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acme.BookStore.Books; // This line already exists, it was a different error.
using MiniExcelLibs;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;
// Correcting the actual missing using:
using System.ComponentModel.DataAnnotations;
using Acme.BookStore.ExcelImport; // For IExcelImportAppService, ExcelPreviewDto
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.ExcelImport
{
    public class ExcelImportAppService : ApplicationService, IExcelImportAppService
    {
        private readonly IRepository<Book, Guid> _bookRepository;

        public ExcelImportAppService(IRepository<Book, Guid> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<ExcelPreviewDto> UploadAsync(byte[] fileBytes)
        {
            var preview = new ExcelPreviewDto { Data = new List<ExcelImportDto>(), Errors = new List<string>() };
            using (var stream = new MemoryStream(fileBytes))
            {
                var rows = stream.Query<ExcelImportDto>(sheetName: "MyBooks").ToList();
                if (rows.Count > 100)
                {
                    preview.Errors.Add("The Excel file cannot have more than 100 rows.");
                    return preview;
                }
                foreach (var row in rows)
                {
                    var validationResults = new List<ValidationResult>();
                    if (!Validator.TryValidateObject(row, new ValidationContext(row), validationResults, true))
                    {
                        preview.Errors.AddRange(validationResults.Select(r => r.ErrorMessage));
                    }
                    preview.Data.Add(row);
                }
            }
            return preview;
        }

        public async Task ImportAsync(ExcelPreviewDto previewDto)
        {
            if (previewDto.Errors.Any())
            {
                throw new AbpValidationException("Data validation failed. Please correct the errors and try again.", previewDto.Errors.Select(e => new ValidationResult(e)).ToList());
            }

            foreach (var item in previewDto.Data)
            {
                // This is a simplified example. You'll likely want to map ExcelImportDto to your Book entity.
                // You might also want to check for existing books or handle other business logic here.
                await _bookRepository.InsertAsync(
                    new Book
                    {
                        Name = item.Name,
                        Type = item.Type,
                        PublishDate = item.PublishDate,
                        Price = item.Price
                    }
                );
            }
        }
    }
}
