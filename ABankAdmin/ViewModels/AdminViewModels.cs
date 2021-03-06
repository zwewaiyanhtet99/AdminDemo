using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ABankAdmin.ViewModels
{
    //public class ExternalLoginConfirmationViewModel
    //{
    //    [Required]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}

    //public class ExternalLoginListViewModel
    //{
    //    public string ReturnUrl { get; set; }
    //}

    //public class SendCodeViewModel
    //{
    //    public string SelectedProvider { get; set; }
    //    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    //    public string ReturnUrl { get; set; }
    //    public bool RememberMe { get; set; }
    //}

    //public class VerifyCodeViewModel
    //{
    //    [Required]
    //    public string Provider { get; set; }

    //    [Required]
    //    [Display(Name = "Code")]
    //    public string Code { get; set; }
    //    public string ReturnUrl { get; set; }

    //    [Display(Name = "Remember this browser?")]
    //    public bool RememberBrowser { get; set; }

    //    public bool RememberMe { get; set; }
    //}

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class AdminViewModel
    {
        [Required]
        public string ID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        [Display(Name ="Branch")]
        public int BranchID { get; set; }
        public string Branch { get; set; }
        public int Role { get; set; }
        [StringLength(50)]
        public string StaffID { get; set; }
        [StringLength(50)]
        //[RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        public string Phone { get; set; }
        [EmailAddress]
        [RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        public string Email { get; set; }
        [Display(Name ="Lock")]
        public bool IsLock { get; set; }
        public string RoleName { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        [Display(Name = "Branch")]
        public int BranchID { get; set; }
        public int Role { get; set; }
        [StringLength(50)]
        public string StaffID { get; set; }
        [StringLength(50)]
        //[RegularExpression(@"[0][9]\d{7,9}", ErrorMessage = "Phone No must start with 09. Minimum length is 9 and Maximum length is 11.")]
        public string Phone { get; set; }
       
        //[Required]
        [EmailAddress]
        //[RegularExpression(@"^[_A-Za-z0-9-]+([_A-Za-z0-9-\.\+]+)*@abank.com.mm$", ErrorMessage = "You can only use @abank.com.mm email!")]//_.+- @abank.com.mm
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    //public class ResetPasswordViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm password")]
    //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }

    //    public string Code { get; set; }
    //}

    //public class ForgotPasswordViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}

    //change password
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
