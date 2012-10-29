using System;

namespace ZakCore.Utils.Commons
{
	public static class BitUtils
	{
		public static bool IsSet(int value, int bitId)
		{
			bitId = (int) Math.Pow(2, bitId);
			return (value & bitId) != 0;
		}

		public static void Set(ref int value, int bitId)
		{
			bitId = (int) Math.Pow(2, bitId);
			value = value | bitId;
		}
	}
}