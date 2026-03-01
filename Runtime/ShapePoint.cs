using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace CanvasPlus
{
	[BurstCompile]
	[StructLayout(LayoutKind.Explicit, Size = sizeof(float) * 4 + sizeof(byte) * 4)]
	public struct ShapePoint
	{
		public const int PositionOffset = 0;
		public const int NormalOffset = 8;
		public const int ColorOffset = 16;

		[FieldOffset(PositionOffset)] public float2 position;
		[FieldOffset(NormalOffset)] public float2 normal;
		[FieldOffset(ColorOffset)] public Color32 color;

		public ShapePoint(float2 position, float2 normal)
		{
			this.position = position;
			this.normal = normal;
			this.color = default;
		}

		public Vector3 GetVertexPosition()
			=> new Vector3(position.x, position.y, 0);

		public Vector3 GetVertexPosition(float offset)
			=> new Vector3(position.x + normal.x * offset, position.y + normal.y * offset, 0);

		public Vector3 GetVertexNormal()
			=> new Vector3(normal.x, normal.y, 0);

		public Vector4 GetVertexTangent()
			=> new Vector4(-normal.y, normal.x, 0, 0);
	}
}