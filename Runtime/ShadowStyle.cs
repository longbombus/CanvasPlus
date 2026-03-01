using UnityEngine;

namespace CanvasPlus
{
	[System.Serializable]
	public struct ShadowStyle
	{
		[SerializeField] public Color32 color;
		[SerializeField] public Vector2 offset;
		[SerializeField] public float blur;
		[SerializeField] public float spread;
	}
}