using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAF.WebSite.Application.Services.Articles
{
    public class ArticleViewEvent
    {
        public ArticleViewEvent(int articleId, int articleClick)
        {
            this.ArticleId = articleId;
            this.ArticleClick = articleClick;
        }
        public int ArticleId { get; set; }

        public int ArticleClick { get; set; }
    }
}
