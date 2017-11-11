using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryItem : MonoBehaviour {
    private static Image gameObjectImage;
    private static Vector3 originalPos;
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
    /// <summary>
    /// Animate the item sprite in an up and down motion
    /// </summary>
    /// <returns></returns>
    private IEnumerator AnimateSprite() {
        itemRendered = false;
        for (int animLoop = 0; animLoop < 3; animLoop++) {
            yield return StartCoroutine(GoUp());
            yield return StartCoroutine(GoDown());
            yield return new WaitForSeconds(0.1f);
        }
        SetImageSprite(null);
        SetImageActive(false);
        ResetImagePosition();
    }
    /// <summary>
    /// stops animation of the item 
    /// </summary>
    /// <param name="active">determines whether to activate stop animation</param>
    /// <returns></returns>
    public bool StopAnimation(bool active) {
        if (active) {
            StopAllCoroutines();
            return true;
        }
        return false;
    }
    /// <summary>
    /// assigns sprite to popup item sprite
    /// </summary>
    /// <param name="sprite"></param>
    public static void SetImageSprite(Sprite sprite) {
        gameObjectImage.sprite = sprite;
    }

    /// <summary>
    /// Set whether sprite is visible in-game or not
    /// </summary>
    /// <param name="active"></param>
    public static void SetImageActive(bool active) {
        Color currcolor = gameObjectImage.color;
        currcolor.a = (active) ? 1 : 0;
        gameObjectImage.color = currcolor;
    }
    /// <summary>
    /// after each animation, reset the popup item position to its original position
    /// </summary>
    public static void ResetImagePosition() {
        gameObjectImage.transform.position = originalPos;
    }

}