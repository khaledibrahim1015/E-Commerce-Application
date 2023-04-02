using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Project_.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Category Name is Required")]
        public string Name { get; set; }

        //[Display(Name ="Display Order ")]
        
        //[MinLength(0,ErrorMessage ="should greater than 0")]
        [Required(ErrorMessage ="Order Display is Required")]
        [Range(1,int.MaxValue,ErrorMessage = "Display Order Must Greater Than 0:")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }

    }
}
