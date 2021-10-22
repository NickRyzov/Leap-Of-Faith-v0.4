using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс отвечает за вращение платформы (врага или бонуса) вокруг главной башни. Класс подключается к платформе, бонусу или врагу
/// </summary>
public class PlatformRotate : MonoBehaviour
{
    bool rotate = false;
    [SerializeField]
    float rotateSpeed;

    public void DelayedStart(bool wouldRotate, float s)
    {
        rotate = wouldRotate;
        rotateSpeed = s;
    }

    
    void Update()
    {
        if (rotate)
        {
            //Store.instanse.ROT_SPEED можификатор скорости вращения он меняется третим типом бонуса
            //(временно замедляет движение платформ бонусов и врагов)
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime*Store.instanse.ROT_SPEED);
        }
    }
}
