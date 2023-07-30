using System.ComponentModel.DataAnnotations;

namespace SYT.BnBCheckIn.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}