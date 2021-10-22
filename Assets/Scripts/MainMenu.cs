using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

/// <summary>
/// ����� �������� ����, ���������� ������ ����������. �������� ������� �������� ������.
/// ��������� ���������� ����� ���� �� ���������� � ���� ������, �� ����� ���� �� ������� �� ��������� � � 
/// ����� �� ����� ���������� � ������ �������
/// </summary>
public class MainMenu : MonoBehaviour
{
    public InputField inputField;
    public Text leadersTitleText;
    public Text leaderNamesText;
    public Text leaderScoresText;
    public Text pretenderText;
    public Text startButtonText;
    public Text exitButtonText;
    public Text languageText;
    public Text inputFieldText;
    public Text rulesText;
    public Text rulesButtonText;
    public Text backBText;
    public Text backB2Text;
    public Text panelTitle;
    public Text creditButtonText;
    public GameObject [] panel;
    public GameObject gameTitle;

    //����������� ����� ��� �������� �� �������/����������
    string[,] constantText =
    {
        {"Leaderboard:","Challenger","Start Game","�������","Quit Game","Enter your name...","Rules",
            "The goal of the game is to score as many points as possible and break the highest record " +
                "set by the champion.\n\nPoints are earned by jumping the yellow ball on the platform of the lower floors." +
                " Jumping on the same floor does not give points. Jumping over several floors is worth more points at once" +
                " than jumping over one floor. Landing on yellow platforms doubles the result for the current jump.\n\n" +
                "The decision should be made as soon as possible, as multiple jumps on the same level gradually decreases" +
                " the multiplier from 7 to 0. More than seven jumps on one floor leads to the fact that you will not" +
                " receive points at all.\n\nRed means danger when the yellow ball collides with red objects, the game ends.\n\n" +
                "At the lower levels, various bonuses appear that give the player's ball different temporary boosts." +
                " Try them yourself.\n\nControl is carried out with the arrow keys left and right or with the keys \"A\"" +
                " and \"D\".\n\nBecome our new champion!",
            "Back","Rules of the game","Credits"},
        {"������ ������:", "����������", "������ ����", "English", "����� �� ����","������� ���� ���...",
            "�������",
            "���� ���� ������� ��� ����� ������ ����� � ������ ��������� ������, ������������� ���������.\n\n" +
            " ���� ���������� ��� ������ ������� ���� �� ��������� ������ ������. ������ �� ����� � ��� �� �����" +
            " �� ���� �����. ������ ����� ��������� ������ ����� �������� ������ ����� ��� ������ ����� ���� ����." +
            " ����������� �� ������ ��������� ��������� ��������� �� ������� ������.\n\n������� ����� ��������� ���" +
            " ����� ������ ���, ��� ������������ �������� �� ����� ����� ���������� ������� ��������" +
            " ��������������� � 7 �� 0. ������ ���� ������� �� ����� ����� �������� � ���� ��� �� �� " +
            "�������� ����� �����.\n\n������� ���� ������ ��������� ��� ������������ ������� ���� � " +
            "�������� ��������� ���� �����������.\n\n�� ������ ������� ���������� ������ ������, �������" +
            " ���� ���� ������ ������ ��������� ��������. ���������� �� �������� ��������������.\n\n"+
            "���������� �������������� ��������� ������� ����� � ������ ��� ��������� \"A\" � \"D\".\n\n" +
            " ������� ����� ����� ���������!"
        ,"�����","�������� �������","�������������"}
    };

    int languageNumber = 0;
    int panelNumber;

    private void Awake()
    {
        inputField.onEndEdit.AddListener(ChangeName);
    }

    void Start()
    {
        languageNumber = Store.LANGUAGE;
        Display();
    }

    public void StartGame()
    {
        //print(Store.PLAYER_NAME);
        if (Store.PLAYER_NAME != null && Store.PLAYER_NAME != "")
        {
            Store.LANGUAGE = languageNumber;
            SceneManager.LoadScene(1);
        }
    }

    public void ChangeLanguage()
    {
        languageNumber++;
        Display();
    }

    public void AuxiliaryPanel(int n)
    {
        switch (n)
        {
            //�������� ��������������� ������
            case 0:
            case 1:
                panelNumber = n;
                gameTitle.SetActive(false);
                panel[panelNumber].SetActive(true);
                break;
            //��������� ��������������� ������
            default:
                panel[panelNumber].SetActive(false);
                gameTitle.SetActive(true);
                break;
        }
    }

    

    public void Exit()
    {

#if UNITY_EDITOR

        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
        Application.Quit();
    }

    public void ChangeName(string name)
    {
        //print("name added " + name);
        Store.PLAYER_NAME = name;
        pretenderText.text = constantText[languageNumber % 2, 1]+": " + name;
    }



    public void Display()
    {
        //������ ������ ��������� ������ � ����������� �� ���������� �����
        leadersTitleText.text = constantText[languageNumber % 2, 0];
        pretenderText.text = constantText[languageNumber % 2, 1] + ": " + Store.PLAYER_NAME;
        leaderNamesText.text = Store.LEADERS;
        leaderScoresText.text = Store.LEADER_SCORES;
        startButtonText.text = constantText[languageNumber % 2, 2];
        languageText.text = constantText[languageNumber % 2, 3];
        exitButtonText.text = constantText[languageNumber % 2, 4];
        inputFieldText.text = constantText[languageNumber % 2, 5];
        rulesButtonText.text = constantText[languageNumber % 2, 6];
        rulesText.text = constantText[languageNumber % 2, 7];
        backBText.text = constantText[languageNumber % 2, 8];//������ �������� ������ ������
        backB2Text.text = constantText[languageNumber % 2, 8];//������ �������� ������ ������
        panelTitle.text = constantText[languageNumber % 2, 9];
        creditButtonText.text = constantText[languageNumber % 2, 10];
    }
}
