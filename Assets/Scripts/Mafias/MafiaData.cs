using UnityEngine;

/// <summary>
/// ���Ǿ� ���� �׸��� �ʿ��� ������ ��� �� �ؽ�Ʈ ǥ�� ������ ���� Ŭ����
/// </summary>
public static class MafiaData
{
    //����
    public const string SCENE = "Mafia";                        //�� �̸� ���ڿ�
    public const string START_MAFIA = "start_mafia";            //���� ��
    public const string START_MONEY = "start_money";            //���� �ڱ�
    public const string DAILY_MONEY = "daily_money";            //���� �ݾ�
    public const string COST_SUFFRAGE = "cost_suffrage";        //��ǥ�� ����
    public const string COST_NUTRITION = "cost_nutrition";      //������ ����
    public const string COST_AMULET = "cost_amulet";            //ȣ�ź� ����
    public const string COST_TELESCOPE = "cost_telescope";      //������ ����
    public const string COST_FABRICATION = "cost_fabrication";  //���۱� ����

    //���ο�
    public const string IDENTITY = "identity";                  //���� ����
    public const string ENERGY = "energy";                      //������
    public const string MONEY = "money";                        //��
    public const string SUFFRAGE = "suffrage";                  //��ǥ��
    public const string NUTRITION = "nutrition";                //������
    public const string AMULET = "amulet";                      //ȣ�ź�
    public const string EXCLUSIVE = "exclusive";                //���� ������ (�ù�: ������, ����: ���۱�)
    public const string OPTION = "option";

    //ǥ�� �ؽ�Ʈ
    public static string TEXT_TITLE = "���Ǿ�";
    public static string TEXT_START_MAFIA = "���� ��";
    public static string TEXT_START_MONEY = "���� �ڱ�";
    public static string TEXT_DAILY_MONEY = "���� �޿�";
    public static string TEXT_APPLY_RANDOM = "���� ���";
    public static string TEXT_APPLY_CITIZEN= "�ù� ���";
    public static string TEXT_APPLY_MAFIA = "���� ���";
    public static string TEXT_INFO_START_MAFIA = "���ӿ��� ������ ���ε���\n�� �ο��� �����մϴ�.";
    public static string TEXT_INFO_START_MONEY = "�� ������ ���۰� ���ÿ�\n���޵Ǵ� �������Դϴ�.";
    public static string TEXT_INFO_DAILY_MONEY = "�� ������ ����Ǵ� ����\n��ħ���� ������ �� ��\n��Ģ������ ���޵Ǵ� �ݾ��Դϴ�.";
    public static string TEXT_INFO_COST_SUFFRAGE = "�ڽ��� ��ǥ�� �� ����\n�߰�ǥ 1���� �� �ݴϴ�.\n�ù� ���� ���� ������";
    public static string TEXT_INFO_COST_NUTRITION = "�� �������� �Ҹ��Ͽ�\n�ڽ��� ����� +1 ȸ�� �մϴ�.\n�ù� ���� ���� ������";
    public static string TEXT_INFO_COST_AMULET = "������ ���⿡ ó���� ��\n�̰��� �����ϸ� ���� �ʽ��ϴ�.\n�ù� ���� ���� ������";
    public static string TEXT_INFO_COST_TELESCOPE = "�� �������� ����Ͽ� Ư������\n�ſ� Ȯ���� �� �� �ֽ��ϴ�.\n�ù� ���� ������";
    public static string TEXT_INFO_COST_FABRICATION = "�� �������� ����� �Ϸ� ����\n�ù��� ��Ī �� �� �ֽ��ϴ�.\n���� ���� ������";
    public static string TEXT_INFO_ENERGY = "����� 1�� �Ǹ�\n��ǥ ������ �� �� ���� �ǰ�\n�����̶�� ��ħ �ð���\n������ �ڹ��� �ϰ� �Ǵ�\n�������� ������ �� ���ݽô�.";
    public static string TEXT_UNNECESSARY_ITEM = "���� �������� ���ٸ�\n��Ī �����۵� ����� �մϴ�.";
    public static string TEXT_MINIMUM_PLAYERS = "���� �ο��� �ּ� {0}�� �Դϴ�.";
    public static string TEXT_MAXIMUM_PLAYERS = "������ �ִ� �ο��� {0}�� �Դϴ�.";

    public static string TEXT_TAG_ENERGY = "���";
    public static string TEXT_TAG_PRICE = "����";
    public static string TEXT_TAG_MAFIA = "����";
    public static string TEXT_TAG_CITIZEN = "�ù�";
    public static string TEXT_TAG_DAY = "{0}��";
    public static string TEXT_TAG_MONEY = "{0}��";
    public static string TEXT_TAG_VOTE = "{0}ǥ";
    public static string TEXT_TAG_TIME = "{0}��";
    public static string TEXT_TAG_MORNING = "��ħ";
    public static string TEXT_TAG_AFTERNOON = "����";
    public static string TEXT_TAG_NIGHT = "����";
    public static string TEXT_TAG_ALIVE = "����";
    public static string TEXT_TAG_DEATH = "���";
    public static string TEXT_TAG_DISCUSSION = "��ǥ �ð�";
    public static string TEXT_TAG_EXECUTION = "ó�� ��ǥ";
    public static string TEXT_TAG_CONSPIRACY = "���� ����";
    public static string TEXT_TAG_INCREASING = "�ð� ����";
    public static string TEXT_TAG_DECREASING = "�ð� ����";
    public static string TEXT_TAG_YES = "����";
    public static string TEXT_TAG_NO = "�ݴ�";
    public static string TEXT_TAG_USE = "���";
    public static string TEXT_TAG_BUY = "����";
    public static string TEXT_TAG_OWN_ITEM = "���� ������";
    public static string TEXT_TAG_SHOP_ITEM = "���� ������";
    public static string TEXT_TAG_FUNDAGE = "������";
    public static string TEXT_TAG_AMULET = "ȣ�ź�";
    public static string TEXT_TAG_SUFFRAGE = "��ǥ��";
    public static string TEXT_TAG_NUTRITION = "������";
    public static string TEXT_TAG_TELESCOPE = "����";
    public static string TEXT_TAG_FABRICATION = "��Ī";
    public static string TEXT_TAG_ITEM_REFRESH = "ǰ�� ����";
    public static string TEXT_COUNT_OF_SURVIVORS = "���� ������";
    public static string TEXT_COUNT_OF_MAFIAS = "���� ����";

    public static string TEXT_BECOME_CITIZEN = "����� �ù��� �Ǿ����ϴ�.";
    public static string TEXT_BECOME_MAFIA = "����� ������ �Ǿ����ϴ�.";
    public static string TEXT_BECOME_MORNING = "��ħ�� �Ǿ����ϴ�.";
    public static string TEXT_BECOME_AFTERNOON = "������ �Ǿ����ϴ�.";
    public static string TEXT_BECOME_NIGHT = "������ �Ǿ����ϴ�.";

    public static string TEXT_INCREASE_TIME = "{0}���� �ð� ������ �Ͽ����ϴ�.";
    public static string TEXT_DECREASE_TIME = "{0}���� �ð� ������ �Ͽ����ϴ�.";
    public static string TEXT_SELECT_SUSPECT_PERSON = "{0}���� {1}���� �����ڷ� �����߽��ϴ�.";
    public static string TEXT_SELECT_VICTIM_PERSON = "{0}���� {1}���� ����ڷ� �����߽��ϴ�.";
    public static string TEXT_USE_SUFFRAGE = "{0}���� �߰� ��ǥ���� �̿��߽��ϴ�.";
    public static string TEXT_RECEIVE_DAILY_MONEY = "�ϱ� {0}���� �޾ҽ��ϴ�.";
    public static string TEXT_RECEIVE_DIVIDEND = "���� {0}���� �޾ҽ��ϴ�.";
    public static string TEXT_CONFESSION = "{0}���� �˸� �ڹ��մϴ�.";

    public static string TEXT_USE_PLAN_SUFFRAGE = "��ǥ �ð��� ������ ��󿡰� �߰� ǥ�� �� �� �ִ� �����Դϴ�.";
    public static string TEXT_INVESTIGATE_TARGET = "���� �ִ� ����� �����ϸ� �ź��� �����մϴ�.";
    public static string TEXT_PRETEND_TO_CITIZEN = "���ݺ��� �ù���ô �����մϴ�.";
    public static string TEXT_FINISH_PRETEND = "�ù����� ��Ī�ϴ� �ð��� ���� �����ϴ�.";
    public static string TEXT_ALREADY_YES = "�̹� ���� �����Դϴ�.";
    public static string TEXT_ALREADY_NO = "�̹� �ݴ� �����Դϴ�.";

    public static string TEXT_LACK_OF_MONEY = "���� �����մϴ�.";
    public static string TEXT_LACK_OF_ENERGY = "��� �������� ���� �� �� �����ϴ�.";
    public static string TEXT_FULL_ENERGY = "�̹� �ִ� ü���Դϴ�.";
    public static string TEXT_RECOVER_ENERGY = "����� ȸ�� �Ͽ����ϴ�.";
    public static string TEXT_PREPARE_CURSING = "{0}���� ���� �մϴ�.";
    public static string TEXT_DO_CURSING = "{0}���� ���� �Ͽ����ϴ�.";
    public static string TEXT_BE_SUSPECTED = "{0}���� �����ڷ� ���õǾ����ϴ�.";
    public static string TEXT_BE_SHOCKED = "����� ����� �޾� ����� {0} �����˴ϴ�.";
    public static string TEXT_BE_CURSED = "������ {0}���� ����� �����մϴ�.";
    public static string TEXT_BE_KILLED = "{0}���� ������ ���߽��ϴ�.";
    public static string TEXT_PRESERVE_LIFE = "{0}���� ȣ�źη� ���Ͽ� ����� �����Ͽ����ϴ�.";
    public static string TEXT_NOBODY_SENTENCED = "�ƹ��� ���õ��� �ʾҽ��ϴ�.";
    public static string TEXT_NOBODY_EXECUTED = "�ƹ��� ó������ �ʾҽ��ϴ�.";
    public static string TEXT_NOBODY_DAMAGED = "�ƹ��� ���ظ� ���� �ʾҽ��ϴ�.";
    public static string TEXT_TAG_VICTORY = "�¸�";

    //������ ����
    public const int SELECTION_CITIZEN = 1;                 //�ù� ���� ��ȣ
    public const int SELECTION_MAFIA = 2;                   //���� ���� ��ȣ
    public const int SELECTION_NONE = 0;                    //���� ���� ��ȣ
    public const int SELECTION_SUFFRAGE = 1;                //��ǥ ���� ��ȣ
    public const int SELECTION_TELESCOPE = 2;               //������ ��� ��ȣ
    public const int SECTION_MORNING = 0;                   //��ħ ǥ�� ��ȣ
    public const int SECTION_AFTERNOON = 1;                 //���� ǥ�� ��ȣ
    public const int SECTION_NIGHT = 2;                     //���� ǥ�� ��ȣ

    public static readonly int COUNT_MINIMUM_PLAYERS = 3;   //�ּ� ���� �ο�
    public static readonly int COUNT_TIME_MORNING = 51;     //��ħ �ð�
    public static readonly int COUNT_TIME_AFTERNOON = 21;   //���� �ð�
    public static readonly int COUNT_TIME_NIGHT = 11;       //���� �ð�
    public static readonly int VALUE_TIME_INCREASING = 10;  //�ð� ����
    public static readonly int VALUE_TIME_DECREASING = 10;  //�ð� ����
    public static readonly int VALUE_START_ENERGY = 10;     //���� ������
    public static readonly int VALUE_COST_REFRESH = 1;      //ǰ�� ����
    public static readonly int VALUE_GHOST_DAMAGE = 2;      //�ͽ� ����
    public static readonly int VALUE_SHOCK_DAMAGE = 5;      //��� ����

    public static readonly Color COLOR_SELECT = Color.green;
    public static readonly Color COLOR_DESELECT = Color.white;
    public static readonly Color COLOR_MINE = Color.yellow;
    public static readonly Color COLOR_DEAD = Color.red;
}