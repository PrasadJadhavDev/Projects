using System.ComponentModel.DataAnnotations;

namespace Product_Inventory_Management_System.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter product name.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter product description.")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter product price.")]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please enter category of product")]
        [StringLength(100)]
        public string Category { get; set; }

        [Required(ErrorMessage = "Enter StockQuantity")]
        [Range(0, 1000)]
        public int StockQuantity { get; set; }
    }
}
