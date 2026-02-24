using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CanvasPlus
{
	public class TogglePlus : Toggle, ISelectablePlus
	{
		[SerializeField] private UnityEvent m_OnClick = new();
		public UnityEvent onClick => m_OnClick;

		[SerializeField] private UnityEvent m_OnClickNonInteractable;
		public UnityEvent onClickNonInteractable => m_OnClickNonInteractable;

		[SerializeField] private UnityEvent m_OnPress;
		public UnityEvent onPress => m_OnPress;

		[SerializeField] private UnityEvent m_OnRelease;
		public UnityEvent onRelease => m_OnRelease;

		private bool wasPressed;

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);

			var nowPressed = state == SelectionState.Pressed;
			if (nowPressed != wasPressed)
			{
				wasPressed = nowPressed;
				if (wasPressed)
					onPress.Invoke();
				else
					onRelease.Invoke();
			}
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!IsActive())
				return;

			if (IsInteractable())
			{
				base.OnPointerClick(eventData);
				m_OnClick.Invoke();
			}
			else
				m_OnClickNonInteractable.Invoke();
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			if (!IsActive())
				return;

			if (IsInteractable())
			{
				base.OnSubmit(eventData);
				m_OnClick.Invoke();
			}
			else
				m_OnClickNonInteractable.Invoke();
		}
	}
}