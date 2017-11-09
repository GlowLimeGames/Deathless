using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryItem : MonoBehaviour {
    //used to access the image component for this script
    private static Image gameObjectImage;

    //boolean to keep track whether renderItemSprite has been called
    //checks if item image has been rendered yet
    private static bool itemRendered = false;
    
    void Start() {
        gameObjectImage = gameObject.GetComponent<Image>();
        //set image initially to transparent
        Color color = gameObjectImage.color;
        color.a = 0;
        gameObjectImage.color = color;
    }

    /// <summary>
    /// attach the prefab's sprite to current object's sprite in image component and render it
    /// </summary>
    public static void renderItemSprite(InventoryItem prefab) {
        // if not currently in a dialogue: !isDialogueAction
        Image prefabImage = prefab.GetComponent<Image>();
        gameObjectImage.sprite = prefabImage.sprite;
        Color color = gameObjectImage.color;
        color.a = 1;
        gameObjectImage.color = color;
        itemRendered = true;
    }

    //I originally wanted to call startcoroutine inside renderItemSprite, but because
    //StartCoroutine cannot be called in static context, I am calling it in Update function (which might be part of my problem)
    private void Update() {
        if (itemRendered) { StartCoroutine(AnimateSprite()); } 
    }

   private IEnumerator GoUp() {
        for (int i = 0; i < 4; i++) {
            gameObjectImage.transform.Translate(0.0f, 10.0f * Time.deltaTime, 0.0f);
            yield return new WaitForSeconds(0.2f);
        }
        
     }

    private IEnumerator GoDown() {
        for (int i = 0; i < 4; i++) {
            gameObjectImage.transform.Translate(0.0f, -10.5f * Time.deltaTime, 0.0f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator AnimateSprite() {
        itemRendered = false;
        //animate the up and down motion just 4 times
        for (int animLoop = 0; animLoop < 4; animLoop++) {
            yield return StartCoroutine(GoUp());
            yield return StartCoroutine(GoDown());
            yield return new WaitForSeconds(0.1f);
        }
        //delete sprite image from Image component
        gameObjectImage.sprite = null;
        Color currcolor = gameObjectImage.color;
        currcolor.a = 0;
        gameObjectImage.color = currcolor;
    }
}