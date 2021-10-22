using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс управляет подпрыгиванием по вертикали движущегося объекта к которому подключен (бонуса, врага).
/// А так же его вращением вокруг своей оси
/// </summary>
public class SimpleMovement : MonoBehaviour
{
    Vector3 startPos, finishPos;
    float startMoving;
    [SerializeField]
    float movingDuration=1.5f;
    float direction = -1;
    [SerializeField]
    Vector3 rot = new Vector3(45, 45, 0);
    
    void Start()
    {
        Set();
    }

    
    void Update()
    {
        float u = (Time.time - startMoving) / movingDuration;
        Vector3 currentPos = (1 - u) * startPos + u * finishPos;
        transform.localPosition = currentPos;
        if (u >= 1) Set();

        transform.Rotate(rot*Time.deltaTime);
    }

    void Set()
    {
        startMoving = Time.time;
        startPos = transform.localPosition;
        finishPos = new Vector3(transform.localPosition.x,
            transform.localPosition.y+direction*1f, transform.localPosition.z);
        direction *= -1;
    }
}
