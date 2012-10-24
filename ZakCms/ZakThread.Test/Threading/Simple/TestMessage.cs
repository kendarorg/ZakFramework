using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZakThread.Threading;

namespace ZakThread.Test.Threading.Simple
{
	public class TestMessage:IMessage
	{
		public Guid Id {get;set;}

		public DateTime TimeStamp { get; set; }
	}
}
