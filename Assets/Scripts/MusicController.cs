using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/* The MusicController script runs on the MusicPlayer and simply keeps a list of the songs that can play in the background.
 * It uses the singleton pattern to insure that when returning to the main menu a duplicate is not created.
 */

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    // This field will be managed through the Inspector
    public List<AudioClip> playlist;
    public AudioSource player;
    int trackNumber = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();

        trackNumber = Random.Range(0, playlist.Count - 1);
        ChangeSong(trackNumber);
    }

    // Keeps music playing, thusly when a song ends
    void Update()
    {
        if (player.isPlaying)
            return;

        ChangeSong();
        player.Play();
    }

    // sets the current track to the track at the index of playlist specified by track
    public void ChangeSong(int track)
    {
        player.clip = playlist[track];
    }

    // overloaded method picks the next song in the list
    public void ChangeSong()
    {
        // simple ternary to advance or wrap around
        trackNumber = (trackNumber + 1 < playlist.Count) ? trackNumber + 1 : 0;

        ChangeSong(trackNumber);
        player.Play();
    }
}
