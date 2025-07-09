using System.ComponentModel.DataAnnotations;

namespace authapi.DTOs
{
    public class UserSignupDto
    {
        [Required, MinLength(6)]
        public string FirstName{ get; set; }
        [Required, MinLength(2)]
        public string LastName { get; set; }
        [Required , MinLength(2)]
        public string Email { get; set; }   

        public string Password { get; set; }    

        public string ConfirmPassword { get; set; }


    }
}
