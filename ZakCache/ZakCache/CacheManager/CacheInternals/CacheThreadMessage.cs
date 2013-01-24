using System;
using ZakThread.Threading;

namespace ZakCache.CacheManager.CacheInternals
{
	public delegate object RetrieveDataCallback(object[] pars);
	
	public class CacheThreadMessage : IMessage
	{
		public CacheThreadMessage(string key, CacheMessageType messageType,params string[] tags)
		{
			Key = key;
			MessageType = messageType;
			Id = Guid.NewGuid();
			TimeStamp = DateTime.Now;
			Tags = tags;
			
		}

		public long CacheExpirationMs {get;set;}
		public string[] Tags { get; set; }
		public string Key { get; set; }
		public object Content { get; set; }
		public CacheMessageType MessageType { get; set; }
		public Guid Id { get; set; }
		public DateTime TimeStamp { get; set; }
		public string SourceThread { get; set; }
		public RetrieveDataCallback DataCallback {get;set;}
		public object[] DataCallbackParameters {get;set;}

		public object Clone()
		{
			var ret = new CacheThreadMessage(Key, MessageType, Tags);
			ret.Content = Content;
			ret.Id = Id;
			ret.TimeStamp = TimeStamp;
			ret.DataCallback = DataCallback;
			ret.DataCallbackParameters = ret.DataCallbackParameters;
			ret.SourceThread = SourceThread;
			return ret;
		}
	}
}
