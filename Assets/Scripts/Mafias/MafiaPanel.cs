using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
/// <summary>
/// 마피아 게임에 필요한 기본 설정을 수정할 수 있다.
/// </summary>
public class MafiaPanel : MonoBehaviour
{
    [Serializable]
    private struct Bundle
    {
        [SerializeField]
        private TMP_Text infoText;
        [SerializeField]
        private TMP_Text countText;
        [SerializeField]
        private Button leftButton;
        [SerializeField]
        private Button centerButton;
        [SerializeField]
        private Button rightButton;

        public void Set(bool interactable)
        {
            Summarizer.Set(leftButton, interactable);
            Summarizer.Set(rightButton, interactable);
        }

        public void Set(int value)
        {
            Summarizer.Set(countText, value.ToString());
        }

        public void Set(string text)
        {
            Summarizer.Set(infoText, text);
        }

        public void Set(bool interactable, int value, string text)
        {
            Set(interactable);
            Set(value);
            Set(text);
        }

        public void Set(UnityAction leftAction, UnityAction centerAction, UnityAction rightAction)
        {
            Summarizer.Set(leftButton, leftAction);
            Summarizer.Set(centerButton, centerAction);
            Summarizer.Set(rightButton, rightAction);
        }
    }

    [SerializeField]
    private Bundle _startMafiaBundle = new Bundle();
    [SerializeField]
    private Bundle _startMoneyBundle = new Bundle();
    [SerializeField]
    private Bundle _dailyMoneyBundle = new Bundle();
    [SerializeField]
    private Bundle _suffrageBundle = new Bundle();
    [SerializeField]
    private Bundle _nutritionBundle = new Bundle();
    [SerializeField]
    private Bundle _amuletBundle = new Bundle();
    [SerializeField]
    private Bundle _telescopeBundle = new Bundle();
    [SerializeField]
    private Bundle _fabricationBundle = new Bundle();

    [SerializeField]
    private TMP_Text _randomText = null;
    [SerializeField]
    private TMP_Text _citizenText = null;
    [SerializeField]
    private TMP_Text _mafiaText = null;

    [SerializeField]
    private Button _randomButton = null;
    [SerializeField]
    private Button _citizenButton = null;
    [SerializeField]
    private Button _mafiaButton = null;

    private UnityAction<string> infoAction = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _startMafiaBundle.Set(false, 0, MafiaData.TEXT_START_MAFIA);
        _startMoneyBundle.Set(false, 0, MafiaData.TEXT_START_MONEY);
        _dailyMoneyBundle.Set(false, 0, MafiaData.TEXT_DAILY_MONEY);
        _suffrageBundle.Set(false, 0, MafiaData.TEXT_TAG_SUFFRAGE + " " + MafiaData.TEXT_TAG_PRICE);
        _nutritionBundle.Set(false, 0, MafiaData.TEXT_TAG_NUTRITION + " " + MafiaData.TEXT_TAG_PRICE);
        _amuletBundle.Set(false, 0, MafiaData.TEXT_TAG_AMULET + " " + MafiaData.TEXT_TAG_PRICE);
        _telescopeBundle.Set(false, 0, MafiaData.TEXT_TAG_TELESCOPE + " " + MafiaData.TEXT_TAG_PRICE);
        _fabricationBundle.Set(false, 0, MafiaData.TEXT_TAG_FABRICATION + " " + MafiaData.TEXT_TAG_PRICE);
        Summarizer.Set(_randomText, MafiaData.TEXT_APPLY_RANDOM);
        Summarizer.Set(_citizenText, MafiaData.TEXT_APPLY_CITIZEN);
        Summarizer.Set(_mafiaText, MafiaData.TEXT_APPLY_MAFIA);
        Summarizer.Set(_randomButton, MafiaData.COLOR_DESELECT, false);
        Summarizer.Set(_citizenButton, MafiaData.COLOR_DESELECT, false);
        Summarizer.Set(_mafiaButton, MafiaData.COLOR_DESELECT, false);
    }
#endif

    public void Initialize(UnityAction<string> infoAction, Hashtable hashtable, bool master)
    {
        this.infoAction = infoAction;
        UnityAction<string, bool> bundleAction = (data, increasing) =>
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null && PhotonNetwork.IsMasterClient == true)
            {
                Hashtable hashtable = room.CustomProperties;
                if (hashtable != null)
                {
                    int count = hashtable[data] != null && int.TryParse(hashtable[data].ToString(), out int value) ? value : 0;
                    if (increasing == true)
                    {
                        switch(data)
                        {
                            case MafiaData.START_MAFIA:
                                if ((count + 1) * 2 < room.PlayerCount)
                                {
                                    room.SetCustomProperties(new Hashtable() { { data, count + 1 } });
                                }
                                else
                                {
                                    this.infoAction?.Invoke(string.Format(MafiaData.TEXT_MAXIMUM_PLAYERS, Mathf.FloorToInt((room.PlayerCount - 1) * 0.5f)));
                                }
                                break;
                            default:
                                if(count < int.MaxValue)
                                {
                                    room.SetCustomProperties(new Hashtable() { { data, count + 1 } });
                                }
                                else
                                {
                                    this.infoAction?.Invoke(PlayData.TEXT_MAXIMUM_VALUE);
                                }
                                break;
                        }
                    }
                    else
                    {
                        if(count > 0)
                        {
                            room.SetCustomProperties(new Hashtable() { { data, count - 1 } });
                        }
                        else
                        {
                            this.infoAction?.Invoke(PlayData.TEXT_MINIMUM_VALUE);
                        }
                    }
                }
            }
        };
        _startMafiaBundle.Set(() => bundleAction(MafiaData.START_MAFIA, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_START_MAFIA), () => bundleAction(MafiaData.START_MAFIA, true));
        _startMoneyBundle.Set(() => bundleAction(MafiaData.START_MONEY, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_START_MONEY), () => bundleAction(MafiaData.START_MONEY, true));
        _dailyMoneyBundle.Set(() => bundleAction(MafiaData.DAILY_MONEY, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_DAILY_MONEY), () => bundleAction(MafiaData.DAILY_MONEY, true));
        _suffrageBundle.Set(() => bundleAction(MafiaData.COST_SUFFRAGE, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_COST_SUFFRAGE), () => bundleAction(MafiaData.COST_SUFFRAGE, true));
        _nutritionBundle.Set(() => bundleAction(MafiaData.COST_NUTRITION, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_COST_NUTRITION), () => bundleAction(MafiaData.COST_NUTRITION, true));
        _amuletBundle.Set(() => bundleAction(MafiaData.COST_AMULET, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_COST_AMULET), () => bundleAction(MafiaData.COST_AMULET, true));
        _telescopeBundle.Set(() => bundleAction(MafiaData.COST_TELESCOPE, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_COST_TELESCOPE), () => bundleAction(MafiaData.COST_TELESCOPE, true));
        _fabricationBundle.Set(() => bundleAction(MafiaData.COST_FABRICATION, false), () => this.infoAction?.Invoke(MafiaData.TEXT_INFO_COST_FABRICATION), () => bundleAction(MafiaData.COST_FABRICATION, true));
        UnityAction<int> selectAction = (value) =>
        {
            Player player = PhotonNetwork.LocalPlayer;
            Room room = PhotonNetwork.CurrentRoom;
            if (player != null && room != null)
            {
                Hashtable hashtable = player.CustomProperties;
                if (hashtable != null)
                {
                    string nickname = hashtable[PlayData.NICKNAME] != null ? hashtable[PlayData.NICKNAME].ToString() : null;
                    string password = hashtable[PlayData.PASSWORD] != null ? hashtable[PlayData.PASSWORD].ToString() : null;
                    hashtable = room.CustomProperties;
                    if(hashtable != null)
                    {
                        int count = room.PlayerCount;
                        for (int i = 0; i < count; i++)
                        {
                            int index = i + 1;
                            if (Equals(nickname, hashtable[PlayData.NICKNAME + index]) && Equals(password, hashtable[PlayData.PASSWORD + index]))
                            {
                                if (Equals(value, hashtable[PlayData.SELECTION + index]) == false)
                                {
                                    room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, value } });
                                }
                                return;
                            }
                        }
                    }
                }
            }
        };
        Summarizer.Set(_randomButton, () => selectAction(0), true);
        Summarizer.Set(_citizenButton, () => selectAction(1), true);
        Summarizer.Set(_mafiaButton, () => selectAction(2), true);
        int startMafia = 0;
        int startMoney = 0;
        int dividend = 0;
        int suffrage = 0;
        int nutrition = 0;
        int amulet = 0;
        int telescope = 0;
        int fabrication = 0;
        if (hashtable != null)
        {
            if (hashtable[MafiaData.START_MAFIA] != null)
            {
                int.TryParse(hashtable[MafiaData.START_MAFIA].ToString(), out startMafia);
            }
            if (hashtable[MafiaData.START_MONEY] != null)
            {
                int.TryParse(hashtable[MafiaData.START_MONEY].ToString(), out startMoney);
            }
            if (hashtable[MafiaData.DAILY_MONEY] != null)
            {
                int.TryParse(hashtable[MafiaData.DAILY_MONEY].ToString(), out dividend);
            }
            if (hashtable[MafiaData.COST_SUFFRAGE] != null)
            {
                int.TryParse(hashtable[MafiaData.COST_SUFFRAGE].ToString(), out suffrage);
            }
            if (hashtable[MafiaData.COST_NUTRITION] != null)
            {
                int.TryParse(hashtable[MafiaData.COST_NUTRITION].ToString(), out nutrition);
            }
            if (hashtable[MafiaData.COST_AMULET] != null)
            {
                int.TryParse(hashtable[MafiaData.COST_AMULET].ToString(), out amulet);
            }
            if (hashtable[MafiaData.COST_TELESCOPE] != null)
            {
                int.TryParse(hashtable[MafiaData.COST_TELESCOPE].ToString(), out telescope);
            }
            if (hashtable[MafiaData.COST_FABRICATION] != null)
            {
                int.TryParse(hashtable[MafiaData.COST_FABRICATION].ToString(), out fabrication);
            }
        }
        _startMafiaBundle.Set(master, startMafia, MafiaData.TEXT_START_MAFIA);
        _startMoneyBundle.Set(master, startMoney, MafiaData.TEXT_START_MONEY);
        _dailyMoneyBundle.Set(master, dividend, MafiaData.TEXT_DAILY_MONEY);
        _suffrageBundle.Set(master, suffrage, MafiaData.TEXT_TAG_SUFFRAGE + " " + MafiaData.TEXT_TAG_PRICE);
        _nutritionBundle.Set(master, nutrition, MafiaData.TEXT_TAG_NUTRITION + " " + MafiaData.TEXT_TAG_PRICE);
        _amuletBundle.Set(master, amulet, MafiaData.TEXT_TAG_AMULET + " " + MafiaData.TEXT_TAG_PRICE);
        _telescopeBundle.Set(master, telescope, MafiaData.TEXT_TAG_TELESCOPE + " " + MafiaData.TEXT_TAG_PRICE);
        _fabricationBundle.Set(master, fabrication, MafiaData.TEXT_TAG_FABRICATION + " " + MafiaData.TEXT_TAG_PRICE);
        Summarizer.Set(_randomText, MafiaData.TEXT_APPLY_RANDOM);
        Summarizer.Set(_citizenText, MafiaData.TEXT_APPLY_CITIZEN);
        Summarizer.Set(_mafiaText, MafiaData.TEXT_APPLY_MAFIA);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetInteractable(bool mine)
    {
        _startMafiaBundle.Set(mine);
        _startMoneyBundle.Set(mine);
        _dailyMoneyBundle.Set(mine);
        _suffrageBundle.Set(mine);
        _nutritionBundle.Set(mine);
        _amuletBundle.Set(mine);
        _telescopeBundle.Set(mine);
        _fabricationBundle.Set(mine);
    }

    public void OnRoomPropertyUpdate(KeyValuePair<string, object> kvp)
    {
        string key = kvp.Key;
        int count = kvp.Value != null && int.TryParse(kvp.Value.ToString(), out int value) ? value : 0;
        if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true && Regex.Replace(key, @"\d", "") == PlayData.SELECTION)
        {
            Player player = PhotonNetwork.LocalPlayer;
            Room room = PhotonNetwork.CurrentRoom;
            if (player != null && room != null)
            {
                Hashtable hashtable = player.CustomProperties;
                if(hashtable != null)
                {
                    string nickname = hashtable[PlayData.NICKNAME] != null ? hashtable[PlayData.NICKNAME].ToString() : null;
                    string password = hashtable[PlayData.PASSWORD] != null ? hashtable[PlayData.PASSWORD].ToString() : null;
                    hashtable = room.CustomProperties;
                    if (hashtable != null && Equals(nickname, hashtable[PlayData.NICKNAME + index]) && Equals(password, hashtable[PlayData.PASSWORD + index]))
                    {
                        switch (count)
                        {
                            case MafiaData.SELECTION_CITIZEN:
                                Summarizer.Set(_randomButton, MafiaData.COLOR_DESELECT);
                                Summarizer.Set(_citizenButton, MafiaData.COLOR_SELECT);
                                Summarizer.Set(_mafiaButton, MafiaData.COLOR_DESELECT);
                                break;
                            case MafiaData.SELECTION_MAFIA:
                                Summarizer.Set(_randomButton, MafiaData.COLOR_DESELECT);
                                Summarizer.Set(_citizenButton, MafiaData.COLOR_DESELECT);
                                Summarizer.Set(_mafiaButton, MafiaData.COLOR_SELECT);
                                break;
                            default:
                                Summarizer.Set(_randomButton, MafiaData.COLOR_SELECT);
                                Summarizer.Set(_citizenButton, MafiaData.COLOR_DESELECT);
                                Summarizer.Set(_mafiaButton, MafiaData.COLOR_DESELECT);
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            switch (kvp.Key)
            {
                case MafiaData.START_MAFIA:
                    _startMafiaBundle.Set(count);
                    break;
                case MafiaData.START_MONEY:
                    _startMoneyBundle.Set(count);
                    break;
                case MafiaData.DAILY_MONEY:
                    _dailyMoneyBundle.Set(count);
                    break;
                case MafiaData.COST_SUFFRAGE:
                    _suffrageBundle.Set(count);
                    break;
                case MafiaData.COST_NUTRITION:
                    _nutritionBundle.Set(count);
                    break;
                case MafiaData.COST_AMULET:
                    _amuletBundle.Set(count);
                    break;
                case MafiaData.COST_TELESCOPE:
                    _telescopeBundle.Set(count);
                    break;
                case MafiaData.COST_FABRICATION:
                    _fabricationBundle.Set(count);
                    break;
            }
        }
    }

    public bool CanPlaying(int playerCount, Hashtable hashtable)
    {
        if (playerCount >= MafiaData.COUNT_MINIMUM_PLAYERS)
        {
            if (hashtable != null)
            {
                int mafia = hashtable[MafiaData.START_MAFIA] != null && int.TryParse(hashtable[MafiaData.START_MAFIA].ToString(), out int value) ? value : 0;
                if (playerCount > mafia * 2 && mafia > 0)
                {
                    int telescope = hashtable[MafiaData.COST_TELESCOPE] != null && int.TryParse(hashtable[MafiaData.COST_TELESCOPE].ToString(), out telescope) ? telescope:0;
                    int fabrication = hashtable[MafiaData.COST_FABRICATION] != null && int.TryParse(hashtable[MafiaData.COST_FABRICATION].ToString(), out fabrication) ? fabrication : 0;
                    if((telescope <= 0 && fabrication > 0) == false)
                    {
                        return true;
                    }
                    else
                    {
                        infoAction?.Invoke(MafiaData.TEXT_UNNECESSARY_ITEM);
                    }
                }
                else
                {
                    infoAction?.Invoke(string.Format(MafiaData.TEXT_MAXIMUM_PLAYERS, Mathf.FloorToInt((playerCount - 1) * 0.5f)));
                }
            }
        }
        else
        {
            infoAction?.Invoke(string.Format(MafiaData.TEXT_MINIMUM_PLAYERS, MafiaData.COUNT_MINIMUM_PLAYERS));
        }
        return false;
    }
}