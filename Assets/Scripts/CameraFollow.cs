using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Этот класс управляет камерой и фоном которые с некоторой задержкой следуют за желтым шаром, когда он
/// падает на нижние этажи
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject background;
    public float yLimit; //место игрока ниже которого значит что он упал на другой этаж
    public SimpleSpawner simpleSpawner;
    
    Vector3 startPos, finishPos;
    float timeToOvercome=1.5f;
    float startTime;
    bool changePos = false;
    float yShift=2.5f;

    Vector3 backStartPos, backFinishPos;




    void Update()
    {
        if (changePos)//смена позиции камеры начата и пока не завершится не начинается снова
        {
            Vector3 currenPos;
            float u = (Time.time - startTime) / timeToOvercome;
            currenPos = (1 - u) * startPos + u * finishPos;
            transform.position = currenPos;
            
            //блок касающийся сдвига фона
            currenPos = (1 - u) * backStartPos + u * backFinishPos;
            background.transform.position = currenPos;
            if (u >= 1)
            {
                changePos = false;
            }
        }
        else//проверка необходимости сменить камеру
        {
            if (player!=null && player.transform.position.y <= yLimit)
            {
                simpleSpawner.DellHigherPlatforms(yLimit);//убираем платформы которые выше
                                                          //и добавляем новые ниже
                startPos = transform.position;
                float y = transform.position.y - 2.48f;
                finishPos = new Vector3(transform.position.x, y, transform.position.z);
                startTime = Time.time;
                changePos = true;

                //Тестовый блок касающийся сдвига фона
                backStartPos = background.transform.position;
                y = background.transform.position.y - 2.48f;
                backFinishPos = new Vector3(background.transform.position.x, y, background.transform.position.z);
                
                
                yLimit -= yShift;
                //print("начинаю смену позиции камеры");
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
