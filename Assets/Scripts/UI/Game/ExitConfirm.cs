using UnityEngine;
using UnityEngine.UI;

public class ExitConfirm : MonoBehaviour
{
    [SerializeField] Button exitButton;

    void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            SceneManagement.Instance.Load("menu");
        });
    }

    void Update()
    {
        
    }
}
