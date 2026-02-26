using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace CanvasPlus
{
	[BurstCompile]
	[System.Serializable]
	public class RoundedRect : RectProvider
	{
		[BurstCompile]
		public override void Generate(
			NativeList<ShapePoint> line,
			in float2 center,
			in float2 size,
			float unitsPerPixel
		)
		{
			var extents = size * .5f;
			var radius = GetRadius(size);

			GenerateCorner(line, center, new float2(-extents.x, extents.y), radius.x, unitsPerPixel, -math.PIHALF);
			GenerateCorner(line, center, new float2(extents.x, extents.y), radius.y, unitsPerPixel, 0);
			GenerateCorner(line, center, new float2(extents.x, -extents.y), radius.z, unitsPerPixel, math.PIHALF);
			GenerateCorner(line, center, new float2(-extents.x, -extents.y), radius.w, unitsPerPixel, math.PI);
		}

		[BurstCompile]
		private void GenerateCorner(NativeList<ShapePoint> line, float2 rectCenter, float2 rectExtents, float radius, float unitsPerPixel, float beginAngle)
		{
			int segmentsCount = (int)(1.125f * math.sqrt(radius / unitsPerPixel) + 1) * 2;
			int arcSegmentsCount = segmentsCount - 1;
			float angleRate = math.PIHALF / (arcSegmentsCount - 1);
			float2 normal = default;

			switch (segmentsCount)
			{
				case < 3:
					math.sincos(beginAngle + math.PIHALF * .5f, out normal.x, out normal.y);
					line.Add(new ShapePoint(rectCenter + rectExtents, normal));
					math.sincos(beginAngle + math.PIHALF, out normal.x, out normal.y);
					break;

				default:
				{
					var radius2 = new float2(radius);
					var circleDirection = -math.chgsign(radius2, rectExtents);
					var circleCenter = rectCenter + rectExtents + circleDirection;
					for (int i = 0; i < arcSegmentsCount; ++i)
					{
						float angle = i * angleRate + beginAngle;
						math.sincos(angle, out normal.x, out normal.y);
						line.Add(new ShapePoint(math.mad(normal, radius2, circleCenter), normal));
					}
					break;
				}
			}

			line.Add(new ShapePoint(rectCenter + math.abs(rectExtents) * normal, normal));
		}
	}
}