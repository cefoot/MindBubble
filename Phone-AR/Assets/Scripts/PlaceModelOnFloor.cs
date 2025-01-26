using System;
using System.Collections;
using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.Samples.Geospatial;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceModelOnFloor : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public AREarthManager EarthManager;
    private bool _isPlaced = false;
    public UnityEvent ModelPlacedOnFloor;

    void Start()
    {

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager not found in the scene.");
        }
    }

    private void OnEnable()
    {
        StartCoroutine(PlaceOnFloorAsync());
    }

    private IEnumerator PlaceOnFloorAsync()
    {
        yield return new WaitUntil(() => EarthManager.EarthTrackingState >= TrackingState.Limited);
        // Find a horizontal plane in front of the user
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 4);
        System.Collections.Generic.List<ARRaycastHit> hits = new System.Collections.Generic.List<ARRaycastHit>();
        while (!_isPlaced)
        {
            if (raycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
            {
                // Get the hit pose
                Pose hitPose = hits[0].pose;

                var camDir = Camera.main.transform.position - hitPose.position;
                camDir.Scale(new Vector3(1f, 0f, 1f));
                // Optionally align it with the detected plane
                transform.rotation = Quaternion.LookRotation(camDir.normalized, Vector3.up);
                transform.position = hitPose.position;
                gameObject.AddComponent<ARAnchor>();

                // Lock the model to the plane
                //var anchor = hits[0].trackableId;
                //transform.parent = planeManager.GetPlane(anchor).transform;

                Debug.Log("Model placed at: " + hitPose.position);
                _isPlaced = true;
                ModelPlacedOnFloor?.Invoke();
                GeospatialController.Instance.PlaceOldKeywords(hitPose.position);
            }
            else
            {
                yield return new WaitForSecondsRealtime(.5f);
            }

        }
    }

    void Update()
    {
    }
}
