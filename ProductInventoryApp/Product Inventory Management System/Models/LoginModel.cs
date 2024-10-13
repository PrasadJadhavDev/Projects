using System.ComponentModel.DataAnnotations;

namespace Product_Inventory_Management_System.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter user name.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter password.")]
        public string Password { get; set; }
        public string Role {  get; set; }
    }
}
