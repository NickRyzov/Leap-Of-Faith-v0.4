using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����� �������� �� �������� ��������� (����� ��� ������) ������ ������� �����. ����� ������������ � ���������, ������ ��� �����
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
            //Store.instanse.ROT_SPEED ����������� �������� �������� �� �������� ������ ����� ������
            //(�������� ��������� �������� �������� ������� � ������)
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime*Store.instanse.ROT_SPEED);
        }
    }
}
