using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// UI 컴포넌트의 값 변경 내용들을 짧게 요약하는 기능을 한다.
/// </summary>
public static class Summarizer 
{
    public static void Set(TMP_Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }

    public static void Set(TMP_InputField inputField, bool interactable)
    {
        if (inputField != null)
        {
            inputField.interactable = interactable;
        }
    }

    public static void Set(TMP_InputField inputField, string value, bool placeholder)
    {
        if(inputField != null)
        {
            if(placeholder == true)
            {
                TextMeshProUGUI textMeshProUGUI = (TextMeshProUGUI)inputField.placeholder;
                if (textMeshProUGUI != null)
                {
                    textMeshProUGUI.text = value;
                }
            }
            else
            {
                inputField.text = value;
            }
        }
    }

    public static void Set(TMP_InputField inputField, string value, bool placeholder, bool interactable)
    {
        if (inputField != null)
        {
            if (placeholder == true)
            {
                TextMeshProUGUI textMeshProUGUI = (TextMeshProUGUI)inputField.placeholder;
                if (textMeshProUGUI != null)
                {
                    textMeshProUGUI.text = value;
                }
            }
            else
            {
                inputField.text = value;
            }
            inputField.interactable = interactable;
        }
    }

    public static void Set(Button button, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    public static void Set(Button button, Color color)
    {
        if(button != null && button.image != null)
        {
            button.image.color = color;
        }
    }

    public static void Set(Button button, Color color, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
            if (button.image != null)
            {
                button.image.color = color;
            }
        }
    }

    public static void Set(Button button, UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (action != null)
                {
                    action.Invoke();
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning("델리게이트 없음");
#endif
                }
            }
            );
        }
    }

    public static void Set(Button button, UnityAction action, bool interactable)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (action != null)
                {
                    action.Invoke();
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning("델리게이트 없음");
#endif
                }
            }
            );
            button.interactable = interactable;
        }
    }

    public static void Set(Slider slider, float value)
    {
        if (slider != null)
        {
            slider.value = value;
        }
    }

    public static void Set(ScrollRect scrollRect, bool interactable)
    {
        if (scrollRect != null)
        {
            if (scrollRect.verticalScrollbar != null)
            {
                scrollRect.verticalScrollbar.interactable = interactable;
            }
            if (scrollRect.horizontalScrollbar != null)
            {
                scrollRect.horizontalScrollbar.interactable = interactable;
            }
        }
    }

    public static string Get(TMP_InputField inputField)
    {
        if (inputField != null)
        {
            return inputField.text;
        }
        return null;
    }
}