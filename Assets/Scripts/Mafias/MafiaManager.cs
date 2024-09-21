using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
[RequireComponent(typeof(Partition))]
/// <summary>
/// �� Ŭ������ ���Ǿ� ������ ����ϴ� ������Ʈ�� �� �ȿ��� ���� �ϳ��� ��ü�θ� �����Ѵ�.
/// </summary>
public class MafiaManager : MonoBehaviourPunCallbacks
{
    private static MafiaManager instance = null;

    [SerializeField]
    private MafiaTop _mafiaTop = null;
    [SerializeField]
    private MafiaBottom _mafiaBottom = null;
    [SerializeField]
    private Popup _popup = null;
#if UNITY_EDITOR
    [SerializeField]
    private Tester _tester = null;
#endif

    private float _timer = 0;

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
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null)
            {
                Hashtable hashtable = room.CustomProperties;
                int round = hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out round) ? round : 0;
                int startMafia = hashtable[MafiaData.START_MAFIA] != null && int.TryParse(hashtable[MafiaData.START_MAFIA].ToString(), out startMafia) ? startMafia : 0;
                if (round > 0 && startMafia > 0)
                {
                    _mafiaTop?.Initialize(hashtable);
                    _mafiaBottom?.Initialize((text) => { _popup?.Show(text, () => _popup?.Hide()); }, hashtable);
                    _popup?.Initialize();
                    if (round == 1 && PhotonNetwork.IsMasterClient == true) //ù ���� ������ �� ������ �ź��� �������ִµ� �������� �÷��̰� �ƴ϶�� �Ǵܵ� �� ���带 0���� ����� ������ ���� ��Ų��.
                    {
                        int startMoney = hashtable[MafiaData.START_MONEY] != null && int.TryParse(hashtable[MafiaData.START_MONEY].ToString(), out startMoney) == true ? startMoney : 0;
                        int startEnergy = MafiaData.VALUE_START_ENERGY;
                        int survivorCount = 0;
                        int mafiaCount = 0;
                        List<int> citizens = new List<int>();
                        List<int> randoms = new List<int>();
                        Dictionary<object, object> dictionary = hashtable.OrderBy(guid => Guid.NewGuid()).ToDictionary(item => item.Key, item => item.Value);   //����Ʈ ������ �������� ���´�
                        foreach (string key in dictionary.Keys)
                        {
                            if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true && key == PlayData.SELECTION + index)
                            {
                                int selection = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out selection) ? selection : 0;
                                switch (selection)
                                {
                                    case MafiaData.SELECTION_CITIZEN:
                                        citizens.Add(index);
                                        break;
                                    case MafiaData.SELECTION_MAFIA:
                                        if (mafiaCount < startMafia)
                                        {
                                            room.SetCustomProperties(new Hashtable() { { MafiaData.IDENTITY + index, true }, { MafiaData.MONEY + index, startMoney }, { MafiaData.ENERGY + index, startEnergy }, { key, 0 }, {PlayData.MESSAGE + index, MafiaData.TEXT_BECOME_MAFIA } });
                                            mafiaCount++;
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { MafiaData.IDENTITY + index, false }, { MafiaData.MONEY + index, startMoney }, { MafiaData.ENERGY + index, startEnergy }, { key, 0 }, { PlayData.MESSAGE + index, MafiaData.TEXT_BECOME_CITIZEN } });
                                        }
                                        survivorCount++;
                                        break;
                                    default:
                                        randoms.Add(index);
                                        break;
                                }
                            }
                        }
                        for (int i = 0; i < randoms.Count; i++)
                        {
                            int index = randoms[i];
                            if (mafiaCount < startMafia)
                            {
                                room.SetCustomProperties(new Hashtable() { { MafiaData.IDENTITY + index, true }, { MafiaData.MONEY + index, startMoney }, { MafiaData.ENERGY + index, startEnergy }, { PlayData.SELECTION + index, 0 }, { PlayData.MESSAGE + index, MafiaData.TEXT_BECOME_MAFIA } });
                                mafiaCount++;
                            }
                            else
                            {
                                room.SetCustomProperties(new Hashtable() { { MafiaData.IDENTITY + index, false }, { MafiaData.MONEY + index, startMoney }, { MafiaData.ENERGY + index, startEnergy }, { PlayData.SELECTION + index, 0 }, { PlayData.MESSAGE + index, MafiaData.TEXT_BECOME_CITIZEN } });
                            }
                            survivorCount++;
                        }
                        for (int i = 0; i < citizens.Count; i++)
                        {
                            int index = citizens[i];
                            if (mafiaCount < startMafia)
                            {
                                room.SetCustomProperties(new Hashtable() { { MafiaData.IDENTITY + index, true }, { MafiaData.MONEY + index, startMoney }, { MafiaData.ENERGY + index, startEnergy }, { PlayData.SELECTION + index, 0 }, { PlayData.MESSAGE + index, MafiaData.TEXT_BECOME_MAFIA } });
                                mafiaCount++;
                            }
                            else
                            {
                                room.SetCustomProperties(new Hashtable() { { MafiaData.IDENTITY + index, false }, { MafiaData.MONEY + index, startMoney }, { MafiaData.ENERGY + index, startEnergy }, { PlayData.SELECTION + index, 0 }, { PlayData.MESSAGE + index, MafiaData.TEXT_BECOME_CITIZEN } });
                            }
                            survivorCount++;
                        }
                        if (survivorCount <= mafiaCount * 2 || mafiaCount == 0) //���� ����
                        {
                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 0 }});
                        }
                        else if(survivorCount > (mafiaCount * 2) + 1) //���Ǿư� �� �� �׿��� ���� ��� ������ ���� �ο��̶�� �ϴ� �ٷ� ������ ������ش�.
                        {
                            _timer = PlayData.VALUE_ONE_SECOND;
                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 3 }, { PlayData.TIMER, MafiaData.COUNT_TIME_NIGHT }});
                        }
                        else //���ǾƷ� ���� �ٷ� �״� ��찡 �߻��� �� �����Ƿ� ��ǥ�ϱ� �� ��ħ���� ������ش�.(���� �� �����ִ� ������ ���� ���� ������ ���� �˸��� ������� �̽��� �ֱ� �����̴�.)
                        {
                            _timer = PlayData.VALUE_ONE_SECOND;
                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 1 }, { PlayData.TIMER, MafiaData.COUNT_TIME_MORNING }});
                        }
                    }
                    return;
                }
                PhotonNetwork.LeaveRoom();
                _popup?.Show(PlayData.TEXT_ALREADY_FINISH, () => LoadScene(PlayData.SCENE_LOBBY), true);
                return;
            }
            _popup?.Show(PlayData.TEXT_JOIN_ROOM_FAILED, () => LoadScene(PlayData.SCENE_LOBBY), true);
        }
    }

    private void LateUpdate()
    {
        if(_timer > 0)
        {
            float deltaTime = Time.deltaTime;
            _timer -= deltaTime;
            if (_timer <= 0)
            {
                _timer += PlayData.VALUE_ONE_SECOND;
                if (PhotonNetwork.IsMasterClient == true)
                {
                    Room room = PhotonNetwork.CurrentRoom;
                    if (room != null)
                    {
                        Hashtable hashtable = room.CustomProperties;
                        if (hashtable != null && hashtable[PlayData.TIMER] != null && int.TryParse(hashtable[PlayData.TIMER].ToString(), out int timer))
                        {
                            if(timer > 0)
                            {
                                room.SetCustomProperties(new Hashtable() { {PlayData.TIMER, timer - 1 } });
                            }
                            else if (hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round) && round > 0)
                            {
                                room.SetCustomProperties(new Hashtable() { {PlayData.MESSAGE, null } });    //���� �ʱ�ȭ
                                int citizenCount = 0;
                                int mafiaCount = 0;
                                int section = (round - 1) % 3;
                                Dictionary<int, int> selections = new Dictionary<int, int>();   //���� �󸶳� ��ǥ �ߴ����� �ľ��ϴ� ����
                                Dictionary<int, int> cursings = new Dictionary<int, int>();     //�����ϴ� ���� ���
                                List<int> conspirators = new List<int>();                       //�̹� ���ο� ������ �����ڰ� �������� �ľ��Ѵ�.
                                foreach (string key in hashtable.Keys)
                                {
                                    if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true && key == MafiaData.ENERGY + index)
                                    {
                                        int energy = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out energy) ? energy : 0;
                                        int selection = hashtable[PlayData.SELECTION + index] != null && int.TryParse(hashtable[PlayData.SELECTION + index].ToString(), out selection) ? selection : 0;
                                        if (energy > 0) //��� �ִ��� �߿���
                                        {
                                            bool identity = hashtable[MafiaData.IDENTITY + index] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + index].ToString(), out identity) ? identity : false;
                                            if (identity == false)
                                            {
                                                citizenCount++; //�ù� ���� ����
                                            }
                                            else
                                            {
                                                mafiaCount++;   //���� ���� ����.
                                                if (hashtable[MafiaData.EXCLUSIVE + index] != null && int.TryParse(hashtable[MafiaData.EXCLUSIVE + index].ToString(), out int exclusive) && exclusive > 0)
                                                {
                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.EXCLUSIVE + index, exclusive - 1 } });
                                                }
                                            }
                                            switch (section)
                                            {
                                                case MafiaData.SECTION_MORNING:     //��ħ���� �������� �Ѿ ��
                                                    if (energy > 1 && selection > 0) //����� �ְ� �������� �����ߴٸ� ���� �ĺ��� ��´�
                                                    {
                                                        int count = 1;          //�� ��ǥ���� �ִٸ� �� ǥ�� �߰��ϰ� �ش� �÷��̾��� �� ��ǥ�� �ϳ��� �Ҹ��Ѵ�.
                                                        if (hashtable[MafiaData.OPTION + index] != null && int.TryParse(hashtable[MafiaData.OPTION + index].ToString(), out int option) && option == MafiaData.SELECTION_SUFFRAGE && hashtable[MafiaData.SUFFRAGE + index] != null && int.TryParse(hashtable[MafiaData.SUFFRAGE + index].ToString(), out int suffrage) && suffrage > 0)
                                                        {
                                                            room.SetCustomProperties(new Hashtable() { { MafiaData.SUFFRAGE + index, suffrage - 1 }, { MafiaData.OPTION + index, MafiaData.SELECTION_NONE }, { PlayData.MESSAGE, string.Format(MafiaData.TEXT_USE_SUFFRAGE, index) } });
                                                            count += 1;
                                                        }
                                                        if (selections.ContainsKey(selection) == true)
                                                        {
                                                            selections[selection] += count;
                                                        }
                                                        else
                                                        {
                                                            selections.Add(selection, count);
                                                        }
                                                    }
                                                    conspirators.Add(index);        //���� ���� �ο����� �߰�
                                                    break;
                                                case MafiaData.SECTION_AFTERNOON:   //���ɿ��� �������� �Ѿ ��
                                                    if(selection > 0) //�̹� ������ �����Ѵٸ� �����ڷ� ����Ѵ�.
                                                    {
                                                        conspirators.Add(index);
                                                        room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, 0 } }); //���� �ʱ�ȭ
                                                    }
                                                    if (selections.ContainsKey(selection) == true)
                                                    {
                                                        selections[selection] += 1;
                                                    }
                                                    else
                                                    {
                                                        selections.Add(selection, 1);
                                                    }
                                                    break;
                                                case MafiaData.SECTION_NIGHT:       //���ῡ�� ��ħ���� �Ѿ ��
                                                    if(identity == true)            //���θ� ����
                                                    {
                                                        if(energy > 1)              //����� �ִ� ���¶��
                                                        {
                                                            if (selection > 0)  //������ �������� ���� �ĺ��� ��´�
                                                            {
                                                                if (selections.ContainsKey(selection) == true)
                                                                {
                                                                    selections[selection] += 1;
                                                                }
                                                                else
                                                                {
                                                                    selections.Add(selection, 1);
                                                                }
                                                                room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, 0 } });//�ƹ��͵� �������� ���� ���·� �����.
                                                            }
                                                            conspirators.Add(index);    //���ο� �����ϸ� �й��� �ο��� �߰��Ѵ�.
                                                        }
                                                        else                        //����� ���� ���¶��
                                                        {
                                                            if (selection > 0)//Ư�� ����� ���� �߿� �־��ٸ� �� ������ �����ϰ� �ڹ��Ѵ�.
                                                            {
                                                                room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, 0 }, { PlayData.MESSAGE, hashtable[PlayData.NICKNAME + index] + " " + string.Format(MafiaData.TEXT_CONFESSION, index) } });
                                                            }
                                                            else//�׳� �ڹ��Ѵ�
                                                            {
                                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, hashtable[PlayData.NICKNAME + index] + " " + string.Format(MafiaData.TEXT_CONFESSION, index) } });
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                        else if (/*section == MafiaData.SECTION_NIGHT && */selection > 0)  //���� �� �߿��� ���ῡ�� ��ħ���� �Ѿ �� �������� �����ߴٸ�
                                        {
                                            if (cursings.ContainsKey(selection) == true)
                                            {
                                                cursings[selection] += 1;
                                            }
                                            else
                                            {
                                                cursings.Add(selection, 1);
                                            }
                                            //if (hashtable[MafiaData.ENERGY + selection] != null && int.TryParse(hashtable[MafiaData.ENERGY + selection].ToString(), out energy) && energy > 0) //�����ϰ�
                                            //{
                                            //    if (energy > MafiaData.VALUE_GHOST_DAMAGE)  //����� ����� �游�ϴٸ� �� ��ŭ ü���� ��ƹ����� ������ ���ڸ� ������
                                            //    {
                                            //        room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + selection, energy - MafiaData.VALUE_GHOST_DAMAGE }, {PlayData.MESSAGE + selection, string.Format(MafiaData.TEXT_BE_CURSED, index)}});
                                            //    }
                                            //    else //�ܿ� ����� �پ��ִٸ� ü���� 1�� ����� ������ ���ڸ� ������.
                                            //    {
                                            //        room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + selection, 1 }, { PlayData.MESSAGE + selection, string.Format(MafiaData.TEXT_BE_CURSED, index) } });
                                            //    }
                                            //}
                                            room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, 0 }, { PlayData.MESSAGE + index, string.Format(MafiaData.TEXT_DO_CURSING, selection) } });
                                        }
                                    }
                                }
                                foreach(KeyValuePair<int, int> kvp in cursings)
                                {
                                    if (hashtable[MafiaData.ENERGY + kvp.Key] != null && int.TryParse(hashtable[MafiaData.ENERGY + kvp.Key].ToString(), out int energy) && energy > 0) //�����ϰ�
                                    {
                                        int damage = MafiaData.VALUE_GHOST_DAMAGE * kvp.Value;
                                        if (energy > damage)  //����� ����� �游�ϴٸ� �� ��ŭ ü���� ��ƹ����� ������ ���ڸ� ������
                                        {
                                            room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, energy - damage }, { PlayData.MESSAGE + kvp.Key, string.Format(MafiaData.TEXT_BE_CURSED, kvp.Value) } });
                                        }
                                        else //�ܿ� ����� �پ��ִٸ� ü���� 1�� ����� ������ ���ڸ� ������.
                                        {
                                            room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, 1 }, { PlayData.MESSAGE + kvp.Key, string.Format(MafiaData.TEXT_BE_CURSED, kvp.Value) } });
                                        }
                                    }
                                }
                                switch (section)
                                {
                                    case MafiaData.SECTION_MORNING:     //��ħ���� �������� �Ѿ ��
                                        if (hashtable[MafiaData.DAILY_MONEY] != null && int.TryParse(hashtable[MafiaData.DAILY_MONEY].ToString(), out int daily) && daily > 0)
                                        {
                                            for(int i = 0; i < conspirators.Count; i++) //���� �޿��� �����Ѵ�.
                                            {
                                                int money = hashtable[MafiaData.MONEY + conspirators[i]] != null && int.TryParse(hashtable[MafiaData.MONEY + conspirators[i]].ToString(), out money) ? money : 0;
                                                room.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + conspirators[i], money + daily }, { PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_RECEIVE_DAILY_MONEY, daily) } });
                                            }
                                        }
                                        if (selections.Count > 0) //selections Ư�Ⳮ �ĺ��� ã�� ������ �ĺ��� �����Ѵ�.
                                        {
                                            foreach (KeyValuePair<int, int> selection in selections) //������ ��ǥ ����� �˷��ش�.
                                            {
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, string.Format(PlayData.TEXT_TAG_INDEX, selection.Key) + " " + hashtable[PlayData.NICKNAME + selection.Key] + ":" + string.Format(MafiaData.TEXT_TAG_VOTE, selection.Value) } });
                                            }
                                            KeyValuePair<int, int> kvp = selections.Aggregate((x, y) => x.Value > y.Value ? x : y);
                                            if (selections.Count(x => (x.Value == kvp.Value)) == 1 && hashtable[MafiaData.ENERGY + kvp.Key] != null && int.TryParse(hashtable[MafiaData.ENERGY + kvp.Key].ToString(), out int energy) && energy > 0)
                                            {
                                                List<int> list = new List<int>();
                                                for (int i = 0; i < conspirators.Count; i++) //
                                                {
                                                    int selection = hashtable[PlayData.SELECTION + conspirators[i]] != null && int.TryParse(hashtable[PlayData.SELECTION + conspirators[i]].ToString(), out selection) ? selection : 0;
                                                    if (selection > 0)
                                                    {
                                                        if (selection != kvp.Key) //�� �ĺ��� Ư������ ���� ���� ���ð��� 0���� ������ش�.
                                                        {
                                                            room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + conspirators[i], 0 } });
                                                        }
                                                        else
                                                        {
                                                            list.Add(conspirators[i]);
                                                        }
                                                    }
                                                }
                                                room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, round + 1 }, { PlayData.TIMER, MafiaData.COUNT_TIME_AFTERNOON }, {PlayData.MESSAGE, kvp.Key } });
                                                room.SetCustomProperties(new Hashtable() { {PlayData.MESSAGE, MafiaData.TEXT_BECOME_AFTERNOON} });
                                            }
                                            else //�ĺ� �̱⿡ �����ߴٸ�
                                            {
                                                for (int i = 0; i < conspirators.Count; i++) //���ð��� 0���� ������ش�.
                                                {
                                                    int selection = hashtable[PlayData.SELECTION + conspirators[i]] != null && int.TryParse(hashtable[PlayData.SELECTION + conspirators[i]].ToString(), out selection) ? selection : 0;
                                                    if (selection > 0)
                                                    {
                                                        room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + conspirators[i], 0 } });
                                                    }
                                                }
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_SENTENCED }, { PlayData.ROUND, round + 2 }, { PlayData.TIMER, MafiaData.COUNT_TIME_NIGHT } });
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_BECOME_NIGHT } });
                                            }
                                        }
                                        else //���� �ĺ��� ���ٸ� ���� ��Ƣ��
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_SENTENCED }, { PlayData.ROUND, round + 2 }, { PlayData.TIMER, MafiaData.COUNT_TIME_NIGHT }});
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_BECOME_NIGHT } });
                                        }
                                        break;
                                    case MafiaData.SECTION_AFTERNOON:   //���ɿ��� �������� �Ѿ ��
                                        if (selections.Count > 0) //selections Ư�Ⳮ �ĺ��� ã�� ������ �ĺ��� �����Ѵ�.
                                        {
                                            KeyValuePair<int, int> kvp = selections.Aggregate((x, y) => x.Value > y.Value ? x : y);
                                            if (kvp.Key > 0 && selections.Count(x => (x.Value == kvp.Value)) == 1 && hashtable[MafiaData.ENERGY + kvp.Key] != null && int.TryParse(hashtable[MafiaData.ENERGY + kvp.Key].ToString(), out int energy) && energy > 0)
                                            {
                                                if (hashtable[MafiaData.AMULET + kvp.Key] != null && int.TryParse(hashtable[MafiaData.AMULET + kvp.Key].ToString(), out int amulet) && amulet > 0)
                                                {
                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.AMULET + kvp.Key, amulet - 1 }, { PlayData.MESSAGE, string.Format(MafiaData.TEXT_PRESERVE_LIFE, kvp.Key) } });
                                                }
                                                else
                                                {
                                                    //������ ������ �����ڵ��� ����� �ް�(���� ���� ����) �ֹ��� ������ �����ڵ��� ��Ż�� ��������.
                                                    bool identity = hashtable[MafiaData.IDENTITY + kvp.Key] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + kvp.Key].ToString(), out identity) ? identity : false;
                                                    if (identity == false)
                                                    {
                                                        room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, 0 }, { MafiaData.MONEY + kvp.Key, 0 }, { PlayData.MESSAGE, string.Format(MafiaData.TEXT_BE_KILLED, kvp.Key) + "(" + MafiaData.TEXT_TAG_CITIZEN+ ")" } });
                                                        citizenCount--;
                                                        for (int i = 0; i < conspirators.Count; i++) //�����ڵ��� ���ð��� 0���� ������ش�. �ڱ⵵ �ڱ� �ڽſ��� ��ǥ�� �� ������ conspirators[i] != kvp.Key�� ������� �� �׷��� ��Ȱ��
                                                        {
                                                            if (conspirators[i] != kvp.Key && hashtable[MafiaData.IDENTITY + conspirators[i]] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + conspirators[i]].ToString(), out identity) && identity == false)
                                                            {
                                                                if (int.Parse(hashtable[MafiaData.ENERGY + conspirators[i]].ToString()) > MafiaData.VALUE_SHOCK_DAMAGE)
                                                                {
                                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + conspirators[i], energy - MafiaData.VALUE_SHOCK_DAMAGE },{PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_BE_SHOCKED, MafiaData.VALUE_SHOCK_DAMAGE) } });
                                                                }
                                                                else //�������� ���������� 1�� �����.
                                                                {
                                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + conspirators[i], 1 }, { PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_BE_SHOCKED, MafiaData.VALUE_SHOCK_DAMAGE) } });
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        mafiaCount--;
                                                        int money = hashtable[MafiaData.MONEY + kvp.Key] != null && int.TryParse(hashtable[MafiaData.MONEY + kvp.Key].ToString(), out money) ? money : 0;
                                                        room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, 0 }, { MafiaData.MONEY + kvp.Key, 0 }, { PlayData.MESSAGE, string.Format(MafiaData.TEXT_BE_KILLED, kvp.Key) + "(" + MafiaData.TEXT_TAG_MAFIA + ")" } });
                                                        if (money > 0)
                                                        {
                                                            for (int i = conspirators.Count - 1; i >= 0; i--) //�ڱ� �ڽſ��� �����ߴٰ� �ͽſ��� �������� �ִ� �� ����
                                                            {
                                                                if (conspirators[i] == kvp.Key)
                                                                {
                                                                    conspirators.RemoveAt(i);
                                                                }
                                                            }
                                                            conspirators = conspirators.OrderBy(guid => Guid.NewGuid()).ToList();   //����Ʈ ������ �������� ���´�
                                                            int count = conspirators.Count;
                                                            int division = Mathf.CeilToInt(money / count);  //�������� �����ݿ��� ����ϴ� ���̹Ƿ� �Ϻο��� ������ ���� �� �ִ�
                                                            for (int i = 0; i < count; i++)
                                                            {
                                                                int principal = hashtable[MafiaData.MONEY + conspirators[i]] != null && int.TryParse(hashtable[MafiaData.MONEY + conspirators[i]].ToString(), out principal) ? principal : 0;//����
                                                                if (division <= money)
                                                                {
                                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + conspirators[i], division + principal}, { PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_RECEIVE_DIVIDEND, division) } });
                                                                    money -= division;
                                                                }
                                                                else if(money > 0)
                                                                {
                                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + conspirators[i], money + principal }, { PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_RECEIVE_DIVIDEND, money) } });
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else //ó�� ��ǥ�� �����ߴٸ�
                                            {
                                                for (int i = 0; i < conspirators.Count; i++) //�����ڵ��� ���ð��� 0���� ������ش�.
                                                {
                                                    room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + conspirators[i], 0 } });
                                                }
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_EXECUTED } });  //�ƹ��� ó�� ���� ����
                                            }
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_EXECUTED } });  //�ƹ��� ó�� ���� ����
                                        }
                                        if (mafiaCount > 0 && mafiaCount * 2 < citizenCount + mafiaCount)
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, round + 1 }, { PlayData.TIMER, MafiaData.COUNT_TIME_NIGHT } ,{ PlayData.MESSAGE, MafiaData.TEXT_BECOME_NIGHT} });// ���� ����� ����
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 0 } });        //���� ����
                                        }
                                        break;
                                    case MafiaData.SECTION_NIGHT:       //���ῡ�� ��ħ���� �Ѿ ��(�ϼ�)
                                        if (selections.Count > 0)
                                        {
                                            KeyValuePair<int, int> kvp = selections.Aggregate((x, y) => x.Value > y.Value ? x : y);
                                            if (selections.Count(x => (x.Value == kvp.Value)) == 1 && hashtable[MafiaData.ENERGY + kvp.Key] != null && int.TryParse(hashtable[MafiaData.ENERGY + kvp.Key].ToString(), out int energy) && energy > 0)
                                            {
                                                if(hashtable[MafiaData.AMULET + kvp.Key] != null && int.TryParse(hashtable[MafiaData.AMULET + kvp.Key].ToString(), out int amulet) && amulet > 0)
                                                {
                                                    room.SetCustomProperties(new Hashtable() { {MafiaData.AMULET + kvp.Key, amulet - 1},{PlayData.MESSAGE, string.Format(MafiaData.TEXT_PRESERVE_LIFE, kvp.Key)} });
                                                }
                                                else
                                                {
                                                    bool identity = hashtable[MafiaData.IDENTITY + kvp.Key] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + kvp.Key].ToString(), out identity) ? identity : false;
                                                    int money = hashtable[MafiaData.MONEY + kvp.Key] != null && int.TryParse(hashtable[MafiaData.MONEY + kvp.Key].ToString(), out money) ? money : 0;
                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, 0 }, { MafiaData.MONEY + kvp.Key, 0 }, { PlayData.MESSAGE, string.Format(MafiaData.TEXT_BE_KILLED, kvp.Key) } });
                                                    if (money > 0)
                                                    {
                                                        conspirators = conspirators.OrderBy(guid => Guid.NewGuid()).ToList();   //����Ʈ ������ �������� ���´�
                                                        int count = conspirators.Count;
                                                        int division = Mathf.CeilToInt(money / count);  //�������� �����ݿ��� ����ϴ� ���̹Ƿ� �Ϻο��� ������ ���� �� �ִ�
                                                        for (int i = 0; i < count; i++)
                                                        {
                                                            int principal = hashtable[MafiaData.MONEY + conspirators[i]] != null && int.TryParse(hashtable[MafiaData.MONEY + conspirators[i]].ToString(), out principal) ? principal : 0;//����
                                                            if (division <= money)
                                                            {
                                                                room.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + conspirators[i], division + principal }, { PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_RECEIVE_DIVIDEND, division) } });
                                                                money -= division;
                                                            }
                                                            else if (money > 0)
                                                            {
                                                                room.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + conspirators[i], money + principal }, { PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_RECEIVE_DIVIDEND, money) } });
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (identity == false)
                                                    {
                                                        citizenCount--;
                                                    }
                                                    else
                                                    {
                                                        mafiaCount--;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_DAMAGED } });  //�ƹ��� ���� ���� ����
                                            }
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_DAMAGED } });  //�ƹ��� ���� ���� ����
                                        }
                                        if (mafiaCount > 0 && mafiaCount * 2 < citizenCount + mafiaCount)
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, round + 1 }, {PlayData.TIMER, MafiaData.COUNT_TIME_MORNING },{ PlayData.MESSAGE, MafiaData.TEXT_BECOME_MORNING} });
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 0 } });        //���� ����
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
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
            if (IsGameOver(hashtable, false) == true)
            {
                return;
            }
            _mafiaTop?.OnRoomPropertiesUpdate(hashtable);
            _mafiaBottom?.OnRoomPropertiesUpdate(hashtable);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _popup?.Show(PlayData.TEXT_DISCONNECT, () => LoadScene(PlayData.SCENE_ENTRY), true);
    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
#if UNITY_EDITOR
        Room room = PhotonNetwork.CurrentRoom;
        _tester?.Set(room.CustomProperties);
#endif
        if (IsGameOver(hashtable, true) == true)
        {
            return;
        }
        else if (PhotonNetwork.IsMasterClient == false && hashtable != null && hashtable[PlayData.TIMER] != null)
        {
            _timer = PlayData.VALUE_ONE_SECOND;
        }
        _mafiaTop?.OnRoomPropertiesUpdate(hashtable);
        _mafiaBottom?.OnRoomPropertiesUpdate(hashtable);
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
            PhotonNetwork.LeaveRoom();
            LoadScene(PlayData.SCENE_LOBBY);
        };
        Action noAction = () =>
        {
            _popup?.Hide();
        };
        _popup?.Show(PlayData.TEXT_TRY_LEAVE_ROOM, yesAction, noAction);
    }

    private bool IsGameOver(Hashtable hashtable, bool recently)
    {
        if(hashtable != null)
        {
            if(hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round)) //��� ���� �߿� ���� ������ �ִٸ�
            {
                if(0 < round)//���尡 ����� �����Ѵ�.
                {
                    return false;
                }
                else //�װ� �ƴ϶��
                {
                    if(recently == true)
                    {
                        Room room = PhotonNetwork.CurrentRoom;
                        if(room != null)
                        {
                            hashtable = room.CustomProperties;
                            if(hashtable == null)
                            {
                                _timer = 0;
                                return true;
                            }
                        }
                        else
                        {
                            _timer = 0;
                            return true;
                        }
                    }
                    int mafiaCount = 0;
                    int citizenCount = 0;
                    StringBuilder mafiaString = new StringBuilder();
                    StringBuilder citizenString = new StringBuilder();
                    foreach (string key in hashtable.Keys)
                    {
                        if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true && key == MafiaData.ENERGY + index)
                        {
                            bool identity = hashtable[MafiaData.IDENTITY + index] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + index].ToString(), out identity) ? identity : false;
                            int energy = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out energy) ? energy : 0;
                            if (identity == false)
                            {
                                citizenString.Append(hashtable[PlayData.NICKNAME + index] + " (" + MafiaData.TEXT_TAG_CITIZEN + ")");
                                if (energy > 0)
                                {
                                    citizenCount++;
                                    citizenString.Append("(" + MafiaData.TEXT_TAG_ALIVE + ")\n");
                                }
                                else
                                {
                                    citizenString.Append("(" + MafiaData.TEXT_TAG_DEATH + ")\n");
                                }
                            }
                            else
                            {
                                mafiaString.Append(hashtable[PlayData.NICKNAME + index] + " (" + MafiaData.TEXT_TAG_MAFIA + ")");
                                if (energy > 0)
                                {
                                    mafiaCount++;
                                    mafiaString.Append("(" + MafiaData.TEXT_TAG_ALIVE + ")\n");
                                }
                                else
                                {
                                    mafiaString.Append("(" + MafiaData.TEXT_TAG_DEATH + ")\n");
                                }
                            }
                        }
                    }
                    PhotonNetwork.LeaveRoom();
                    if (citizenCount > 0 && mafiaCount == 0)
                    {
                        _popup?.Show(MafiaData.TEXT_TAG_CITIZEN + " " + MafiaData.TEXT_TAG_VICTORY + "\n" + citizenString.ToString() + "\n" + mafiaString.ToString(), () => LoadScene(PlayData.SCENE_LOBBY), true);
                    }
                    else if (citizenCount <= mafiaCount)
                    {
                        _popup?.Show(MafiaData.TEXT_TAG_MAFIA + " "+ MafiaData.TEXT_TAG_VICTORY + "\n" + mafiaString.ToString() + "\n" + citizenString.ToString(), () => LoadScene(PlayData.SCENE_LOBBY), true);
                    }
                    else
                    {
                        _popup?.Show(PlayData.TEXT_ALREADY_FINISH, () => LoadScene(PlayData.SCENE_LOBBY), true);
                    }
                }
            }
            else if(recently == true)
            {
                return false;
            }
        }
        _timer = 0;
        return true;
    }
}