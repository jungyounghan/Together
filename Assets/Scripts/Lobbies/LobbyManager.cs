using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[DisallowMultipleComponent]
[RequireComponent(typeof(Partition))]
/// <summary>
/// 이 클래스는 유저의 룸 입장을 담당하는 컴포넌트로 씬 안에서 오직 하나의 객체로만 존재한다.
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks
{
    private static LobbyManager instance = null;

    [SerializeField]
    private LobbyTop _lobbyTop = null;
    [SerializeField]
    private LobbyBottom _lobbyBottom = null;
    [SerializeField]
    private Popup _popup = null;

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
            UnityAction lockingAction = () => SetInteractable(false);
            UnityAction releasingAction = () => { _popup?.Show(PlayData.TEXT_INPUT_ROOM, () => { SetInteractable(true); _popup?.Hide(); }); };
            UnityAction exitAction = () => Exit();
            _lobbyTop?.Initialize(lockingAction, releasingAction, exitAction);
            _lobbyBottom?.Initialize(lockingAction);
            _popup?.Initialize();
            PhotonNetwork.IsMessageQueueRunning = true;
            if (PhotonNetwork.InLobby == false)
            {
                if (PhotonNetwork.IsConnected == false)
                {
                    PhotonNetwork.ConnectUsingSettings();
                }
                else
                {
                    PhotonNetwork.JoinLobby();
                }
            }
            else
            {
                SetPlayerInfo();
            }
        }
    }

    private void LateUpdate()
    {
        _lobbyTop?.OnUpdate(PhotonNetwork.CountOfPlayers, PhotonNetwork.CountOfRooms);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SetPlayerInfo();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetInteractable(false);
        _popup?.Show(PlayData.TEXT_DISCONNECT, () => LoadScene(PlayData.SCENE_ENTRY), true);
    }

    public override void OnLeftLobby()
    {
        SetInteractable(false);
        LoadScene(PlayData.SCENE_ENTRY);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomInfos)
    {
        _lobbyBottom?.OnRoomListUpdate(roomInfos);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _popup?.Show(PlayData.TEXT_CREATE_ROOM_FAILED, () => { SetInteractable(true); _popup?.Hide(); });
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _popup?.Show(PlayData.TEXT_JOIN_ROOM_FAILED, () => { SetInteractable(true); _popup?.Hide(); });
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        _popup?.Show(PlayData.TEXT_JOIN_ROOM_FAILED, () => { SetInteractable(true); _popup?.Hide(); });
    }

    public override void OnJoinedRoom()
    {
        LoadScene(PlayData.SCENE_ROOM);
    }

    private void SetPlayerInfo()
    {
        string nickname = PlayerPrefs.GetString(PlayData.NICKNAME);
        string password = PlayerPrefs.GetString(PlayData.PASSWORD);
        Player player = PhotonNetwork.LocalPlayer;
        player?.SetCustomProperties(new Hashtable() { { PlayData.NICKNAME, nickname }, { PlayData.PASSWORD, password } });
        if(_lobbyTop != null)
        {
            _lobbyTop.SetPlayerNickname(nickname);
            _lobbyTop.SetInteractable(true);
        }
        _lobbyBottom?.SetInteractable(true);
    }

    private void SetInteractable(bool interactable)
    {
        _lobbyTop?.SetInteractable(interactable);
        _lobbyBottom?.SetInteractable(interactable);     
    }

    private void LoadScene(string scene)
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadSceneAsync(scene);
    }

    private void Exit()
    {
        Action yesAction = () =>
        {
            SetInteractable(false);
            _popup?.Stop();
            if (PhotonNetwork.IsConnected == true)
            {
                PhotonNetwork.Disconnect();
            }
            LoadScene(PlayData.SCENE_ENTRY);
        };
        Action noAction = () =>
        {
            _popup?.Hide();
        };
        _popup?.Show(PlayData.TEXT_POP_LEAVE_LOBBY, yesAction, noAction);
    }
}