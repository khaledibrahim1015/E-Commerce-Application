using System.ComponentModel.DataAnnotations;

namespace E_Project_.Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="ApplicationType Is Required !")]
        public string Name { get; set; }
    }
}
