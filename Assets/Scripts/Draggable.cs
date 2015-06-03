using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ICanvasRaycastFilter
{
	public static GameObject CurrentDragging;

	public bool DragEnabled;
	public bool FollowPointer;
	public Vector3 DragOffset;
	public string RootObject;

	[SerializeField]
	private UnityEvent OnBeginDragEvent;
	[SerializeField]
	private UnityEvent OnEndDragEvent;

	private Transform mRootObject;
	private Vector3 mPointerOffset;
	private Transform mStartParent;
	private int mStartSiblingIndex;
	private Vector3 mStartPosition;


	public void OnEnable()
	{
		GameObject root = GameObject.FindGameObjectWithTag(RootObject);
		if(root != null) mRootObject = root.transform;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!DragEnabled) return;
		if (FollowPointer)
		{
			mStartParent = transform.parent;
			mStartSiblingIndex = transform.GetSiblingIndex();
			mStartPosition = transform.localPosition;

			transform.SetParent(mRootObject, true);
			RectTransformUtility.ScreenPointToWorldPointInRectangle(mRootObject as RectTransform, eventData.position, eventData.pressEventCamera, out mPointerOffset);
			mPointerOffset -= transform.position;
			mPointerOffset += DragOffset;
		}
		CurrentDragging = gameObject;
		OnBeginDragEvent.Invoke();
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!DragEnabled)
		{
			OnEndDrag(eventData);
			return;
		}
		if (FollowPointer)
		{
			Vector3 pointerPos;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(mRootObject as RectTransform, eventData.position, eventData.pressEventCamera, out pointerPos);
			transform.position = pointerPos - mPointerOffset;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (CurrentDragging != gameObject) return;

		if (FollowPointer && transform.parent == mRootObject)
		{
			transform.SetParent(mStartParent, true);
			transform.SetSiblingIndex(mStartSiblingIndex);
			transform.localPosition = mStartPosition;
		}

		OnEndDragEvent.Invoke();

		CurrentDragging = null;

	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		return DragEnabled && CurrentDragging != gameObject;
	}

}
