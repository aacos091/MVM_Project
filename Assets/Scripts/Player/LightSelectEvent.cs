using UnityEngine;
using UnityEngine.Events;

// Add listener to events in LightSwitcher.cs
// Use this scriptable object to invoke the light selection events from any monobehaviour scripts
[CreateAssetMenu(fileName = "LightSelectEvent", menuName = "ScriptableObjects/LightSelectEvent", order = 0)]
public class LightSelectEvent : ScriptableObject {
    public UnityEvent onUVLightSelected;
    public UnityEvent onNormalLightSelected;

    public void InvokeSelectUVLight() {
        onUVLightSelected.Invoke();
    }

    public void InvokeSelectNormalLight() {
        onNormalLightSelected.Invoke();
    }
}