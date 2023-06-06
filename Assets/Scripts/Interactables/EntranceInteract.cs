using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceInteract : MonoBehaviour, IInteractable
{
    public int sceneNumber;
    public bool isLocked;
    private AudioSource _playerAudio;
    public AudioClip openSound, lockedSound;

    void Start()
    {
        _playerAudio = GetComponent<AudioSource>();
    }
    public void Interact()
    {
        if (!isLocked)
        {
            StartCoroutine(GameManager.instance.NewScene(sceneNumber));
            _playerAudio.PlayOneShot(openSound, 10);
        }
        else
        {
            _playerAudio.PlayOneShot(lockedSound, 10);
        }
    }

}
