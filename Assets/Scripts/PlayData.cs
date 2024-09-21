/// <summary>
/// 모든 게임 테마에 공통된 데이터 양식 및 텍스트 표시 내용을 담은 클래스
/// </summary>
public static class PlayData
{
    //데이터 상수 문자열(개인용)
    public const string NICKNAME = "nickname";      //플레이어 이름
    public const string PASSWORD = "password";      //플레이어 비밀번호
    public const string SELECTION = "selection";    //플레이어 선택

    //데이터 상수 문자열(공용)
    public const string THEME = "theme";            //게임 테마
    public const string ROUND = "round";            //게임 라운드
    public const string TIMER = "timer";            //게임 타이머 
    public const string MESSAGE = "message";        //게임 문자

    //씬 이름 문자열
    public static readonly string SCENE_ENTRY = "Entry";
    public static readonly string SCENE_LOBBY = "Lobby";
    public static readonly string SCENE_ROOM = "Room";

    //글자 표시 문자열
    public static string TEXT_TAG_TITLE = "투게더!";
    public static string TEXT_TAG_VERSION = " 버전";
    public static string TEXT_TAG_NICKNAME = "아이디";
    public static string TEXT_TAG_PASSWORD = "비밀번호";
    public static string TEXT_TAG_ROOM = "방 이름";
    public static string TEXT_TAG_THEME = "테마";
    public static string TEXT_TAG_MINE = "나";
    public static string TEXT_TAG_MASTER = "방장";
    public static string TEXT_TAG_CREATE = "만들기";
    public static string TEXT_TAG_JOIN = "입장하기";
    public static string TEXT_TAG_QUIT = "종료하기";
    public static string TEXT_TAG_START = "시작";
    public static string TEXT_TAG_STOP = "중지";
    public static string TEXT_TAG_EXIT = "나가기";
    public static string TEXT_TAG_NONE = "없음";
    public static string TEXT_TAG_INDEX = "{0}번";
    public static string TEXT_TAG_NUMBER = "{0}개";
    public static string TEXT_TAG_PEOPLE = "{0}명";
    public static string TEXT_TAG_GAMEOVER = "게임 종료";
    //public static string TEXT_TAG_READY = "(준비 중)";
    //public static string TEXT_TAG_PLAYING = "(진행 중)";


    public static string TEXT_COUNT_OF_PLAYERS = "인원";
    public static string TEXT_COUNT_OF_ROOMS = "방";
    public static string TEXT_CONNECT_PHOTON = "포톤에 연결됨";
    public static string TEXT_TRY_CONNECTION = "연결 시도 중";
    public static string TEXT_DISCONNECT = "연결이 끊어졌습니다.";
    public static string TEXT_CREATE_ROOM_FAILED = "방 만들기 실패";
    public static string TEXT_JOIN_ROOM_FAILED = "방 입장 실패";
    public static string TEXT_INPUT_NICKNAME = "닉네임을 입력해주세요";
    public static string TEXT_INPUT_PASSWORD = "비밀번호를 입력해주세요";
    public static string TEXT_INPUT_ROOM = "방 이름을 입력해주세요";
    public static string TEXT_INPUT_THEME = "방 테마를 설정해주세요";
    public static string TEXT_ALREADY_ACCESS = "이미 접속 중입니다.";
    public static string TEXT_ALREADY_PLAYING = "이미 게임이 진행 중입니다.";
    public static string TEXT_ALREADY_FINISH = "게임이 종료 되었습니다.";
    public static string TEXT_RECONNECT_PLAYING = "게임에 재접속 중입니다.";
    public static string TEXT_MINIMUM_VALUE = "최소값에 도달 했습니다.";
    public static string TEXT_MAXIMUM_VALUE = "최대값에 도달 했습니다.";
    public static string TEXT_POP_QUIT = "앱을 종료 하시겠습니까?";
    public static string TEXT_POP_LEAVE_LOBBY = "로비를 나가시겠습니까?";
    public static string TEXT_TRY_LEAVE_ROOM = "방에서 나가시겠습니까?";

    public static readonly int VALUE_COUNT_DOWN = 5;   //카운트 다운 기본 값
    public static readonly float VALUE_ONE_SECOND = 1.0f; //1초
}