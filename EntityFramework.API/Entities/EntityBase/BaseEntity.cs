﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EntityFramework.API.Entities.EntityBase
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "UserCreator", ResourceType = typeof(Resources.EntityValidation))]
        public long UserCreator { get; set; }
        [Display(Name = "DateCreator", ResourceType = typeof(Resources.EntityValidation))]
        public DateTime DateCreator { get; set; }
        [Display(Name = "UserModify", ResourceType = typeof(Resources.EntityValidation))]
        public long? UserModify { get; set; }
        [Display(Name = "DateModify", ResourceType = typeof(Resources.EntityValidation))]
        public DateTime? DateModify { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }
        [Display(Name = "UserDeleted", ResourceType = typeof(Resources.EntityValidation))]
        [JsonIgnore]
        public long? UserDeleted { get; set; }
        [Display(Name = "DateDeleted", ResourceType = typeof(Resources.EntityValidation))]
        [JsonIgnore]
        public DateTime? DateDeleted { get; set; }
    }
}
