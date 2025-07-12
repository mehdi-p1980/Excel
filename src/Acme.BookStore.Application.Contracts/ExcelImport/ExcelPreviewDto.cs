using System;
using System.Collections.Generic;

namespace Acme.BookStore.ExcelImport
{
    public class ExcelPreviewDto
    {
        public List<ExcelImportDto> Data { get; set; }
        public List<string> Errors { get; set; }
    }
}
