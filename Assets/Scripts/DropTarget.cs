using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DropTarget : MonoBehaviour, IDropHandler
{
	public bool DropEnabled;

	public GameObjectEvent OnDropEvent;

	public void OnDrop(PointerEventData eventData)
	{
		if (DropEnabled && Draggable.CurrentDragging != null && Draggable.CurrentDragging != gameObject)
		{
			OnDropEvent.Invoke(Draggable.CurrentDragging);
		}
	}


}
