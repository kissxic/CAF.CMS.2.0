

using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.PageSettings
{
    public class AdminMenu : AdminMenuProvider
    {
        protected override void BuildMenuCore(TreeNode<MenuItem> pluginsNode)
        {
            var menuItem = new MenuItem().ToBuilder()
                .Text("SMS Provider")
                .ResKey("Plugins.FriendlyName.CAF.WebSite.PageSettings")
                .Icon("send-o")
                .Action("ConfigurePlugin", "Plugin", new { systemName = "CAF.WebSite.PageSettings", area = "Admin" })
                .ToItem();

            pluginsNode.Prepend(menuItem);
        }

        public override int Ordinal
        {
            get
            {
                return 100;
            }
        }

    }
}
