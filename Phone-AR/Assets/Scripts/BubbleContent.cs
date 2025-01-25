using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.XR.ARCoreExtensions.Samples.Geospatial;
using NUnit.Framework;
using UnityEngine;

public class BubbleContent : MonoBehaviour
{

    public string[] Posts;

    public void Pop(bool createChildContent = true)
    {
        StartCoroutine(PopAsync(createChildContent));
    }

    private IEnumerator PopAsync(bool createChildContent)
    {
        if (createChildContent)
        {
            GeospatialController.Instance.CreateBubbles(Posts.ToDictionary(s => s, s => new System.Collections.Generic.List<string>()), Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.forward);
        }
        var sounds = GetComponentsInChildren<AudioSource>(true);
        var sound = sounds[UnityEngine.Random.Range(0, sounds.Length - 1)];
        sound.gameObject.SetActive(true);
        //wait until clip started to play
        yield return new WaitUntil(() => sound.isPlaying);
        //wait until clip finished playing
        yield return new WaitUntil(() => !sound.isPlaying);
        Destroy(gameObject);
    }
}
