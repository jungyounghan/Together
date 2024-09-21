using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class Item : MonoBehaviour
{
    private bool _hasRectTransform = false;

    private RectTransform _rectTransform = null;

    private RectTransform getRectTransform {
        get
        {
            if (_hasRectTransform == false)
            {
                _rectTransform = GetComponent<RectTransform>();
                _hasRectTransform = true;
            }
            return _rectTransform;
        }
    }

    private bool _hasButton = false;

    private Button _button = null;

    private Button getButton {
        get
        {
            if (_hasButton == false)
            {
                _button = GetComponent<Button>();
                _hasButton = true;
            }
            return _button;
        }
    }

    private bool _hasImage = false;

    private Image _image = null;

    private Image getImage {
        get
        {
            if (_hasImage == false)
            {
                _image = GetComponent<Image>();
                _hasImage = true;
            }
            return _image;
        }
    }

    [SerializeField]
    private TMP_Text _text = null;

    public void SetInteractable(bool interactable)
    {
        getButton.interactable = interactable;
    }

    public void SetInteractable(string text, bool interactable)
    {
        Summarizer.Set(_text, text);
        getButton.interactable = interactable;
    }

    public void SetInteractable(string text, bool interactable, Color color)
    {
        Summarizer.Set(_text, text);
        getButton.interactable = interactable;
        getImage.color = color;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetActive(string text, bool value)
    {
        Summarizer.Set(_text, text);
        gameObject.SetActive(value);
    }

    public void Set(string text, bool active, bool interactable)
    {
        Summarizer.Set(_text, text);
        gameObject.SetActive(active);
        getButton.interactable = interactable;
    }

    public void Set(UnityAction action)
    {
        getButton.onClick.RemoveAllListeners();
        getButton.onClick.AddListener(action);
        gameObject.SetActive(true);
    }

    public void Set(string text, UnityAction action)
    {
        Summarizer.Set(_text, text);
        getButton.onClick.RemoveAllListeners();
        getButton.onClick.AddListener(action);
        gameObject.SetActive(true);
    }

    public void Set(Color color)
    {
        getImage.color = color;
    }
}