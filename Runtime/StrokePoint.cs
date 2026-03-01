using System.Runtime.InteropServices;
using UnityEngine;

namespace CanvasPlus
{
	[StructLayout(LayoutKind.Explicit, Size = sizeof(float) + sizeof(byte) * 4)]
	public struct StrokePoint
	{
		public const int WidthOffset = 0;
		public const int ColorOffset = 4;

		[FieldOffset(WidthOffset)] public float width;
		[FieldOffset(ColorOffset)] public Color32 color;
	}
}