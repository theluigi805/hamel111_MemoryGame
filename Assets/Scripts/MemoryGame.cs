using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* The script actually in charge of the game logic.
 * A list of cardPrefabs contains all of my prefabs, and cards will contain
 * the list of active cards on the board, but the cards are actually the GameObjects
 * that are wrapping the Button G.O. that has the Card.cs script on board. The reason for this
 * is so that their positions on the Grid will be maintained after the card has been disabled (matched).
 * Once cards is empty, the game is won.
 * */

public class MemoryGame : MonoBehaviour
{
    // Card Types is managed by the Inspector - it contains a list of all the possible card types.
    public List<GameObject> cardPrefabs;
    // The card types will be duped into this list and then distributed randomly into the Grid Layout on CardPanel
    public List<GameObject> cards;

    // Text object displaying the remaining card count
    public Text cardCount;

    // MemoryGame will use this to play hit, miss, win and lose sound effects.
    public SoundManager player;
    // A panel displayed when the game is done. It's text is determined by whether the game was won or lost.
    public GameObject EndPanel;

    // start time stores the time that the game began as reported by Time.time
    private float startTime;
    // the number of card pairs in play. controlled by MenuManager
    private int pairCount;

    // true when the cards are just being shown to the user (for one full second)
    // during this time, the Update method of this script is skipped each frame, and 
    // cards can not be flipped over
    public bool PauseAction = false;

    // list used in Update and CheckCards to store the cards being considered for a comparison
    // when it is > 2, PauseAction is triggered
    public List<Card> flippedCards;

    // The player's score; Initialized to 1000 and reduced by 40 for every incorrect guess.
    private short score;

    public short Score { get => score; set => score = value; }

    // Start is called before the first frame update
    void Start()
    {
        // Find the CardPanel. It's size changes across different pair counds
        GameObject cardPanel = GameObject.Find("CardPanel");

        if (cardPanel == null)
            Debug.Log("No panel found.");

        // We also need the SoundManager. This controls sound effects
        player = GameObject.Find("SoundPlayer").GetComponent<SoundManager>();

        if (player == null)
            Debug.Log("No player found.");

        // retrieve pair count from menumanager, which holds the player's desired value
        pairCount = GameObject.Find("MenuManager").GetComponent<MenuManager>().PairCount;

        // Get paircount from 
        switch (pairCount)
        {
            case 6:
                BuildCardPanel(cardPanel, new Vector2(0, -45), new Vector2(550, 296), new Vector2(1, 1), new RectOffset(23, 0, 25, 0), new Vector2(76, 120));
                break;
            case 7:
                BuildCardPanel(cardPanel, new Vector2(0, -45), new Vector2(640, 296), new Vector2(1, 1), new RectOffset(23, 0, 25, 0), new Vector2(76, 120));
                break;
            case 8:
                BuildCardPanel(cardPanel, new Vector2(0, -45), new Vector2(720, 296), new Vector2(1, 1), new RectOffset(23, 0, 25, 0), new Vector2(76, 120));
                break;
            case 9:
                BuildCardPanel(cardPanel, new Vector2(0, -33), new Vector2(500, 346), new Vector2(1, 1), new RectOffset(13, 0, 16, 0), new Vector2(70, 98));
                break;
            case 10:
                BuildCardPanel(cardPanel, new Vector2(0, -25), new Vector2(455, 450), new Vector2(0.8f, 0.8f), new RectOffset(13, 0, 16, 0), new Vector2(77, 98));
                break;
        }

        // Will contain two of every card prefab. These will be instantiated and added to cards.
        List<GameObject> tempCards = new List<GameObject>(pairCount * 2);

        for (int i = 0; i < pairCount; i++)
        {
            // Add each card twice, to this list.
            tempCards.Add(cardPrefabs[i]);
            tempCards.Add(cardPrefabs[i]);
        }

        if (tempCards.Count != pairCount * 2)
        {
            Debug.Log("Only " + tempCards.Count + " cards written to cards list.");
        }

        // A list to hold the Instantiated Card GO's
        cards = new List<GameObject>();
        int size = tempCards.Count;

        // And add all the cards to the CardPanel
        for (int i = 0; i < size; i++)
        {
            // randomly pick a card
            int rand = Random.Range(0, size - (1 + i));
            GameObject newCard = Instantiate(tempCards[rand]);

            // move it to the cardPanel
            newCard.transform.SetParent(cardPanel.transform, false);

            // add it to the cards list (we will use this to address cards in Update)
            cards.Add(newCard);

            // and remove the prefab from the list, since we don't want it to be picked more than once
            // noting that there is 2 of each prefab in the list.
            tempCards.RemoveAt(rand);
        }

        // it's a surprise tool we'll use for later
        flippedCards = new List<Card>();

        // score initialized to a thousand.
        score = 1000;

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // If the cards are already on display for this frame, get out
        if (PauseAction) return;

        // volatile cards list
        flippedCards = new List<Card>();

        List<GameObject> tempCards = new List<GameObject>(cards);

        // counting up when isFlipped == true
        // also store them into a list, since we'll need to compare them with Equals()

        // for loop iterating through card GOs. A tempCards list is used since we need to be able
        // to remove null cards from cards, but modifying a list while iterating through it is forbidden
        foreach (GameObject cardGO in tempCards)
        //for (int i = 0; i < cards.Count; i++)
        {
            // get access to Card card script
            Card card = cardGO.GetComponentInChildren<Card>();
            // access card field to check if it is flipped

            if (card == null)
            {
                Debug.Log("Null Card.");
                cards.Remove(cardGO);
                Debug.Log("Null Card Removed.");
                continue;
            }

            if (card.isFlipped)
                flippedCards.Add(card);
        }

        // if count > 1, do checking logic
        if (flippedCards.Count > 1)
        {
            // two cards flipped, wait for one second before checking if they're the same
            PauseAction = true;
            // gives the user more time to look at the cards before they are flipped, in case they are matching
            Invoke("CheckCards", 1f);
        }

        cardCount.text = "Cards Remaining: " + cards.Count;

        // No more cards!
        if (cards.Count == 0)
        {
            // game win!
            Text EndText = EndPanel.GetComponentInChildren<Text>();
            int playTime = (int)(Time.time - startTime);
            EndText.text = "You Won in\n" + (playTime) + " seconds!";
            EndPanel.SetActive(true);

            PauseAction = true;

            player.PlaySound("Win");
        }
        // You ran out of score :(
        else if (score <= 0)
        {
            // game lose
            Text EndText = EndPanel.GetComponentInChildren<Text>();
            int playTime = (int)(Time.time - startTime);
            EndText.text = "You Lost in\n" + (playTime) + " seconds.";
            EndPanel.SetActive(true);

            PauseAction = true;

            player.PlaySound("Lose");
        }
    }

    private void CheckCards(List<Card> flippedCards)
    {
        Debug.Log("Entering checkCards");

        // Minor checking to prevent a null list from getting in and spewing errors (better safe than shut down)
        if (flippedCards.Count == 0)
        {
            PauseAction = false;
            return;
        }

        foreach (Card card in flippedCards)
        {
            if (card == null) flippedCards.Remove(null);
        }

        // if, after removing null cards, there actually isn't enough to check the cards, get out
        if (flippedCards.Count < 2) return;

        if (flippedCards[0].Equals(flippedCards[1]))
        {
            Debug.Log("Equals cards found.");
            // the cards are identical, disable the button objects
            // Card attached to CardButton, whose parent (GrassCard,StoneCard,etc)
            // stays in the grid to maintain neat alignment

            flippedCards[0].gameObject.SetActive(false);
            flippedCards[1].gameObject.SetActive(false);

            player.PlaySound("Hit");
        }
        else
        {
            // the cards are not matching, they must be turned over.
            flippedCards[0].FlipCard();
            flippedCards[1].FlipCard();

            // and the player loses 40 points.
            score -= 40;
            Text scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
            scoreText.text = "Score: " + score;

            player.PlaySound("Miss");
        }

        // remove these from the parameter cards list, since they mustn't be considered again
        flippedCards.Clear();

        // allow for cards to be reassessed
        PauseAction = false;
    }

    // Overloaded method calls the main CheckCards with the local flippedCards list
    private void CheckCards()
    {
        CheckCards(flippedCards);
    }

    // A function I designed to modify the given cardPanel to fit the correct size for the number of cards.
    private void BuildCardPanel(GameObject cardPanel, Vector2 pos, Vector2 size, Vector2 scale, RectOffset padding, Vector2 cellSize)
    {
        RectTransform dimensions = cardPanel.GetComponent<RectTransform>();
        if (dimensions == null)
            Debug.Log("Null rect");

        dimensions.anchoredPosition = pos;
        dimensions.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        dimensions.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        dimensions.transform.localScale = new Vector3(scale.x, scale.y, 1);

        GridLayoutGroup grid = cardPanel.GetComponent<GridLayoutGroup>();
        if (grid == null)
            Debug.Log("Null grid");
        grid.padding = padding;
        grid.cellSize = cellSize;
    }
}
