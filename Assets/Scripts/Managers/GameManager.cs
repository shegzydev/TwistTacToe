using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManagement.Instance.AddScene("menu", 0);
        SceneManagement.Instance.AddScene("maingame", 1);
        
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("click");
            });
        }
    }

    void Update(){

    }
}