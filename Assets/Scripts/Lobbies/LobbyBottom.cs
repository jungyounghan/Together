using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

[DisallowMultipleComponent]
[RequireComponent(typeof(ScrollRect))]
/// <summary>
/// 로비의 하단 UI 기능을 담당한다.
/// </summary>
public class LobbyBottom : MonoBehaviour
{
    private bool _hasScrollRect = false;

    private ScrollRect _scrollRect = null;

    private ScrollRect getScrollRect {
        get
        {
            if (_hasScrollRect == false)
            {
                _scrollRect = GetComponent<ScrollRect>();
                _hasScrollRect = true;
            }
            return _scrollRect;
        }
    }

    [SerializeField]
    private Item _item = null;
    private List<Item> _keepingItems = new List<Item>();
    private Dictionary<string, Item> _usingItems = new Dictionary<string, Item>();

    private UnityAction lockingAction = null;

    public void Initialize(UnityAction lockingAction)
    {
        this.lockingAction = lockingAction;
    }

    public void SetInteractable(bool interactable)
    {
        foreach(KeyValuePair<string, Item> kvp in _usingItems)
        {
            Item item = kvp.Value;
            item?.SetInteractable(interactable);
        }
    }

    public void OnRoomListUpdate(List<RoomInfo> roomInfos)
    {
        bool prefab = _item != null && getScrollRect.content != null;
        int roomCount = roomInfos != null ? roomInfos.Count : 0;
        for (int a = 0; a < roomCount; a++)
        {
            RoomInfo roomInfo = roomInfos[a];
            if(roomInfo != null)
            {
                string roomName = roomInfo.Name;
                int playerCount = roomInfo.PlayerCount;
                if (playerCount > 0)
                {
                    UnityAction action = () => { lockingAction?.Invoke(); PhotonNetwork.JoinRoom(roomName); };
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(roomName + " " + PlayData.TEXT_COUNT_OF_PLAYERS + ": " + string.Format(PlayData.TEXT_TAG_PEOPLE, playerCount));
                    //RoomInfo에서 Room의 Hashtable을 설정할 수 없다.
                    //hashtable = roomInfo.CustomProperties;
                    //if (hashtable != null)
                    //{
                    //    Debug.Log(hashtable.Count);
                    //    foreach(string key in hashtable.Keys)
                    //    {
                    //        Debug.Log("key:" + key + " value" + hashtable[key]);
                    //    }

                    //    if (hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round) == true && round > 0)
                    //    {
                    //        for (int b = 0; b < playerCount; b++)
                    //        {
                    //            if (hashtable[PlayData.NICKNAME + (b + 1)] != null && hashtable[PlayData.NICKNAME + (b + 1)].ToString() == nickname
                    //                && hashtable[PlayData.PASSWORD + (b + 1)] != null && hashtable[PlayData.PASSWORD + (b + 1)].ToString() == password)
                    //            {
                    //                action?.Invoke();
                    //                return;
                    //            }
                    //        }
                    //        stringBuilder.Append(PlayData.TEXT_TAG_PLAYING + " ");
                    //    }
                    //    else
                    //    {
                    //        stringBuilder.Append(PlayData.TEXT_TAG_READY + " ");
                    //    }
                    //    string theme = hashtable[PlayData.THEME] != null ? hashtable[PlayData.THEME].ToString() : null;
                    //    switch (theme)
                    //    {
                    //        case MafiaData.SCENE:
                    //            stringBuilder.Append(MafiaData.TEXT_TITLE + " ");
                    //            break;
                    //        default:
                    //            stringBuilder.Append(" ");
                    //            break;
                    //    }
                    //}
                    if(_usingItems.ContainsKey(roomName))
                    {
                        _usingItems[roomName]?.Set(stringBuilder.ToString(), action);
                    }
                    else if(_keepingItems.Count > 0)
                    {
                        int index = _keepingItems.Count - 1;
                        Item item = _keepingItems[index];
                        item?.Set(stringBuilder.ToString(), action);
                        _usingItems.Add(roomName, item);
                        _keepingItems.RemoveAt(index);
                    }
                    else if(prefab == true)
                    {
                        Item item = Instantiate(_item, getScrollRect.content);
                        item.Set(stringBuilder.ToString(), action);
                        _usingItems.Add(roomName, item);
                    }
                }
                else if(_usingItems.ContainsKey(roomName))
                {
                    Item item = _usingItems[roomName];
                    item?.SetActive(false);
                    _usingItems.Remove(roomName);
                    _keepingItems.Add(item);
                }
            }
        }
    }
}