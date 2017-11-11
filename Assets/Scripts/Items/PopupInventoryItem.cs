using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryItem : MonoBehaviour {
    private static Image gameObjectImage;
    private static Vector3 originalPos; 

    //checks if item image has been rendered yet
    private static bool itemRendered = false;
    [SerializeField]
    private float bounceMagnitude;
    [SerializeField]
    private float maxHeight;

    void Start() {
        gameObjectImage = gameObject.GetComponent<Image>();
        originalPos = new Vector3(gameObjectImage.transform.position.x, gameObjectImage.transform.position.y, gameObjectImage.transform.position.z);
        SetImageActive(false);
        bounceMagnitude = 1.0f;
        maxHeight = 5.0f;
    }

    /// <summary>
    /// attach the prefab's sprite to current object's sprite in image component and render it
    /// </summary>
    public static void renderItemSprite(InventoryItem prefab) {
        Image prefabImage = prefab.GetComponent<Image>();
        SetImageSprite(prefabImage.sprite);
        SetImageActive(true);
        itemRendered = true;
    }

   
    private void Update() {
        if (itemRendered) { StartCoroutine(AnimateSprite()); } 
    }

   private IEnumerator GoUp() {
        float currHeight = gameObjectImage.transform.position.y;
        Debug.Log("currHeight in GoUp: " + currHeight);
        while (currHeight < maxHeight) {
            gameObjectImage.transform.Translate(0.0f, bounceMagnitude * Time.deltaTime, 0.0f);
            currHeight = gameObjectImage.transform.position.y;
            yield return null;
        }
     }

    private IEnumerator GoDown() {
        float currHeight = gameObjectImage.transform.position.y;
        while (currHeight > originalPos.y) {
            gameObjectImage.transform.Translate(0.0f, -bounceMagnitude * Time.deltaTime, 0.0f);
            currHeight = gameObjectImage.transform.position.y;
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
        SetImageSprite(null);
        SetImageActive(false);
        ResetImagePosition();
    }

    public bool StopAnimation(bool dialogueActive) {
        if (dialogueActive) {
            StopAllCoroutines();
            return true;
        }
        return false;
    }
    public static void SetImageSprite(Sprite sprite) {
        gameObjectImage.sprite = sprite;
    }

    public static void SetImageActive(bool active) {
        Color currcolor = gameObjectImage.color;
        currcolor.a = (active) ? 1 : 0;
        gameObjectImage.color = currcolor;
    }

    public static void ResetImagePosition() {
        gameObjectImage.transform.position = originalPos;
    }

}