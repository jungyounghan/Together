using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
/// <summary>
/// 마피아 게임의 하단 UI 기능을 담당한다.
/// </summary>
public class MafiaBottom : MonoBehaviour
{
    private class Index
    {
        public int number {
            private set;
            get;
        }

        public Index(int number)
        {
            this.number = number;
        }
    }

    private Index _index = null;

    private class Identity
    {
        public bool value {
            private set;
            get;
        }

        public Identity(bool value)
        {
            this.value = value;
        }
    }

    private Identity _identity = null;

    [SerializeField]
    private TMP_Text _selectText = null;
    [SerializeField]
    private TMP_Text _energyText = null;
    [SerializeField]
    private TMP_Text _sliderText = null;
    [SerializeField]
    private TMP_Text _ownItemText = null;
    [SerializeField]
    private TMP_Text _shopItemText = null;
    [SerializeField]
    private TMP_Text _moneyText = null;
    [SerializeField]
    private TMP_Text _amuletText = null;
    [SerializeField]
    private Button _energyButton = null;
    [SerializeField]
    private Slider _energySlider = null;
    [SerializeField]
    private ScrollRect _scrollRect = null;

    [SerializeField]
    private Item _item = null;
    private List<Item> _usingItems = new List<Item>();
    private List<Item> _keepingItems = new List<Item>();

    [Serializable]
    private struct Bundle
    {
        [SerializeField]
        private TMP_Text infoText;
        [SerializeField]
        private TMP_Text buttontext;
        [SerializeField]
        private Button clickButton;

        public void SetInteractable(bool interactable)
        {
            Summarizer.Set(clickButton, interactable);
        }

        public void SetActive(bool value)
        {
            if (infoText != null)
            {
                infoText.gameObject.SetActive(value);
            }
            if (buttontext != null)
            {
                buttontext.gameObject.SetActive(value);
            }
            if(clickButton != null)
            {
                clickButton.gameObject.SetActive(value);
            }
        }

        public void SetListener(UnityAction unityAction)
        {
            Summarizer.Set(clickButton, unityAction);
            SetActive(false);
        }

        public void Set(string info, string button, bool interactable)
        {
            Summarizer.Set(infoText, info);
            Summarizer.Set(buttontext, button);
            Summarizer.Set(clickButton, interactable);
        }

        public void Set(string info, string button, UnityAction unityAction)
        {
            Summarizer.Set(infoText, info);
            Summarizer.Set(buttontext, button);
            Summarizer.Set(clickButton, unityAction);
            SetActive(true);
        }
    }

    [SerializeField]
    private Bundle _suffrageBundle = new Bundle();
    [SerializeField]
    private Bundle _nutritionBundle = new Bundle();
    [SerializeField]
    private Bundle _exclusiveBundle = new Bundle();
    [SerializeField]
    private Bundle _refreshBundle = new Bundle();
    [SerializeField]
    private Bundle _item1Bundle = new Bundle();
    [SerializeField]
    private Bundle _item2Bundle = new Bundle();
    [SerializeField]
    private Bundle _item3Bundle = new Bundle();
    [SerializeField]
    private Bundle _item4Bundle = new Bundle();

    private Action refreshAction = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        Summarizer.Set(_selectText, "");
        Summarizer.Set(_energyText, MafiaData.TEXT_TAG_ENERGY);
        Summarizer.Set(_sliderText, 0 + "/" + MafiaData.VALUE_START_ENERGY);
        if (Application.isPlaying == false)
        {
            UnityEditor.EditorApplication.delayCall += () => Summarizer.Set(_energySlider, 0);
        }
        Summarizer.Set(_ownItemText, MafiaData.TEXT_TAG_OWN_ITEM);
        Summarizer.Set(_shopItemText, MafiaData.TEXT_TAG_SHOP_ITEM);
        Summarizer.Set(_moneyText, MafiaData.TEXT_TAG_FUNDAGE + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, 0));
        Summarizer.Set(_amuletText, MafiaData.TEXT_TAG_AMULET + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, 0));
        _suffrageBundle.Set(MafiaData.TEXT_TAG_SUFFRAGE + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, 0), MafiaData.TEXT_TAG_USE, false);
        _nutritionBundle.Set(MafiaData.TEXT_TAG_NUTRITION + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, 0), MafiaData.TEXT_TAG_USE, false);
        _exclusiveBundle.SetActive(false);
        _refreshBundle.Set(MafiaData.TEXT_TAG_ITEM_REFRESH + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, 0), MafiaData.TEXT_TAG_BUY, false);
        _item1Bundle.SetActive(false);
        _item2Bundle.SetActive(false);
        _item3Bundle.SetActive(false);
        _item4Bundle.SetActive(false);
    }
#endif

    private void Refresh()
    {
        int suffrage = 0;
        int nutrition = 0;
        int amulet = 0;
        int telescope = 0;
        int fabrication = 0;
        Room room = PhotonNetwork.CurrentRoom;
        if (room != null)
        {
            Hashtable hashtable = room.CustomProperties;
            if (hashtable != null)
            {
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
                if (hashtable[MafiaData.COST_TELESCOPE] != null && _identity != null && _identity.value == false)
                {
                    int.TryParse(hashtable[MafiaData.COST_TELESCOPE].ToString(), out telescope);
                }
                if (hashtable[MafiaData.COST_FABRICATION] != null && _identity != null && _identity.value == true)
                {
                    int.TryParse(hashtable[MafiaData.COST_FABRICATION].ToString(), out fabrication);
                }
            }
        }
        _refreshBundle.SetInteractable(suffrage > 0 || nutrition > 0 || amulet > 0 || (_identity != null && ((_identity.value == false && telescope > 0) || (_identity.value == true && fabrication > 0))));
        _refreshBundle.SetActive(suffrage > 0 || nutrition > 0 || amulet > 0 || (_identity != null && ((_identity.value == false && telescope > 0) || (_identity.value == true && fabrication > 0))));
        refreshAction?.Invoke();
    }

    private void Receive(string message)
    {
        if (string.IsNullOrEmpty(message) == false)
        {
            if (int.TryParse(message, out int suspect) == true)
            {
                int count = _keepingItems.Count;
                if (count > 0)
                {
                    Item item = _keepingItems[count - 1];
                    item?.SetActive(string.Format(MafiaData.TEXT_BE_SUSPECTED, suspect), true);
                    _usingItems.Add(item);
                    _keepingItems.RemoveAt(count - 1);
                }
                else if(_item != null && _scrollRect != null && _scrollRect.content != null)
                {
                    Item item = Instantiate(_item, _scrollRect.content);
                    item.SetInteractable(string.Format(MafiaData.TEXT_BE_SUSPECTED, suspect), false);
                    _usingItems.Add(item);
                }
            }
            else
            {
                int count = _keepingItems.Count;
                if (count > 0)
                {
                    Item item = _keepingItems[count - 1];
                    item?.SetActive(message, true);
                    _usingItems.Add(item);
                    _keepingItems.RemoveAt(count - 1);
                }
                else if (_item != null && _scrollRect != null && _scrollRect.content != null)
                {
                    Item item = Instantiate(_item, _scrollRect.content);
                    item.SetInteractable(message, false);
                    _usingItems.Add(item);
                }
            }
            if (_scrollRect != null && _scrollRect.verticalScrollbar != null)
            {
                _scrollRect.verticalScrollbar.value = 0.0f;
            }
        }
        else
        {
            int count = _usingItems.Count;
            for (int i = count - 1; 0 <= i; i--)
            {
                Item item = _usingItems[i];
                item?.SetActive(false);
                _keepingItems.Add(item);
                _usingItems.RemoveAt(i);
            }
        }
    }

    private bool IsLocal(int index)
    {
        if (_index != null)
        {
            return _index.number == index;
        }
        else
        {
            Player player = PhotonNetwork.LocalPlayer;
            if (player != null)
            {
                Room room = PhotonNetwork.CurrentRoom;
                if (room != null)
                {
                    Hashtable hashtable = player.CustomProperties;
                    if (hashtable != null)
                    {
                        string nickname = hashtable[PlayData.NICKNAME] != null ? hashtable[PlayData.NICKNAME].ToString() : null;
                        string password = hashtable[PlayData.PASSWORD] != null ? hashtable[PlayData.PASSWORD].ToString() : null;
                        if (string.IsNullOrEmpty(nickname) == false && string.IsNullOrEmpty(password) == false)
                        {
                            hashtable = room.CustomProperties;
                            if (hashtable != null && Equals(hashtable[PlayData.NICKNAME + index], nickname) && Equals(hashtable[PlayData.PASSWORD + index], password))
                            {
                                _index = new Index(index);
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public void Initialize(Action<string> infoAction, Hashtable hashtable)
    {
        Summarizer.Set(_energyButton, () => infoAction?.Invoke(MafiaData.TEXT_INFO_ENERGY));
        Summarizer.Set(_energyText, MafiaData.TEXT_TAG_ENERGY);
        Summarizer.Set(_ownItemText, MafiaData.TEXT_TAG_OWN_ITEM);
        if (_amuletText != null)
        {
            _amuletText.gameObject.SetActive(false);
        }
        UnityAction suffrageAction = () =>
        {
            if (hashtable != null && _index != null && hashtable[MafiaData.ENERGY + _index.number] != null && int.TryParse(hashtable[MafiaData.ENERGY + _index.number].ToString(), out int energy) && energy > 0 && hashtable[MafiaData.SUFFRAGE + _index.number] != null && int.TryParse(hashtable[MafiaData.SUFFRAGE + _index.number].ToString(), out int suffrage) && suffrage > 0)
            {
                int option = hashtable[MafiaData.OPTION + _index.number] != null && int.TryParse(hashtable[MafiaData.OPTION + _index.number].ToString(), out option) ? option : MafiaData.SELECTION_NONE;
                if (option != MafiaData.SELECTION_SUFFRAGE)
                {
                    PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.OPTION + _index.number, MafiaData.SELECTION_SUFFRAGE } });
                }
                else
                {
                    PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.OPTION + _index.number, MafiaData.SELECTION_NONE } });
                }
            }
        };
        _suffrageBundle.SetListener(suffrageAction);
        UnityAction nutritionAction = () =>
        {
            if(hashtable != null && _index != null && hashtable[MafiaData.ENERGY + _index.number] != null && int.TryParse(hashtable[MafiaData.ENERGY + _index.number].ToString(), out int energy) && energy > 0 && hashtable[MafiaData.NUTRITION + _index.number] != null && int.TryParse(hashtable[MafiaData.NUTRITION + _index.number].ToString(), out int nutrition) && nutrition > 0)
            {
                if(energy < MafiaData.VALUE_START_ENERGY)
                {
                    PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { {MafiaData.NUTRITION + _index.number, nutrition - 1}, {MafiaData.ENERGY + _index.number, energy + 1} });
                    Receive(MafiaData.TEXT_RECOVER_ENERGY);
                }
                else
                {
                    Receive(MafiaData.TEXT_FULL_ENERGY);
                }
            }
        };
        _nutritionBundle.SetListener(nutritionAction);
        UnityAction exclusiveAction = () =>
        {
            if (hashtable != null && _identity != null && _identity.value == false && _index != null && hashtable[MafiaData.ENERGY + _index.number] != null && int.TryParse(hashtable[MafiaData.ENERGY + _index.number].ToString(), out int energy) && energy > 0 && hashtable[MafiaData.EXCLUSIVE + _index.number] != null && int.TryParse(hashtable[MafiaData.EXCLUSIVE + _index.number].ToString(), out int exclusive) && exclusive > 0)
            {
                int option = hashtable[MafiaData.OPTION + _index.number] != null && int.TryParse(hashtable[MafiaData.OPTION + _index.number].ToString(), out option) ? option : MafiaData.SELECTION_NONE;
                if (option != MafiaData.SELECTION_TELESCOPE)
                {
                    PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.OPTION + _index.number, MafiaData.SELECTION_TELESCOPE } });
                }
                else
                {
                    PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.OPTION + _index.number, MafiaData.SELECTION_NONE } });
                }
            }
        };
        _exclusiveBundle.SetListener(exclusiveAction);
        Summarizer.Set(_shopItemText, MafiaData.TEXT_TAG_SHOP_ITEM);
        int suffrage = 0;
        int nutrition = 0;
        int amulet = 0;
        int telescope = 0;
        int fabrication = 0;
        if (hashtable != null)
        {
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
        this.refreshAction = () =>
        {
            Dictionary<int, UnityAction<int, Bundle>> actions = new Dictionary<int, UnityAction<int, Bundle>>();
            if (suffrage > 0)
            {
                UnityAction<int, Bundle> action = (count, bundle) =>
                {
                    if (hashtable != null && _index != null && hashtable[MafiaData.MONEY + _index.number] != null && int.TryParse(hashtable[MafiaData.MONEY + _index.number].ToString(), out int money))
                    {
                        if (money >= suffrage * count)
                        {
                            int remains = hashtable[MafiaData.SUFFRAGE + _index.number] != null && int.TryParse(hashtable[MafiaData.SUFFRAGE + _index.number].ToString(), out remains) ? remains : 0;
                            PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + _index.number, money - (suffrage * count) }, { MafiaData.SUFFRAGE + _index.number, remains + count } });
                            bundle.SetInteractable(false);
                        }
                        else
                        {
                            Receive(MafiaData.TEXT_LACK_OF_MONEY);
                        }
                    }
                };
                actions.Add(1, action);
            }
            if (nutrition > 0)
            {
                UnityAction<int, Bundle> action = (count, bundle) =>
                {
                    if (hashtable != null && _index != null && hashtable[MafiaData.MONEY + _index.number] != null && int.TryParse(hashtable[MafiaData.MONEY + _index.number].ToString(), out int money))
                    {
                        if (money >= nutrition * count)
                        {
                            int remains = hashtable[MafiaData.NUTRITION + _index.number] != null && int.TryParse(hashtable[MafiaData.NUTRITION + _index.number].ToString(), out remains) ? remains : 0;
                            PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + _index.number, money - (nutrition * count) }, { MafiaData.NUTRITION + _index.number, remains + count } });
                            bundle.SetInteractable(false);
                        }
                        else
                        {
                            Receive(MafiaData.TEXT_LACK_OF_MONEY);
                        }
                    }
                };
                actions.Add(2, action);
            }
            if (amulet > 0)
            {
                UnityAction<int, Bundle> action = (count, bundle) =>
                {
                    if (hashtable != null && _index != null && hashtable[MafiaData.MONEY + _index.number] != null && int.TryParse(hashtable[MafiaData.MONEY + _index.number].ToString(), out int money))
                    {
                        if (money >= amulet * count)
                        {
                            int remains = hashtable[MafiaData.AMULET + _index.number] != null && int.TryParse(hashtable[MafiaData.AMULET + _index.number].ToString(), out remains) ? remains : 0;
                            PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + _index.number, money - (amulet * count) }, { MafiaData.AMULET + _index.number, remains + count } });
                            bundle.SetInteractable(false);
                        }
                        else
                        {
                            Receive(MafiaData.TEXT_LACK_OF_MONEY);
                        }
                    }
                };
                actions.Add(3, action);
            }
            if (telescope > 0 && _identity != null && _identity.value == false)
            {
                UnityAction<int, Bundle> action = (count, bundle) =>
                {
                    if (hashtable != null && _index != null && hashtable[MafiaData.MONEY + _index.number] != null && int.TryParse(hashtable[MafiaData.MONEY + _index.number].ToString(), out int money))
                    {
                        if (money >= telescope * count)
                        {
                            int remains = hashtable[MafiaData.EXCLUSIVE + _index.number] != null && int.TryParse(hashtable[MafiaData.EXCLUSIVE + _index.number].ToString(), out remains) ? remains : 0;
                            PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + _index.number, money - (telescope * count) }, { MafiaData.EXCLUSIVE + _index.number, remains + count } });
                            bundle.SetInteractable(false);
                        }
                        else
                        {
                            Receive(MafiaData.TEXT_LACK_OF_MONEY);
                        }
                    }
                };
                actions.Add(4, action);
            }
            if (fabrication > 0 && _identity != null && _identity.value == true)
            {
                UnityAction<int, Bundle> action = (count, bundle) =>
                {
                    if (hashtable != null && _index != null && hashtable[MafiaData.MONEY + _index.number] != null && int.TryParse(hashtable[MafiaData.MONEY + _index.number].ToString(), out int money))
                    {
                        if (money >= fabrication * count)
                        {
                            int remains = hashtable[MafiaData.EXCLUSIVE + _index.number] != null && int.TryParse(hashtable[MafiaData.EXCLUSIVE + _index.number].ToString(), out remains) ? remains : 0;
                            PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + _index.number, money - (fabrication * count) }, { MafiaData.EXCLUSIVE + _index.number, remains + count } });
                            bundle.SetInteractable(false);
                        }
                        else
                        {
                            Receive(MafiaData.TEXT_LACK_OF_MONEY);
                        }
                    }
                };
                actions.Add(4, action);
            }
            int range = Random.Range(1, 4);
            if (actions.Count > 0 && hashtable != null)
            {
                for(int i = 0; i < range; i++)
                {
                    System.Random rand = new System.Random();
                    KeyValuePair<int, UnityAction<int, Bundle>> kvp = actions.ElementAt(rand.Next(0, actions.Count));
                    string text = null;
                    int count = Random.Range(1, 3);
                    switch (kvp.Key)
                    {
                        case 1:
                            text = MafiaData.TEXT_TAG_SUFFRAGE + " " + string.Format(PlayData.TEXT_TAG_NUMBER, count) + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, suffrage * count);
                            break;
                        case 2:
                            text = MafiaData.TEXT_TAG_NUTRITION + " " + string.Format(PlayData.TEXT_TAG_NUMBER, count) + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, nutrition * count);
                            break;
                        case 3:
                            text = MafiaData.TEXT_TAG_AMULET + " " + string.Format(PlayData.TEXT_TAG_NUMBER, count) + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, amulet * count);
                            break;
                        case 4:
                            if(_identity.value == false)
                            {
                                text = MafiaData.TEXT_TAG_TELESCOPE + " " + string.Format(MafiaData.TEXT_TAG_TIME, count) + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, telescope * count);
                            }
                            else
                            {
                                text = MafiaData.TEXT_TAG_FABRICATION + " " + string.Format(MafiaData.TEXT_TAG_TIME, count) + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, fabrication * count);
                            }
                            break;
                    }
                    switch (i)
                    {
                        case 0:
                            _item1Bundle.Set(text, MafiaData.TEXT_TAG_BUY, () => kvp.Value(count, _item1Bundle));
                            _item1Bundle.SetInteractable(true);
                            break;
                        case 1:
                            _item2Bundle.Set(text, MafiaData.TEXT_TAG_BUY, () => kvp.Value(count, _item2Bundle));
                            _item2Bundle.SetInteractable(true);
                            break;
                        case 2:
                            _item3Bundle.Set(text, MafiaData.TEXT_TAG_BUY, () => kvp.Value(count, _item3Bundle));
                            _item3Bundle.SetInteractable(true);
                            break;
                        case 3:
                            _item4Bundle.Set(text, MafiaData.TEXT_TAG_BUY, () => kvp.Value(count, _item4Bundle));
                            _item4Bundle.SetInteractable(true);
                            break;
                    }
                }
            }
            else
            {
                range = 0;
            }
            for (int i = range; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        _item1Bundle.SetActive(false);
                        break;
                    case 1:
                        _item2Bundle.SetActive(false);
                        break;
                    case 2:
                        _item3Bundle.SetActive(false);
                        break;
                    case 3:
                        _item4Bundle.SetActive(false);
                        break;
                }
            }
        };
        UnityAction refreshAction = () =>
        {
            if (hashtable != null && _index != null && hashtable[MafiaData.MONEY + _index.number] != null && int.TryParse(hashtable[MafiaData.MONEY + _index.number].ToString(), out int money))
            {
                if (money >= MafiaData.VALUE_COST_REFRESH)
                {
                    this.refreshAction?.Invoke();
                    PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + _index.number, money - MafiaData.VALUE_COST_REFRESH } });
                }
                else
                {
                    Receive(MafiaData.TEXT_LACK_OF_MONEY);
                }
            }
        };
        _refreshBundle.Set(MafiaData.TEXT_TAG_ITEM_REFRESH + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, MafiaData.VALUE_COST_REFRESH), MafiaData.TEXT_TAG_BUY, refreshAction);
        _refreshBundle.SetActive(false);
        OnRoomPropertiesUpdate(hashtable);
    }

    public void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if(hashtable != null)
        {
            foreach(string key in hashtable.Keys)
            {
                if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true)
                {
                    switch (Regex.Replace(key, @"\d", ""))
                    {
                        case PlayData.MESSAGE:
                            if(key == PlayData.MESSAGE + index && IsLocal(index) == true && hashtable[key] != null)
                            {
                                Receive(hashtable[key].ToString());
                                PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable() { {key, null} });
                            }
                            break;
                        case MafiaData.ENERGY:
                            if (key == MafiaData.ENERGY + index && IsLocal(index) == true)
                            {
                                int energy = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out energy) ? energy : 0;
                                Summarizer.Set(_sliderText, Mathf.Clamp(energy, 0, MafiaData.VALUE_START_ENERGY) + "/" + MafiaData.VALUE_START_ENERGY);
                                Summarizer.Set(_energySlider, (float)energy / MafiaData.VALUE_START_ENERGY);
                                _suffrageBundle.SetInteractable(energy > 0);
                                _nutritionBundle.SetInteractable(energy > 0);
                                _exclusiveBundle.SetInteractable(energy > 0);
                            }
                            break;
                        case MafiaData.MONEY:
                            if(key == MafiaData.MONEY + index && IsLocal(index) == true)
                            {
                                int money = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out money) ? money : 0;
                                Summarizer.Set(_moneyText, MafiaData.TEXT_TAG_FUNDAGE + ": " + string.Format(MafiaData.TEXT_TAG_MONEY, money));
                            }
                            break;
                        case MafiaData.IDENTITY:
                            if(key == MafiaData.IDENTITY + index && IsLocal(index) == true && hashtable[key] != null && bool.TryParse(hashtable[key].ToString(), out bool identity))
                            {
                                _identity = new Identity(identity);
                                Refresh();
                            }
                            break;
                        case MafiaData.AMULET:
                            if(key == MafiaData.AMULET + index && IsLocal(index) == true && hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int amulet))
                            {
                                Summarizer.Set(_amuletText, MafiaData.TEXT_TAG_AMULET + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, amulet));
                            }
                            break;
                        case MafiaData.SUFFRAGE:
                            if (key == MafiaData.SUFFRAGE + index && IsLocal(index) == true)
                            {
                                int suffrage = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out suffrage)? suffrage: 0;
                                _suffrageBundle.SetActive(suffrage > 0);
                                _suffrageBundle.Set(MafiaData.TEXT_TAG_SUFFRAGE + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, suffrage), MafiaData.TEXT_TAG_USE, suffrage > 0);
                            }
                            break;
                        case MafiaData.NUTRITION:
                            if(key == MafiaData.NUTRITION + index && IsLocal(index) == true)
                            {
                                int nutrition = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out nutrition) ? nutrition : 0;
                                _nutritionBundle.SetActive(nutrition > 0);
                                _nutritionBundle.Set(MafiaData.TEXT_TAG_NUTRITION + ": " + string.Format(PlayData.TEXT_TAG_NUMBER, nutrition), MafiaData.TEXT_TAG_USE, nutrition > 0);
                            }
                            break;
                        case MafiaData.EXCLUSIVE:
                            if(key == MafiaData.EXCLUSIVE + index && IsLocal(index) == true && _identity != null)
                            {
                                int exclusive = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out exclusive) ? exclusive : 0;
                                _exclusiveBundle.SetActive(exclusive > 0);
                                if (_identity.value == false) //주민
                                {
                                    _exclusiveBundle.Set(MafiaData.TEXT_TAG_TELESCOPE + ": " + string.Format(MafiaData.TEXT_TAG_TIME, exclusive), MafiaData.TEXT_TAG_USE, exclusive > 0);
                                }
                                else //범인
                                {
                                    if(exclusive > 0)
                                    {
                                        Receive(MafiaData.TEXT_PRETEND_TO_CITIZEN);
                                    }
                                    else
                                    {
                                        Receive(MafiaData.TEXT_FINISH_PRETEND);
                                    }
                                    _exclusiveBundle.Set(MafiaData.TEXT_TAG_FABRICATION + ": " + string.Format(MafiaData.TEXT_TAG_TIME, exclusive), MafiaData.TEXT_TAG_USE, false);
                                }
                            }
                            break;
                        case MafiaData.OPTION:
                            if(key == MafiaData.OPTION + index && IsLocal(index) == true && hashtable[key] != null)
                            {
                                int option = int.TryParse(hashtable[key].ToString(), out option) ? option : 0;
                                switch(option)
                                {
                                    case MafiaData.SELECTION_SUFFRAGE:
                                        Summarizer.Set(_selectText, MafiaData.TEXT_USE_PLAN_SUFFRAGE);
                                        break;
                                    case MafiaData.SELECTION_TELESCOPE:
                                        if(_identity != null && _identity.value == false)
                                        {
                                            Summarizer.Set(_selectText, MafiaData.TEXT_INVESTIGATE_TARGET);
                                        }
                                        break;
                                    default:
                                        Summarizer.Set(_selectText, "");
                                        break;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (key)
                    {
                        case PlayData.ROUND:
                            if(hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int round) && round > 0 && (round - 1) % 3 == MafiaData.SECTION_MORNING)
                            {
                                refreshAction?.Invoke();
                            }
                            break;
                        case PlayData.MESSAGE:
                            Receive(hashtable[key] != null ? hashtable[key].ToString(): null);
                            break;
                        case MafiaData.COST_AMULET:
                            if(_amuletText != null)
                            {
                                _amuletText.gameObject.SetActive(hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int amulet) && amulet > 0);
                            }
                            break;
                    }
                }
            }
        }
    }
}