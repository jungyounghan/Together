using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

[DisallowMultipleComponent]
/// <summary>
/// 로비의 상단 UI 기능을 담당한다.
/// </summary>
public class LobbyTop : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _nicknameText = null;
    [SerializeField]
    private TMP_Text _playerCountText = null;
    [SerializeField]
    private TMP_Text _roomCountText = null;
    [SerializeField]
    private TMP_Text _createText = null;
    [SerializeField]
    private TMP_Text _joinText = null;
    [SerializeField]
    private TMP_Text _exitText = null;

    [SerializeField]
    private TMP_InputField _roomInputField = null;

    [SerializeField]
    private Button _createButton = null;
    [SerializeField]
    private Button _joinButton = null;
    [SerializeField]
    private Button _exitButton = null;

    private UnityAction releasingAction = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        Summarizer.Set(_nicknameText, PlayData.TEXT_TAG_NICKNAME);
        Summarizer.Set(_playerCountText, PlayData.TEXT_COUNT_OF_PLAYERS + ": " + string.Format(PlayData.TEXT_TAG_PEOPLE, 0));
        Summarizer.Set(_roomCountText, PlayData.TEXT_COUNT_OF_ROOMS + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, 0));
        Summarizer.Set(_createText, PlayData.TEXT_TAG_CREATE);
        Summarizer.Set(_joinText, PlayData.TEXT_TAG_JOIN);
        Summarizer.Set(_exitText, PlayData.TEXT_TAG_EXIT);
        Summarizer.Set(_roomInputField, PlayData.TEXT_TAG_ROOM, true);
    }
#endif

    private void TryAccessRoom(bool create)
    {
        string roomName = Summarizer.Get(_roomInputField);
        if (create == true)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.IsOpen = true;
            if (string.IsNullOrEmpty(roomName) == true)
            {
                releasingAction?.Invoke();
            }
            else
            {
                PhotonNetwork.CreateRoom(roomName, roomOptions);
            }
        }
        else
        {
            if (string.IsNullOrEmpty(roomName) == true)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        }
    }

    public void Initialize(UnityAction lockingAction, UnityAction releasingAction, UnityAction exitAction)
    {
        Summarizer.Set(_nicknameText, PlayData.TEXT_TAG_NICKNAME);
        Summarizer.Set(_playerCountText, PlayData.TEXT_COUNT_OF_PLAYERS + ": " + string.Format(PlayData.TEXT_TAG_PEOPLE, 0));
        Summarizer.Set(_roomCountText, PlayData.TEXT_COUNT_OF_ROOMS + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, 0));
        Summarizer.Set(_createText, PlayData.TEXT_TAG_CREATE);
        Summarizer.Set(_joinText, PlayData.TEXT_TAG_JOIN);
        Summarizer.Set(_exitText, PlayData.TEXT_TAG_EXIT);
        Summarizer.Set(_roomInputField, PlayData.TEXT_TAG_ROOM, true);
        Summarizer.Set(_createButton, () => { lockingAction?.Invoke(); TryAccessRoom(true); }, false);
        Summarizer.Set(_joinButton, () => { lockingAction?.Invoke(); TryAccessRoom(false); }, false);
        this.releasingAction = releasingAction;
        Summarizer.Set(_exitButton, exitAction, false);
    }

    public void SetInteractable(bool interactable)
    {
        Summarizer.Set(_roomInputField, interactable);
        Summarizer.Set(_createButton, interactable);
        Summarizer.Set(_joinButton, interactable);
        Summarizer.Set(_exitButton, interactable);
    }
    
    public void SetPlayerNickname(string text)
    {
        Summarizer.Set(_nicknameText, text);
    }

    public void OnUpdate(int playerCount, int roomCount)
    {
        Summarizer.Set(_playerCountText, PlayData.TEXT_COUNT_OF_PLAYERS + ": " + string.Format(PlayData.TEXT_TAG_PEOPLE, playerCount));
        Summarizer.Set(_roomCountText, PlayData.TEXT_COUNT_OF_ROOMS + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, roomCount));
    }
}