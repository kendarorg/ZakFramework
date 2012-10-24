namespace ZakWeb.Utils.Renderers
{
	public interface IRenderer
	{
		string Render(string toRender, string siteRoot, string imagesRoot);
	}
}