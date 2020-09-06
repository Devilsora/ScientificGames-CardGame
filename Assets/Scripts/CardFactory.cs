using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFactory : MonoBehaviour
{
    public List<Sprite> cardImages;     //list of all the card images in the order they were dragged into the inspector 
    public Sprite cardBack;             //single sprite used as the card back for every card
    public GameObject cardPrefab;       //card prefab to initialize

    public static CardFactory instance; //instance to allow other functions to access card factory functions
 
    void Start()
    {
        //makes sure this is the only instance existing
        if(instance == null)
        {
            Debug.Log("Instance is null");
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //create a new card given a suit and a value
    //returns a ref to the new card
    public GameObject CreateCard(Card.Suit s, int value)
    {
        Debug.Log("Card suit in creation: " + s);

        //create a new card and change its' values
        GameObject newCard = Instantiate(cardPrefab);

        int spriteIndex = (((int)s) * Card.MAX_CARD_VALUE) + (value - 1);
        newCard.GetComponent<Card>().cardSuit = s;
        newCard.GetComponent<Card>().val = value;

        //add the images to the cards (flipX on the front since the sprite was rendering it the other way)
        newCard.GetComponent<Card>().front.GetComponent<SpriteRenderer>().sprite = cardImages[spriteIndex];
        newCard.GetComponent<Card>().front.GetComponent<SpriteRenderer>().flipX = true;
        newCard.GetComponent<Card>().back.GetComponent<SpriteRenderer>().sprite = cardBack;

        newCard.GetComponent<Card>().ChangeName();

        Debug.Log("Card suit after creation: " + newCard.GetComponent<Card>().cardSuit);
        
        return newCard;
    }
}
