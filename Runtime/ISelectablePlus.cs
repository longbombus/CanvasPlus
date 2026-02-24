using UnityEngine;
using UnityEngine.Events;

namespace CanvasPlus
{
	public interface ISelectablePlus
	{
		GameObject gameObject { get; }
		UnityEvent onClick { get; }
		UnityEvent onClickNonInteractable { get; }
		UnityEvent onPress { get; }
		UnityEvent onRelease { get; }
	}
}