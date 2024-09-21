using UnityEngine;

/// <summary>
/// 마피아 게임 테마에 필요한 데이터 양식 및 텍스트 표시 내용을 담은 클래스
/// </summary>
public static class MafiaData
{
    //공용
    public const string SCENE = "Mafia";                        //씬 이름 문자열
    public const string START_MAFIA = "start_mafia";            //범인 수
    public const string START_MONEY = "start_money";            //시작 자금
    public const string DAILY_MONEY = "daily_money";            //일일 금액
    public const string COST_SUFFRAGE = "cost_suffrage";        //투표권 가격
    public const string COST_NUTRITION = "cost_nutrition";      //영양제 가격
    public const string COST_AMULET = "cost_amulet";            //호신부 가격
    public const string COST_TELESCOPE = "cost_telescope";      //망원경 가격
    public const string COST_FABRICATION = "cost_fabrication";  //자작극 가격

    //개인용
    public const string IDENTITY = "identity";                  //범인 여부
    public const string ENERGY = "energy";                      //에너지
    public const string MONEY = "money";                        //돈
    public const string SUFFRAGE = "suffrage";                  //투표권
    public const string NUTRITION = "nutrition";                //영양제
    public const string AMULET = "amulet";                      //호신부
    public const string EXCLUSIVE = "exclusive";                //전용 아이템 (시민: 망원경, 범인: 자작극)
    public const string OPTION = "option";

    //표시 텍스트
    public static string TEXT_TITLE = "마피아";
    public static string TEXT_START_MAFIA = "범인 수";
    public static string TEXT_START_MONEY = "시작 자금";
    public static string TEXT_DAILY_MONEY = "일일 급여";
    public static string TEXT_APPLY_RANDOM = "랜덤 희망";
    public static string TEXT_APPLY_CITIZEN= "시민 희망";
    public static string TEXT_APPLY_MAFIA = "범인 희망";
    public static string TEXT_INFO_START_MAFIA = "게임에서 선정될 범인들의\n총 인원을 설정합니다.";
    public static string TEXT_INFO_START_MONEY = "이 게임의 시작과 동시에\n지급되는 소지금입니다.";
    public static string TEXT_INFO_DAILY_MONEY = "이 게임이 진행되는 동안\n아침에서 점심이 될 때\n규칙적으로 지급되는 금액입니다.";
    public static string TEXT_INFO_COST_SUFFRAGE = "자신이 투표한 한 명에게\n추가표 1개를 더 줍니다.\n시민 범인 공용 아이템";
    public static string TEXT_INFO_COST_NUTRITION = "이 아이템을 소모하여\n자신의 기력을 +1 회복 합니다.\n시민 범인 공용 아이템";
    public static string TEXT_INFO_COST_AMULET = "죽음의 위기에 처했을 때\n이것을 소지하면 죽지 않습니다.\n시민 범인 공용 아이템";
    public static string TEXT_INFO_COST_TELESCOPE = "이 아이템을 사용하여 특정인의\n신원 확인을 할 수 있습니다.\n시민 전용 아이템";
    public static string TEXT_INFO_COST_FABRICATION = "이 아이템을 사용한 하루 동안\n시민을 사칭 할 수 있습니다.\n범인 전용 아이템";
    public static string TEXT_INFO_ENERGY = "기력이 1이 되면\n투표 참여를 할 수 없게 되고\n범인이라면 아침 시간에\n스스로 자백을 하게 되니\n영양제로 관리를 잘 해줍시다.";
    public static string TEXT_UNNECESSARY_ITEM = "조사 아이템이 없다면\n사칭 아이템도 없어야 합니다.";
    public static string TEXT_MINIMUM_PLAYERS = "시작 인원은 최소 {0}명 입니다.";
    public static string TEXT_MAXIMUM_PLAYERS = "범인의 최대 인원은 {0}명 입니다.";

    public static string TEXT_TAG_ENERGY = "기력";
    public static string TEXT_TAG_PRICE = "가격";
    public static string TEXT_TAG_MAFIA = "범인";
    public static string TEXT_TAG_CITIZEN = "시민";
    public static string TEXT_TAG_DAY = "{0}일";
    public static string TEXT_TAG_MONEY = "{0}원";
    public static string TEXT_TAG_VOTE = "{0}표";
    public static string TEXT_TAG_TIME = "{0}번";
    public static string TEXT_TAG_MORNING = "아침";
    public static string TEXT_TAG_AFTERNOON = "점심";
    public static string TEXT_TAG_NIGHT = "저녁";
    public static string TEXT_TAG_ALIVE = "생존";
    public static string TEXT_TAG_DEATH = "사망";
    public static string TEXT_TAG_DISCUSSION = "투표 시간";
    public static string TEXT_TAG_EXECUTION = "처형 투표";
    public static string TEXT_TAG_CONSPIRACY = "살인 공모";
    public static string TEXT_TAG_INCREASING = "시간 연장";
    public static string TEXT_TAG_DECREASING = "시간 단축";
    public static string TEXT_TAG_YES = "찬성";
    public static string TEXT_TAG_NO = "반대";
    public static string TEXT_TAG_USE = "사용";
    public static string TEXT_TAG_BUY = "구매";
    public static string TEXT_TAG_OWN_ITEM = "보유 아이템";
    public static string TEXT_TAG_SHOP_ITEM = "상점 아이템";
    public static string TEXT_TAG_FUNDAGE = "소지금";
    public static string TEXT_TAG_AMULET = "호신부";
    public static string TEXT_TAG_SUFFRAGE = "투표권";
    public static string TEXT_TAG_NUTRITION = "영양제";
    public static string TEXT_TAG_TELESCOPE = "조사";
    public static string TEXT_TAG_FABRICATION = "사칭";
    public static string TEXT_TAG_ITEM_REFRESH = "품목 갱신";
    public static string TEXT_COUNT_OF_SURVIVORS = "남은 생존자";
    public static string TEXT_COUNT_OF_MAFIAS = "남은 범인";

    public static string TEXT_BECOME_CITIZEN = "당신은 시민이 되었습니다.";
    public static string TEXT_BECOME_MAFIA = "당신은 범인이 되었습니다.";
    public static string TEXT_BECOME_MORNING = "아침이 되었습니다.";
    public static string TEXT_BECOME_AFTERNOON = "점심이 되었습니다.";
    public static string TEXT_BECOME_NIGHT = "저녁이 되었습니다.";

    public static string TEXT_INCREASE_TIME = "{0}번이 시간 연장을 하였습니다.";
    public static string TEXT_DECREASE_TIME = "{0}번이 시간 단축을 하였습니다.";
    public static string TEXT_SELECT_SUSPECT_PERSON = "{0}번이 {1}번을 용의자로 지목했습니다.";
    public static string TEXT_SELECT_VICTIM_PERSON = "{0}번이 {1}번을 희생자로 지목했습니다.";
    public static string TEXT_USE_SUFFRAGE = "{0}번이 추가 투표권을 이용했습니다.";
    public static string TEXT_RECEIVE_DAILY_MONEY = "일급 {0}원을 받았습니다.";
    public static string TEXT_RECEIVE_DIVIDEND = "배당금 {0}원을 받았습니다.";
    public static string TEXT_CONFESSION = "{0}번이 죄를 자백합니다.";

    public static string TEXT_USE_PLAN_SUFFRAGE = "투표 시간에 선택한 대상에게 추가 표를 줄 수 있는 상태입니다.";
    public static string TEXT_INVESTIGATE_TARGET = "위에 있는 대상을 선택하면 신분을 조사합니다.";
    public static string TEXT_PRETEND_TO_CITIZEN = "지금부터 시민인척 위장합니다.";
    public static string TEXT_FINISH_PRETEND = "시민으로 사칭하는 시간이 끝이 났습니다.";
    public static string TEXT_ALREADY_YES = "이미 찬성 상태입니다.";
    public static string TEXT_ALREADY_NO = "이미 반대 상태입니다.";

    public static string TEXT_LACK_OF_MONEY = "돈이 부족합니다.";
    public static string TEXT_LACK_OF_ENERGY = "기력 부족으로 참여 할 수 없습니다.";
    public static string TEXT_FULL_ENERGY = "이미 최대 체력입니다.";
    public static string TEXT_RECOVER_ENERGY = "기력을 회복 하였습니다.";
    public static string TEXT_PREPARE_CURSING = "{0}번을 저주 합니다.";
    public static string TEXT_DO_CURSING = "{0}번을 저주 하였습니다.";
    public static string TEXT_BE_SUSPECTED = "{0}번이 용의자로 선택되었습니다.";
    public static string TEXT_BE_SHOCKED = "당신은 충격을 받아 기력이 {0} 차감됩니다.";
    public static string TEXT_BE_CURSED = "죽은자 {0}명이 당신을 저주합니다.";
    public static string TEXT_BE_KILLED = "{0}번이 죽임을 당했습니다.";
    public static string TEXT_PRESERVE_LIFE = "{0}번은 호신부로 인하여 목숨을 보전하였습니다.";
    public static string TEXT_NOBODY_SENTENCED = "아무도 선택되지 않았습니다.";
    public static string TEXT_NOBODY_EXECUTED = "아무도 처형되지 않았습니다.";
    public static string TEXT_NOBODY_DAMAGED = "아무도 피해를 보지 않았습니다.";
    public static string TEXT_TAG_VICTORY = "승리";

    //열거형 정수
    public const int SELECTION_CITIZEN = 1;                 //시민 선택 번호
    public const int SELECTION_MAFIA = 2;                   //범인 선택 번호
    public const int SELECTION_NONE = 0;                    //선택 해제 번호
    public const int SELECTION_SUFFRAGE = 1;                //투표 선택 번호
    public const int SELECTION_TELESCOPE = 2;               //망원경 사용 번호
    public const int SECTION_MORNING = 0;                   //아침 표시 번호
    public const int SECTION_AFTERNOON = 1;                 //점심 표시 번호
    public const int SECTION_NIGHT = 2;                     //저녁 표시 번호

    public static readonly int COUNT_MINIMUM_PLAYERS = 3;   //최소 시작 인원
    public static readonly int COUNT_TIME_MORNING = 51;     //아침 시간
    public static readonly int COUNT_TIME_AFTERNOON = 21;   //점심 시간
    public static readonly int COUNT_TIME_NIGHT = 11;       //저녁 시간
    public static readonly int VALUE_TIME_INCREASING = 10;  //시간 연장
    public static readonly int VALUE_TIME_DECREASING = 10;  //시간 단축
    public static readonly int VALUE_START_ENERGY = 10;     //시작 에너지
    public static readonly int VALUE_COST_REFRESH = 1;      //품목 갱신
    public static readonly int VALUE_GHOST_DAMAGE = 2;      //귀신 피해
    public static readonly int VALUE_SHOCK_DAMAGE = 5;      //충격 피해

    public static readonly Color COLOR_SELECT = Color.green;
    public static readonly Color COLOR_DESELECT = Color.white;
    public static readonly Color COLOR_MINE = Color.yellow;
    public static readonly Color COLOR_DEAD = Color.red;
}