using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� ���������� � ���������� ��������� ���� � ��������� ����, � ��� �� ����� ���� ��������� �����
/// </summary>
public class Scorecounter : MonoBehaviour
{
    //��������� ������ � �������� �������� ����� ��� ���� ����� ��� �������� ����� �������/����������
    //�������������� ���������� ��������
    public Text scoreTextLeft;
    public Text highScoreTextLeft;
    public Text scoreTextRight;
    public Text highScoreTextRight;
    [SerializeField]
    Text restartName;
    public int score {get; private set;}

    string[,] textConstants = { { "Score", "HighScore", "Game Over!", "Floor", "Coolness Timer","You have set a new record and got into the leaderboard","Unfortunately,\nyou did not set a new record and did not make it to the leaderboard","Deceleration Timer", "Coolness", "Restart" },
                                { "����","������", "���� ��������!","����","������ ��������","�� ���������� ����� ������ � ������ � ������� �������","� ���������,\n�� �� ���������� ����� ������ � �� ������ � ������� �������","������ ����������", "��������", "����������"} };


    private void Start()
    {
        highScoreTextLeft.text = textConstants[Store.LANGUAGE%2, 1] + ":";
        scoreTextLeft.text = textConstants[Store.LANGUAGE%2, 0] + ":";
        highScoreTextRight.text = ""+Store.HIGHSCORE;
        scoreTextRight.text = "0";
        restartName.text = textConstants[Store.LANGUAGE % 2, 9];
        
    }
    public void AddScore(int nS)
    {
        score += nS;
        scoreTextRight.text = "" + score;
    }

    public string this [int x, int y]
    {
        get
        {
        return textConstants[x,y];
        }
    }

}
