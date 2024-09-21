using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
[RequireComponent(typeof(Partition))]
/// <summary>
/// 이 클래스는 게임 시작을 담당하는 컴포넌트로 씬 안에서 오직 하나의 객체로만 존재한다.
/// </summary>
public class RoomManager : MonoBehaviourPunCallbacks
{
    private static RoomManager instance = null;

    [SerializeField]
    private RoomTop _roomTop = null;
    [SerializeField]
    private RoomBottom _roomBottom = null;
    [SerializeField]
    private Popup _popup = null;
#if UNITY_EDITOR
    [SerializeField]
    private Tester _tester = null;
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
            PhotonNetwork.IsMessageQueueRunning = true;
            Player localPlayer = PhotonNetwork.LocalPlayer;
            Room room = PhotonNetwork.CurrentRoom;
            if (localPlayer != null && room != null)
            {
                Hashtable hashtable = localPlayer.CustomProperties;
                if (hashtable != null)
                {
                    string nickname = hashtable[PlayData.NICKNAME] != null ? hashtable[PlayData.NICKNAME].ToString() : null;
                    string password = hashtable[PlayData.PASSWORD] != null ? hashtable[PlayData.PASSWORD].ToString() : null;
                    Dictionary<int, Player> players = room.Players;
                    if (players != null)
                    {
                        bool confirm = false;
                        foreach (KeyValuePair<int, Player> kvp in players)
                        {
                            Player player = kvp.Value;
                            hashtable = player != null ? player.CustomProperties : null;
                            if (hashtable != null && Equals(nickname, hashtable[PlayData.NICKNAME]) && Equals(password, hashtable[PlayData.PASSWORD]))
                            {
                                if (confirm == false)
                                {
                                    confirm = true;
                                }
                                else
                                {
                                    PhotonNetwork.Disconnect();
                                    _popup?.Show(PlayData.TEXT_ALREADY_ACCESS, () => LoadScene(PlayData.SCENE_ENTRY), true);
                                    return;
                                }
                            }
                        }
                        bool master = false;
                        int count = players.Count;
                        Player masterPlayer = PhotonNetwork.MasterClient;
                        if (localPlayer == masterPlayer)
                        {
                            master = true;
                            room.SetCustomProperties(new Hashtable() { {PlayData.NICKNAME + count, nickname },{ PlayData.PASSWORD + count, password }, { PlayData.SELECTION + count, 0 } });
                        }
                        hashtable = room.CustomProperties;
                        if (hashtable != null && hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round) == true && round > 0)
                        {
                            string theme = hashtable[PlayData.THEME] != null ? hashtable[PlayData.THEME].ToString() : null;
                            if (string.IsNullOrWhiteSpace(theme) == false)
                            {
                                Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
                                foreach(string key in hashtable.Keys)
                                {
                                    if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true)
                                    {
                                        switch (Regex.Replace(key, @"\d", ""))
                                        {
                                            case PlayData.NICKNAME:
                                                if(key == PlayData.NICKNAME + index && Equals(nickname, hashtable[PlayData.NICKNAME + index]))
                                                {
                                                    if(dictionary.ContainsKey(index) == true)
                                                    {
                                                        _popup?.Show(PlayData.TEXT_RECONNECT_PLAYING, null, true);
                                                        LoadScene(theme);
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        dictionary.Add(index, false);
                                                    }
                                                }
                                                break;
                                            case PlayData.PASSWORD:
                                                if (key == PlayData.PASSWORD + index && Equals(password, hashtable[PlayData.PASSWORD + index]))
                                                {
                                                    if (dictionary.ContainsKey(index) == true)
                                                    {
                                                        _popup?.Show(PlayData.TEXT_RECONNECT_PLAYING, null, true);
                                                        LoadScene(theme);
                                                        return;
                                                    }
                                                    else
                                                    {
                                                        dictionary.Add(index, false);
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            PhotonNetwork.LeaveRoom();
                            _popup?.Show(PlayData.TEXT_ALREADY_PLAYING, () => LoadScene(PlayData.SCENE_LOBBY), true);
                            return;
                        }
                        _roomTop?.Initialize(room.Name, masterPlayer, localPlayer, hashtable);
                        _roomBottom?.Initialize(() => TouchCountDown(true), Exit, (text) => { _popup?.Show(text, ()=> _popup?.Hide()); }, hashtable, master);
                        _popup?.Initialize();
                        return;
                    }
                }
            }
            _popup?.Show(PlayData.TEXT_JOIN_ROOM_FAILED, () => LoadScene(PlayData.SCENE_LOBBY), true);
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room != null)
        {
            Hashtable hashtable = room.CustomProperties;
#if UNITY_EDITOR
            _tester?.Set(hashtable);
#endif
            if (hashtable != null && hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round) && round > 0)
            {
                string theme = hashtable[PlayData.THEME] != null ? hashtable[PlayData.THEME].ToString() : null;
                switch (theme)
                {
                    case MafiaData.SCENE:
                        LoadScene(theme);
                        return;
                }
            }
            _roomTop?.OnRoomPropertiesUpdate(hashtable);
            _roomBottom?.OnRoomPropertiesUpdate(hashtable);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _popup?.Show(PlayData.TEXT_DISCONNECT, () => LoadScene(PlayData.SCENE_ENTRY), true);
    }

//    public override void OnLeftRoom()
//    {
//        if (PhotonNetwork.IsConnectedAndReady == true)
//        {
//#if UNITY_EDITOR
//            Debug.LogWarning("강퇴 당함");
//#endif
//            _popup?.Show(PlayData.TEXT_DISCONNECT, () => LoadScene(PlayData.SCENE_LOBBY), true);
//        }
//    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
#if UNITY_EDITOR
        if(PhotonNetwork.CurrentRoom != null)
        {
            _tester?.Set(PhotonNetwork.CurrentRoom.CustomProperties);
        }
#endif
        if (hashtable != null && hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round) && round > 0)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if(room != null)
            {
                hashtable = room.CustomProperties;
                if(hashtable != null)
                {
                    string theme = hashtable[PlayData.THEME] != null ? hashtable[PlayData.THEME].ToString() : null;
                    switch (theme)
                    {
                        case MafiaData.SCENE:
                            LoadScene(theme);
                            return;
                    }
                }
            }
        }
        _roomTop?.OnRoomPropertiesUpdate(hashtable);
        _roomBottom?.OnRoomPropertiesUpdate(hashtable);
    }

    public override void OnMasterClientSwitched(Player player)
    {
        _roomTop?.OnMasterClientSwitched(player);
        _roomBottom?.OnMasterClientSwitched(player);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null && newPlayer != null)
            {
                Hashtable hashtable = newPlayer.CustomProperties;
                if (hashtable != null)
                {
                    string nickname = hashtable[PlayData.NICKNAME] != null ? hashtable[PlayData.NICKNAME].ToString() : null;
                    string password = hashtable[PlayData.PASSWORD] != null ? hashtable[PlayData.PASSWORD].ToString() : null;
                    Dictionary<int, Player> players = room.Players;
                    if (players != null)
                    {
                        bool confirm = false;
                        foreach (KeyValuePair<int, Player> kvp in players)
                        {
                            Player player = kvp.Value;
                            hashtable = player != null ? player.CustomProperties : null;
                            if (hashtable != null && Equals(nickname, hashtable[PlayData.NICKNAME]) && Equals(password, hashtable[PlayData.PASSWORD]))
                            {
                                if (confirm == false)
                                {
                                    confirm = true;
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        if (confirm == true)
                        {
                            int index = players.Count;
                            room.SetCustomProperties(new Hashtable() { { PlayData.NICKNAME + index, nickname }, { PlayData.PASSWORD + index, password }, { PlayData.SELECTION + index, 0 } });
                        }
                    }
                }
            }
        }
        TouchCountDown(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null && otherPlayer != null)
            {
                Hashtable hashtable = otherPlayer.CustomProperties;
                if(hashtable != null)
                {
                    string nickname = hashtable[PlayData.NICKNAME] != null ? hashtable[PlayData.NICKNAME].ToString() : null;
                    string password = hashtable[PlayData.PASSWORD] != null ? hashtable[PlayData.PASSWORD].ToString() : null;
                    Dictionary<int, Player> players = room.Players;
                    if (players != null)
                    {
                        foreach (KeyValuePair<int, Player> kvp in players)
                        {
                            Player player = kvp.Value;
                            hashtable = player != null ? player.CustomProperties : null;
                            if (hashtable != null && Equals(nickname, hashtable[PlayData.NICKNAME]) && Equals(password, hashtable[PlayData.PASSWORD]))
                            {
                                return;
                            }
                        }
                    }
                    hashtable = room.CustomProperties;
                    if(hashtable != null)
                    {
                        bool find = false;
                        int count = room.PlayerCount;
                        for (int i = 0; i < count + 1; i++)
                        {
                            if (find == false)
                            {
                                find = Equals(nickname, hashtable[PlayData.NICKNAME + (i + 1)]) && Equals(password, hashtable[PlayData.PASSWORD + (i + 1)]);
                            }
                            if (find == true)
                            {
                                if (i < count)
                                {
                                    nickname = hashtable[PlayData.NICKNAME + (i + 2)] != null ? hashtable[PlayData.NICKNAME + (i + 2)].ToString() : null;
                                    password = hashtable[PlayData.PASSWORD + (i + 2)] != null ? hashtable[PlayData.PASSWORD + (i + 2)].ToString() : null;
                                    room.SetCustomProperties(new Hashtable() { { PlayData.NICKNAME + (i + 1), nickname }, { PlayData.PASSWORD + (i + 1), password} });
                                    if (hashtable[PlayData.SELECTION + (i + 2)] != null && int.TryParse(hashtable[PlayData.SELECTION + (i + 2)].ToString(), out int number) == true)
                                    {
                                        room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + (i + 1), number } });
                                    }
                                }
                                else
                                {
                                    room.SetCustomProperties(new Hashtable() { { PlayData.NICKNAME + (i + 1), null }, { PlayData.PASSWORD + (i + 1), null } });
                                    if (hashtable[PlayData.SELECTION + (i + 1)] != null)
                                    {
                                        room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + (i + 1), null } });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        TouchCountDown(false);
    }


    private void TouchCountDown(bool start)
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            StopAllCoroutines();
            Room room = PhotonNetwork.CurrentRoom;
            if(room != null)
            {
                if (start == true)
                {
                    int timer = 0;
                    Hashtable hashtable = room.CustomProperties;
                    if (hashtable != null && hashtable[PlayData.TIMER] != null)
                    {
                        int.TryParse(hashtable[PlayData.TIMER].ToString(), out timer);
                    }
                    if (timer == 0)
                    {
                        timer = PlayData.VALUE_COUNT_DOWN;
                        room.SetCustomProperties(new Hashtable() { { PlayData.TIMER, timer } });
                        IEnumerator DoCountDown()
                        {
                            WaitForSeconds waitForSeconds = new WaitForSeconds(PlayData.VALUE_ONE_SECOND);
                            while (PhotonNetwork.IsMasterClient && timer > 0)
                            {
                                yield return waitForSeconds;
                                if (room != null)
                                {
                                    Hashtable hashtable = room.CustomProperties;
                                    if (hashtable != null && hashtable[PlayData.TIMER] != null && int.TryParse(hashtable[PlayData.TIMER].ToString(), out int count) && count == timer)
                                    {
                                        timer--;
                                        if (timer > 0)
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.TIMER, timer } });
                                            continue;
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.TIMER, timer }, { PlayData.ROUND, 1 } });
                                            break;
                                        }
                                    }
                                }
                                _roomBottom?.SetInteractable(true);
                                break;
                            }
                        }
                        StartCoroutine(DoCountDown());
                        return;
                    }
                }
                _roomBottom?.SetInteractable(true);
                room.SetCustomProperties(new Hashtable() { { PlayData.TIMER, 0 } });
            }
        }
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
            _popup?.Stop();
            if (PhotonNetwork.InRoom == true)
            {
                PhotonNetwork.LeaveRoom();
            }
            LoadScene(PlayData.SCENE_LOBBY);
        };
        Action noAction = () =>
        {
            _popup?.Hide();
        };
        _popup?.Show(PlayData.TEXT_TRY_LEAVE_ROOM, yesAction, noAction);
    }
}