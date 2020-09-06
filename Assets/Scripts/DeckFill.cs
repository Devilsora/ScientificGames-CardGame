using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckFill : MonoBehaviour
{
    public const int DECK_SIZE = 52;    //constant deck size

    //lists to hold the special case cards
    public List<GameObject> specialHearts = new List<GameObject>();
    public List<GameObject> specialAces = new List<GameObject>();
    
    //creates the deck on start
    void Start()
    {
        CreateDeck();   
    }

    //creates a new deck with the given requirements of 3x appearance of Ace of Spades, 2x appearance of Hearts cards
    public void CreateDeck()
    {
        //positional modifier for the cards so they look like they're an actual stack vs. all congregating in 1 place
        float stack_modifier = 0.01f;

        //loop through each of the suits
        for (int i = 0; i < (int)Card.Suit.MAX_SUITS; i++)
        {
            Debug.Log("Current suit: " + (Card.Suit)i);

            //loop through value 1 to the max card value
            for (int j = 1; j <= Card.MAX_CARD_VALUE; j++)
            {
                //create a new card using the card factory
                Debug.Log("Current suit added: " + (Card.Suit)i);
                GameObject newCard = CardFactory.instance.CreateCard((Card.Suit)i, j);

                //place the card on the stack 
                newCard.transform.SetParent(this.transform);
                newCard.transform.localPosition = new Vector3(0, -transform.localPosition.y, transform.localPosition.z + stack_modifier);
                newCard.transform.localEulerAngles = Vector3.zero;

                //if it's a special case, add it to the list of special case cards
                if (newCard.GetComponent<Card>().cardSuit == Card.Suit.HEARTS)
                    specialHearts.Add(newCard);
                else if (newCard.GetComponent<Card>().cardSuit == Card.Suit.SPADES && newCard.GetComponent<Card>().val == 1)
                {
                    Debug.Log("adding ace of spades to special deck");
                    specialAces.Add(newCard);
                }
                
                //raise the stack up
                stack_modifier += 0.01f;
            }
        }

        //add the rest of the duplicate cards to get ratio of 2x hearts, 3x ace of spades
        for (int i = 1; i <= Card.MAX_CARD_VALUE; i++)
        {
            stack_modifier += 0.01f;
            GameObject newCard = CardFactory.instance.CreateCard(Card.Suit.HEARTS, i);

            specialHearts.Add(newCard);

            newCard.transform.SetParent(this.transform);
            newCard.transform.localPosition = new Vector3(0, -transform.localPosition.y, transform.localPosition.z + stack_modifier);
            newCard.transform.localEulerAngles = Vector3.zero;
        }

        for (int i = 0; i < 2; i++)
        {
            Debug.Log("adding ace of spades to special deck");

            stack_modifier += 0.01f;
            GameObject newCard = CardFactory.instance.CreateCard(Card.Suit.SPADES, 1);

            newCard.transform.SetParent(this.transform);
            newCard.transform.localPosition = new Vector3(0, -transform.localPosition.y, transform.localPosition.z + stack_modifier);
            newCard.transform.localEulerAngles = Vector3.zero;

            specialAces.Add(newCard);
        }

        //shuffle the deck
        Shuffle();
        Shuffle();
        Shuffle();

        PlayShuffleAnim();
    }
    
    //shuffles the deck by swapping child indicies and positions (swap positions so cards keep their relative positions when they're drawn from the deck)
    public void Shuffle()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            int randIndex_one = Random.Range(0, transform.childCount);
            int randIndex_two = Random.Range(0, transform.childCount);

            int t_int = randIndex_two;

            //make sure the random indicies aren't the same
            while(randIndex_one == randIndex_two)
            {
                randIndex_two = Random.Range(0, transform.childCount);
            }

            //swap the objs and their positions
            GameObject firstObj = transform.GetChild(randIndex_one).gameObject;
            GameObject secndObj = transform.GetChild(randIndex_two).gameObject;

            Vector3 firstPos = firstObj.transform.localPosition;
            Vector3 secndPos = secndObj.transform.localPosition;
            Vector3 temp = firstPos;

            firstObj.transform.localPosition = secndPos;
            secndObj.transform.localPosition = temp;

            firstObj.transform.SetSiblingIndex(t_int);
            secndObj.transform.SetSiblingIndex(randIndex_one);
        }
    }

    public void PlayShuffleAnim()
    {
        //choose a few random cards to rotate
        int cards = Random.Range(3, 10);

        List<GameObject> cardsToPick = new List<GameObject>();

        for(int i = 0; i < cards; i++)
        {
            int randIndex = Random.Range(0, transform.childCount);
            GameObject card = transform.GetChild(randIndex).gameObject;

            //add each random card to a list
            if (!cardsToPick.Contains(card))
                cardsToPick.Add(card);
        }

        //spin each card in the list
        foreach(GameObject card in cardsToPick)
        {
            StartCoroutine(card.GetComponent<Card>().rotate_full(1f));
        }
    }
}
