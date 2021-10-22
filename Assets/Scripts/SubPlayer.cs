using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubPlayer : MonoBehaviour
{
    [SerializeField]
    GameObject[] prefabBlut;

    [SerializeField]
    Text[] texts;

    [SerializeField]
    AudioSource[] sounds;//все виды звуков связанные с игроком

    //Варианты поворота пятна отпечатка при ударе желтого шара о платформу
    float[] yRot = { 0, 45, 90, 135, 180, 225, 270, 315 };

    //Варианты уровня питча для звука удара об платформу
    float[] pitchLevel = { 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, };

    public GameObject GetBlut
    {
        get
        {
            return prefabBlut[Random.Range(0, prefabBlut.Length)];
        }
    }

    public Text this [int i]
    {
        get
        {
            return texts[i];
        }
    }

    public AudioSource GetSounds (int i)
    {
        return sounds[i];
    }

    public float GetRot
    {
        get
        {
            return yRot[Random.Range(0,yRot.Length)];
        }
    }

    public float GetPitch (int i)
    {
        if (i > (pitchLevel.Length - 1)) return pitchLevel[pitchLevel.Length - 1];
        return pitchLevel[i];
    }

}
