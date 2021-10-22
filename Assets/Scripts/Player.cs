using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The only candidate for splitting into multiple classes, but I find it clearly written.
/// ������������ �������� �� ���������� �� ��������� �������, �� � ������ ��� ���� ����������.
/// Everything is clear to me, but it seems that I will have to separate it so that it is not cumbersome.
/// ��� ��� �������, �� ������ �������� ��� ���������, ����� �� �� ��� ����������.
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

    //���������� ������� ����� �� �����
    Rigidbody rigid;
    SubPlayer subPlayer;
    Controller controller;
    Transform canvas;//����� ��� �������� ������������ ������
    Scorecounter scorecounter;
    SimpleSpawner simpleSpawner;
    Utils utils;//������ �� �������
    new Renderer renderer;
    GameObject haloCarrier;
    //���������� ������� ����� ��� ���������� ������ � �����
    new Collider collider;
    TrailRenderer trailRenderer;

    //���� ��������� � �������������� ������� ���� ����� ������������ � ����������
    bool flyUp = false;
    Vector3 startPos, finishPos;
    float startTime;

    float yLimit = 7.5f;//������� ������ ���� ������� ���������� �������
    int level = 0;//������� ������
    int counter = 0;//������� ����������� ������ ��� �������
    int multiPl = 7;

    bool reinforced = false;//���������� �������� �� �������� ������ ������������ ������
    bool punchy = false;//���������� �������� �� �������� ������ ���������� ��������
    int brokenPlatform = 0;
    int tTime = 0;//������� ������� ��������
    int sTime = 0;//������� ������� ����������

    //���� ����� ��������� � ������� �������� ������� ����
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

        //���� ��������� � ���������� ��������           
        healthBarTransform = powerImage.GetComponent<RectTransform>();
        healthBarInitWidth = healthBarTransform.rect.width;
        DisplayPower();

        subPlayer[0].text = scorecounter[Store.LANGUAGE % 2, 3] + ": " + level;//��������� �������
        subPlayer[2].text = "x" + multiPl;//��������������

        cAnim = Camera.main.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Danger")
        {
            //������ ����� � ����� ������
            GameObject go = Instantiate(explodeParticle, transform.position, Quaternion.identity); ;
            Destroy(go, 1);
            if (reinforced)//�������� �������� ������� ���������� �����
            {
                simpleSpawner.DeleteEnemy(other.gameObject);
                Destroy(other.gameObject);
                subPlayer.GetSounds(1).Play();
                //��������� ������� �� ����������� �����
                CreateFloatingScore(25);
                ChangePower();
            }
            else//�� �������� �������� ���� � ���� �������������
            {
                cAnim.SetBool("Shake",true);
                eButton.SetActive(false);
                EndGameCalculations();
            }
        }
        else if (other.gameObject.tag == "Bonus")//����������� ��������� �����
        {
            RemoveBonusTrail(other.gameObject);//������ ����������� �������� ��� ������������ � ����� �������
            punchy = true;
            controller.Block(true);
            brokenPlatform = 0;

        }
        else if (other.gameObject.tag == "Bonus2")//����� �������� ������ ������� ��� ������
        {
            RemoveBonusTrail(other.gameObject);//������ ����������� �������� ��� ������������ � ����� �������

            reinforced = true;
            haloCarrier.SetActive(true);

            //������ �� ������������� ������ ��������
            if (tTime == 0)//������ ������� �� �������
            {
                Invoke("ChangeTime", 1);
            }

            tTime = 30;
            subPlayer[1].text = scorecounter[Store.LANGUAGE % 2, 4] + ": " + tTime;//������
            renderer.material = materials[1];
        }
        else if (other.gameObject.tag == "Bonus3")
        {
            RemoveBonusTrail(other.gameObject);//������ ����������� �������� ��� ������������ � ����� �������
            float t = Store.instanse.ROT_SPEED / 2;
            Store.instanse.ROT_SPEED = t;
            //������ �� ������������� ������ ��������
            if (sTime == 0)//������ ������� �� �������
            {
                Invoke("ChangeSlowingTimer", 1);
            }
            sTime = 30;
            subPlayer[5].text = scorecounter[Store.LANGUAGE % 2, 7] + ": " + sTime;//������
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

    //������������ ������������ ��� ���������� ������ � �����
    public bool GET_ENTRANCE
    {
        get
        {
            //������ �������� ���� ��������� ����� ������, ����� ����� ������ ����� 75% ���� �����
            if (opossiteTheEntrance && flyUp && (transform.position.y-startPos.y)>0.75f)
            {
               //flyUp = false;//�������������
               return true;
            }
            return false; ;
        }
    }

    //�������� ������ ��� � ���� �� ���� ��������� ������������ �� ������� ������
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

    //�������� ���������� ��� "������" ������
    void EndGameCalculations()
    {
        Destroy(gameObject);//���������� ������
        subPlayer[4].text = scorecounter[Store.LANGUAGE%2,2];//���������� ��������� ��������� (����� ���� �������� ����� ��� ��� ���� ����������)
        button.SetActive(true);//�������� ������ ��������
        controller.Block(true, 1);// ��������� ����������� �������� ����� � ������ ���� ������

        //������� ��������� ���������
        if (Store.DidItGetIntoTable(scorecounter.score))//��������� ��������� �������� ���������� � ������� �������
        {
            subPlayer[3].text = scorecounter[Store.LANGUAGE%2, 5];//�������� �����
            Store.instanse.SaveD();
        }
        else
        {
            subPlayer[3].text = scorecounter[Store.LANGUAGE%2, 6];//�������� �����
        }

        Store.instanse.SaveD();
    }
    void ChangeTime()
    {
            tTime--;
            subPlayer[1].text = scorecounter[Store.LANGUAGE%2, 4] + ": " + tTime;//������
            if (tTime==0)
            {
                reinforced = false;
                renderer.material = materials[0];
                haloCarrier.SetActive(false);
                subPlayer[1].text = "";//������ �� ����� ���� �� ������� ��� �����
                //�������� ������� ���� ������� ����
                powerCount = 0;
                DisplayPower();
            }
            else
            {
                if (tTime <= 5) subPlayer.GetSounds(4).Play();
                Invoke("ChangeTime", 1);
            }
    }

    //����� ������� ��������� ��������� ���� ������� ����, ������� ������ �� "��������" ������� ������ 
    void ChangePower()
    {
        if (powerCount==9)//������������ ����� ����������
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
        subPlayer[5].text = scorecounter[Store.LANGUAGE % 2, 7] + ": " + sTime;//������
        if (sTime == 0)
        {
            subPlayer[5].text = "";
            Store.instanse.ROT_SPEED = 1;
        }
        else 
        {
            //if (sTime <= 5) sounds[4].Play();//����� ���������� ��������� ����� ������ ������ �� ���������
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
                controller.Block(false);//������� ����������� �������������� �����
            }
        }
        else //�������� �������� ������������
        {
            //���� ����� �� ���������
            subPlayer.GetSounds(0).pitch = subPlayer.GetPitch(counter);
            subPlayer.GetSounds(0).Play();

            //���� ��������� � �������� �����
            GameObject go = Instantiate(subPlayer.GetBlut, collision.contacts[0].point,
                Quaternion.Euler(new Vector3(0, subPlayer.GetRot, 0)));
            go.transform.SetParent(collision.gameObject.transform);
            Destroy(go, 0.8f);

            //���� ������������� ���� ����� ������������
            flyUp = true;
            startPos = transform.position;
            float y = transform.position.y + height;
            finishPos = new Vector3(transform.position.x, y, transform.position.z);
            startTime = Time.time;

            if (collision.gameObject.tag == "Danger")
            {
                if (!reinforced)
                {
                    //�������� ������� ������ � �������� ���� 
                    go = Instantiate(explodeParticle, transform.position, Quaternion.identity);
                    Destroy(go, 1);
                    cAnim.SetBool("Shake", true);
                    EndGameCalculations();
                }
                else//������� ������� ���������, �� ����� ������� �� ��� ���� ������� ��������� ����������
                    //������ � ���������������
                {
                    if (counter > 0)
                    {
                        //���������� ���������������
                        UpdateMulti();
                    }
                    else
                    {
                        //���������� ��������������� ��� ��� ������ �� ��� �� ������
                        multiPl = ChangeMultiplicator(multiPl);
                    }
                }
            }
            else //��������� ���������
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
                    //���������� ��������������� ��� ��� ������ �� ��� �� ������
                    multiPl = ChangeMultiplicator(multiPl);
                }
            }
            //����� ��������������� � ���������
            subPlayer[2].text = "x" + multiPl;
            counter = 0;//����� ����� � ���������� ������� �������� ����������
        }
    }

    void CreateFloatingScore(int newScore)
    {
        //print("����� ��������� � " + test.x + " �������� �����, � � " + test.y + "�������� �� ����");
        GameObject go = Instantiate(prefabFloating);
        go.transform.SetParent(canvas);
        FloatingScore fs = go.GetComponent<FloatingScore>();
        //�������� �������� � ��������� ���� � ��� ��������� ������� ��� ��� ��� ������� ����
        fs.Init(newScore, Camera.main.WorldToScreenPoint(transform.position), utils, scorecounter);
    }

    //����� ������� ���������� ������ ����������� �������. ��� ��� ���������� � ���������������� 7
    //� ���� �� ������ ��������� �������������� �� 7, � ����������� �� �� ����
    void UpdateMulti()
    {
        if (multiPl >= 7) multiPl++;
        else multiPl = 7;
    }

    //����� �������� �� ��������� �������� ������� ����
    void DisplayPower()
    {
        healthBarWidth = (powerCount / 10) * healthBarInitWidth;
        //����� ��������� ��� �� ������� ��� ������� ������ ���������� ������
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
        //������������ �������� ������� �� ������ �������
        if (transform.position.y <= yLimit)
        {
            yLimit -= 2.5f;
            level++;
            counter++;
            subPlayer[0].text = scorecounter[Store.LANGUAGE%2,3]+": " + level;//������� ����
        }
    }
}
