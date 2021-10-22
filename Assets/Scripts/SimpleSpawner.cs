using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс фабрика генерирующий башню и ее содержимое в процессе игры включая этажи платформ, врагов и бонусу
/// считаю, что разделать элементы не целесообразно
/// </summary>

public class SimpleSpawner : MonoBehaviour
{
    public GameObject[] prefabPlatforms;
    public GameObject [] prefabDymanicObjects;
    public GameObject prefabEntrance;
    List<GameObject> platformsList;
    List<GameObject> enemiesList;
    int floorNumber=-12;

    //Блок полей отвечающий за возможное вращение ряда
    bool needRotateRow = false;
    float rotateSpeed;
    //Распределение скоростей для платформ этажей
    float[] speeds = {0, 10, -10, 5, -5 };
    //Распределение скоростей для плавающий врагов
    float[] enemySpeeds = {-20,20,-30,30};
    //Возможные начальные повороты для бонусов и врагов
    Vector3 [] aRotations = {new Vector3(0, 0), new Vector3(0,45), new Vector3(0, 90), new Vector3(0,135),
        new Vector3(0, 180), new Vector3(0,225), new Vector3(0, 270), new Vector3(0,315)};

    //Меняем массив влияющий на распределение платформ разных цветов
    //Можно конечно было и поменять на список тогда было бы проще
    private void ChangProbability()
    {
        GameObject[] tmp = new GameObject[prefabPlatforms.Length + 1];
        for (int i=0;i<prefabPlatforms.Length;i++)
        {
            tmp[i] = prefabPlatforms[i];
        }
        int t = Random.Range(0, 2);
        if (t==0) tmp[tmp.Length - 1] = tmp[0];//в этом у нас красная
        else tmp[tmp.Length - 1] = tmp[1];//в этом у нас желтая
        prefabPlatforms = tmp;
    }

    //Увеличиваем возможные скорости
    void ChangeSpeeds()
    {
        print("Меняем скорости!");
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

    //Метод используется при пробивании платформ шаром игрока
    public void DeletePlatform(GameObject pl)
    {
        platformsList.Remove(pl);
    }
    
    void StartSpawn ()
    {
        //Создаем верний ряд всегда стабильный
        GameObject go = Instantiate(prefabPlatforms[2], new Vector3(0, 7.5f, 0), Quaternion.Euler(new Vector3(0, 180, 0)));
        go.transform.SetParent(gameObject.transform);
        go.transform.localScale = new Vector3(1, 1, 1);
        platformsList.Add(go);

        //Создаем базовые ряды
        for (int j = -1; j > floorNumber; j--)
        {
            SpawnNewLevel((float)j*2.5f+7.5f,j);
        }
    }
    
    void SpawnNewLevel(float j, int f)//важно вторая переменная это конкретно тот этаж который строится
                                      //при текущем обращении
    {
            List<GameObject> thisLevel = new List<GameObject>();
            bool entranceExists = false;
            DeterminateRotatingSpeed();//определили будет ли вращаться этот ряд и если да то с
                                       //какой скоростью и в каком направлении
            for (int i = 0; i < 12; i++)
            {
                //Этот блок отвечает за наличие хотя бы одного прохода в уровне   
                int n = Random.Range(0, prefabPlatforms.Length + 1);
                if (n == prefabPlatforms.Length)//появилось окошко
                {
                    entranceExists = true;
                    //print("Выход есть. На высоте "+j);
                    continue;
                }
                //print("Добавляют одну платформу на уровень " + j);
                GameObject go = Instantiate(prefabPlatforms[n], new Vector3(0, j, 0), Quaternion.Euler(new Vector3(0, i * 30, 0)));
                go.transform.SetParent(gameObject.transform);
                go.transform.localScale = new Vector3(1, 1, 1);
                //Даем информацию о вращении
                go.GetComponent<PlatformRotate>().DelayedStart(needRotateRow,rotateSpeed);
                thisLevel.Add(go);//добавили платформу в список платформ этого этажа
                platformsList.Add(go);
            }

            //Если вдруг в уровне нет прохода, делаем его принудительно удаляя случайную платформу
            if (!entranceExists)
            {
                int n = Random.Range(0,thisLevel.Count);//выбрали где будет окошко
                platformsList.Remove(thisLevel[n]);//убрали платформу из общего списка платформ
                Destroy(thisLevel[n]);//уничтожили платформу
            }

            //Базовый спам бонусов 1 и 2 и врагов начинается после 6 этажа
            if (f<-6 && f>=-20)
            {
                //В конце массива предположительно будет лежать бонус третьего типа, которые появляется
                //после 50 этажа
                StandartSpam(prefabDymanicObjects.Length - 1, j);
                SpamEntrance(j);
            }    
            //Продвинутый вариант спамим все предыдущее, но иногда дважды на этаж
            else if (f<-20 && f>=-50)
            {
                StandartSpam(prefabDymanicObjects.Length - 1, j);
                StandartSpam(prefabDymanicObjects.Length - 1, j);
                SpamEntrance(j);
            }
            //Предельный вариант спамим все предыдущее плюс бонус третьего типа меняющий время
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
            //Для тестов создаем врага фиксированно
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
        //Отрицательные значения отвечают за вероятность пустых этажей
    }


    void SpawnMovingEnemyOrBonus(float j, int n)
    {
        //Для тестов создаем врага фиксированно
        GameObject eGo = Instantiate(prefabDymanicObjects[n], new Vector3(0, j, 0),
            Quaternion.Euler(aRotations[Random.Range(0,aRotations.Length)]));
        eGo.transform.SetParent(gameObject.transform);
        //Хитрый рассчет масштаба связан с тем, что ПОЧТИ все привязывается к пропорциям главной башни
        //и если вдруг мы решим ее изменить ПОЧТИ все может исказить свои размеры
        eGo.transform.localScale = new Vector3(1 / gameObject.transform.localScale.x,
           1 / gameObject.transform.localScale.y, 1 / gameObject.transform.localScale.z);
        //Даем информацию о вращении но не всем
        if (n > 4 && n<10 )
        {
            //враг который не вращается сам вокруг башни, но резко прыгает на месте
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
        //Убрать вышестоящие платформы
        DelHigherObjects(y - 0.5f, platformsList);
        DelHigherObjects(y+0.5f, enemiesList);
        //этот блок создает новые платформы ниже игрока
        SpawnNewLevel((float)floorNumber * 2.5f + 7.5f, floorNumber);
        floorNumber--;
        //Каждые 20 этажей усложняем игру
        if (floorNumber % 20 == 0)
        {
            ChangProbability();//Меняем вероятность выпадения платформ в сторону увеличения красных или желтых
            ChangeSpeeds();//Меняем возможные скорости движения платформ, врагов и бонусом
        }
    }

    //Этот метод можно использовать как для удаления вышестоящих платформ
    //так и для удаления вышестоящих врагов
    void DelHigherObjects(float y, List <GameObject> objs)
    {
        List<GameObject> onDelete = new List<GameObject>();
        //Этот блок убирает платформы которые остались выше
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
