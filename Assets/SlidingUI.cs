using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SlidingUI : MonoBehaviour
{
    public RectTransform uiPanel;
    public Button arrowButton;
    public TextMeshProUGUI arrowImage;

    public float slideDistance = 300f;
    public float slideDuration = 0.5f;
    public Ease slideEase = Ease.OutExpo;

    public Sprite arrowLeft;
    public Sprite arrowRight;

    private bool isOpen = false;
    private Vector2 closedPosition;
    private Vector2 openPosition;

    void Start()
    {
        closedPosition = uiPanel.anchoredPosition;
        openPosition = closedPosition + Vector2.left * slideDistance;

        arrowButton.onClick.AddListener(TogglePanel);
        UpdateArrow();
    }

    void TogglePanel()
    {
        isOpen = !isOpen;

        uiPanel.DOKill(); // varsa eski animasyonu durdur
        uiPanel.DOAnchorPos(isOpen ? openPosition : closedPosition, slideDuration).SetEase(slideEase);

       UpdateArrow();
    }

    void UpdateArrow()
    {
        arrowImage.rectTransform.localScale = new Vector3(isOpen ? 1 : -1, 1, 1);

    }
}
