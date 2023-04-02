using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Project_.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name ="Short Description")]
        public string ShortDescription { get; set; }

        [Range(1,int.MaxValue)]
        public double Price { get; set; }

        public string Image { get; set; }


        // relationship

        [Display(Name ="Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        //Relationship 


        [Display(Name = "Application Type")]
        [ForeignKey("ApplicationType")]
        public int ApplicationTypeId { set; get; }
        public ApplicationType ApplicationType { get; set; }



    }
}
