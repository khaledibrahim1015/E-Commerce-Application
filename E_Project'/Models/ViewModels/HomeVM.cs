using System.Collections;
using System.Collections.Generic;

namespace E_Project_.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { set; get; }
        public IEnumerable<Category> Categories { set; get; }


    }
}
