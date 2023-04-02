using System.Collections.Generic;

namespace E_Project_.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductLst = new List<Product>();
        }
        public ApplicationUser applicationUser { get; set; }
        public IList<Product> ProductLst { get; set; }
    }
}
