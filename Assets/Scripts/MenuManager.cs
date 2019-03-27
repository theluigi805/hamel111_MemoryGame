using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Simple class maintains the desired pairCount across scene changes. */

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    private int pairCount;
    public int PairCount { get; set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
