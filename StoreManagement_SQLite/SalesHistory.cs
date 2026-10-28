using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement_SQLite
{
    [Table("SalesHistory")]
    public class SalesHistory
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string CategoryName { get; set; }

        public int Quantity { get; set; }

        public DateTime SaleDate { get; set; }
    }
}
