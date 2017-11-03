using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryItem : MonoBehaviour {
    //make this into a queue or a list in case we need to process more than one image
    //currently trying to work with one item 
    //i might just be able to make this a local variable to renderItemSprite
    private static Image gameObjectImage;
    private static bool itemRendered = false;
    int counter = 0;
    int animationLoop = 0;

    void Start() {
        gameObjectImage = gameObject.GetComponent<Image>();   
    }
    /// <summary>
    /// attach the prefab's sprite to current object's sprite in image component and render it
    /// </summary>
    public static void renderItemSprite(InventoryItem prefab) {
        // if not currently in a dialogue: !isDialogueAction
        Image prefabImage = prefab.GetComponent<Image>();
        gameObjectImage.sprite = prefabImage.sprite;
        itemRendered = true;
    
    }
   
    private void Update() {
        if (itemRendered) {
           if (counter >= 0 || counter < 60) {
                gameObjectImage.transform.Translate(0.0f, 10.0f * Time.deltaTime, 0.0f);
                Debug.Log("if " + counter);
                counter++;
            }
           if (counter >= 60) {
                gameObjectImage.transform.Translate(0.0f, -20.0f* Time.deltaTime  , 0.0f); 
                Debug.Log("else if " + counter);
                counter++;
            }
            if (counter > 200 && animationLoop != 2) {
                counter = 0;
                animationLoop++;
            }
            else if (counter > 200 && animationLoop == 2) {
                gameObjectImage.sprite = null;
                itemRendered = false;
            }
        }
        
    }//end of Update
}
