using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
///  лассс который сохран€ет и загружает данных на диск и с диска.
/// ѕереносит ключевую информацию между сценами как то им€ текущего игрока, рекорд, €зык интерфейса.
/// «десь также хранитс€ глобальна€ переменна€ скорости перемещений врагов, платформ и бонусов
/// </summary>

public class Store : MonoBehaviour
{
    public MainMenu mainMenu;

    public static Store instanse;
    static string playerName;
    static string leaderNames;
    static string leadersScore;
    string path;

    //ѕредустановленные данные на случай если игра запускаетс€ впервые
    string[] leaderNamesList = { "Nick", "David", "Jana", "Jacob", "Sergey" };
    int[] leaderScoresList = { 1000, 800, 600, 400, 200 };
    int language;

    float rotSpeed = 1;

    public float ROT_SPEED
    {
        get
        {
            return instanse.rotSpeed;
        }
        set
        {
            instanse.rotSpeed = value;
        }
    }

    public static int LANGUAGE
    {
        get
        {
            return instanse.language;
        }
        set
        {
            instanse.language = value;
        }
    }

    public static int HIGHSCORE
    {
        get
        {
            return instanse.leaderScoresList[0];
        }
    }


    void Awake()
    {
        if (instanse == null)
        {
            instanse = this;
            DontDestroyOnLoad(gameObject);
            path = Application.persistentDataPath + "/savefile.json";
            //Ѕазовые данные на случай если нет сохраненных
            leaderNames = CreateLeadersList(leaderNamesList);
            leadersScore = CreateLeadersScore(leaderScoresList);
        }
        LoadData();
    }

    public static string PLAYER_NAME
    {
        get
        {
            return playerName;
        }
        set
        {
            playerName = value;
        }
    }

    public static string LEADERS
    {
        get
        {
            return leaderNames;
        }
        set
        {
            leaderNames = value;
        }
    }

    public static string LEADER_SCORES
    {
        get
        {
            return leadersScore;
        }
        set
        {
            leadersScore = value;
        }
    }

    public void LoadData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            //ќбновл€ем содержимое полей
            playerName = data.currentPlayerName;
            leaderNames = CreateLeadersList(data.leaderNames);
            leadersScore = CreateLeadersScore(data.leaderScores);
            language = data.currentLanguage;
            //ќбновл€ем содержимое массивов
            leaderNamesList = data.leaderNames;
            leaderScoresList = data.leaderScores;
            
            mainMenu.Display();
        }
    }

    //»з списка строк делает одну строку дл€ вывода в таблицу лидеров
    static string CreateLeadersList(string [] ldrs)
    {
        string txt = "";
        for (int i=0; i<ldrs.Length-1;i++)
        {
            txt = txt + ldrs[i] + "\n";
        }
        txt += ldrs[ldrs.Length - 1];
        return txt;
    }

    //»з списка чисел делает строкку дл€ вывода в таблицу лидеров
    static string CreateLeadersScore(int[] score)
    {
        string txt = "";
        for (int i = 0; i < score.Length-1; i++)
        {
            txt = txt + score[i] + "\n";
        }
        txt += score[score.Length - 1];
        return txt;
    }

    
    public void SaveD()
    {
        SaveData sd = new SaveData();
        sd.leaderNames = leaderNamesList;
        sd.leaderScores = leaderScoresList;
        sd.currentPlayerName = playerName;
        sd.currentLanguage = language;
        string json = JsonUtility.ToJson(sd);
        File.WriteAllText(path, json);
    }


    [SerializeField]
    class SaveData
    {
        public int [] leaderScores;
        public string [] leaderNames;
        public string currentPlayerName;
        public int currentLanguage;
    }

    public static bool DidItGetIntoTable(int cScore)
    {
        //провер€ем больше ли текущие очки чем минимальные в таблице рекордов
        bool insideLeaderBoard = false;
        List<int> tScore = new List<int>();//создаем временный спискок очков
        List<string> tNames = new List<string>();//создаем временый список имен

        for (int i=0; i < instanse.leaderScoresList.Length;i++)
        {
            //текущие очки больше провер€емой строки списка лидеров и мы еще не в списке лидеров
            if (cScore> instanse.leaderScoresList[i] && !insideLeaderBoard)
            {
                tScore.Add(cScore);
                tNames.Add(playerName);
                insideLeaderBoard = true;//защите от включени€ очков на нижнюю позицию
                i--;//вернуть счетчик на цикл назад
            }
            //текущие очки больше провер€мой строки списка лидеров, но мы уже в списке
            else if (cScore>instanse.leaderScoresList[i] && insideLeaderBoard)
            {
                tScore.Add(instanse.leaderScoresList[i]);
                tNames.Add(instanse.leaderNamesList[i]);
            }
            else//текущие очки просто меньше
            {
                tScore.Add(instanse.leaderScoresList[i]);
                tNames.Add(instanse.leaderNamesList[i]);
            }
        }

        if (insideLeaderBoard)
        {
            //копируем в ключевые массивы п€ть верхних строк
            for (int i=0; i<instanse.leaderScoresList.Length; i++)
            {
                instanse.leaderScoresList[i] = tScore[i];
                instanse.leaderNamesList[i] = tNames[i];
            }
            leaderNames = CreateLeadersList(instanse.leaderNamesList);
            leadersScore = CreateLeadersScore(instanse.leaderScoresList);
        }
        return insideLeaderBoard;
    }

}
