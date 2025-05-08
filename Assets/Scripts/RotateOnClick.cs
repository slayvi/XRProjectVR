using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RotateOnClick : MonoBehaviour
{
    public float rotationAngle = 90f;
    private XRBaseInteractable interactable;

    void Start()
    {
        // Get the XRBaseInteractable component and attach a listener for the click event
        interactable = GetComponent<XRBaseInteractable>();

        // Check if the interactable component is found
        if (interactable != null)
        {
            Debug.Log("Interactable component found on prefab.");
            interactable.selectEntered.AddListener(OnClicked);
        }
        else
        {
            Debug.LogError("No XRBaseInteractable found on the object.");
        }
    }

    private void OnClicked(SelectEnterEventArgs args)
    {
        // Rotate the object by 90 degrees on the Y-axis
        Debug.Log("Object clicked! Rotating now.");
        transform.Rotate(0, rotationAngle, 0);
    }

    private void OnDestroy()
    {
        // Ensure to remove the listener when the object is destroyed
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnClicked);
        }
    }
}
