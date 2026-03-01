using Domain.News;

namespace Domain.NewsSections;

public class NewsSection
{
    public NewsSectionId Id { get; private set; }
    public LocalizedString Title { get; set; }
    
    // Зміст: зберігатиме HTML або JSON від Next.js редактора (об'єднаний текст зі списками)
    public LocalizedString Content { get; set; } 
    public int Order { get; set; } // Для збереження послідовності розділів
    
    public NewsId NewsId { get; set; }
    public News.News? News { get; set; }

    private NewsSection(NewsSectionId id, LocalizedString title, LocalizedString content, int order, NewsId newsId)
    {
        Id = id;
        Title = title;
        Content = content;
        Order = order;
        NewsId = newsId;
    }

    public static NewsSection New(LocalizedString title, LocalizedString content, int order, NewsId newsId) =>
        new(NewsSectionId.New(), title, content, order, newsId);

    public void Update(LocalizedString title, LocalizedString content, int order)
    {
        Title = title;
        Content = content;
        Order = order;
    }
}