

using CAF.Infrastructure.Core.Collections;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Im
{
	public class AdminMenu : AdminMenuProvider
	{
		protected override void BuildMenuCore(TreeNode<MenuItem> pluginsNode)
		{
			var menuItem = new MenuItem().ToBuilder()
				.Text("IM Provider")
				.ResKey("Plugins.FriendlyName.CAF.WebSite.Im")
				.Icon("send-o")
				.Action("ConfigurePlugin", "Plugin", new { systemName = "CAF.WebSite.Im", area = "Admin" })
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
