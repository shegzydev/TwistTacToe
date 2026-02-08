using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class SceneManagement : MonoBehaviour
{
    public static SceneManagement Instance;

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
    }

    Dictionary<string, int> scenes = new();

    public void AddScene(string name, int index)
    {
        scenes.TryAdd(name, index);
    }

    public void Load(string name)
    {
        if (scenes.TryGetValue(name, out int index))
        {
            SceneManager.LoadScene(index);
        }
        else
        {
            Debug.LogError($"Scene {name} not registered");
        }
    }
}