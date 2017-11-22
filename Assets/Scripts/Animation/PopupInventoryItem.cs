using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupInventoryItem : MonoBehaviour {
    private Image gameObjectImage;
    private Vector3 originalPos;
    
    [SerializeField]
    private float bounceSpeed = 1.5f;
    [SerializeField]
    private float bounceHeight = 0.5f;
    [SerializeField]
    private int bounceRepetitions = 2;

    private void Start() {
        gameObjectImage = gameObject.GetComponent<Image>();
        originalPos = transform.position;
        gameObject.SetActive(false);
    }

    private void OnEnable() {
        StartCoroutine(AnimateSprite());
    }

    /// <summary>
    /// Attach the prefab's sprite to current object's sprite in image component and render it
    /// </summary>
    public void RenderItemSprite(InventoryItem prefab) {
        gameObjectImage.sprite = prefab.GetComponent<Image>().sprite;
        gameObject.SetActive(true);
    }

   private IEnumerator GoUp() {
        while (transform.position.y < originalPos.y + bounceHeight) {
            transform.Translate(0.0f, bounceSpeed * Time.deltaTime, 0.0f);
            yield return null;
        }
     }

    private IEnumerator GoDown() {
        while (transform.position.y > originalPos.y) {
            transform.Translate(0.0f, -bounceSpeed * Time.deltaTime, 0.0f);
            yield return null;
        }
    }

    /// <summary>
    /// Animate the item sprite in an up and down motion
    /// </summary>
    private IEnumerator AnimateSprite() {
        for (int animLoop = 0; animLoop < bounceRepetitions; animLoop++) {
            yield return StartCoroutine(GoUp());
            yield return StartCoroutine(GoDown());
            yield return null;
        }
        ResetImage();
    }

    /// <summary>
    /// Stops animation of the item 
    /// </summary>
    public void StopAnimation() {
        StopAllCoroutines();
        ResetImage();
    }

    private void ResetImage() {
        gameObjectImage.sprite = null;
        transform.position = originalPos;
        gameObject.SetActive(false);
    }
}