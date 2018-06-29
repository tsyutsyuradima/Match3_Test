using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class CustomButton : Button
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        audioSource.Play();
        base.OnPointerDown(eventData);
    }
}
