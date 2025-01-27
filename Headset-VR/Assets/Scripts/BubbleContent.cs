using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using static UnityEngine.Application;

public class BubbleContent : MonoBehaviour
{

    public string[] Posts;

    public Action<string> PopCallback;

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("pop"))
        {
            Pop();
        }
        GUILayout.EndVertical();
    }
#endif

    public void Pop(bool createChildContent = true)
    {
        StartCoroutine(PopAsync(createChildContent));
    }

    public string Text
    {
        get
        {
            return GetComponentInChildren<TMP_Text>().text;
        }
        set
        {
            GetComponentInChildren<TMP_Text>().text = value;
        }
    }

    private IEnumerator PopAsync(bool createChildContent)
    {
        if (createChildContent)
        {
            AppController.Instance.CreateBubbles(Posts.ToDictionary(s => s, s => new System.Collections.Generic.List<string>()), Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.forward, .5f);
        }
        var sounds = GetComponentsInChildren<AudioSource>(true);
        var sound = sounds[UnityEngine.Random.Range(0, sounds.Length - 1)];
        sound.gameObject.SetActive(true);
        //wait until clip started to play
        yield return new WaitUntil(() => sound.isPlaying);
        //wait until clip finished playing
        yield return new WaitUntil(() => !sound.isPlaying);
        PopCallback?.Invoke(Text);
        Destroy(gameObject);
    }
}
