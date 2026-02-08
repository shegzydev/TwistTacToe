using UnityEngine;
using UnityEngine.UI;

class Back : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>() ?? gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => ScreenManager.Back());
    }

    void Update()
    {

    }
}