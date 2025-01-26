using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ButtonRouter : MonoBehaviour
{
    public TMP_InputField TextSource;

    [SerializeField]
    public UnityEvent<string> StringClick;
    public void DoStringClick()
    {
        StringClick?.Invoke(TextSource.text);
    }

}
