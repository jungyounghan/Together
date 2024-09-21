using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

[DisallowMultipleComponent]
[RequireComponent(typeof(Partition))]
/// <summary>
/// 이 클래스는 유저의 게임 입장을 담당하는 컴포넌트로 씬 안에서 오직 하나의 객체로만 존재한다.
/// </summary>
public class EntryManager : MonoBehaviourPunCallbacks
{
    private static EntryManager instance = null;
    [SerializeField]
    private TMP_Text _titleText = null;
    [SerializeField]
    private TMP_Text _versionText = null;
    [SerializeField]
    private TMP_Text _joinText = null;
    [SerializeField]
    private TMP_Text _quitText = null;
    [SerializeField]
    private TMP_Text _stateText = null;

    [SerializeField]
    private TMP_InputField _nicknameInputField = null;
    [SerializeField]
    private TMP_InputField _passwordInputField = null;

    [SerializeField]
    private Button _joinButton = null;
    [SerializeField]
    private Button _quitButton = null;

    [SerializeField]
    private Popup _popup = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        Summarizer.Set(_titleText, PlayData.TEXT_TAG_TITLE);
        Summarizer.Set(_versionText, Application.version + PlayData.TEXT_TAG_VERSION);
        Summarizer.Set(_joinText, PlayData.TEXT_TAG_JOIN);
        Summarizer.Set(_quitText, PlayData.TEXT_TAG_QUIT);
        Summarizer.Set(_stateText, "");
        Summarizer.Set(_nicknameInputField, PlayData.TEXT_TAG_NICKNAME, true);
        Summarizer.Set(_passwordInputField, PlayData.TEXT_TAG_PASSWORD, true);
    }
#endif

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if (instance == this)
        {
            Summarizer.Set(_titleText, PlayData.TEXT_TAG_TITLE);
            Summarizer.Set(_versionText, Application.version + PlayData.TEXT_TAG_VERSION);
            Summarizer.Set(_joinText, PlayData.TEXT_TAG_JOIN);
            Summarizer.Set(_quitText, PlayData.TEXT_TAG_QUIT);
            string nickname = PlayerPrefs.GetString(PlayData.NICKNAME);
            if (string.IsNullOrEmpty(nickname) == true)
            {
                Summarizer.Set(_nicknameInputField, PlayData.TEXT_TAG_NICKNAME, true, true);
            }
            else
            {
                Summarizer.Set(_nicknameInputField, nickname, false, true);
            }
            string password = PlayerPrefs.GetString(PlayData.PASSWORD);
            if (string.IsNullOrEmpty(password) == true)
            {
                Summarizer.Set(_passwordInputField, PlayData.TEXT_TAG_PASSWORD, true, true);
            }
            else
            {
                Summarizer.Set(_passwordInputField, password, false, true);
            }
            Summarizer.Set(_joinButton, () => Join(), true);
            Summarizer.Set(_quitButton, () => Quit(), true);
            Summarizer.Set(_stateText, "");
            _popup?.Initialize();
            PhotonNetwork.IsMessageQueueRunning = true;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public override void OnConnectedToMaster()
    {
        Summarizer.Set(_stateText, PlayData.TEXT_CONNECT_PHOTON);
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetInteractable(true);
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadSceneAsync(PlayData.SCENE_LOBBY);
    }

    private void SetInteractable(bool interactable)
    {
        Summarizer.Set(_nicknameInputField, interactable);
        Summarizer.Set(_passwordInputField, interactable);
        Summarizer.Set(_joinButton, interactable);
        Summarizer.Set(_quitButton, interactable);
    }

    private void Join()
    {
        if (PhotonNetwork.IsConnectedAndReady == false)
        {
            string nickname = Summarizer.Get(_nicknameInputField);
            string password = Summarizer.Get(_passwordInputField);
            if (string.IsNullOrWhiteSpace(nickname) == true)
            {
                Summarizer.Set(_stateText, PlayData.TEXT_INPUT_NICKNAME);
            }
            else if (string.IsNullOrWhiteSpace(password) == true)
            {
                Summarizer.Set(_stateText, PlayData.TEXT_INPUT_PASSWORD);
            }
            else
            {
                SetInteractable(false);
                PlayerPrefs.SetString(PlayData.NICKNAME, nickname);
                PlayerPrefs.SetString(PlayData.PASSWORD, password);
                Summarizer.Set(_stateText, PlayData.TEXT_TRY_CONNECTION);
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    private void Quit()
    {
        Action yesAction = () =>
        {
            SetInteractable(false);
            _popup?.Stop();
            Application.Quit();
        };
        Action noAction = () =>
        {
            _popup?.Hide();
        };
        _popup?.Show(PlayData.TEXT_POP_QUIT, yesAction, noAction);
    }
}