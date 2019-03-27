using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* sound manager is a component of the audioSource responsible for card flips AND the AudioSource for the other sounds.
 * They are on separate sources so that the sound of flipping a card will not interrupt the miss or hit sounds, when
 * a player is playing quickly.
*/

public class SoundManager : MonoBehaviour
{
    public static SoundManager flipInstance;
    public static SoundManager otherInstance;
    public AudioSource player;
    public AudioSource music;

    public AudioClip hit;
    public AudioClip fail;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip flip;

    private void Awake()
    {
        // Singleton pattern protocol modified to allow two objects with this script running and no more.
        if (GameObject.Find("SoundPlayer") == this.gameObject)
        {
            if (otherInstance == null)
                otherInstance = this;
            else if (otherInstance != null)
                Destroy(gameObject);
        }
        else if (GameObject.Find("FlipPlayer") == this.gameObject)
        {
            if (flipInstance == null)
                flipInstance = this;
            else if (flipInstance != null)
                Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "Hit":
                player.clip = hit;
                break;
            case "Miss":
                player.clip = fail;
                break;
            case "Win":
                player.clip = win;
                // mute the music
                music.mute = true;
                // and unmute after 10 seconds
                Invoke("Resume", 10);
                break;
            case "Lose":
                player.clip = lose;
                // mute the music
                music.mute = true;
                // and unmute after 6 seconds
                Invoke("Resume", 6);
                break;
            case "Flip":
                player.clip = flip;
                break;
        }

        player.Play();
    }

    // Resume the playing music on the MusicPlayer
    private void Resume()
    {
        music.mute = false;
    }
}
