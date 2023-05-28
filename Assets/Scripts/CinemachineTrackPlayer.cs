using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineTrackPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera c_VirtualCamera;
    //[SerializeField] private Transform target;
    private PlayerController player;
 
    private void Awake()
    {
        c_VirtualCamera = GetComponent<CinemachineVirtualCamera>();
        player = FindObjectOfType<PlayerController>();
    }
    private void Start()
    {
        //c_VirtualCamera.m_LookAt = player.transform;
        c_VirtualCamera.m_Follow = player.transform;
    }
}
