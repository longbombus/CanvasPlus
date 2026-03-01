using System;
using TypeDropdown;
using UnityEngine;

namespace CanvasPlus
{
	[Serializable]
	public struct StrokeStyle
	{
		[SerializeField] public StrokeAlignment alignment;
		[SerializeReference, TypeDropdown] public IStrokeWidthProvider width;
		[SerializeReference, TypeDropdown] public IStrokeColorProvider color;
	}
}