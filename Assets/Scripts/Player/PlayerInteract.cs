using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private bool _nearInteractable;
    private IInteractable _interactableObject;

    private void OnTriggerEnter2D(Collider2D col)
    {
        _nearInteractable = true;
        _interactableObject = col.gameObject.GetComponent<IInteractable>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _nearInteractable = false;
        _interactableObject = null;
    }

    private void Update()
    {
        if (_nearInteractable && Input.GetKeyDown(KeyCode.E))
        {
            if (_interactableObject != null)
            {
                _interactableObject.Interact();
            }
        }
    }
}
