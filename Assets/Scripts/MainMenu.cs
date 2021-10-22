using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

/// <summary>
/// Класс главного меню, отображает данные интерфейса. Содержит команды основных кнопок.
/// Текстовые переменные можно было бы объединить в один массив, но тогда было бы сложнее их назначать и я 
/// решил от этого отказаться в пользу ясности
/// </summary>
public class MainMenu : MonoBehaviour
{
    public InputField inputField;
    public Text leadersTitleText;
    public Text leaderNamesText;
    public Text leaderScoresText;
    public Text pretenderText;
    public Text startButtonText;
    public Text exitButtonText;
    public Text languageText;
    public Text inputFieldText;
    public Text rulesText;
    public Text rulesButtonText;
    public Text backBText;
    public Text backB2Text;
    public Text panelTitle;
    public Text creditButtonText;
    public GameObject [] panel;
    public GameObject gameTitle;

    //Статический текст для перевода на русский/английский
    string[,] constantText =
    {
        {"Leaderboard:","Challenger","Start Game","Русский","Quit Game","Enter your name...","Rules",
            "The goal of the game is to score as many points as possible and break the highest record " +
                "set by the champion.\n\nPoints are earned by jumping the yellow ball on the platform of the lower floors." +
                " Jumping on the same floor does not give points. Jumping over several floors is worth more points at once" +
                " than jumping over one floor. Landing on yellow platforms doubles the result for the current jump.\n\n" +
                "The decision should be made as soon as possible, as multiple jumps on the same level gradually decreases" +
                " the multiplier from 7 to 0. More than seven jumps on one floor leads to the fact that you will not" +
                " receive points at all.\n\nRed means danger when the yellow ball collides with red objects, the game ends.\n\n" +
                "At the lower levels, various bonuses appear that give the player's ball different temporary boosts." +
                " Try them yourself.\n\nControl is carried out with the arrow keys left and right or with the keys \"A\"" +
                " and \"D\".\n\nBecome our new champion!",
            "Back","Rules of the game","Credits"},
        {"Лучшие игроки:", "Претендент", "Начать игру", "English", "Выйти из игры","Введите свое имя...",
            "Правила",
            "Цель игры набрать как можно больше очков и побить наивысший рекорд, установленный чемпионом.\n\n" +
            " Очки набираются при прыжке желтого шара на платформы нижних этажей. Прыжки на одном и том же этаже" +
            " не дают очков. Прыжок через несколько этажей сразу приносит больше очков чем прыжок через один этаж." +
            " Приземление на желтые платформы удваивает результат за текущий прыжок.\n\nРешение нужно принимать как" +
            " можно скорее так, как многократное прыганье на одном этаже постепенно снижает величину" +
            " мультипликатора с 7 до 0. Больше семи прыжков на одном этаже приводит к тому что вы не " +
            "получите очков вовсе.\n\nКрасный цвет значит опасность при столкновении желтого шара с " +
            "красными объектами игра завершается.\n\nНа нижних уровнях появляются разные бонусы, которые" +
            " дают шару игрока разные временные усиления. Попробуйте их действие самостоятельно.\n\n"+
            "Управление осуществляется стрелками курсора влево и вправо или клавишами \"A\" и \"D\".\n\n" +
            " Станьте нашим новым чемпионом!"
        ,"Назад","Основные правила","Благодарности"}
    };

    int languageNumber = 0;
    int panelNumber;

    private void Awake()
    {
        inputField.onEndEdit.AddListener(ChangeName);
    }

    void Start()
    {
        languageNumber = Store.LANGUAGE;
        Display();
    }

    public void StartGame()
    {
        //print(Store.PLAYER_NAME);
        if (Store.PLAYER_NAME != null && Store.PLAYER_NAME != "")
        {
            Store.LANGUAGE = languageNumber;
            SceneManager.LoadScene(1);
        }
    }

    public void ChangeLanguage()
    {
        languageNumber++;
        Display();
    }

    public void AuxiliaryPanel(int n)
    {
        switch (n)
        {
            //включаем вспомогательную панель
            case 0:
            case 1:
                panelNumber = n;
                gameTitle.SetActive(false);
                panel[panelNumber].SetActive(true);
                break;
            //выключаем вспомогательную панель
            default:
                panel[panelNumber].SetActive(false);
                gameTitle.SetActive(true);
                break;
        }
    }

    

    public void Exit()
    {

#if UNITY_EDITOR

        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player
#endif
        Application.Quit();
    }

    public void ChangeName(string name)
    {
        //print("name added " + name);
        Store.PLAYER_NAME = name;
        pretenderText.text = constantText[languageNumber % 2, 1]+": " + name;
    }



    public void Display()
    {
        //Делаем замену текстовых данных в зависимости от выбранного языка
        leadersTitleText.text = constantText[languageNumber % 2, 0];
        pretenderText.text = constantText[languageNumber % 2, 1] + ": " + Store.PLAYER_NAME;
        leaderNamesText.text = Store.LEADERS;
        leaderScoresText.text = Store.LEADER_SCORES;
        startButtonText.text = constantText[languageNumber % 2, 2];
        languageText.text = constantText[languageNumber % 2, 3];
        exitButtonText.text = constantText[languageNumber % 2, 4];
        inputFieldText.text = constantText[languageNumber % 2, 5];
        rulesButtonText.text = constantText[languageNumber % 2, 6];
        rulesText.text = constantText[languageNumber % 2, 7];
        backBText.text = constantText[languageNumber % 2, 8];//кнопка возврата первой панели
        backB2Text.text = constantText[languageNumber % 2, 8];//кнопка возврата второй панели
        panelTitle.text = constantText[languageNumber % 2, 9];
        creditButtonText.text = constantText[languageNumber % 2, 10];
    }
}
