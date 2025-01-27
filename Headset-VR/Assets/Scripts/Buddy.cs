using UnityEngine;

public class Buddy : MonoBehaviour
{

    public static Buddy Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
