using System;
using Unity.Collections;
using Unity.Mathematics;

namespace CanvasPlus
{
	public struct Figure : IDisposable
	{
		private const int MaxStrokesCount = 16;

		public readonly float2 center;
		public readonly float2 size;
		public readonly float2 extents;
		public readonly Allocator allocator;

		public NativeList<ShapePoint> shape;
		public NativeList<NativeArray<StrokePoint>> strokes;

		public Figure(float2 center, float2 size, Allocator allocator)
		{
			this.center = center;
			this.size = size;
			this.extents = size * .5f;
			this.allocator = allocator;

			shape = new NativeList<ShapePoint>(1 + 24 * 4, allocator);
			strokes = new NativeList<NativeArray<StrokePoint>>(MaxStrokesCount, allocator);
		}

		public NativeArray<StrokePoint> GetStroke(int strokeIndex)
		{
			if (strokeIndex >= MaxStrokesCount)
				throw new ArgumentOutOfRangeException(nameof(strokeIndex), $"Max supported strokes count is {MaxStrokesCount}");

			while (strokeIndex >= strokes.Length)
				strokes.Add(new NativeArray<StrokePoint>(shape.Length, allocator, NativeArrayOptions.UninitializedMemory));

			return strokes[strokeIndex];
		}

		public void Dispose()
		{
			shape.Dispose();

			foreach (var stroke in strokes)
				stroke.Dispose();

			strokes.Dispose();
		}
	}
}