using Domain.Hashtags;
using Domain.NewsCategories;
using Domain.NewsSections;

namespace Domain.News;

public class News
{
    public NewsId Id { get; private set; }
    public LocalizedString Title { get; set; }
    
    // SEO url також робимо двомовним, щоб мати /uk/novyna та /en/news
    public LocalizedString SeoUrl { get; set; } 
    public string PhotoUrl { get; set; }
    
    public LocalizedString Preface { get; set; }
    public LocalizedString Afterword { get; set; }
    
    public LocalizedString CtaButtonText { get; set; }
    public string CtaButtonLink { get; set; } // Зазвичай лінк один, але якщо потрібні різні - теж роби LocalizedString
    
    public bool IsImportant { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Зв'язки
    public NewsCategoryId CategoryId { get; set; }
    public NewsCategory? Category { get; set; }
    
    public ICollection<NewsSection> Sections { get; set; } = new List<NewsSection>();
    
    // Зв'язок багато-до-багатьох для хештегів
    public ICollection<Hashtag> Hashtags { get; set; } = new List<Hashtag>();

    private News(
        NewsId id, 
        LocalizedString title, 
        LocalizedString seoUrl, 
        string photoUrl, 
        LocalizedString preface, 
        LocalizedString afterword, 
        LocalizedString ctaButtonText, 
        string ctaButtonLink, 
        bool isImportant,
        NewsCategoryId categoryId)
    {
        Id = id;
        Title = title;
        SeoUrl = seoUrl;
        PhotoUrl = photoUrl;
        Preface = preface;
        Afterword = afterword;
        CtaButtonText = ctaButtonText;
        CtaButtonLink = ctaButtonLink;
        IsImportant = isImportant;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static News New(
        LocalizedString title, 
        LocalizedString seoUrl, 
        string photoUrl, 
        LocalizedString preface, 
        LocalizedString afterword, 
        LocalizedString ctaButtonText, 
        string ctaButtonLink, 
        bool isImportant,
        NewsCategoryId categoryId) =>
        new(NewsId.New(), title, seoUrl, photoUrl, preface, afterword, ctaButtonText, ctaButtonLink, isImportant, categoryId);

    public void Update(
        LocalizedString title, 
        LocalizedString seoUrl, 
        string photoUrl, 
        LocalizedString preface, 
        LocalizedString afterword, 
        LocalizedString ctaButtonText, 
        string ctaButtonLink, 
        bool isImportant,
        NewsCategoryId categoryId)
    {
        Title = title;
        SeoUrl = seoUrl;
        PhotoUrl = photoUrl;
        Preface = preface;
        Afterword = afterword;
        CtaButtonText = ctaButtonText;
        CtaButtonLink = ctaButtonLink;
        IsImportant = isImportant;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    // Методи для управління колекціями
    public void AddSection(NewsSection section) => Sections.Add(section);
    
    public void ClearSections() => Sections.Clear();

    public void UpdateHashtags(IEnumerable<Hashtag> newHashtags)
    {
        Hashtags.Clear();
        foreach (var tag in newHashtags)
        {
            Hashtags.Add(tag);
        }
    }

    public void ClearCategory() => Category = null;
}