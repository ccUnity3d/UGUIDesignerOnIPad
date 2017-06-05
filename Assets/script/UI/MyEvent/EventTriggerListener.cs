using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EventTriggerListener :EventTrigger {
    public Button my;
	public delegate void VoidDelegate();
	public VoidDelegate OnClick;
	public VoidDelegate OnDown;
	public VoidDelegate OnEnter;
	public VoidDelegate OnExit;
	public VoidDelegate OnUp;
	public VoidDelegate Onselect;
	public VoidDelegate OnUpdateselect;
	// Use this for initialization
	public static EventTriggerListener Get(GameObject go)
	{
        if (go.GetComponent<EventTriggerListener>()==null)
        {
            go.AddComponent<EventTriggerListener>();
        }
        EventTriggerListener listener = go.GetComponent<EventTriggerListener> ();
		return listener;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (OnClick != null) {
			OnClick();
		}
	}
	public override void OnPointerDown (PointerEventData eventData)
	{
		if (OnDown != null) {
			OnDown();
		}
	}
	public override void OnPointerExit (PointerEventData eventData)
	{
		if (OnExit != null) {
			OnExit();
		}
	}
	public override void OnPointerEnter (PointerEventData eventData)
	{
		if (OnEnter != null) {
			OnEnter();
		}
	}
	public override void OnPointerUp (PointerEventData eventData)
	{
		if (OnUp != null) {
			OnUp();
		}
	}
	public override void OnSelect(BaseEventData  eventData)
	{
		if (Onselect != null) {
			Onselect();
		}
	}
	public override void OnUpdateSelected(BaseEventData  eventData)
	{
		if (OnUpdateselect != null) {
			OnUpdateselect();
		}
	}
}
