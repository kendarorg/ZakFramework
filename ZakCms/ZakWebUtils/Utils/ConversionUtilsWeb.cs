using System;
using System.Text;
using System.Web.Script.Serialization;
using ZakCore.Utils.Commons;

namespace ZakWeb.Utils.Utils
{
	public static class ConversionUtilsWeb
	{

		public static byte[] Jsonizable2Bytes(Object par, Encoding enc = null)
		{
			var jss = new JavaScriptSerializer();
			return ConversionUtils.String2Bytes(jss.Serialize(par), enc);
		}

		public static T Bytes2Jsonizable<T>(out int len, byte[] val, int offset = 0, Encoding enc = null)
		{
			var jss = new JavaScriptSerializer();
			string vals = ConversionUtils.Bytes2String(out len, val, offset, enc);
			return jss.Deserialize<T>(vals);
		}
	}
}
