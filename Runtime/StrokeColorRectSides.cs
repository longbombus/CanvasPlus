using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace CanvasPlus
{
	[BurstCompile]
	[System.Serializable]
	public class StrokeColorRectSides : IStrokeColorProvider
	{
		[SerializeField] private Color32[] color;

		[BurstCompile]
		public unsafe void Generate(ref Figure figure, int strokeIndex)
		{
			float4 maxByteValue = new float4(255.999f);

			var right = RectSide.Right.DecodeValue(color);
			var top = RectSide.Top.DecodeValue(color);
			var left = RectSide.Left.DecodeValue(color);
			var bottom = RectSide.Bottom.DecodeValue(color);

			float4x4 sides = new float4x4(
				right.r, right.g, right.b, right.a,
				top.r, top.g, top.b, top.a,
				left.r, left.g, left.b, left.a,
				bottom.r, bottom.g, bottom.b, bottom.a
			);

			var stroke = (StrokePoint*)figure.GetStroke(strokeIndex).GetUnsafePtr();

			for (int i = 0; i < figure.shape.Length; ++i)
			{
				float2 normal = figure.shape[i].normal;
				float4 weights = new float4(
					math.max(float2.zero, normal),
					math.max(float2.zero, -normal)
				);

				float weightSum = math.csum(weights);
				if (weightSum > math.EPSILON)
				{
					var colorVector = math.clamp(math.mul(weights, sides) / weightSum, float4.zero, maxByteValue);
					stroke[i].color = new Color32((byte)colorVector.x, (byte)colorVector.y, (byte)colorVector.z, (byte)colorVector.w);
				}
			}
		}
	}
}