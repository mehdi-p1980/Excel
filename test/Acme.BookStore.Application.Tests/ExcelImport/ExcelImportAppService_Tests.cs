using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acme.BookStore.ExcelImport;
using Shouldly;
using Xunit;
using MiniExcelLibs;
using Volo.Abp.Validation;
using Acme.BookStore.Books;

namespace Acme.BookStore.Application.Tests.ExcelImport
{
    public class ExcelImportAppService_Tests : BookStoreApplicationTestBase
    {
        private readonly IExcelImportAppService _excelImportAppService;

        public ExcelImportAppService_Tests()
        {
            _excelImportAppService = GetRequiredService<IExcelImportAppService>();
        }

        [Fact]
        public async Task UploadAsync_Should_Return_Preview_With_Valid_Data()
        {
            // Arrange
            var sheet = new[]
            {
                new { Name = "Book1", Type = BookType.Adventure, PublishDate = new System.DateTime(2023, 1, 1), Price = 10.0f },
                new { Name = "Book2", Type = BookType.Biography, PublishDate = new System.DateTime(2023, 2, 1), Price = 20.0f }
            };
            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(sheet);
            var fileBytes = memoryStream.ToArray();

            // Act
            var result = await _excelImportAppService.UploadAsync(fileBytes);

            // Assert
            result.ShouldNotBeNull();
            result.Data.Count.ShouldBe(2);
            result.Errors.Count.ShouldBe(0);
            result.Data[0].Name.ShouldBe("Book1");
            result.Data[1].Price.ShouldBe(20.0f);
        }

        [Fact]
        public async Task UploadAsync_Should_Return_Preview_With_Errors_For_Invalid_Data()
        {
            // Arrange
             var sheet = new[]
            {
                new { Name = "Book1", Type = BookType.Adventure, PublishDate = new System.DateTime(2023, 1, 1), Price = 10.0f },
                new { Name = "", Type = BookType.Biography, PublishDate = new System.DateTime(2023, 2, 1), Price = -5.0f } // Invalid: Name is empty, Price is negative
            };
            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(sheet);
            var fileBytes = memoryStream.ToArray();

            // Act
            var result = await _excelImportAppService.UploadAsync(fileBytes);

            // Assert
            result.ShouldNotBeNull();
            result.Data.Count.ShouldBe(2);
            result.Errors.Count.ShouldBe(2); // Expecting two errors
            // Note: Specific error messages depend on your validation attributes and localization.
            // You might want to make these assertions more specific if needed.
            result.Errors.ShouldContain(e => e.Contains("Name"));
            result.Errors.ShouldContain(e => e.Contains("Price"));
        }

        [Fact]
        public async Task ImportAsync_Should_Throw_AbpValidationException_If_Preview_Has_Errors()
        {
            // Arrange
            var previewDtoWithErrors = new ExcelPreviewDto
            {
                Data = new System.Collections.Generic.List<ExcelImportDto>(), // Empty data for simplicity
                Errors = new System.Collections.Generic.List<string> { "Error 1", "Error 2" }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AbpValidationException>(async () => await _excelImportAppService.ImportAsync(previewDtoWithErrors));
            exception.ValidationErrors.Count.ShouldBe(2);
        }
    }
}
