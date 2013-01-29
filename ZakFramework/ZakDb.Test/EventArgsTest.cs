using NUnit.Framework;
using ZakDb.Repositories.Queries;
using ZakDb.Repositories.Utils;

namespace ZakDb.Test
{
	[TestFixture]
	public class EventArgsTest
	{
		[Test]
		public void ShouldBeAbleToBuildCustomEventArgs()
		{
			var a = new OnExecuteActionEventArgs("action", "p1", "p2");
			Assert.AreEqual("action",a.ActionName);
			Assert.AreEqual("p1", a.Parameters[0]);
			Assert.AreEqual("p2", a.Parameters[1]);

			a = new OnExecuteActionEventArgs("action");
			Assert.AreEqual("action", a.ActionName);
			Assert.IsNotNull(a.Parameters);
			Assert.AreEqual(0, a.Parameters.Length);

			var b = new OnPostCreateEventArgs("action");
			Assert.AreEqual("action", b.Item);

			var c = new OnPostDeleteEventArgs(long.MaxValue);
			Assert.AreEqual(long.MaxValue, c.Id);

			var d = new OnPostInstantiateEventArgs("action");
			Assert.AreEqual("action", d.Item);

			var e = new OnPostUpdateEventArgs("action");
			Assert.AreEqual("action", e.Item);

			var f = new OnPreCreateEventArgs("action");
			Assert.AreEqual("action", f.Item);

			var g = new OnPreDeleteEventArgs(long.MaxValue);
			Assert.AreEqual(long.MaxValue, g.Id);

			var qo = new QueryObject();
			var h = new OnPreSelectEventArgs(qo);
			Assert.AreSame(qo, h.Query);

			var i = new OnPreUpdateEventArgs("action");
			Assert.AreEqual("action", i.Item);

			var j = new OnVerifyEventArgs("action","operation");
			Assert.AreEqual("action", j.Item);
			Assert.AreEqual("operation", j.Operation);
		}
	}
}
