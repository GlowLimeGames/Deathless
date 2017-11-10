using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryItem : MonoBehaviour {
    private static Image gameObjectImage;
    //checks if item image has been rendered yet
    private static bool itemRendered = false;
    [SerializeField]
    private float speedOfBounce;
    [SerializeField]
    private float bounceMagnitude; 

    void Start() {
        gameObjectImage = gameObject.GetComponent<Image>();
        //set image initially to transparent
        Color color = gameObjectImage.color;
        color.a = 0;
        gameObjectImage.color = color;
        speedOfBounce = 0.03f;
        bounceMagnitude = 20.0f;
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
        Debug.Log("GoUp");
        for (int i = 0; i < 15; i++) {
            gameObjectImage.transform.Translate(0.0f, bounceMagnitude * Time.deltaTime, 0.0f);
            //yield return new WaitForSeconds(speedOfBounce);
            yield return null;
        }
     }

    private IEnumerator GoDown() {
        Debug.Log("Go Down");
        for (int i = 0; i < 15; i++) {
            gameObjectImage.transform.Translate(0.0f, -bounceMagnitude * Time.deltaTime, 0.0f);
            //yield return new WaitForSeconds(speedOfBounce);
            yield return null;
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
        //create separate method for this
        gameObjectImage.sprite = null;
        Color currcolor = gameObjectImage.color;
        currcolor.a = 0;
        gameObjectImage.color = currcolor;
    }

    public void StopAnimation(bool dialogueActive) {
        if (dialogueActive) {
            StopAllCoroutines();
            SetImageActive(true);
        } 
    }

    private void SetImageActive(bool destroyImage ) {
        if (destroyImage) {
            gameObjectImage.enabled = false;
        }
        else { gameObjectImage.enabled = true; }
    }

    //stopanimation
    //stopallcoroutines
}