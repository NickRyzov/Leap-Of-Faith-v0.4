using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� ������� ������������ ����� � �� ���������� � �������� ���� ������� ����� ��������, ������ � ������
/// ������, ��� ��������� �������� �� �������������
/// </summary>

public class SimpleSpawner : MonoBehaviour
{
    public GameObject[] prefabPlatforms;
    public GameObject [] prefabDymanicObjects;
    public GameObject prefabEntrance;
    List<GameObject> platformsList;
    List<GameObject> enemiesList;
    int floorNumber=-12;

    //���� ����� ���������� �� ��������� �������� ����
    bool needRotateRow = false;
    float rotateSpeed;
    //������������� ��������� ��� �������� ������
    float[] speeds = {0, 10, -10, 5, -5 };
    //������������� ��������� ��� ��������� ������
    float[] enemySpeeds = {-20,20,-30,30};
    //��������� ��������� �������� ��� ������� � ������
    Vector3 [] aRotations = {new Vector3(0, 0), new Vector3(0,45), new Vector3(0, 90), new Vector3(0,135),
        new Vector3(0, 180), new Vector3(0,225), new Vector3(0, 270), new Vector3(0,315)};

    //������ ������ �������� �� ������������� �������� ������ ������
    //����� ������� ���� � �������� �� ������ ����� ���� �� �����
    private void ChangProbability()
    {
        GameObject[] tmp = new GameObject[prefabPlatforms.Length + 1];
        for (int i=0;i<prefabPlatforms.Length;i++)
        {
            tmp[i] = prefabPlatforms[i];
        }
        int t = Random.Range(0, 2);
        if (t==0) tmp[tmp.Length - 1] = tmp[0];//� ���� � ��� �������
        else tmp[tmp.Length - 1] = tmp[1];//� ���� � ��� ������
        prefabPlatforms = tmp;
    }

    //����������� ��������� ��������
    void ChangeSpeeds()
    {
        print("������ ��������!");
        for (int i = 0; i<speeds.Length;i++)
        {
            float z = speeds[i];
            z*= 1.15f;
            speeds[i] = z;
        }

        for (int i = 0; i < enemySpeeds.Length; i++)
        {
            float z = enemySpeeds[i];
            z *= 1.15f;
            enemySpeeds[i] = z;
        }
    }

    void Start()
    {
        platformsList = new List<GameObject>();
        enemiesList = new List<GameObject>();
        StartSpawn();
    }

    public void DeleteEnemy(GameObject en)
    {
        enemiesList.Remove(en);
    }

    //����� ������������ ��� ���������� �������� ����� ������
    public void DeletePlatform(GameObject pl)
    {
        platformsList.Remove(pl);
    }
    
    void StartSpawn ()
    {
        //������� ������ ��� ������ ����������
        GameObject go = Instantiate(prefabPlatforms[2], new Vector3(0, 7.5f, 0), Quaternion.Euler(new Vector3(0, 180, 0)));
        go.transform.SetParent(gameObject.transform);
        go.transform.localScale = new Vector3(1, 1, 1);
        platformsList.Add(go);

        //������� ������� ����
        for (int j = -1; j > floorNumber; j--)
        {
            SpawnNewLevel((float)j*2.5f+7.5f,j);
        }
    }
    
    void SpawnNewLevel(float j, int f)//����� ������ ���������� ��� ��������� ��� ���� ������� ��������
                                      //��� ������� ���������
    {
            List<GameObject> thisLevel = new List<GameObject>();
            bool entranceExists = false;
            DeterminateRotatingSpeed();//���������� ����� �� ��������� ���� ��� � ���� �� �� �
                                       //����� ��������� � � ����� �����������
            for (int i = 0; i < 12; i++)
            {
                //���� ���� �������� �� ������� ���� �� ������ ������� � ������   
                int n = Random.Range(0, prefabPlatforms.Length + 1);
                if (n == prefabPlatforms.Length)//��������� ������
                {
                    entranceExists = true;
                    //print("����� ����. �� ������ "+j);
                    continue;
                }
                //print("��������� ���� ��������� �� ������� " + j);
                GameObject go = Instantiate(prefabPlatforms[n], new Vector3(0, j, 0), Quaternion.Euler(new Vector3(0, i * 30, 0)));
                go.transform.SetParent(gameObject.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
                //���� ���������� � ��������
                go.GetComponent<PlatformRotate>().DelayedStart(needRotateRow,rotateSpeed);
                thisLevel.Add(go);//�������� ��������� � ������ �������� ����� �����
                platformsList.Add(go);
            }

            //���� ����� � ������ ��� �������, ������ ��� ������������� ������ ��������� ���������
            if (!entranceExists)
            {
                int n = Random.Range(0,thisLevel.Count);//������� ��� ����� ������
                platformsList.Remove(thisLevel[n]);//������ ��������� �� ������ ������ ��������
                Destroy(thisLevel[n]);//���������� ���������
            }

            //������� ���� ������� 1 � 2 � ������ ���������� ����� 6 �����
            if (f<-6 && f>=-20)
            {
                //� ����� ������� ���������������� ����� ������ ����� �������� ����, ������� ����������
                //����� 50 �����
                StandartSpam(prefabDymanicObjects.Length - 1, j);
                SpamEntrance(j);
            }    
            //����������� ������� ������ ��� ����������, �� ������ ������ �� ����
            else if (f<-20 && f>=-50)
            {
                StandartSpam(prefabDymanicObjects.Length - 1, j);
                StandartSpam(prefabDymanicObjects.Length - 1, j);
                SpamEntrance(j);
            }
            //���������� ������� ������ ��� ���������� ���� ����� �������� ���� �������� �����
            else if (f<-50)
            {
                StandartSpam(prefabDymanicObjects.Length, j);
                StandartSpam(prefabDymanicObjects.Length, j);
                SpamEntrance(j);
            }
    }

    void SpamEntrance(float j)
    {
        int t = Random.Range(0, 5);
        if (t==0)
        {
            //��� ������ ������� ����� ������������
            GameObject eGo = Instantiate(prefabEntrance, new Vector3(0, j-0.1f, 0),
                Quaternion.Euler(aRotations[Random.Range(0, aRotations.Length)]));
            eGo.transform.SetParent(gameObject.transform);
            eGo.transform.localScale = new Vector3(0.5f, 0.1f, 0.36f);
            enemiesList.Add(eGo);
        }
    }



    void StandartSpam(int end, float j)
    {
        int t = Random.Range(-5, end);
        if (t >= 0) SpawnMovingEnemyOrBonus(j + 0.3f, t);
        //������������� �������� �������� �� ����������� ������ ������
    }


    void SpawnMovingEnemyOrBonus(float j, int n)
    {
        //��� ������ ������� ����� ������������
        GameObject eGo = Instantiate(prefabDymanicObjects[n], new Vector3(0, j, 0),
            Quaternion.Euler(aRotations[Random.Range(0,aRotations.Length)]));
        eGo.transform.SetParent(gameObject.transform);
        //������ ������� �������� ������ � ���, ��� ����� ��� ������������� � ���������� ������� �����
        //� ���� ����� �� ����� �� �������� ����� ��� ����� �������� ���� �������
        eGo.transform.localScale = new Vector3(1 / gameObject.transform.localScale.x,
           1 / gameObject.transform.localScale.y, 1 / gameObject.transform.localScale.z);
        //���� ���������� � �������� �� �� ����
        if (n > 4 && n<10 )
        {
            //���� ������� �� ��������� ��� ������ �����, �� ����� ������� �� �����
        }
        else
        {
            eGo.GetComponent<PlatformRotate>().DelayedStart(true,
              enemySpeeds[Random.Range(0, enemySpeeds.Length)]);
        }
        enemiesList.Add(eGo);
    }

    void DeterminateRotatingSpeed()
    {
        int n = Random.Range(0, speeds.Length);
        if (n==0)
        {
            needRotateRow = false;
        }
        else
        {
            needRotateRow = true;
        }
        rotateSpeed = speeds[n];
    }

    public void DellHigherPlatforms(float y)
    {
        //������ ����������� ���������
        DelHigherObjects(y - 0.5f, platformsList);
        DelHigherObjects(y+0.5f, enemiesList);
        //���� ���� ������� ����� ��������� ���� ������
        SpawnNewLevel((float)floorNumber * 2.5f + 7.5f, floorNumber);
        floorNumber--;
        //������ 20 ������ ��������� ����
        if (floorNumber % 20 == 0)
        {
            ChangProbability();//������ ����������� ��������� �������� � ������� ���������� ������� ��� ������
            ChangeSpeeds();//������ ��������� �������� �������� ��������, ������ � �������
        }
    }

    //���� ����� ����� ������������ ��� ��� �������� ����������� ��������
    //��� � ��� �������� ����������� ������
    void DelHigherObjects(float y, List <GameObject> objs)
    {
        List<GameObject> onDelete = new List<GameObject>();
        //���� ���� ������� ��������� ������� �������� ����
        foreach (GameObject go in objs)
        {
            if (go.transform.position.y >= y)
            {
                onDelete.Add(go);
            }
        }

        foreach (GameObject go in onDelete)
        {
            objs.Remove(go);
            Destroy(go);
        }
    }
}
