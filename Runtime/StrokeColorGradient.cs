using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace CanvasPlus
{
	[BurstCompile]
	[System.Serializable]
	public class StrokeColorGradient : IStrokeColorProvider
	{
		private const float TauInv = 1f / math.TAU;

		[SerializeField] private Gradient gradient;

		[BurstCompile]
		public unsafe void Generate(ref Figure figure, int strokeIndex)
		{
			var stroke = (StrokePoint*)figure.GetStroke(strokeIndex).GetUnsafePtr();

			for (int i = 0; i < figure.shape.Length; ++i)
			{
				var clock = -figure.shape[i].normal;
				float t = math.atan2(clock.x, clock.y);
				stroke[i].color = gradient.Evaluate(math.mad(t, TauInv, 0.5f));
			}
		}
	}
}