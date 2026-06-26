using System;
using GezginTravel.Models.Identity;
using GezginTravel.Models.Enums;

namespace GezginTravel.Models.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; } // blog başlığı
        public string Slug { get; set; } // blog başlığının URL dostu versiyonu
        public string Content { get; set; } // blog içeriği

        public string ThumbnailUrl { get; set; } // card yapısında gösterilecek küçük resim URL'si
        public int EstimatedReadingTime { get; set; } // okuma süresi (dakika cinsinden)

        public int ViewCount { get; set; } // blog görüntülenme sayısı
        public int LikeCount { get; set; } // blog beğeni sayısı
        public int CommentCount { get; set; } // blog yorum sayısı
        public int SaveCount { get; set; } // blog kaydetme sayısı

        public decimal TrendScore { get; set; }     // blogun trend olma skorunu temsil eden bir değer
        public DateTime? TrendScoreLastCalculatedAt { get; set; } // trend skorunun en son ne zaman hesaplandığını gösteren bir tarih

        public BlogStatus Status { get; set; } = BlogStatus.Draft;
        // Foreign Keys
        public int AuthorId { get; set; } // blogu yazan kullanıcının ID'si
        public AppUser Author { get; set; } 

        public int? CityId { get; set; } // blogun hangi şehirle ilişkili olduğunu belirten opsiyonel bir ID
        public City City { get; set; }

        public int? CountryId { get; set; } // blogun hangi ülkeyle ilişkili olduğunu belirten opsiyonel bir ID
        public Country Country { get; set; }

        // Navigation Properties
        public ICollection<BlogImage> Images { get; set; }
        public ICollection<BlogComment> Comments { get; set; }
        public ICollection<BlogLike> Likes { get; set; }
        public ICollection<BlogSave> Saves { get; set; }
        public ICollection<BlogTag> BlogTags { get; set; }
        public ICollection<BlogCategory> BlogCategories { get; set; }
    }
}