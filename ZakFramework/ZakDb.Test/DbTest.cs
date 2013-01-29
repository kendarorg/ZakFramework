using NUnit.Framework;
using ZakDb.Plugins;

namespace ZakDb.Test
{
	[TestFixture]
	public class DbTest
	{
			[Test]
			public void ItShouldBePossibleToCreateAPluginOutcome()
			{
				var po = new PluginOutcome();
				Assert.IsFalse(po.Success);
				Assert.IsNull(po.Result);
			}
	}
}
