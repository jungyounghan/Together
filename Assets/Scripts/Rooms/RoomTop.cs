using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
/// <summary>
/// 룸의 상단 UI 기능을 담당한다.
/// </summary>
public class RoomTop : MonoBehaviour
{
    private struct Account
    {
        public object nickname {
            private set;
            get;
        }

        public object password {
            private set;
            get;
        }

        public Account(object nickname, object password)
        {
            this.nickname = nickname;
            this.password = password;
        }
    }

    private Account _localAccount = new Account();
    private Account _masterAccount = new Account();

    [SerializeField]
    private TMP_Text _nameText = null;  //방이름 텍스트
    [SerializeField]
    private TMP_Text _themeText = null; //테마 텍스트
    [SerializeField]
    private TMP_Text _masterText = null;//방장 텍스트
    [SerializeField]
    private TMP_Text _timerText = null; //타이머 텍스트

    [SerializeField]
    private ScrollRect _scrollRect = null;

    [SerializeField]
    private Item _item = null;
    private List<Item> _itemList = new List<Item>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        Summarizer.Set(_nameText, PlayData.TEXT_TAG_ROOM);
        Summarizer.Set(_themeText, PlayData.TEXT_TAG_THEME);
        Summarizer.Set(_masterText, PlayData.TEXT_TAG_MASTER);
        Summarizer.Set(_timerText, "");
        Summarizer.Set(_scrollRect, false);
    }
#endif

    private void SetLocalAccount(Player player)
    {
        if (player != null)
        {
            Hashtable hashtable = player.CustomProperties;
            if (hashtable != null)
            {
                _localAccount = new Account(hashtable[PlayData.NICKNAME], hashtable[PlayData.PASSWORD]);
            }
        }
    }

    public void Initialize(string room, Player masterPlayer, Player localPlayer, Hashtable hashtable)
    {
        Summarizer.Set(_nameText, PlayData.TEXT_TAG_ROOM + "\n" + room);
        Summarizer.Set(_scrollRect, true);
        OnMasterClientSwitched(masterPlayer);
        SetLocalAccount(localPlayer);
        Summarizer.Set(_themeText, PlayData.TEXT_TAG_THEME + "\n" + PlayData.TEXT_TAG_NONE);
        if (localPlayer != masterPlayer)
        {
            OnRoomPropertiesUpdate(hashtable);
        }
    }

    public void OnMasterClientSwitched(Player player)
    {
        if (player != null)
        {
            Hashtable hashtable = player.CustomProperties;
            if (hashtable != null)
            {
                _masterAccount = new Account(hashtable[PlayData.NICKNAME], hashtable[PlayData.PASSWORD]);
                string nickname = _masterAccount.nickname != null ? _masterAccount.nickname.ToString() : null;
                Summarizer.Set(_masterText, PlayData.TEXT_TAG_MASTER + "\n" + nickname);
            }
        }
    }

    public void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if (hashtable != null)
        {
            Dictionary<int, Account> dictionary = new Dictionary<int, Account>();
            foreach (string key in hashtable.Keys)
            {
                if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true)
                {
                    switch (Regex.Replace(key, @"\d", ""))
                    {
                        case PlayData.NICKNAME:
                            if (key == PlayData.NICKNAME + index)
                            {
                                if (dictionary.ContainsKey(index) == true)
                                {
                                    dictionary[index] = new Account(hashtable[key], dictionary[index].password);
                                }
                                else
                                {
                                    dictionary.Add(index, new Account(hashtable[key], null));
                                }
                            }
                            break;
                        case PlayData.PASSWORD:
                            if (key == PlayData.PASSWORD + index)
                            {
                                if (dictionary.ContainsKey(index) == true)
                                {
                                    dictionary[index] = new Account(dictionary[index].nickname, hashtable[key]);
                                }
                                else
                                {
                                    dictionary.Add(index, new Account(null, hashtable[key]));
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (key)
                    {
                        case PlayData.THEME:
                            string theme = hashtable[key] != null ? hashtable[key].ToString() : null;
                            switch (theme)
                            {
                                case MafiaData.SCENE:
                                    Summarizer.Set(_themeText, PlayData.TEXT_TAG_THEME + "\n" + MafiaData.TEXT_TITLE);
                                    break;
                                default:
                                    Summarizer.Set(_themeText, PlayData.TEXT_TAG_THEME + "\n" + PlayData.TEXT_TAG_NONE);
                                    break;
                            }
                            break;
                        case PlayData.TIMER:
                            if(hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int timer))
                            {
                                if(timer > 0)
                                {
                                    Summarizer.Set(_timerText, timer.ToString());
                                }
                                else
                                {
                                    Summarizer.Set(_timerText, "");
                                }
                            }
                            else
                            {
                                Summarizer.Set(_timerText, "");
                            }
                            break;
                    }                  
                }
            }
            bool prefab = _item != null && _scrollRect != null && _scrollRect.content != null;
            int count = _itemList.Count;
            foreach (KeyValuePair<int, Account> kvp in dictionary)
            {
                int index = kvp.Key;
                if (index > count && prefab == true)
                {
                    for (int i = count; i < index; i++)
                    {
                        Item item = Instantiate(_item, _scrollRect.content);
                        _itemList.Add(item);
                    }
                    count = _itemList.Count;
                }
                if (index > 0 && index <= count && _itemList[index - 1] != null)
                {
                    Account account = kvp.Value;
                    if (account.nickname == null && account.password == null)
                    {
                        _itemList[index - 1].SetActive(false);
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(account.nickname);
                        if (Equals(account.nickname, _masterAccount.nickname) && Equals(account.password, _masterAccount.password))
                        {
                            stringBuilder.Append("(" + PlayData.TEXT_TAG_MASTER + ")");
                        }
                        if (Equals(account.nickname, _localAccount.nickname) && Equals(account.password, _localAccount.password))
                        {
                            stringBuilder.Append("(" + PlayData.TEXT_TAG_MINE + ")");
                        }
                        _itemList[index - 1].Set(stringBuilder.ToString(), true, false);
                    }
                }
            }
        }
    }
}