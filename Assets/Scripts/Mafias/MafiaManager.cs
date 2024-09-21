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
/// 이 클래스는 마피아 게임을 담당하는 컴포넌트로 씬 안에서 오직 하나의 객체로만 존재한다.
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
                    if (round == 1 && PhotonNetwork.IsMasterClient == true) //첫 판을 시작할 때 방장은 신분을 배정해주는데 정상적인 플레이가 아니라고 판단될 시 라운드를 0으로 만들어 게임을 종료 시킨다.
                    {
                        int startMoney = hashtable[MafiaData.START_MONEY] != null && int.TryParse(hashtable[MafiaData.START_MONEY].ToString(), out startMoney) == true ? startMoney : 0;
                        int startEnergy = MafiaData.VALUE_START_ENERGY;
                        int survivorCount = 0;
                        int mafiaCount = 0;
                        List<int> citizens = new List<int>();
                        List<int> randoms = new List<int>();
                        Dictionary<object, object> dictionary = hashtable.OrderBy(guid => Guid.NewGuid()).ToDictionary(item => item.Key, item => item.Value);   //리스트 내용을 랜덤으로 섞는다
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
                        if (survivorCount <= mafiaCount * 2 || mafiaCount == 0) //게임 오류
                        {
                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 0 }});
                        }
                        else if(survivorCount > (mafiaCount * 2) + 1) //마피아가 한 명 죽여도 게임 운영에 지장이 없을 인원이라면 일단 바로 밤으로 만들어준다.
                        {
                            _timer = PlayData.VALUE_ONE_SECOND;
                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 3 }, { PlayData.TIMER, MafiaData.COUNT_TIME_NIGHT }});
                        }
                        else //마피아로 인해 바로 죽는 경우가 발생할 수 있으므로 투표하기 전 아침으로 만들어준다.(값을 또 보내주는 이유는 라운드 값을 받으면 따로 알림을 해줘야할 이슈가 있기 때문이다.)
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
                                room.SetCustomProperties(new Hashtable() { {PlayData.MESSAGE, null } });    //내용 초기화
                                int citizenCount = 0;
                                int mafiaCount = 0;
                                int section = (round - 1) % 3;
                                Dictionary<int, int> selections = new Dictionary<int, int>();   //누가 얼마나 득표 했는지를 파악하는 내용
                                Dictionary<int, int> cursings = new Dictionary<int, int>();     //저주하는 내용 명단
                                List<int> conspirators = new List<int>();                       //이번 살인에 가담한 공모자가 누구인지 파악한다.
                                foreach (string key in hashtable.Keys)
                                {
                                    if (int.TryParse(Regex.Replace(key, @"[^0-9]", ""), out int index) == true && key == MafiaData.ENERGY + index)
                                    {
                                        int energy = hashtable[key] != null && int.TryParse(hashtable[key].ToString(), out energy) ? energy : 0;
                                        int selection = hashtable[PlayData.SELECTION + index] != null && int.TryParse(hashtable[PlayData.SELECTION + index].ToString(), out selection) ? selection : 0;
                                        if (energy > 0) //살아 있는자 중에서
                                        {
                                            bool identity = hashtable[MafiaData.IDENTITY + index] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + index].ToString(), out identity) ? identity : false;
                                            if (identity == false)
                                            {
                                                citizenCount++; //시민 수를 센다
                                            }
                                            else
                                            {
                                                mafiaCount++;   //범인 수를 센다.
                                                if (hashtable[MafiaData.EXCLUSIVE + index] != null && int.TryParse(hashtable[MafiaData.EXCLUSIVE + index].ToString(), out int exclusive) && exclusive > 0)
                                                {
                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.EXCLUSIVE + index, exclusive - 1 } });
                                                }
                                            }
                                            switch (section)
                                            {
                                                case MafiaData.SECTION_MORNING:     //아침에서 점심으로 넘어갈 때
                                                    if (energy > 1 && selection > 0) //기력이 있고 누군가를 선택했다면 죽일 후보로 담는다
                                                    {
                                                        int count = 1;          //비리 투표권이 있다면 한 표를 추가하고 해당 플레이어의 비리 투표권 하나를 소모한다.
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
                                                    conspirators.Add(index);        //배당금 받을 인원으로 추가
                                                    break;
                                                case MafiaData.SECTION_AFTERNOON:   //점심에서 저녁으로 넘어갈 때
                                                    if(selection > 0) //이번 사형에 참여한다면 공모자로 등록한다.
                                                    {
                                                        conspirators.Add(index);
                                                        room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, 0 } }); //선택 초기화
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
                                                case MafiaData.SECTION_NIGHT:       //저녁에서 아침으로 넘어갈 때
                                                    if(identity == true)            //범인만 참여
                                                    {
                                                        if(energy > 1)              //기력이 있는 상태라면
                                                        {
                                                            if (selection > 0)  //선택한 누군가를 죽일 후보로 담는다
                                                            {
                                                                if (selections.ContainsKey(selection) == true)
                                                                {
                                                                    selections[selection] += 1;
                                                                }
                                                                else
                                                                {
                                                                    selections.Add(selection, 1);
                                                                }
                                                                room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, 0 } });//아무것도 선택하지 않은 상태로 만든다.
                                                            }
                                                            conspirators.Add(index);    //살인에 성공하면 분배할 인원에 추가한다.
                                                        }
                                                        else                        //기력이 없는 상태라면
                                                        {
                                                            if (selection > 0)//특정 대상을 선택 중에 있었다면 그 선택을 해제하고 자백한다.
                                                            {
                                                                room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + index, 0 }, { PlayData.MESSAGE, hashtable[PlayData.NICKNAME + index] + " " + string.Format(MafiaData.TEXT_CONFESSION, index) } });
                                                            }
                                                            else//그냥 자백한다
                                                            {
                                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, hashtable[PlayData.NICKNAME + index] + " " + string.Format(MafiaData.TEXT_CONFESSION, index) } });
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                        }
                                        else if (/*section == MafiaData.SECTION_NIGHT && */selection > 0)  //죽은 자 중에서 저녁에서 아침으로 넘어갈 때 누군가를 지목했다면
                                        {
                                            if (cursings.ContainsKey(selection) == true)
                                            {
                                                cursings[selection] += 1;
                                            }
                                            else
                                            {
                                                cursings.Add(selection, 1);
                                            }
                                            //if (hashtable[MafiaData.ENERGY + selection] != null && int.TryParse(hashtable[MafiaData.ENERGY + selection].ToString(), out energy) && energy > 0) //저주하고
                                            //{
                                            //    if (energy > MafiaData.VALUE_GHOST_DAMAGE)  //상대의 기력이 충만하다면 그 만큼 체력을 깎아버리고 저주의 문자를 날리고
                                            //    {
                                            //        room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + selection, energy - MafiaData.VALUE_GHOST_DAMAGE }, {PlayData.MESSAGE + selection, string.Format(MafiaData.TEXT_BE_CURSED, index)}});
                                            //    }
                                            //    else //겨우 목숨만 붙어있다면 체력을 1로 만들고 저주의 문자를 날린다.
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
                                    if (hashtable[MafiaData.ENERGY + kvp.Key] != null && int.TryParse(hashtable[MafiaData.ENERGY + kvp.Key].ToString(), out int energy) && energy > 0) //저주하고
                                    {
                                        int damage = MafiaData.VALUE_GHOST_DAMAGE * kvp.Value;
                                        if (energy > damage)  //상대의 기력이 충만하다면 그 만큼 체력을 깎아버리고 저주의 문자를 날리고
                                        {
                                            room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, energy - damage }, { PlayData.MESSAGE + kvp.Key, string.Format(MafiaData.TEXT_BE_CURSED, kvp.Value) } });
                                        }
                                        else //겨우 목숨만 붙어있다면 체력을 1로 만들고 저주의 문자를 날린다.
                                        {
                                            room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, 1 }, { PlayData.MESSAGE + kvp.Key, string.Format(MafiaData.TEXT_BE_CURSED, kvp.Value) } });
                                        }
                                    }
                                }
                                switch (section)
                                {
                                    case MafiaData.SECTION_MORNING:     //아침에서 점심으로 넘어갈 때
                                        if (hashtable[MafiaData.DAILY_MONEY] != null && int.TryParse(hashtable[MafiaData.DAILY_MONEY].ToString(), out int daily) && daily > 0)
                                        {
                                            for(int i = 0; i < conspirators.Count; i++) //일일 급여를 지급한다.
                                            {
                                                int money = hashtable[MafiaData.MONEY + conspirators[i]] != null && int.TryParse(hashtable[MafiaData.MONEY + conspirators[i]].ToString(), out money) ? money : 0;
                                                room.SetCustomProperties(new Hashtable() { { MafiaData.MONEY + conspirators[i], money + daily }, { PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_RECEIVE_DAILY_MONEY, daily) } });
                                            }
                                        }
                                        if (selections.Count > 0) //selections 특출난 후보를 찾고 사형할 후보로 결정한다.
                                        {
                                            foreach (KeyValuePair<int, int> selection in selections) //용의자 투표 결과를 알려준다.
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
                                                        if (selection != kvp.Key) //그 후보를 특정하지 않은 자의 선택값을 0으로 만들어준다.
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
                                            else //후보 뽑기에 실패했다면
                                            {
                                                for (int i = 0; i < conspirators.Count; i++) //선택값을 0으로 만들어준다.
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
                                        else //사형 후보가 없다면 라운드 뻥튀기
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_SENTENCED }, { PlayData.ROUND, round + 2 }, { PlayData.TIMER, MafiaData.COUNT_TIME_NIGHT }});
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_BECOME_NIGHT } });
                                        }
                                        break;
                                    case MafiaData.SECTION_AFTERNOON:   //점심에서 저녁으로 넘어갈 때
                                        if (selections.Count > 0) //selections 특출난 후보를 찾고 사형할 후보로 결정한다.
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
                                                    //범인이 죽으면 공모자들은 배당을 받고(같은 범인 포함) 주민이 죽으면 공모자들은 멘탈이 약해진다.
                                                    bool identity = hashtable[MafiaData.IDENTITY + kvp.Key] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + kvp.Key].ToString(), out identity) ? identity : false;
                                                    if (identity == false)
                                                    {
                                                        room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + kvp.Key, 0 }, { MafiaData.MONEY + kvp.Key, 0 }, { PlayData.MESSAGE, string.Format(MafiaData.TEXT_BE_KILLED, kvp.Key) + "(" + MafiaData.TEXT_TAG_CITIZEN+ ")" } });
                                                        citizenCount--;
                                                        for (int i = 0; i < conspirators.Count; i++) //공모자들의 선택값을 0으로 만들어준다. 자기도 자기 자신에게 투표할 수 있으니 conspirators[i] != kvp.Key로 해줘야함 안 그러면 부활함
                                                        {
                                                            if (conspirators[i] != kvp.Key && hashtable[MafiaData.IDENTITY + conspirators[i]] != null && bool.TryParse(hashtable[MafiaData.IDENTITY + conspirators[i]].ToString(), out identity) && identity == false)
                                                            {
                                                                if (int.Parse(hashtable[MafiaData.ENERGY + conspirators[i]].ToString()) > MafiaData.VALUE_SHOCK_DAMAGE)
                                                                {
                                                                    room.SetCustomProperties(new Hashtable() { { MafiaData.ENERGY + conspirators[i], energy - MafiaData.VALUE_SHOCK_DAMAGE },{PlayData.MESSAGE + conspirators[i], string.Format(MafiaData.TEXT_BE_SHOCKED, MafiaData.VALUE_SHOCK_DAMAGE) } });
                                                                }
                                                                else //에너지를 지속적으로 1로 만든다.
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
                                                            for (int i = conspirators.Count - 1; i >= 0; i--) //자기 자신에게 찬성했다고 귀신에게 아이템을 주는 것 방지
                                                            {
                                                                if (conspirators[i] == kvp.Key)
                                                                {
                                                                    conspirators.RemoveAt(i);
                                                                }
                                                            }
                                                            conspirators = conspirators.OrderBy(guid => Guid.NewGuid()).ToList();   //리스트 내용을 랜덤으로 섞는다
                                                            int count = conspirators.Count;
                                                            int division = Mathf.CeilToInt(money / count);  //죽은자의 소지금에서 양분하는 것이므로 일부에겐 배당되지 않을 수 있다
                                                            for (int i = 0; i < count; i++)
                                                            {
                                                                int principal = hashtable[MafiaData.MONEY + conspirators[i]] != null && int.TryParse(hashtable[MafiaData.MONEY + conspirators[i]].ToString(), out principal) ? principal : 0;//원금
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
                                            else //처형 투표에 실패했다면
                                            {
                                                for (int i = 0; i < conspirators.Count; i++) //공모자들의 선택값을 0으로 만들어준다.
                                                {
                                                    room.SetCustomProperties(new Hashtable() { { PlayData.SELECTION + conspirators[i], 0 } });
                                                }
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_EXECUTED } });  //아무도 처형 되지 않음
                                            }
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_EXECUTED } });  //아무도 처형 되지 않음
                                        }
                                        if (mafiaCount > 0 && mafiaCount * 2 < citizenCount + mafiaCount)
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, round + 1 }, { PlayData.TIMER, MafiaData.COUNT_TIME_NIGHT } ,{ PlayData.MESSAGE, MafiaData.TEXT_BECOME_NIGHT} });// 다음 라운드로 진행
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 0 } });        //게임 종료
                                        }
                                        break;
                                    case MafiaData.SECTION_NIGHT:       //저녁에서 아침으로 넘어갈 때(완성)
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
                                                        conspirators = conspirators.OrderBy(guid => Guid.NewGuid()).ToList();   //리스트 내용을 랜덤으로 섞는다
                                                        int count = conspirators.Count;
                                                        int division = Mathf.CeilToInt(money / count);  //죽은자의 소지금에서 양분하는 것이므로 일부에겐 배당되지 않을 수 있다
                                                        for (int i = 0; i < count; i++)
                                                        {
                                                            int principal = hashtable[MafiaData.MONEY + conspirators[i]] != null && int.TryParse(hashtable[MafiaData.MONEY + conspirators[i]].ToString(), out principal) ? principal : 0;//원금
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
                                                room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_DAMAGED } });  //아무도 피해 받지 않음
                                            }
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.MESSAGE, MafiaData.TEXT_NOBODY_DAMAGED } });  //아무도 피해 받지 않음
                                        }
                                        if (mafiaCount > 0 && mafiaCount * 2 < citizenCount + mafiaCount)
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, round + 1 }, {PlayData.TIMER, MafiaData.COUNT_TIME_MORNING },{ PlayData.MESSAGE, MafiaData.TEXT_BECOME_MORNING} });
                                        }
                                        else
                                        {
                                            room.SetCustomProperties(new Hashtable() { { PlayData.ROUND, 0 } });        //게임 종료
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
            if(hashtable[PlayData.ROUND] != null && int.TryParse(hashtable[PlayData.ROUND].ToString(), out int round)) //목록 내용 중에 라운드 내용이 있다면
            {
                if(0 < round)//라운드가 제대로 동작한다.
                {
                    return false;
                }
                else //그게 아니라면
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