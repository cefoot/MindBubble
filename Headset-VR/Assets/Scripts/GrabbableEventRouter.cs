using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class GrabbableEventRouter : MonoBehaviour
{
    private Grabbable _grabbable;

    public UnityEvent Grabbed;

    public UnityEvent Released;

    private void OnEnable()
    {
        _grabbable = GetComponent<Grabbable>();
        if (_grabbable == null)
        {
            Debug.LogError("Grabbable component is missing!");
            return;
        }

        // Subscribe to interaction events
        _grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent pointerEvent)
    {
        if (pointerEvent.Type == PointerEventType.Select)
        {
            Debug.Log("Object grabbed!");
            OnGrabStart();
        }
        else if (pointerEvent.Type == PointerEventType.Unselect)
        {
            Debug.Log("Object released!");
            OnGrabEnd();
        }
    }

    private void OnGrabStart()
    {
        // Your logic for when grabbing starts
        Grabbed?.Invoke();
    }

    private void OnGrabEnd()
    {
        // Your logic for when grabbing ends
        Released?.Invoke();
    }

    private void OnDestroy()
    {
        if (_grabbable != null)
        {
            _grabbable.WhenPointerEventRaised -= HandlePointerEvent;
        }
    }

}
