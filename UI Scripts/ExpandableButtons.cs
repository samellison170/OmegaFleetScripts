using UnityEngine;
using System.Collections.Generic;

public class ButtonTogglePopup : MonoBehaviour
{
    public List<GameObject> buttons; // Assign the button GameObjects in the inspector

    public void OnButtonClick(GameObject popup)
    {
        popup.SetActive(!popup.activeSelf);

    }
}
