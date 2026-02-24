using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CanvasPlus
{
	public class ButtonPlus : Button, ISelectablePlus
	{
		UnityEvent ISelectablePlus.onClick => base.onClick;

		[SerializeField] private UnityEvent m_onClickNonInteractable;
		public UnityEvent onClickNonInteractable => m_onClickNonInteractable;

		[SerializeField] private UnityEvent m_onPress;
		public UnityEvent onPress => m_onPress;

		[SerializeField] private UnityEvent m_onRelease;
		public UnityEvent onRelease => m_onRelease;

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
				base.OnPointerClick(eventData);
			else
				onClickNonInteractable.Invoke();
		}

		public override void OnSubmit(BaseEventData eventData)
		{
			if (!IsActive())
				return;

			if (IsInteractable())
				base.OnSubmit(eventData);
			else
				onClickNonInteractable.Invoke();
		}
	}
}