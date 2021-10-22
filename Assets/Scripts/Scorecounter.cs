using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Класс отображает в интерфейсе набранные очки и рекордные очки, а так же ведет счет набранным очкам
/// </summary>
public class Scorecounter : MonoBehaviour
{
    //Разбиение текста и цифровых значений нужно для того чтобы при переводе текст русский/английский
    //форматирование оставалось красивым
    public Text scoreTextLeft;
    public Text highScoreTextLeft;
    public Text scoreTextRight;
    public Text highScoreTextRight;
    [SerializeField]
    Text restartName;
    public int score {get; private set;}

    string[,] textConstants = { { "Score", "HighScore", "Game Over!", "Floor", "Coolness Timer","You have set a new record and got into the leaderboard","Unfortunately,\nyou did not set a new record and did not make it to the leaderboard","Deceleration Timer", "Coolness", "Restart" },
                                { "Очки","Рекорд", "Игра окончена!","Этаж","Таймер крутости","Вы установили новый рекорд и попали в таблицу лидеров","К сожалению,\nвы не установили новый рекорд и не попали в таблицу лидеров","Таймер замедления", "Крутость", "Перезапуск"} };


    private void Start()
    {
        highScoreTextLeft.text = textConstants[Store.LANGUAGE%2, 1] + ":";
        scoreTextLeft.text = textConstants[Store.LANGUAGE%2, 0] + ":";
        highScoreTextRight.text = ""+Store.HIGHSCORE;
        scoreTextRight.text = "0";
        restartName.text = textConstants[Store.LANGUAGE % 2, 9];
        
    }
    public void AddScore(int nS)
    {
        score += nS;
        scoreTextRight.text = "" + score;
    }

    public string this [int x, int y]
    {
        get
        {
        return textConstants[x,y];
        }
    }

}
