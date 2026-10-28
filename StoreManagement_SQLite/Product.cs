using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement_SQLite
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime EntryDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int Quantity { get; set; }

        // Category Foreign Key
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ProductCategory Category { get; set; }
    }
}
