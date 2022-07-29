using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class ProfileInputModel
    {
        public bool IsCompany { get; set; }

        [StringLength(128, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string CompanyName { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        [EmailAddress(ErrorMessageResourceName = "EmailIsNotValid", ErrorMessageResourceType = typeof(LanguageAll.Language))]
        public string Email { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Address { get; set; }

        [StringLength(128, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Fullname { get; set; }

        public DateTime? Birthday { get; set; }


        [StringLength(20, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string PersonID { get; set; }


        public DateTime? IssueDate { get; set; }

        [StringLength(126, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string IssuePlace { get; set; }

        public string Avatar { get; set; }
    }

    public class ProfileOutputModel
    {
        public bool IsCompany { get; set; }

        [StringLength(128, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string CompanyName { get; set; }

        [StringLength(20, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        [EmailAddress(ErrorMessageResourceName = "EmailIsNotValid", ErrorMessageResourceType = typeof(LanguageAll.Language))]
        public string Email { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        [StringLength(128, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Fullname { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(20, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string PersonID { get; set; }

        public DateTime? IssueDate { get; set; }

        [StringLength(126, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string IssuePlace { get; set; }

        [StringLength(30, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string WaterCompany { get; set; }

        public List<string> CustomerCodeList { get; set; }

        public string DeviceId { get; set; }
        public bool IsGetNotice { get; set; } = false;
        public string Avatar { get; set; }
        public ProfileOutputModel()
        {
            CustomerCodeList = new List<string>();
            IsCompany = false;
        }
    }

}
