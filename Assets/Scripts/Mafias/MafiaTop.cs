using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
/// <summary>
/// ���Ǿ� ������ ��� UI ����� ����Ѵ�.
/// </summary>
public class MafiaTop : MonoBehaviour
{
    [SerializeField]
    private int _suspect = 0;
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

    private Action<int> selectAction = null;

    [SerializeField]
    private TMP_Text _dayText = null;
    [SerializeField]
    private TMP_Text _survivorText = null;
    [SerializeField]
    private TMP_Text _mafiaText = null;
    [SerializeField]
    private TMP_Text _decisionText = null;
    [SerializeField]
    private TMP_Text _timerText = null;

    [SerializeField]
    private ScrollRect _scrollRect = null;
    [SerializeField]
    private Item _item = null;

    [Serializable]
    private struct Bundle
    {
        [SerializeField]
        private TMP_Text text;
        [SerializeField]
        private Button button;

        public void Set(bool interactable)
        {
            Summarizer.Set(button, interactable);
        }

        public void Set(string text, bool interactable)
        {
            Summarizer.Set(this.text, text);
            Summarizer.Set(button, interactable);
        }

        public void Set(UnityAction action)
        {
            Summarizer.Set(button, action);
        }
    }

    [SerializeField]
    private Bundle _leftBundle = new Bundle();
    [SerializeField]
    private Bundle _rightBundle = new Bundle();

    private struct Member
    {
        public Item item {
            private set;
            get;
        }

        public string nickname {
            private set;
            get;
        }

        public bool alive {
            private set;
            get;
        }

        public bool identity {
            private set;
            get;
        }

        public Member(Item item, string nickname, bool alive = true, bool identity = false) //�ʱ�ȭ�� �⺻���� �̷��� ��Ƴ��ƾ� ��ü�� ��Ű�� �����縦 ������ �� �ִ�.
        {
            this.nickname = nickname;
            this.alive = alive;
            this.identity = identity;
            this.item = item;
        }
    }

    private Dictionary<int, Member> _members = new Dictionary<int, Member>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        Summarizer.Set(_dayText, PlayData.TEXT_TAG_GAMEOVER);
        Summarizer.Set(_survivorText, MafiaData.TEXT_COUNT_OF_SURVIVORS + ": " +  string.Format(PlayData.TEXT_TAG_PEOPLE, 0));
        Summarizer.Set(_mafiaText, MafiaData.TEXT_COUNT_OF_MAFIAS + ": " +  string.Format(PlayData.TEXT_TAG_PEOPLE, 0));
        Summarizer.Set(_decisionText, "");
        Summarizer.Set(_timerText, "");
        _leftBundle.Set("", false);
        _rightBundle.Set("", false);
    }
#endif

    private void Align()
    {
        _members = _members.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.Value);
        foreach (KeyValuePair<int, Member> kvp in _members)
        {
            Item item = kvp.Value.item;
            if (item != null)
            {
                item.transform.SetSiblingIndex(kvp.Key - 1);
            }
        }
    }

    private void Set(Item item, int index, string nickname, bool mine, bool alive, bool identity, bool open)
    {
        if (mine == true) //�� ������� ��� �Ż��� �ٷ� �����ص� �ȴ�.
        {
            if (alive == true) //����ִ� ���
            {
                if (identity == false) //�ù��� ���
                {
                    item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + PlayData.TEXT_TAG_MINE + ")(" + MafiaData.TEXT_TAG_ALIVE + ")(" + MafiaData.TEXT_TAG_CITIZEN + ")", false, MafiaData.COLOR_MINE);
                }
                else //������ ���
                {
                    item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + PlayData.TEXT_TAG_MINE + ")(" + MafiaData.TEXT_TAG_ALIVE + ")(" + MafiaData.TEXT_TAG_MAFIA + ")", false, MafiaData.COLOR_MINE);
                }
            }
            else //���� ���
            {
                if (identity == false) //�ù��� ���
                {
                    item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + PlayData.TEXT_TAG_MINE + ")(" + MafiaData.TEXT_TAG_DEATH + ")(" + MafiaData.TEXT_TAG_CITIZEN + ")", false, MafiaData.COLOR_DEAD);
                }
                else //������ ���
                {
                    item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + PlayData.TEXT_TAG_MINE + ")(" + MafiaData.TEXT_TAG_DEATH + ")(" + MafiaData.TEXT_TAG_MAFIA + ")", false, MafiaData.COLOR_DEAD);
                }
            }
        }
        else //�� ������ �ƴ϶��
        {
            if (alive == true) //����ִ� ���
            {
                if (open == false) //���� �ù��̶�� Ÿ���� ������ �������� �ʴ´�.
                {
                    item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + MafiaData.TEXT_TAG_ALIVE + ")", true);
                }
                else //���� �����̶�� �μ� �������� Ÿ���� ������ �����ص� �ȴ�.
                {
                    if (identity == false) //�ù� ǥ��
                    {
                        item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + MafiaData.TEXT_TAG_ALIVE + ")(" + MafiaData.TEXT_TAG_CITIZEN + ")", true);
                    }
                    else //���� ǥ��
                    {
                        item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + MafiaData.TEXT_TAG_ALIVE + ")(" + MafiaData.TEXT_TAG_MAFIA + ")", true);
                    }
                }
            }
            else //���� ��� �Ż��� �����ȴ�.
            {
                if (identity == false) //�ù��� ���
                {
                    item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + MafiaData.TEXT_TAG_DEATH + ")(" + MafiaData.TEXT_TAG_CITIZEN + ")", false, MafiaData.COLOR_DEAD);
                }
                else //������ ���
                {
                    item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, index) + " " + nickname + "(" + MafiaData.TEXT_TAG_DEATH + ")(" + MafiaData.TEXT_TAG_MAFIA + ")", false, MafiaData.COLOR_DEAD);
                }
            }
        }
    }

    private void Set(int index, bool identity, bool mine, bool open)
    {
        if (_members.ContainsKey(index) == true)
        {
            Item item = _members[index].item;
            Set(item, index, _members[index].nickname, mine, _members[index].alive, identity, open);
            _members[index] = new Member(item, _members[index].nickname, _members[index].alive, identity);
        }
        else
        {
            Room room = PhotonNetwork.CurrentRoom;
            if(room != null)
            {
                Hashtable hashtable = room.CustomProperties;
                if(hashtable != null && hashtable[PlayData.NICKNAME + index] != null && _scrollRect != null && _scrollRect.content != null && _item != null)
                {
                    string nickname = hashtable[PlayData.NICKNAME + index].ToString();
                    Item item = Instantiate(_item, _scrollRect.content);
                    item.Set(() => selectAction?.Invoke(index));
                    _members.Add(index, new Member(item, nickname, true, identity));
                    Set(item, index, nickname, mine, _members[index].alive, identity, open);
                    Align();
                }
            }
        }
    }

    private void Insert(int index, bool identity)
    {
        if (IsLocal(index) == true) //�� �����̸�
        {
            Set(index, identity, true, true);
            if(identity == true)  //���� ������ �Ǿ��ٸ� ���� ������ �ٸ� ����� ���� ��ȸ
            {
                foreach(KeyValuePair<int, Member> kvp in _members) //������ �ڿ� �ƴ� �ڸ� �ĺ�����
                {
                    if (kvp.Key != index)
                    {
                        Member member = kvp.Value;
                        if (member.alive == true) //������
                        {
                            if(member.identity == false) //�ù� ǥ��
                            {
                                member.item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, kvp.Key) + " " + member.nickname + "(" + MafiaData.TEXT_TAG_ALIVE + ")(" + MafiaData.TEXT_TAG_CITIZEN + ")", true);
                            }
                            else //���� ǥ��
                            {
                                member.item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, kvp.Key) + " " + member.nickname + "(" + MafiaData.TEXT_TAG_ALIVE + ")(" + MafiaData.TEXT_TAG_MAFIA + ")", true);
                            }
                        }
                        else //�����
                        {
                            if (member.identity == false) //�ù� ǥ��
                            {
                                member.item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, kvp.Key) + " " + member.nickname + "(" + MafiaData.TEXT_TAG_DEATH + ")(" + MafiaData.TEXT_TAG_CITIZEN + ")", false);
                            }
                            else //���� ǥ��
                            {
                                member.item?.SetInteractable(string.Format(PlayData.TEXT_TAG_INDEX, kvp.Key) + " " + member.nickname + "(" + MafiaData.TEXT_TAG_DEATH + ")(" + MafiaData.TEXT_TAG_MAFIA + ")", false);
                            }
                        }
                    }
                }
            }
        }
        else //�� ������ �ƴ϶��
        {
            Set(index, identity, false, _index != null && _members.ContainsKey(_index.number) == true && _members[_index.number].identity == true);
        }
    }

    private void Insert(int index, int energy)
    {
        bool open = _index != null && _members.ContainsKey(_index.number) == true && _members[_index.number].identity == true;
        if (_members.ContainsKey(index) == true)
        {
            Item item = _members[index].item;
            Set(item, index, _members[index].nickname, IsLocal(index), energy > 0, _members[index].identity, open);
            _members[index] = new Member(item, _members[index].nickname, energy > 0, _members[index].identity);
        }
        else
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null)
            {
                Hashtable hashtable = room.CustomProperties;
                if (hashtable != null && hashtable[PlayData.NICKNAME + index] != null && _scrollRect != null && _scrollRect.content != null && _item != null)
                {
                    string nickname = hashtable[PlayData.NICKNAME + index].ToString();
                    Item item = Instantiate(_item, _scrollRect.content);
                    item.Set(() => selectAction?.Invoke(index));
                    _members.Add(index, new Member(item, nickname, energy > 0, false));
                    Set(item, index, nickname, IsLocal(index), _members[index].alive, _members[index].identity, open);
                    Align();
                }
            }
        }
    }

    private void Select(int index)
    {
        foreach(KeyValuePair<int, Member> kvp in _members)
        {
            Member member = kvp.Value;
            if(index == kvp.Key)
            {
                member.item?.Set(MafiaData.COLOR_SELECT);
            }
            else
            {
                if(member.alive == false)
                {
                    member.item?.Set(MafiaData.COLOR_DEAD);
                }
                else if(IsLocal(kvp.Key) == true)
                {
                    member.item?.Set(MafiaData.COLOR_MINE);
                }
                else
                {
                    member.item?.Set(MafiaData.COLOR_DESELECT);
                }
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

    public void Initialize(Hashtable hashtable)
    {
        //��ư ����
        Action<bool> binaryAction = (select) =>
        {
            if(hashtable != null && hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round) && round > 0)
            {
                switch ((round - 1) % 3)
                {
                    case MafiaData.SECTION_MORNING: //�ð� ���� ��ư
                        if(hashtable[PlayData.TIMER] != null && int.TryParse(hashtable[PlayData.TIMER].ToString(), out int timer) && timer > 0 && _index != null && hashtable[MafiaData.ENERGY + _index.number] != null && int.TryParse(hashtable[MafiaData.ENERGY + _index.number].ToString(), out int energy) && energy > 0)
                        {
                            Room room = PhotonNetwork.CurrentRoom;
                            if (room != null)
                            {
                                if (select == false) //����
                                {
                                    if (timer > MafiaData.VALUE_TIME_DECREASING)
                                    {
                                        room.SetCustomProperties(new Hashtable() { { PlayData.TIMER, timer - MafiaData.VALUE_TIME_DECREASING } });
                                    }
                                    else
                                    {
                                        room.SetCustomProperties(new Hashtable() { { PlayData.TIMER, 0 } });
                                    }
                                    if (_index != null)
                                    {
                                        room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, string.Format(MafiaData.TEXT_DECREASE_TIME, _index.number) } });
                                    }
                                }
                                else //����
                                {
                                    room.SetCustomProperties(new Hashtable() { { PlayData.TIMER, timer + MafiaData.VALUE_TIME_INCREASING } });
                                    if (_index != null)
                                    {
                                        room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, string.Format(MafiaData.TEXT_INCREASE_TIME, _index.number) } });
                                    }
                                }
                                _leftBundle.Set(false);
                                _rightBundle.Set(false);
                            }
                        }
                        break;
                    case MafiaData.SECTION_AFTERNOON: //ó���� ���� ��ǥ�� �ϰ� �����Ⱑ �ִ�.
                        if(_index != null)
                        {
                            Room room = PhotonNetwork.CurrentRoom;
                            if (room != null)
                            {
                                if (hashtable[PlayData.SELECTION + _index.number] != null && int.TryParse(hashtable[PlayData.SELECTION + _index.number].ToString(), out int selection))
                                {
                                    switch(select)
                                    {
                                        case false:
                                            if(selection == _suspect)
                                            {
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE + _index.number, MafiaData.TEXT_ALREADY_YES } });
                                                return;
                                            }
                                            break;
                                        case true:
                                            if(selection == 0)
                                            {
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE + _index.number, MafiaData.TEXT_ALREADY_NO } });
                                                return;
                                            }
                                            break;
                                    }
                                }
                                if (select == false) //����
                                {
                                    room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + _index.number, _suspect }, { PlayData.MESSAGE, string.Format(PlayData.TEXT_TAG_INDEX, _index.number) + " " + hashtable[PlayData.NICKNAME + _index.number] + " " + MafiaData.TEXT_TAG_YES } });
                                }
                                else //�ݴ�
                                {
                                    room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + _index.number, 0 }, { PlayData.MESSAGE, string.Format(PlayData.TEXT_TAG_INDEX, _index.number) + " " + hashtable[PlayData.NICKNAME + _index.number] + " " + MafiaData.TEXT_TAG_NO } });
                                }
                                _leftBundle.Set(false);
                                _rightBundle.Set(false);
                            }
                        }
                        break;
                }
            }
        };
        selectAction = (index) =>
        {
            if (_index != null && hashtable != null && hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round) && round > 0)
            {
                int number = _index.number;
                int energy = hashtable[MafiaData.ENERGY + number] != null && int.TryParse(hashtable[MafiaData.ENERGY + number].ToString(), out energy) ? energy : 0;
                Room room = PhotonNetwork.CurrentRoom;
                if (energy > 0) //����ִ� ���ε�
                {
                    bool identity = hashtable[MafiaData.IDENTITY + number] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + number].ToString(), out identity) ? identity : false;
                    if (identity == false && hashtable[MafiaData.OPTION + number] != null && int.TryParse(hashtable[MafiaData.OPTION + number].ToString(), out int option) && option == MafiaData.SELECTION_TELESCOPE)
                    {
                        if (hashtable[MafiaData.EXCLUSIVE + number] != null && int.TryParse(hashtable[MafiaData.EXCLUSIVE + number].ToString(), out int telescope) && telescope > 0 && hashtable[MafiaData.IDENTITY + index] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + index].ToString(), out identity))
                        {
                            if (identity == false) //�ù��̶�� ���� �����ش�
                            {
                                room?.SetCustomProperties(new Hashtable() { { MafiaData.EXCLUSIVE + number, telescope - 1 }, { PlayData.MESSAGE + number, MafiaData.TEXT_TAG_CITIZEN } });
                            }
                            else //�����ε� ������ �ִٸ� �ù����� �����ְ� �װ� �ƴ϶�� �ù����� �����Ѵ�
                            {
                                if (hashtable[MafiaData.EXCLUSIVE + index] != null && int.TryParse(hashtable[MafiaData.EXCLUSIVE + index].ToString(), out int fabrication) && fabrication > 0)
                                {
                                    room?.SetCustomProperties(new Hashtable() { { MafiaData.EXCLUSIVE + number, telescope - 1 }, { PlayData.MESSAGE + number, MafiaData.TEXT_TAG_CITIZEN } });
                                }
                                else
                                {
                                    room?.SetCustomProperties(new Hashtable() { { MafiaData.EXCLUSIVE + number, telescope - 1 }, { PlayData.MESSAGE + number, MafiaData.TEXT_TAG_MAFIA } });
                                }
                            }
                        }
                        room?.SetCustomProperties(new Hashtable() { { MafiaData.OPTION + number, MafiaData.SELECTION_NONE } });
                    }
                    else if (hashtable[PlayData.SELECTION + number] != null && int.TryParse(hashtable[PlayData.SELECTION + number].ToString(), out int selection) && selection != index)
                    {
                        if (energy > 1)
                        {
                            switch ((round - 1) % 3)
                            {
                                case MafiaData.SECTION_MORNING://ó�� ��ǥ
                                    room?.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + number, index }, { PlayData.MESSAGE, string.Format(MafiaData.TEXT_SELECT_SUSPECT_PERSON, number, index) } });
                                    break;
                                case MafiaData.SECTION_NIGHT: //���� ����
                                    if (identity == true)
                                    {
                                        room?.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + number, index } });
                                        foreach (KeyValuePair<int, Member> kvp in _members)  //�ٸ� �Ʊ��鿡�� ���� �ڽ��� ���� ������ ������ �Ѵ�
                                        {
                                            if (kvp.Value.identity == identity)
                                            {
                                                room?.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE + kvp.Key, string.Format(MafiaData.TEXT_SELECT_VICTIM_PERSON, number, index) } });
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            room?.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE + number,  MafiaData.TEXT_LACK_OF_ENERGY } });
                        }
                    }
                }
                else if(hashtable[PlayData.SELECTION + number] != null && int.TryParse(hashtable[PlayData.SELECTION + number].ToString(), out int selection) && selection != index)
                {
                    room?.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + number, index }, {PlayData.MESSAGE + number, string.Format(MafiaData.TEXT_PREPARE_CURSING, index) } });;
                }
            }
        };
        _leftBundle.Set(() => binaryAction(false));
        _rightBundle.Set(() => binaryAction(true));
        OnRoomPropertiesUpdate(hashtable);
    }

    public void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
        if (hashtable != null)
        {
            foreach (string key in hashtable.Keys)
            {
                if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true)
                {
                    switch (Regex.Replace(key, @"\d", ""))
                    {
                        case PlayData.SELECTION:
                            if (key == PlayData.SELECTION + index && IsLocal(index) == true && hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int selection))
                            {
                                Select(selection);
                            }
                            break;
                        case MafiaData.ENERGY:
                            if (key == MafiaData.ENERGY + index && hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int energy))
                            {
                                Insert(index, energy);
                            }
                            break;
                        case MafiaData.IDENTITY:
                            if (key == MafiaData.IDENTITY + index && hashtable[key] != null && bool.TryParse(hashtable[key].ToString(), out bool identity))
                            {
                                Insert(index, identity);
                            }
                            break;
                    }
                }
                else
                {
                    switch (key)
                    {
                        case PlayData.ROUND:
                            int survivorCount = 0;
                            int mafiaCount = 0;
                            foreach (KeyValuePair<int, Member> kvp in _members)
                            {
                                Member member = kvp.Value;
                                if (member.alive == true)
                                {
                                    survivorCount++;
                                    if (member.identity == true)
                                    {
                                        mafiaCount++;
                                    }
                                }
                            }
                            Summarizer.Set(_survivorText, MafiaData.TEXT_COUNT_OF_SURVIVORS + ": " + string.Format(PlayData.TEXT_TAG_PEOPLE, survivorCount));
                            Summarizer.Set(_mafiaText, MafiaData.TEXT_COUNT_OF_MAFIAS + ": " + string.Format(PlayData.TEXT_TAG_PEOPLE, mafiaCount));
                            int round = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out round) ? round : 0;
                            if (round > 0)
                            {
                                bool alive = _index != null && _members.ContainsKey(_index.number) == true && _members[_index.number].alive == true;
                                switch ((round - 1) % 3)
                                {
                                    case MafiaData.SECTION_MORNING:
                                        Summarizer.Set(_dayText, string.Format(MafiaData.TEXT_TAG_DAY, (round + 2) / 3) + " (" + MafiaData.TEXT_TAG_MORNING + ")");
                                        Summarizer.Set(_decisionText, MafiaData.TEXT_TAG_DISCUSSION);
                                        _leftBundle.Set(MafiaData.TEXT_TAG_DECREASING, alive);
                                        _rightBundle.Set(MafiaData.TEXT_TAG_INCREASING, alive);
                                        continue;
                                    case MafiaData.SECTION_AFTERNOON:
                                        Summarizer.Set(_dayText, string.Format(MafiaData.TEXT_TAG_DAY, (round + 2) / 3) + " (" + MafiaData.TEXT_TAG_AFTERNOON + ")");
                                        Summarizer.Set(_decisionText, MafiaData.TEXT_TAG_EXECUTION);
                                        _leftBundle.Set(MafiaData.TEXT_TAG_YES, alive);
                                        _rightBundle.Set(MafiaData.TEXT_TAG_NO, alive);
                                        continue;
                                    case MafiaData.SECTION_NIGHT:
                                        Summarizer.Set(_dayText, string.Format(MafiaData.TEXT_TAG_DAY, (round + 2) / 3) + " (" + MafiaData.TEXT_TAG_NIGHT + ")");
                                        Summarizer.Set(_decisionText, MafiaData.TEXT_TAG_CONSPIRACY);
                                        _leftBundle.Set("", false);
                                        _rightBundle.Set("", false);
                                        continue;
                                }
                            }
                            else
                            {
                                Summarizer.Set(_dayText, PlayData.TEXT_TAG_GAMEOVER);
                                Summarizer.Set(_decisionText, "");
                                _leftBundle.Set("", false);
                                _rightBundle.Set("", false);
                            }
                            break;
                        case PlayData.TIMER:
                            if (hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int timer))
                            {
                                if (timer > 0)
                                {
                                    Summarizer.Set(_timerText, timer.ToString());
                                }
                                else
                                {
                                    Summarizer.Set(_timerText, "");
                                    _leftBundle.Set(false);
                                    _rightBundle.Set(false);
                                }
                            }
                            break;
                        case PlayData.MESSAGE:
                            if (hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out int suspect) == true)
                            {
                                _suspect = suspect;
                            }
                            break;
                    }
                }
            }
        }
    }
}