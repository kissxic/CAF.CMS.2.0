

using CAF.WebSite.Application.WebUI.Mvc;
namespace CAF.WebSite.Mvc.Models.Users
{
    public partial class UserGradeModel : ModelBase
    {
        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public string UserGradeId { get; set; }

        public string UserGrade { get; set; }

        public UserNavigationModel NavigationModel { get; set; }
    }
}