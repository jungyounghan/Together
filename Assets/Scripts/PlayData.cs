/// <summary>
/// ��� ���� �׸��� ����� ������ ��� �� �ؽ�Ʈ ǥ�� ������ ���� Ŭ����
/// </summary>
public static class PlayData
{
    //������ ��� ���ڿ�(���ο�)
    public const string NICKNAME = "nickname";      //�÷��̾� �̸�
    public const string PASSWORD = "password";      //�÷��̾� ��й�ȣ
    public const string SELECTION = "selection";    //�÷��̾� ����

    //������ ��� ���ڿ�(����)
    public const string THEME = "theme";            //���� �׸�
    public const string ROUND = "round";            //���� ����
    public const string TIMER = "timer";            //���� Ÿ�̸� 
    public const string MESSAGE = "message";        //���� ����

    //�� �̸� ���ڿ�
    public static readonly string SCENE_ENTRY = "Entry";
    public static readonly string SCENE_LOBBY = "Lobby";
    public static readonly string SCENE_ROOM = "Room";

    //���� ǥ�� ���ڿ�
    public static string TEXT_TAG_TITLE = "���Դ�!";
    public static string TEXT_TAG_VERSION = " ����";
    public static string TEXT_TAG_NICKNAME = "���̵�";
    public static string TEXT_TAG_PASSWORD = "��й�ȣ";
    public static string TEXT_TAG_ROOM = "�� �̸�";
    public static string TEXT_TAG_THEME = "�׸�";
    public static string TEXT_TAG_MINE = "��";
    public static string TEXT_TAG_MASTER = "����";
    public static string TEXT_TAG_CREATE = "�����";
    public static string TEXT_TAG_JOIN = "�����ϱ�";
    public static string TEXT_TAG_QUIT = "�����ϱ�";
    public static string TEXT_TAG_START = "����";
    public static string TEXT_TAG_STOP = "����";
    public static string TEXT_TAG_EXIT = "������";
    public static string TEXT_TAG_NONE = "����";
    public static string TEXT_TAG_INDEX = "{0}��";
    public static string TEXT_TAG_NUMBER = "{0}��";
    public static string TEXT_TAG_PEOPLE = "{0}��";
    public static string TEXT_TAG_GAMEOVER = "���� ����";
    //public static string TEXT_TAG_READY = "(�غ� ��)";
    //public static string TEXT_TAG_PLAYING = "(���� ��)";


    public static string TEXT_COUNT_OF_PLAYERS = "�ο�";
    public static string TEXT_COUNT_OF_ROOMS = "��";
    public static string TEXT_CONNECT_PHOTON = "���濡 �����";
    public static string TEXT_TRY_CONNECTION = "���� �õ� ��";
    public static string TEXT_DISCONNECT = "������ ���������ϴ�.";
    public static string TEXT_CREATE_ROOM_FAILED = "�� ����� ����";
    public static string TEXT_JOIN_ROOM_FAILED = "�� ���� ����";
    public static string TEXT_INPUT_NICKNAME = "�г����� �Է����ּ���";
    public static string TEXT_INPUT_PASSWORD = "��й�ȣ�� �Է����ּ���";
    public static string TEXT_INPUT_ROOM = "�� �̸��� �Է����ּ���";
    public static string TEXT_INPUT_THEME = "�� �׸��� �������ּ���";
    public static string TEXT_ALREADY_ACCESS = "�̹� ���� ���Դϴ�.";
    public static string TEXT_ALREADY_PLAYING = "�̹� ������ ���� ���Դϴ�.";
    public static string TEXT_ALREADY_FINISH = "������ ���� �Ǿ����ϴ�.";
    public static string TEXT_RECONNECT_PLAYING = "���ӿ� ������ ���Դϴ�.";
    public static string TEXT_MINIMUM_VALUE = "�ּҰ��� ���� �߽��ϴ�.";
    public static string TEXT_MAXIMUM_VALUE = "�ִ밪�� ���� �߽��ϴ�.";
    public static string TEXT_POP_QUIT = "���� ���� �Ͻðڽ��ϱ�?";
    public static string TEXT_POP_LEAVE_LOBBY = "�κ� �����ðڽ��ϱ�?";
    public static string TEXT_TRY_LEAVE_ROOM = "�濡�� �����ðڽ��ϱ�?";

    public static readonly int VALUE_COUNT_DOWN = 5;   //ī��Ʈ �ٿ� �⺻ ��
    public static readonly float VALUE_ONE_SECOND = 1.0f; //1��
}