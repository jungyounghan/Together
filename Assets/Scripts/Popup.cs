using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Popup : MonoBehaviour
{
    [SerializeField]
    private bool _working = true;

    [SerializeField]
    private TMP_Text _messageText = null;
    [SerializeField]
    private Button _noButton = null;
    [SerializeField]
    private Button _yesButton = null;
    [SerializeField]
    private Button _confirmButton = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetInteractable(_working);
    }
#endif

    private void SetInteractable(bool interactable)
    {
        Summarizer.Set(_confirmButton, interactable);
        Summarizer.Set(_yesButton, interactable);
        Summarizer.Set(_noButton, interactable);
    }

    public void Initialize()
    {
        _working = true;
        SetInteractable(_working);
        Hide();
    }

    public void Show(string message, Action action = null, bool stop = false)
    {
        if (_working == true)
        {
            Summarizer.Set(_messageText, message);
            if (_confirmButton != null)
            {
                _confirmButton.onClick.RemoveAllListeners();
                if (stop == true)
                {
                    _confirmButton.onClick.AddListener(delegate { SetInteractable(false); });
                }
                _confirmButton.onClick.AddListener(delegate { action?.Invoke(); });
                _confirmButton.gameObject.SetActive(true);
            }
            if (_yesButton != null)
            {
                _yesButton.gameObject.SetActive(false);
            }
            if (_noButton != null)
            {
                _noButton.gameObject.SetActive(false);
            }
            if (stop == true)
            {
                _working = false;
            }
            gameObject.SetActive(true);
        }
    }

    public void Show(string message, Action yesAction, Action noAction)
    {
        if (_working == true)
        {
            Summarizer.Set(_messageText, message);
            if (_confirmButton != null)
            {
                _confirmButton.gameObject.SetActive(false);
            }
            if (_yesButton != null)
            {
                _yesButton.onClick.RemoveAllListeners();
                _yesButton.onClick.AddListener(delegate { yesAction?.Invoke(); });
                _yesButton.gameObject.SetActive(true);
            }
            if (_noButton != null)
            {
                _noButton.onClick.RemoveAllListeners();
                _noButton.onClick.AddListener(delegate { noAction?.Invoke(); });
                _noButton.gameObject.SetActive(true);
            }
            gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Stop()
    {
        _working = false;
        Summarizer.Set(_confirmButton, false);
        Summarizer.Set(_yesButton, false);
        Summarizer.Set(_noButton, false);
    }
}