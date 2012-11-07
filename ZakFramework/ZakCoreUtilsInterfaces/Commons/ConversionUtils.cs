using System;
using System.Net;
using System.Text;

namespace ZakCore.Utils.Commons
{
	public class ConversionUtils
	{
		private ConversionUtils()
		{
		}

		public static DateTime MinDate = new DateTime(1800, 1, 1, 0, 0, 0);
		public const int SIZEOFDATE = sizeof (Int64);
		public const int SIZEOFGUID = 16;

		public static byte[] Date2Bytes(DateTime val)
		{
			return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val.Ticks));
		}

		public static DateTime Bytes2Date(byte[] val, int offset = 0)
		{
			return new DateTime(IPAddress.NetworkToHostOrder(BitConverter.ToInt64(val, offset)));
		}


		public static byte[] Guid2Bytes(Guid val)
		{
			return val.ToByteArray();
		}

		public static Guid Bytes2Guid(byte[] val, int offset = 0)
		{
			if (offset == 0)
			{
				return new Guid(val);
			}
			var vl = new byte[SIZEOFGUID];
			Array.Copy(val, offset, vl, 0, SIZEOFGUID);
			return new Guid(vl);
		}

		public static byte[] String2Bytes(String val, Encoding enc = null)
		{
			// ReSharper disable RedundantCast
			if (enc == null)
			{
				enc = Encoding.ASCII;
			}
			if (string.IsNullOrEmpty(val))
			{
				return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32) 0));
			}
			int cnt = enc.GetByteCount(val);
			var result = new byte[cnt + sizeof (Int32)];
			Array.Copy(enc.GetBytes(val), 0, result, sizeof (Int32), cnt);
			Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32) cnt)), 0, result, 0, sizeof (Int32));
			return result;
			// ReSharper restore RedundantCast
		}

		public static string Bytes2String(out int len, byte[] val, int offset = 0, Encoding enc = null)
		{
			// ReSharper disable RedundantAssignment
			len = 0;
			if (enc == null)
			{
				enc = Encoding.ASCII;
			}
			len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(val, offset));
			if (len == 0)
			{
				return string.Empty;
			}
			len += sizeof (Int32);
			/*if (len > (offset + val.Length + sizeof(Int32)))
			{
				len = val.Length - offset - sizeof(Int32);
			}*/
			return enc.GetString(val, offset + sizeof (Int32), len - sizeof (Int32));
			// ReSharper restore RedundantAssignment
		}

		public static Int32 DateTime2UnixTime(DateTime val)
		{
			TimeSpan ts = (val - new DateTime(1970, 1, 1, 0, 0, 0));
			return (Int32) ts.TotalSeconds;
		}
	}
}