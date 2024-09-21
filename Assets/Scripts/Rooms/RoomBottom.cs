using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
/// <summary>
/// ���� �ϴ� UI ����� ����Ѵ�.
/// </summary>
public class RoomBottom : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _startText = null;  //���� �ؽ�Ʈ
    [SerializeField]
    private TMP_Text _quitText = null;  //���� �ؽ�Ʈ
    [SerializeField]
    private Button _leftButton = null;  //���� ��ư
    [SerializeField]
    private Button _rightButton = null; //������ ��ư
    [SerializeField]
    private Button _startButton = null;  //���� ��ư
    [SerializeField]
    private Button _quitButton = null;  //����, ������ ��ư

    [SerializeField]
    private MafiaPanel _mafiaPanel = null;


#if UNITY_EDITOR
    private void OnValidate()
    {
        Summarizer.Set(_startText, PlayData.TEXT_TAG_START);
        Summarizer.Set(_quitText, PlayData.TEXT_TAG_EXIT);
        _mafiaPanel?.SetActive(false);
    }
#endif

    private void SetTheme(string text)
    {
        switch (text)
        {
            case MafiaData.SCENE:
                _mafiaPanel?.SetActive(true);
                break;
            default:
                _mafiaPanel?.SetActive(false);
                break;
        }
    }

    public void Initialize(UnityAction touchAction, UnityAction quitAction, UnityAction<string> infoAction, Hashtable hashtable, bool master)
    {
        _mafiaPanel?.Initialize(infoAction, hashtable, master);
        SetTheme(hashtable != null && hashtable[PlayData.THEME] != null ? hashtable[PlayData.THEME].ToString() : null);
        UnityAction<bool> arrowAction = (increasing) =>
        {
            if(PhotonNetwork.IsMasterClient == true)
            {
                Room room = PhotonNetwork.CurrentRoom;
                if(room != null)
                {
                    Hashtable hashtable = room.CustomProperties;
                    string theme = hashtable != null && hashtable[PlayData.THEME] != null ? hashtable[PlayData.THEME].ToString() : null;
                    switch(theme)
                    {
                        case MafiaData.SCENE:
                            room.SetCustomProperties(new Hashtable() { {PlayData.THEME, null } });
                            break;
                        default:
                            room.SetCustomProperties(new Hashtable() { { PlayData.THEME, MafiaData.SCENE } });
                            break;
                    }
                }
            }
        };
        Summarizer.Set(_leftButton, () => arrowAction(false) ,master);
        Summarizer.Set(_rightButton, () => arrowAction(true), master);
        UnityAction startAction = () =>
        {
            Room room = PhotonNetwork.CurrentRoom;
            if(room != null)
            {
                if(PhotonNetwork.IsMasterClient == true)
                {
                    Hashtable hashtable = room.CustomProperties;
                    string theme = hashtable != null && hashtable[PlayData.THEME] != null ? hashtable[PlayData.THEME].ToString() : null;
                    switch (theme)
                    {
                        case MafiaData.SCENE:
                            if(_mafiaPanel != null && _mafiaPanel.CanPlaying(room.PlayerCount, hashtable) == true)
                            {
                                SetInteractable(false);
                                touchAction?.Invoke();
                            }
                            break;
                        default:
                            infoAction?.Invoke(PlayData.TEXT_INPUT_THEME);
                            break;
                    }
                }
                else
                {
                    room.SetCustomProperties(new Hashtable() { {PlayData.TIMER, 0} });
                }
            }
        };
        Summarizer.Set(_startButton, startAction, master);
        if (master == true)
        {
            Summarizer.Set(_startText, PlayData.TEXT_TAG_START);
        }
        else
        {
            Summarizer.Set(_startText, PlayData.TEXT_TAG_STOP);
        }
        Summarizer.Set(_quitButton, quitAction, true);
        Summarizer.Set(_quitText, PlayData.TEXT_TAG_EXIT);
    }

    public void SetInteractable(bool interactable)
    {
        Summarizer.Set(_leftButton, interactable);
        Summarizer.Set(_rightButton, interactable);
        if (interactable == true)
        {
            Summarizer.Set(_startText, PlayData.TEXT_TAG_START);
        }
        else
        {
            Summarizer.Set(_startText, PlayData.TEXT_TAG_STOP);
        }
        _mafiaPanel?.SetInteractable(interactable);
    }

    public void OnMasterClientSwitched(Player player)
    {
        bool master = player == PhotonNetwork.LocalPlayer;
        SetInteractable(master);
        Summarizer.Set(_startButton, master);
    }

    public void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if (hashtable != null)
        {
            foreach (string key in hashtable.Keys)
            {
                switch (key)
                {
                    case PlayData.THEME:
                        SetTheme(hashtable[key] != null ? hashtable[key].ToString() : null);
                        break;
                    case PlayData.TIMER:
                        if (PhotonNetwork.IsMasterClient == false)
                        {
                            int timer = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out timer) ? timer : 0;
                            Summarizer.Set(_startButton, timer > 0);
                        }
                        break;
                    default:
                        _mafiaPanel?.OnRoomPropertyUpdate(new KeyValuePair<string, object>(key, hashtable[key]));
                        break;
                }
            }
        }
    }
}