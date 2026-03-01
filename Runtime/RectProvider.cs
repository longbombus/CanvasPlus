using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace CanvasPlus
{
	[BurstCompile]
	[System.Serializable]
	public abstract class RectProvider : IShapeProvider, ISerializationCallbackReceiver
	{
		[SerializeField] private float[] m_Radius;

		public void OnBeforeSerialize()
			=> RectUtility.ValidateArray(ref m_Radius);

		public void OnAfterDeserialize()
			=> RectUtility.ValidateArray(ref m_Radius);

		[BurstCompile]
		protected float4 GetRadius(float2 rectSize)
		{
			// LT, RT, RB, LB
			var radius = new float4(
				RectCorner.TopLeft.DecodeValue(m_Radius),
				RectCorner.TopRight.DecodeValue(m_Radius),
				RectCorner.BottomRight.DecodeValue(m_Radius),
				RectCorner.BottomLeft.DecodeValue(m_Radius)
			);
			var radiusNormalizer = new float4(1);

			// T, R, B, L
			var sideRadius = radius + radius.yzwx;
			if (sideRadius.x > rectSize.x)
			{
				var normalizer = rectSize.x / sideRadius.x;
				radiusNormalizer.x = math.min(radiusNormalizer.x, normalizer);
				radiusNormalizer.y = math.min(radiusNormalizer.y, normalizer);
			}

			if (sideRadius.y > rectSize.y)
			{
				var normalizer = rectSize.y / sideRadius.y;
				radiusNormalizer.y = math.min(radiusNormalizer.y, normalizer);
				radiusNormalizer.z = math.min(radiusNormalizer.z, normalizer);
			}

			if (sideRadius.z > rectSize.x)
			{
				var normalizer = rectSize.x / sideRadius.z;
				radiusNormalizer.z = math.min(radiusNormalizer.z, normalizer);
				radiusNormalizer.w = math.min(radiusNormalizer.w, normalizer);
			}

			if (sideRadius.w > rectSize.y)
			{
				var normalizer = rectSize.y / sideRadius.w;
				radiusNormalizer.w = math.min(radiusNormalizer.w, normalizer);
				radiusNormalizer.x = math.min(radiusNormalizer.x, normalizer);
			}

			radius *= radiusNormalizer;

			return radius;
		}

		public abstract void Generate(ref Figure figure, float unitsPerPixel);
	}
}