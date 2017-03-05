using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserveIcon : MonoBehaviour {
    [SerializeField]
    private Sprite observeIcon;

    public void OnClick() {
        if (Inventory.isItemSelected) {
            Inventory.SelectedItem.Interact(true);
        }
        else {
            Inventory.SelectObserveIcon(observeIcon);
        }
    }
}
