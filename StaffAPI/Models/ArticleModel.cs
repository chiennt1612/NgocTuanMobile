using EntityFramework.API.Entities;
using System;
using System.Collections.Generic;

namespace StaffAPI.Models
{
    public class ArticleModel
    {
        public long Id { get; set; }
        public long CategoryMain { get; set; }
        public long? CategoryReference { get; set; }
        public string TagsReference { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public string ImgBanner { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Publisher { get; set; }
        public DateTime DateCreator { get; set; }
        public DateTime? DateModify { get; set; }
        public string Url { get; set; }
    }

    public class NewsDetails
    {
        public Article article { get; set; }
        public IList<Article> articles { get; set; }
        public IList<Article> articleRelated { get; set; }
    }
}
