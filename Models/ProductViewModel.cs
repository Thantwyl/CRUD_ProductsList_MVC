using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProductMVC.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        [DisplayName("Buying Price")]
        public decimal BuyingPrice { get; set; } = 0;
        [DisplayName("Supplier")]
        public string? Supplier { get; set; }
        [DisplayName("Picture")]
        public string? PictureFileName { get; set; }
        [DisplayName("Manufacturing Date")]
        public string? ManufacturingDate { get; set; }
        [DisplayName("Purchasing Date")]
        public string? PurchasingDate { get; set; }
        [DisplayName("Expiration Date")]
        public DateTime ExpirationDate { get; set; } = DateTime.Now.AddYears(1);
    }
}