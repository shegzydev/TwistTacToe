using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] string defaultScreen;

    MScreen[] screens;
    MScreen last;

    void Awake()
    {
        screens = FindObjectsByType<MScreen>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ActivateScreen(defaultScreen);
    }

    static ScreenManager Instance => FindFirstObjectByType<ScreenManager>();

    static Stack<MScreen> pages = new();

    public static void Reset()
    {
        pages.Clear();
    }

    public static void ActivateScreen(string id)
    {
        try
        {
            var screen = Instance.screens.First(x => x.id.ToLower() == id.ToLower());
            var currScreen = screen;

            List<MScreen> mScreens = new();
            while (currScreen.transform.parent != null && currScreen.transform.parent.TryGetComponent<MScreen>(out var mScreen))
            {
                mScreens.Add(mScreen);
                currScreen = mScreen;
            }

            foreach (var item in Instance.screens)
            {
                if (mScreens.Contains(item))
                {
                    if (!item.gameObject.activeInHierarchy)
                        item.gameObject.SetActive(true);
                }
                else
                {
                    if (screen.isOverlay) continue;
                    item.gameObject.SetActive(false);
                }
            }

            screen.gameObject.SetActive(true);
            Instance.last = screen;

            if (!pages.Contains(screen)) pages.Push(screen);
        }
        catch (InvalidOperationException)
        {
            Debug.Log($"Page [{id}] doesn't exist");
        }
    }

    public static void Back()
    {
        if (pages.Count < 2) return;

        pages.Pop();

        if (!Instance.last.isOverlay)
        {
            ActivateScreen(pages.Peek().id);
        }
        else
        {
            Instance.last.gameObject.SetActive(false);
            Instance.last = pages.Peek();
        }
    }
}