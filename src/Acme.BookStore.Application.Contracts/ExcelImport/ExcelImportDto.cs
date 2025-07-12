using System;
using System.ComponentModel.DataAnnotations;
using Acme.BookStore.Books;

namespace Acme.BookStore.ExcelImport
{
    public class ExcelImportDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public BookType Type { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        [Required]
        public float Price { get; set; }
    }
}
