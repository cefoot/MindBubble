using System;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using TMPro;
using UnityEngine;

public class PopPoke : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var hand = other.GetComponentInParent<Hand>();
        if (!hand) return;
        //if (!hand.IsPointerPoseValid)
        //GetComponentInChildren<TextMeshPro>().text = $"Pointer: {other.name} ({hdn?.Handedness})";
    }

}
