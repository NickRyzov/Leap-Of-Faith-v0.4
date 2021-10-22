using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� �������� �� ����������� ����. ���� ��������� � ����� �� ��������� � �������� �� ������ ����� �� ��
/// �������� � ������� ������ ���� ����� ���� ����������� � ��������� ������� �����. ������������ ������� 
/// �� ���������� �������� �������� �������� �� ������� �������� �������� ����� ������
/// </summary>

public class FloatingScore : MonoBehaviour
{
    bool floating = false;
    float startTime;
    float duration = 1.4f;
    
    RectTransform rectTrans;
    Text txt;
    int score;
    Utils utils;
    Scorecounter scorecounter;


    Vector2 middlePoint = new Vector2(700, 150);
    Vector2 finishPoint = new Vector2(1145, 560);
    List<Vector2> points = new List<Vector2>();

    public void Init(int s, Vector2 firstPos, Utils uT, Scorecounter sC)
    {
        score = s;
        rectTrans = GetComponent<RectTransform>();
        txt = GetComponent<Text>();
        txt.text = "+" + score;

        points.Add(firstPos);
        points.Add(middlePoint);
        points.Add(finishPoint);

        utils = uT;
        scorecounter = sC;
        startTime = Time.time;
        floating = true;
    }


    private void Update()
    {
        if (!floating) return; //������������� ��������������� ��������
        float u = (Time.time - startTime) / duration;
        //��������� �������� �������� �� �������
        //float u2 = 1 - Mathf.Pow(1 - u, 2);
        float u2 = Mathf.Pow(u, 2);
        rectTrans.position = utils.Bezier(u2, points);
        if (u > 1)
        {
            scorecounter.AddScore(score);
            Destroy(gameObject);
        }

    }

}
