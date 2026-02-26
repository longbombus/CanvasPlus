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

			// T, R, B, L
			var sideRadius = radius + radius.yzwx;
			if (sideRadius.x > rectSize.x)
			{
				var normalizer = rectSize.x / sideRadius.x;
				radius.x *= normalizer;
				radius.y *= normalizer;
			}

			if (sideRadius.y > rectSize.y)
			{
				var normalizer = rectSize.y / sideRadius.y;
				radius.y *= normalizer;
				radius.z *= normalizer;
			}

			if (sideRadius.z > rectSize.x)
			{
				var normalizer = rectSize.x / sideRadius.z;
				radius.z *= normalizer;
				radius.w *= normalizer;
			}

			if (sideRadius.w > rectSize.y)
			{
				var normalizer = rectSize.y / sideRadius.w;
				radius.w *= normalizer;
				radius.x *= normalizer;
			}

			return radius;
		}

		public abstract void Generate(NativeList<ShapePoint> shape, in float2 center, in float2 size, float unitsPerPixel);
	}
}