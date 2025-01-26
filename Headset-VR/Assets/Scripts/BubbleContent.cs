using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using static UnityEngine.Application;

public class BubbleContent : MonoBehaviour
{

    public string[] Posts;

    public Action PopCallback;

    public void Pop(bool createChildContent = true)
    {
        StartCoroutine(PopAsync(createChildContent));
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
        PopCallback?.Invoke();
        Destroy(gameObject);
    }
}
