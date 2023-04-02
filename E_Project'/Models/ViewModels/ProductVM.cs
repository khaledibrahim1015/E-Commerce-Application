using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace E_Project_.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
      public  IEnumerable<SelectListItem> CategorySelectListItems { set; get; }

        public IEnumerable<SelectListItem> ApplicationSelectListItems { set; get; }

    }
}
