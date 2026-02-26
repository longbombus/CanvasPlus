using System;

namespace CanvasPlus
{
	public enum RectSide : uint
	{
		Top    = 0b_00_00_00_00_00_00_00_00_00_00_00_00_00_00_00_00,
		Right  = 0b_01_01_01_01_01_01_01_01_01_01_01_01_01_01_00_00,
		Bottom = 0b_10_10_10_10_10_10_10_10_10_10_10_10_10_00_00_00,
		Left   = 0b_11_11_11_11_11_11_11_11_11_11_11_11_01_01_00_00,
	}

	public enum RectCorner : uint
	{
		TopLeft     = 0b_00_00_00_00_00_00_00_00_00_00_00_00_00_00_00_00,
		TopRight    = 0b_01_01_01_01_01_01_01_01_01_01_01_01_01_01_00_00,
		BottomRight = 0b_10_10_10_10_10_10_10_10_10_10_10_10_10_00_00_00,
		BottomLeft  = 0b_11_11_11_11_11_11_11_11_11_11_11_11_01_01_00_00,
	}

	public static class RectUtility
	{
		public static readonly RectSide[] Sides = (RectSide[])Enum.GetValues(typeof(RectSide));

		public static T DecodeValue<T>(this RectSide side, T[] values)
		{
			if (values == null || values.Length == 0)
				return default;

			return values[(uint)side >> (values.Length << 1)];
		}

		public static T DecodeValue<T>(this RectCorner corner, T[] values)
		{
			if (values == null || values.Length == 0)
				return default;

			return values[((uint)corner >> (values.Length << 1)) & 3u];
		}

		public static void ValidateArray<T>(ref T[] array)
		{
			if (array == null)
				array = Array.Empty<T>();
			else if (array.Length > 4)
				Array.Resize(ref array, 4);
		}
	}
}