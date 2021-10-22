using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The only candidate for splitting into multiple classes, but I find it clearly written.
/// Единственный кандидат на разделение на несколько классов, но я считаю его ясно написанным.
/// Everything is clear to me, but it seems that I will have to separate it so that it is not cumbersome.
/// Мне все понятно, но похоже придется его разделить, чтобы он не был громоздким.
/// /// </summary>
public class Player : MonoBehaviour
{
    [Header("Set in inspector")]
    [SerializeField]
    float timeToOvercome = 0.5f;
    [SerializeField]
    float height = 1;
    [SerializeField]
    GameObject button;
    [SerializeField]
    GameObject prefabFloating;
    [SerializeField]
    GameObject explodeParticle;
    [SerializeField]
    Material[] materials;
    [SerializeField]
    GameObject powerImage;
    [SerializeField]
    GameObject eButton;

    //Компоненты которые берем на месте
    Rigidbody rigid;
    SubPlayer subPlayer;
    Controller controller;
    Transform canvas;//Нужен при движении всплывающего текста
    Scorecounter scorecounter;
    SimpleSpawner simpleSpawner;
    Utils utils;//ссылка на утилиты
    new Renderer renderer;
    GameObject haloCarrier;
    //Компоненты которые нужны для реализации прыжка в дверь
    new Collider collider;
    TrailRenderer trailRenderer;

    //Блок связанный с подпрыгиванием желтого щара после столкновения с платформой
    bool flyUp = false;
    Vector3 startPos, finishPos;
    float startTime;

    float yLimit = 7.5f;//базовая высота ниже которой начинается падение
    int level = 0;//счетчик уровня
    int counter = 0;//счетчик пролетаемых этажей без касания
    int multiPl = 7;

    bool reinforced = false;//переменная отвечает за действие бонуса уничтожителя врагов
    bool punchy = false;//переменная отвечает за действие бонуса пробивания платформ
    int brokenPlatform = 0;
    int tTime = 0;//Секунды таймера крутости
    int sTime = 0;//Секунды таймера замедления

    //Блок полей связанных с уровнем КРУТОСТИ желтого шара
    int powerLevel = 1;
    float powerCount = 0;
    RectTransform healthBarTransform;
    float healthBarInitWidth;
    float healthBarWidth;

    bool opossiteTheEntrance = false;
    Animator cAnim;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        subPlayer = GetComponent<SubPlayer>();
        GameObject basePillar = GameObject.Find("BasePillar");
        controller = basePillar.GetComponent<Controller>();
        simpleSpawner = basePillar.GetComponent<SimpleSpawner>();
        canvas = GameObject.Find("Canvas").GetComponent<Transform>();
        scorecounter = GameObject.Find("ScoreBoard").GetComponent<Scorecounter>();
        utils = Camera.main.GetComponent<Utils>();
        renderer = GetComponent<MeshRenderer>();
        haloCarrier = transform.Find("HaloAnchor").gameObject;
        collider = GetComponent<SphereCollider>();
        trailRenderer = GetComponent<TrailRenderer>();

        //Блок связанный с индикацией крутости           
        healthBarTransform = powerImage.GetComponent<RectTransform>();
        healthBarInitWidth = healthBarTransform.rect.width;
        DisplayPower();

        subPlayer[0].text = scorecounter[Store.LANGUAGE % 2, 3] + ": " + level;//Начальный уровень
        subPlayer[2].text = "x" + multiPl;//Мультипликатор

        cAnim = Camera.main.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Danger")
        {
            //Рисуем взрыв в любом случае
            GameObject go = Instantiate(explodeParticle, transform.position, Quaternion.identity); ;
            Destroy(go, 1);
            if (reinforced)//прикрыты защитным бонусом уничтожаем врага
            {
                simpleSpawner.DeleteEnemy(other.gameObject);
                Destroy(other.gameObject);
                subPlayer.GetSounds(1).Play();
                //Маленькая награда за уничтожение врага
                CreateFloatingScore(25);
                ChangePower();
            }
            else//не прикрыты погибаем сами и игра заканчивается
            {
                cAnim.SetBool("Shake",true);
                eButton.SetActive(false);
                EndGameCalculations();
            }
        }
        else if (other.gameObject.tag == "Bonus")//пробивающие платформы бонус
        {
            RemoveBonusTrail(other.gameObject);//делаем стандартные операции при столкновении с любым бонусом
            punchy = true;
            controller.Block(true);
            brokenPlatform = 0;

        }
        else if (other.gameObject.tag == "Bonus2")//бонус делающий игрока опасным для врагов
        {
            RemoveBonusTrail(other.gameObject);//делаем стандартные операции при столкновении с любым бонусом

            reinforced = true;
            haloCarrier.SetActive(true);

            //Защита от паралелльного вызова таймеров
            if (tTime == 0)//другой счетчик не активен
            {
                Invoke("ChangeTime", 1);
            }

            tTime = 30;
            subPlayer[1].text = scorecounter[Store.LANGUAGE % 2, 4] + ": " + tTime;//Таймер
            renderer.material = materials[1];
        }
        else if (other.gameObject.tag == "Bonus3")
        {
            RemoveBonusTrail(other.gameObject);//делаем стандартные операции при столкновении с любым бонусом
            float t = Store.instanse.ROT_SPEED / 2;
            Store.instanse.ROT_SPEED = t;
            //Защита от паралелльного вызова таймеров
            if (sTime == 0)//другой счетчик не активен
            {
                Invoke("ChangeSlowingTimer", 1);
            }
            sTime = 30;
            subPlayer[5].text = scorecounter[Store.LANGUAGE % 2, 7] + ": " + sTime;//Таймер
        }
        else if (other.gameObject.tag == "Entrance")
        {
            opossiteTheEntrance = true;
            eButton.SetActive(true);
        }
    }
      

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Entrance")
        {
            opossiteTheEntrance = false;
            eButton.SetActive(false);
        }
    }

    //Используется контроллером для разрешения прыжка в дверь
    public bool GET_ENTRANCE
    {
        get
        {
            //прыжок разрешен если находимся перед входом, летим вверх прошли более 75% пути вверх
            if (opossiteTheEntrance && flyUp && (transform.position.y-startPos.y)>0.75f)
            {
               //flyUp = false;//перестраховка
               return true;
            }
            return false; ;
        }
    }

    //Добавлен потому что я пока не могу запускать подпрограмму из другого класса
    public void DecreaseAssistant()
    {
        flyUp = false;
        StartCoroutine(DecreaseBall(transform.localScale));
    }

    IEnumerator DecreaseBall (Vector3 bScale)
    {
        trailRenderer.enabled = false;
        collider.enabled = false;
        rigid.isKinematic = true;
        Vector3 fScale = new Vector3(bScale.x / 5, bScale.y / 5, bScale.z / 5);
        float elapsedTime = 0;

        while (elapsedTime<0.2f)
        {
            elapsedTime += Time.deltaTime;
            Vector3 cScale = Vector3.Lerp(bScale,fScale,(elapsedTime/0.2f));
            transform.localScale = cScale;
            yield return null;
        }
        renderer.enabled = false;
        //print("1");
        controller.RotateAssistant();
    }

    public void IncreaseAssistant()
    {
        StartCoroutine(IncreaseBall(transform.localScale));
    }

    IEnumerator IncreaseBall(Vector3 bScale)
    {
        renderer.enabled = true;
        transform.position = new Vector3(transform.position.x, 7.5f-level*2.5f+3.5f,transform.position.z);
        Vector3 fScale = new Vector3(bScale.x * 5, bScale.y * 5, bScale.z * 5);
        float elapsedTime = 0;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            Vector3 cScale = Vector3.Lerp(bScale, fScale, (elapsedTime / 0.2f));
            transform.localScale = cScale;
            yield return null;
        }
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        trailRenderer.enabled = true;
        collider.enabled = true;
        rigid.isKinematic = false;
        rigid.velocity = Vector3.zero;
        controller.Block(false);
    }

    void RemoveBonusTrail(GameObject go)
    {
        subPlayer.GetSounds(3).Play();
        simpleSpawner.DeleteEnemy(go);
        Destroy(go);
    }

    //Итоговые вычисления при "Гибели" игрока
    void EndGameCalculations()
    {
        Destroy(gameObject);//уничтожаем игрока
        subPlayer[4].text = scorecounter[Store.LANGUAGE%2,2];//Активируем состояния поражения (может быть утратило смысл так как игра изменилась)
        button.SetActive(true);//Включаем кнопку рестарта
        controller.Block(true, 1);// отключаем возможность контроля башни и издаем звук взрыва

        //Выводим текстовый результат
        if (Store.DidItGetIntoTable(scorecounter.score))//проверяем попадания текущего результата в таблицу лидеров
        {
            subPlayer[3].text = scorecounter[Store.LANGUAGE%2, 5];//Итоговый текст
            Store.instanse.SaveD();
        }
        else
        {
            subPlayer[3].text = scorecounter[Store.LANGUAGE%2, 6];//Итоговый текст
        }

        Store.instanse.SaveD();
    }
    void ChangeTime()
    {
            tTime--;
            subPlayer[1].text = scorecounter[Store.LANGUAGE%2, 4] + ": " + tTime;//Таймер
            if (tTime==0)
            {
                reinforced = false;
                renderer.material = materials[0];
                haloCarrier.SetActive(false);
                subPlayer[1].text = "";//Таймер не виден пока не активен его бонус
                //обнуляем счетчик силы желтого шара
                powerCount = 0;
                DisplayPower();
            }
            else
            {
                if (tTime <= 5) subPlayer.GetSounds(4).Play();
                Invoke("ChangeTime", 1);
            }
    }

    //Метод который обновляет интерфейс силы желтого шара, которая растет за "убийства" красных врагов 
    void ChangePower()
    {
        if (powerCount==9)//максимальное пятое увеличение
        {
            powerCount = 0;
            powerLevel++;
        }
        else
        {
            powerCount++;
        }
        DisplayPower();
    }

    private void ChangeSlowingTimer()
    {
        sTime--;
        subPlayer[5].text = scorecounter[Store.LANGUAGE % 2, 7] + ": " + sTime;//Таймер
        if (sTime == 0)
        {
            subPlayer[5].text = "";
            Store.instanse.ROT_SPEED = 1;
        }
        else 
        {
            //if (sTime <= 5) sounds[4].Play();//Звука завершение выключили чтобы разные таймер не сливались
            Invoke("ChangeSlowingTimer", 1);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (punchy)
        {
            simpleSpawner.DeletePlatform(collision.gameObject);
            Destroy(collision.gameObject);
            brokenPlatform++;
            subPlayer.GetSounds(2).Play();
            if (brokenPlatform==5)
            {
                punchy = false;
                controller.Block(false);//вернуть возможность контролировать башню
            }
        }
        else //Ситуация обычного столкновения
        {
            //Звук удара об платформу
            subPlayer.GetSounds(0).pitch = subPlayer.GetPitch(counter);
            subPlayer.GetSounds(0).Play();

            //Блок рисования и удаления клякс
            GameObject go = Instantiate(subPlayer.GetBlut, collision.contacts[0].point,
                Quaternion.Euler(new Vector3(0, subPlayer.GetRot, 0)));
            go.transform.SetParent(collision.gameObject.transform);
            Destroy(go, 0.8f);

            //Блок подбрасывания мяча после столкновения
            flyUp = true;
            startPos = transform.position;
            float y = transform.position.y + height;
            finishPos = new Vector3(transform.position.x, y, transform.position.z);
            startTime = Time.time;

            if (collision.gameObject.tag == "Danger")
            {
                if (!reinforced)
                {
                    //Создраем частицы взрыва и убуираем след 
                    go = Instantiate(explodeParticle, transform.position, Quaternion.identity);
                    Destroy(go, 1);
                    cAnim.SetBool("Shake", true);
                    EndGameCalculations();
                }
                else//ударяем красную платформу, но игрок защищен от нее пока поэтому изменения происходят
                    //только в мультипликаторе
                {
                    if (counter > 0)
                    {
                        //обновление мультипликатора
                        UpdateMulti();
                    }
                    else
                    {
                        //Уменьшение мультипликатора так как прыжок на том же уровне
                        multiPl = ChangeMultiplicator(multiPl);
                    }
                }
            }
            else //Неопасные платформы
            {
                if (counter > 0)
                {
                    int addedScore;
                    if (collision.gameObject.tag == "Bonus")
                    {
                        addedScore = 2 * (int)Mathf.Pow(3, (counter)) * multiPl*powerLevel;
                    }
                    else
                    {
                        addedScore = (int)Mathf.Pow(3, (counter)) * multiPl*powerLevel;
                    }
                    UpdateMulti();
                    CreateFloatingScore(addedScore);
                }
                else 
                {
                    //Уменьшение мультипликатора так как прыжок на том же уровне
                    multiPl = ChangeMultiplicator(multiPl);
                }
            }
            //вывод мультипликатора в интерфейс
            subPlayer[2].text = "x" + multiPl;
            counter = 0;//после удара о перекрытие счетчик пролетов обнуляется
        }
    }

    void CreateFloatingScore(int newScore)
    {
        //print("Игрок находится в " + test.x + " пикселях слева, и в " + test.y + "пикселях от низа");
        GameObject go = Instantiate(prefabFloating);
        go.transform.SetParent(canvas);
        FloatingScore fs = go.GetComponent<FloatingScore>();
        //передаем значение в плавающий тест и его начальную позицию так как это позиция мяча
        fs.Init(newScore, Camera.main.WorldToScreenPoint(transform.position), utils, scorecounter);
    }

    //Метод который мотивирует игрока действовать быстрее. Тот кто спрыгивает с мультипликатором 7
    //и выше не просто обновляет мультипликацию до 7, а увеличивает ее на один
    void UpdateMulti()
    {
        if (multiPl >= 7) multiPl++;
        else multiPl = 7;
    }

    //Метод отвечает за индикацию крутости желтого шара
    void DisplayPower()
    {
        healthBarWidth = (powerCount / 10) * healthBarInitWidth;
        //Нужно запомнить это на будущее это удобный способ заполнения строки
        healthBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,healthBarWidth);
        subPlayer[6].text = scorecounter[Store.LANGUAGE % 2, 8] + ": " + powerLevel;
    }


    int ChangeMultiplicator (int oldMulti)
    {
        if (oldMulti>0)
        {
            return (oldMulti - 1);
        }
        return 0;
    }

    private void Update()
    {
        if (flyUp)
        {
            Vector3 currenPos;
            float u = (Time.time - startTime) / timeToOvercome;
            currenPos = (1 - u) * startPos + u * finishPos;
            transform.position = currenPos;
            if (u>=1)
            {
                flyUp = false;
                rigid.velocity = Vector3.zero;
            }
        }
        //тестирования счетчика падения на нижний уровень
        if (transform.position.y <= yLimit)
        {
            yLimit -= 2.5f;
            level++;
            counter++;
            subPlayer[0].text = scorecounter[Store.LANGUAGE%2,3]+": " + level;//Текущий этаж
        }
    }
}
