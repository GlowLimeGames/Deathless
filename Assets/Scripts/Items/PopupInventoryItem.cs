using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryItem : MonoBehaviour {
    //make this into a queue or a list in case we need to process more than one image
    private static Image gameObjectImage;
    //possibly create a queue
    //also check for condition if currently in a dialogue 

    void Start() {
        gameObjectImage = gameObject.GetComponent<Image>();   
    }
    /// <summary>
    /// attach the prefab's sprite to current object's sprite in image component
    /// </summary>
    /// <param name="prefab"></param>
    public static void renderItemSprite(InventoryItem prefab) {
        Image prefabImage = prefab.GetComponent<Image>();
        gameObjectImage.sprite = prefabImage.sprite;
    }


}
