using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace CanvasPlus
{
	[BurstCompile]
	[System.Serializable]
	public class StrokeWidthRectSides : IStrokeWidthProvider
	{
		[SerializeField] private float[] width;

		[BurstCompile]
		public unsafe void Generate(ref Figure figure, int strokeIndex)
		{
			float4 sides = new float4(
				RectSide.Right.DecodeValue(width),
				RectSide.Top.DecodeValue(width),
				RectSide.Left.DecodeValue(width),
				RectSide.Bottom.DecodeValue(width)
			);

			var stroke = (StrokePoint*)figure.GetStroke(strokeIndex).GetUnsafePtr();

			for (int i = 0; i < figure.shape.Length; ++i)
			{
				float2 normal = figure.shape[i].normal;
				float4 weights = math.max(float4.zero, new float4(normal, -normal));

				float weightSum = math.csum(weights);
				if (weightSum > math.EPSILON)
					stroke[i].width = math.dot(sides, weights) / weightSum;
			}
		}
	}
}