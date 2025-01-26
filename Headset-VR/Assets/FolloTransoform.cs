using UnityEngine;

public class FolloTransoform : MonoBehaviour
{
    public Transform _targetTransform;
    public float Speed = .2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var targetPos = _targetTransform.position + Vector3.up * .05f;
        transform.position = Vector3.Lerp(transform.position, _targetTransform.position, Speed * Time.deltaTime);
        var lookDir = (Camera.main.transform.position - transform.position);
        lookDir.Scale(new Vector3(1f, 0f, 1f));
        lookDir.Normalize();
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), Speed * Time.deltaTime);
    }
}
