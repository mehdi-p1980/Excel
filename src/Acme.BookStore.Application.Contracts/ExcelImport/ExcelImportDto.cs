using System;
using System.ComponentModel.DataAnnotations;
using Acme.BookStore.Books;
using MiniExcelLibs.Attributes;

namespace Acme.BookStore.ExcelImport
{
    public class ExcelImportDto
    {
        [ExcelColumn(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [ExcelColumn(Name = "Type")]
        [Required]
        public BookType Type { get; set; }

        [ExcelColumn(Name = "PublishDate")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        [ExcelColumn(Name = "Price")]
        [Required]
        public float Price { get; set; }
    }
}
