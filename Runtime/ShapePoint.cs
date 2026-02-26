using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace CanvasPlus
{
	[BurstCompile]
	public readonly struct ShapePoint
	{
		private readonly float2 m_Position;
		private readonly float2 m_Normal;

		public Vector3 position => new Vector3(m_Position.x, m_Position.y, 0);
		public Vector3 normal => new Vector3(m_Normal.x, m_Normal.y, 0);
		public Vector4 tangent => new Vector4(-normal.y, normal.x, 0, 0);

		public ShapePoint(float2 position, float2 normal)
		{
			m_Position = position;
			m_Normal = normal;
		}
	}
}