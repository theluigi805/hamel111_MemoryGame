﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Probably one of the more varied scripts here is the Card script.
 * It keeps a reference to the MemoryGame.cs to check if a card is allowed to be flipped
 * each time a user selects a card. It also has an overriding Equals and GetHashCode method
 * as generated by Visual Studio to allow me to compare cards in MemoryGame to see if they match.
 * The update method may be hard to follow but I did my best to explain the thought process behind the flip animation.
 */

public class Card : MonoBehaviour
{
    // if the card has been selected (does not infer the state of isFlipping)
    public bool isFlipped;
    // if the card is in the flipping animation
    public bool isFlipping;
    // controls whether the card is in the process of [down,up]scaling
    public bool isRevealing;

    public Sprite blank;
    public Sprite icon;
    public string blockName;

    SoundManager player;

    GameObject MemoryGameControllerGO;
    MemoryGame GameControl;

    // Start is called before the first frame update
    void Start()
    {
        isFlipped = false;
        isFlipping = false;
        isRevealing = false;

        Button me = this.GetComponent<Button>();
        Image myImage = me.GetComponentsInChildren<Image>()[1];
        // store the card's true image in the icon sprite
        icon = myImage.sprite;
        // conceal the card's value with a blank
        myImage.sprite = blank;

        player = GameObject.Find("FlipPlayer").GetComponent<SoundManager>();
        if (player == null)
            Debug.Log("Error retrieving SoundPlayer - Card.cs");

        MemoryGameControllerGO = GameObject.Find("MemoryGameController");
        GameControl = MemoryGameControllerGO.GetComponent<MemoryGame>();
        //GameControl = GameObject.Find("MemoryGameController").GetComponent<MemoryGame>();

        if (GameControl == null)
        {
            Debug.Log("Error retrieving MemoryGameController - Card.cs");
            Application.Quit();
        }
    }

    // The most complicated method I've written in a while. It's split into three parts
    void Update()
    {
        // The escape opportunity -  which is to say that
        // if the card isn't in the midst of a flipping animation the update mustn't continue.
        if (!isFlipping)
            return;

        // The shrink, a.k.a
        if (!isRevealing) // approaching vertical line - 1->2 and 2->3
        {
            if (transform.localScale.x > 0.5)
            {
                Vector3 temp = transform.localScale;
                temp.x = 0.5f;
                transform.localScale = temp;
            }
            else
            {
                Vector3 temp = transform.localScale;
                temp.x = 0.1f;
                transform.localScale = temp;
                isRevealing = true;
                ToggleIcon();
            }
        }
        // The rise, a.k.a
        else // returning to a flat state - 3->4 and 4->5
        {
            if (transform.localScale.x < 0.5)
            {
                Vector3 temp = transform.localScale;
                temp.x = 0.5f;
                transform.localScale = temp;
            }
            else
            {
                Vector3 temp = transform.localScale;
                temp.x = 1f;
                transform.localScale = temp;
                isFlipping = false;
                isRevealing = false;
            }
        }
        // the numbers are the frames of the animation ->
        // 1 and 5 are the flat states, where the card is either belly up or face down
        // 2 and 4 are the half-width states
        // 3 is the sneaky state where the Icon changes
        // The card shrinks and grows to create the illusion of being flipped
    }

    // method to be called by the user when the card is clicked. It acts as a buffer to
    // simplify FlipCard and to prevent the user from performing unauthorized flips
    public void UserFlipCard()
    {
        if (!isFlipped && !GameControl.PauseAction)
        {
            // The card flip sound is only played when the user flips a card. If
            // this was in the FlipCard method it would play when flipped back aswell.
            player.PlaySound("Flip");

            FlipCard();
        }
    }

    // Called by UserFlipCard when allowed and MemoryGame when the cards are reversed after an incorrect match
    public void FlipCard()
    {
        isFlipping = true;

        isFlipped = !isFlipped;
    }

    // Called during the flipping animation when the icon is so small that the user will not perceive the change.
    private void ToggleIcon()
    {
        Button me = this.GetComponent<Button>();
        Image myImage = me.GetComponentsInChildren<Image>()[1];

        myImage.sprite = (myImage.sprite == blank) ? icon : blank;

    }

    // overriding Equals method and GetHashCode to allow easy comparison of cards 
    public override bool Equals(object obj)
    {
        var card = obj as Card;
        return card != null &&
                blockName == card.blockName;
    }

    public override int GetHashCode()
    {
        var hashCode = -1201133409;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(blockName);
        return hashCode;
    }
}
