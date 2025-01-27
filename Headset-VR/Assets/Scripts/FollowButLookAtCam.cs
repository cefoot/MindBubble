using UnityEngine;

public class FollowButLookAtCam : MonoBehaviour
{
    public Transform _targetTransform;
    public float Speed = .2f;

    public bool IgnoreYRotation = true;

    // Update is called once per frame
    void Update()
    {
        if (_targetTransform)
        {
            var targetPos = _targetTransform.position + Vector3.up * .05f;
            transform.position = Vector3.Lerp(transform.position, _targetTransform.position, Speed * Time.deltaTime);
        }
        var lookDir = (Camera.main.transform.position - transform.position);
        if (IgnoreYRotation)
            lookDir.Scale(new Vector3(1f, 0f, 1f));
        lookDir.Normalize();
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Speed * Time.deltaTime);
    }
}
