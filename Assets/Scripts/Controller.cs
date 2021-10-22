using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс отвечающий за основное управление. Вращение главной башни по часовой и против часовой стрелки
/// </summary>
public class Controller : MonoBehaviour
{
    public float turnSpeed = 50;
    public AudioSource explodeSound;
    bool controllBlokced = false;
    Player player;

    float startTime;
    float hRot;
    bool buttonRot = false;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (!controllBlokced)
        {
            if (buttonRot)
            {
                //print("ggg");
                transform.Rotate(Vector3.up * Time.deltaTime * hRot);
                if ((Time.time - startTime) > 0.5f) buttonRot = false;
            }
            else
            {
                float horizontalInput = Input.GetAxis("Horizontal");
                //print(horizontalInput);
                if (horizontalInput != 0)
                {
                    transform.Rotate(Vector3.up * Time.deltaTime * horizontalInput * turnSpeed);
                    startTime = Time.time - 1;//обнуление поворота кнопкой на экране
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) EnterEntrance();
    }

    public void Block (bool block, int e=0)
    {
        controllBlokced = block;
        if (e==1) explodeSound.Play();
    }

    public void RotateAssistant()
    {
        StartCoroutine(HalfRotate());
    }

    IEnumerator HalfRotate()
    {
        float elapsedTime = 0;
        while (elapsedTime<0.8f)
        {
            elapsedTime += Time.deltaTime;
            transform.Rotate(Vector3.up * Time.deltaTime * 180/0.8f);
            yield return null;
        }
        //print("2");
        player.IncreaseAssistant();
    }

    public void EnterEntrance()
    {
        if (!controllBlokced && player.GET_ENTRANCE)
        {
            Block(false);
            player.DecreaseAssistant();
        }
    }

    public void ShiftTower(float hInput)
    {
        startTime = Time.time;
        hRot = hInput;
        buttonRot = true;
    }
}
