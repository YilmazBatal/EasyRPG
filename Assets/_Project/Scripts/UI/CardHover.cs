using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Image))]
public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Hover Settings")]
    [SerializeField,Range(0.1f, 0.5f)] private float fadeTime = 0.2f;
    [SerializeField,Range(1.01f, 1.25f)] private float cardScale = 1.1f;

    private Outline outline;
    private Image card;
    private Color outlineColor;

    private void Start()
    {
        outline = GetComponent<Outline>();
        card = GetComponent<Image>();
        outlineColor = outline.effectColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.value(gameObject, 0f, 1f, fadeTime).setOnUpdate((float value) => outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, value));
        LeanTween.value(gameObject, 1f, cardScale, fadeTime).setOnUpdate((float value) => card.transform.localScale = new Vector3(value, value, value));
    }
    

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.value(gameObject, 1f, 0f, fadeTime).setOnUpdate((float value) => outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, value));
        LeanTween.value(gameObject, cardScale, 1f, fadeTime).setOnUpdate((float value) => card.transform.localScale = new Vector3(value, value, value));
    }
}
