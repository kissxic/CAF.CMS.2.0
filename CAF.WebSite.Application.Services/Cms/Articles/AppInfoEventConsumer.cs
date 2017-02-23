
using CAF.Infrastructure.Core.Domain.Cms.Articles;
using CAF.Infrastructure.Core.Events;
using System;

namespace CAF.WebSite.Application.Services.Articles
{
    public class ArticleEventConsumer : IConsumer<ArticleViewEvent>
    {
        private readonly IArticleService _articleService;
        public ArticleEventConsumer(IArticleService articleService)
        {
            this._articleService = articleService;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(ArticleViewEvent eventMessage)
        {
            var article = _articleService.GetArticleById(eventMessage.ArticleId);
            article.Click = eventMessage.ArticleClick + 1;
            _articleService.UpdateArticle(article);
        }
    }
}