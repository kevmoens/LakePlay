using System.ComponentModel.DataAnnotations;

namespace TriviaForCheeseHeads.Data.Login
{
    public record UserLogin
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;

        public Guid UserId { get; set; } = Guid.Empty;
    }
}
