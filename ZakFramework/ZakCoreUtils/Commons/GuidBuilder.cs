#if THINGS_YET_TO_TEST
using System;
using System.Runtime.InteropServices;

namespace ZakCore.Utils.Commons
{
	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public static class GuidBuilder
	{
		[ThreadStatic] private static GuidCnt _tmpGuid;

		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		private struct GuidCnt
		{
			[FieldOffset(0)] public Guid _guid;

			[FieldOffset(0)] public Int64 _int64_0;

			[FieldOffset(4)] public Int64 _int64_0_5;

			[FieldOffset(8)] public Int64 _int64_1;

			[FieldOffset(0)] public Int32 _int32_0;

			[FieldOffset(4)] public Int32 _int32_1;

			[FieldOffset(8)] public Int32 _int32_2;

			[FieldOffset(12)] public Int32 _int32_3;
		}

		public static Guid GenerateGuid(Int64 val)
		{
			_tmpGuid._int64_1 = val;
			_tmpGuid._int64_0 = 0;
			return _tmpGuid._guid;
		}

		public static bool SetGuid(UInt16 position, Int64 post)
		{
			switch (position)
			{
				case (0):
					_tmpGuid._int64_0 = post;
					break;
				case (4):
					_tmpGuid._int64_0_5 = post;
					break;
				case (8):
					_tmpGuid._int64_1 = post;
					break;
				default:
					return false;
			}
			return true;
		}

		public static bool SetGuid(Guid guid)
		{
			_tmpGuid._guid = guid;
			return true;
		}

		public static bool SetGuid(UInt16 position, Int32 post)
		{
			switch (position)
			{
				case (0):
					_tmpGuid._int32_0 = post;
					break;
				case (4):
					_tmpGuid._int32_1 = post;
					break;
				case (8):
					_tmpGuid._int32_2 = post;
					break;
				case (12):
					_tmpGuid._int32_3 = post;
					break;
				default:
					return false;
			}
			return true;
		}

		public static Guid GetGuid()
		{
			return _tmpGuid._guid;
		}
	}
}
#endif