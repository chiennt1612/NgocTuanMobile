using EntityFramework.API.Entities.EntityBase;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.API.Entities
{
    public class Adv : BaseEntity
    {
        [StringLength(128, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "CustomerName", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string CustomerName { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[Display(Name = "Mobile", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Mobile { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[Display(Name = "Email", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [EmailAddress(ErrorMessageResourceName = "EmailIsNotValid", ErrorMessageResourceType = typeof(LanguageAll.Language))]
        public string Email { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[Display(Name = "Website", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Website { get; set; }

        [ForeignKey("AdvPosition")]
        [Display(Name = "Position", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public int Pos { get; set; }
        [Display(Name = "Position", ResourceType = typeof(Resources.EntityValidation))]
        public AdvPosition AdvPosition { get; set; }

        [StringLength(200)]
        [Display(Name = "AdvImg", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Img { get; set; }

        [StringLength(2000)]
        [Display(Name = "AdvScript", ResourceType = typeof(Resources.EntityValidation))]
        public string AdvScript { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public DateTime StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Resources.EntityValidation))]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Resources.EntityValidation))]
        public bool Status { get; set; }
    }

    public class AdvPosition
    {
        public int Id { get; set; }
        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "StartDate", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Name { get; set; }
    }

    //public enum AdvPosition
    //{
    //    BannerHomePage = 91,
    //    BannerTop = 11,
    //    BannerMiddle = 12,
    //    BannerBottom = 13,
    //    BannerLeft = 14,
    //    BannerRight = 15,
    //    OverLeft = 21,
    //    OverRight = 22,
    //    ShowHideBottomLeft = 31,
    //    ShowHideBottomRight = 32
    //}
}
