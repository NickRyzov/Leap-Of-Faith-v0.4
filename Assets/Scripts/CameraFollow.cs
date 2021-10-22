using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ����� ��������� ������� � ����� ������� � ��������� ��������� ������� �� ������ �����, ����� ��
/// ������ �� ������ �����
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject background;
    public float yLimit; //����� ������ ���� �������� ������ ��� �� ���� �� ������ ����
    public SimpleSpawner simpleSpawner;
    
    Vector3 startPos, finishPos;
    float timeToOvercome=1.5f;
    float startTime;
    bool changePos = false;
    float yShift=2.5f;

    Vector3 backStartPos, backFinishPos;




    void Update()
    {
        if (changePos)//����� ������� ������ ������ � ���� �� ���������� �� ���������� �����
        {
            Vector3 currenPos;
            float u = (Time.time - startTime) / timeToOvercome;
            currenPos = (1 - u) * startPos + u * finishPos;
            transform.position = currenPos;
            
            //���� ���������� ������ ����
            currenPos = (1 - u) * backStartPos + u * backFinishPos;
            background.transform.position = currenPos;
            if (u >= 1)
            {
                changePos = false;
            }
        }
        else//�������� ������������� ������� ������
        {
            if (player!=null && player.transform.position.y <= yLimit)
            {
                simpleSpawner.DellHigherPlatforms(yLimit);//������� ��������� ������� ����
                                                          //� ��������� ����� ����
                startPos = transform.position;
                float y = transform.position.y - 2.48f;
                finishPos = new Vector3(transform.position.x, y, transform.position.z);
                startTime = Time.time;
                changePos = true;

                //�������� ���� ���������� ������ ����
                backStartPos = background.transform.position;
                y = background.transform.position.y - 2.48f;
                backFinishPos = new Vector3(background.transform.position.x, y, background.transform.position.z);
                
                
                yLimit -= yShift;
                //print("������� ����� ������� ������");
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
