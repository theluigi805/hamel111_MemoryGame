using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* MenuButtonController class is responsible for handling any GameObject attached UI.Button's i'm using to control
 * the loaded scene, volume, number of cards, etc. It is mainly driven by a MenuButtonAction function which has a 
 * Parameter passed from the inspector of the Button script.
*/

public class MenuButtonController : MonoBehaviour
{
    // A reference to the MenuManager who exists in this context only to transfer the number of pairs required from one scene to another.
    MenuManager manager;

    public void Awake()
    {
        manager = GameObject.Find("MenuManager").GetComponent<MenuManager>();
        if (manager == null)
            Debug.Log("Manager null.");
    }

    public void MenuButtonAction(int action)
    {
        switch (action)
        {
            case 1:
                SceneManager.LoadScene("StartScene");
                break;

            case 2:
                SceneManager.LoadScene("ChooseScene");
                break;

            case 3:
                // 6 pairs
                manager.PairCount = 6;
                SceneManager.LoadScene("GameScene");
                break;

            case 4:
                // 7 pairs
                manager.PairCount = 7;
                SceneManager.LoadScene("GameScene");
                break;

            case 5:
                // 8 pairs
                manager.PairCount = 8;
                SceneManager.LoadScene("GameScene");
                break;

            case 6:
                // 9 pairs
                manager.PairCount = 9;
                SceneManager.LoadScene("GameScene");
                break;

            case 7:
                // 10 pairs
                manager.PairCount = 10;
                SceneManager.LoadScene("GameScene");
                break;

            case 8:
                // change music
                GameObject.Find("MusicPlayer").GetComponent<MusicController>().ChangeSong();
                break;

            case 9:
                // doesn't change the current paircount, simply reloads the scene.
                SceneManager.LoadScene("GameScene");
                break;

            default:
                Application.Quit();
                break;
        }
    }

    public void ToggleMute()
    {
        // toggles player volume (the music player, to be more precise)
        AudioSource player = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
        player.mute = !player.mute;
    }
}
