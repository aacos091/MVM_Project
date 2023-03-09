using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitcher : MonoBehaviour {
    public GameObject normalLight;
    public GameObject uvLight;
    public LightSelectEvent lightSelectEvent;

    public void SelectUVLight() {
        uvLight.SetActive(true);
        normalLight.SetActive(false);
    }

    public void SelectNormalLight() {
        normalLight.SetActive(true);
        uvLight.SetActive(false);
    }

    private void OnEnable() {
        lightSelectEvent.onUVLightSelected.AddListener(SelectUVLight);
        lightSelectEvent.onNormalLightSelected.AddListener(SelectNormalLight);
    }

    private void OnDisable() {
        lightSelectEvent.onUVLightSelected.RemoveListener(SelectUVLight);
        lightSelectEvent.onNormalLightSelected.RemoveListener(SelectNormalLight);
    }
}
