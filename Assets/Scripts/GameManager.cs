using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public static bool isProblemMode = false;

    [SerializeField] List<CharacterType> unlockedCharacters = new List<CharacterType>();
    public List<CharacterType> UnlockedCharacters => unlockedCharacters;
    [SerializeField] List<string> lockedCharacters = new List<string>();

    public List<Color32> PlayerColors = new List<Color32>();

    public string currentMapName { get; private set; } = "FirstMap";

    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    /// 게임오버 화면 테스트
    void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            var process = SceneManager.LoadSceneAsync("GameOver");
            process.completed += (AsyncOperation operation) =>
            {
                GameOverControl.instance.Init();
                return;
            };
        }
    }
    ///

    public IEnumerator TryMapSelect()
    {
        yield return new WaitForEndOfFrame();
        if (PlayerConfigManager.instance.PlayerConfigs.All(p => p.IsReady == true))
        {
            SceneManager.LoadScene("MapSelect");
        }
    }

    public void LoadMap(string sceneName)
    {
        var process = SceneManager.LoadSceneAsync($"{sceneName}");
        process.completed += (AsyncOperation operation) =>
        {
            PlayerSpawn.instance.SpawnPlayers();
            return;
        };
    }

    // 하연 : MapSelect 화면에서 바로 공격 모드로 진입하여 테스트할 수 있도록 추가
    public void LoadMapForTest(string sceneName, string actionMapName)
    {
        var process = SceneManager.LoadSceneAsync($"{sceneName}");
        process.completed += (AsyncOperation operation) =>
        {
            PlayerSpawn.instance.SpawnPlayersForTest(actionMapName);
            return;
        };
        
    }
    //

    public RuntimeAnimatorController GetCharAnimControl(string target_name)
    {
        for (int i = 0; i < unlockedCharacters.Count; i++) {
            if (unlockedCharacters[i].characterName == target_name)
                return unlockedCharacters[i].charAnimControl;
        }
        Debug.LogError($"Character Name '{target_name}' not found.");
        return null;
    }

    public void ChangeActionMaps(string map_name)
    {
        foreach (PlayerConfiguration playerConfig in PlayerConfigManager.instance.PlayerConfigs) {
            playerConfig.Input.SwitchCurrentActionMap(map_name);
        }
    }

    public IEnumerator StartProblemMode(string mapName, EnemyType enemyType, Vector3 playerPosition)
    {
        GameManager.isProblemMode = true;
        ChangeActionMaps("BattleMode");
        SetCurrentMapName(mapName);

        PlayerSpawn.instance.SetCirclePosition(playerPosition);
        PlayerSpawn.instance.SetCircleTransition(true);
        yield return new WaitForSeconds(1.5f);

        var process = SceneManager.LoadSceneAsync("ProblemMode");
        process.completed += (AsyncOperation operation) =>
        {
            ProblemManager.instance.Init(enemyType);
        };
    }

    public void ReturnToMapMode()
    {
        isProblemMode = false;
        ChangeActionMaps("MapControl");
    }

    public void ReturnToMapSelectMode()
    {
        isProblemMode = false;
        ChangeActionMaps("StartingMenu");
        PlayerConfigManager.instance.ResetAllPlayerConfigs();
        SceneManager.LoadScene("MapSelect");
    }

    public void RetryMapMode()
    {
        ReturnToMapMode();
        PlayerConfigManager.instance.ResetAllPlayerConfigs();
        LoadMap(currentMapName);
    }

    public void SetCurrentMapName(string map_name)
    {
        currentMapName = map_name;
    }
}

public class InputType
{
    public const string UP = "UP";
    public const string DOWN = "DOWN";
    public const string LEFT = "LEFT";
    public const string RIGHT = "RIGHT";
    public const string SOUTHBUTTON = "SOUTHBUTTON";
    public const string EASTBUTTON = "EASTBUTTON";
    public const string NORTHBUTTON = "NORTHBUTTON";
    public const string WESTBUTTON = "WESTBUTTON";
}

[System.Serializable]
public class CharacterType
{
    public string characterName;
    public RuntimeAnimatorController charAnimControl;
}
