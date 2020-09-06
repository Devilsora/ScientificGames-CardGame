using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    //play areas cards are sent over to (really more positional references)
    public PlayArea P1_area;
    public PlayArea P2_area;

    //respective deck/discard piles
    public GameObject discardPile;
    public GameObject deck;

    //player UI scores and texts objects
    public int P1_score;
    public int P2_score;

    public Text p1;
    public Text p2;

    //deal UI button
    public Button dealCardsBtn;

    //reshuffle deck button
    public Button reshuffleDeckBtn;

    //the cards that the players drew
    [SerializeField]
    GameObject P1_card;

    [SerializeField]
    GameObject P2_card;

    //control boolean to handle when to destroy excess Aces in deck
    //only using aces variable instead of one for hearts due to hearts only have 1 extra copy also entered into deck
    //aces has multiple copies that need a flag to know when to destroy
    public bool destroyedAces = false;

    //control booleans to handle waiting for game parts to be down
    public bool doneWaiting_flip = false;
    public bool doneWaiting_judge = false;
    public bool scored = false;

    void Start()
    {
        //hide away the reshuffle button until it's actually needed
        reshuffleDeckBtn.gameObject.SetActive(false);
    }

    void Update()
    {
       
    }

    //deals the top card to a specified position
    //can be additionally modified to change travel time, whether or not it spins, and the spin speed
    public GameObject Deal_Top(Vector3 position_to_send, float travel_time, float spinSpeed, bool spin = false)
    {
        //get the top card of the deck to deal out
        GameObject topCard = deck.transform.GetChild(0).gameObject;

        //check if it's needed to delete extra hearts card or ace of spades included
        if (topCard.GetComponent<Card>().cardSuit == Card.Suit.HEARTS)
        {
            //get card value to reference the hearts copy to delete
            //handles cases to avoid accidentally deleting the card deck is trying to deal
            int cardVal = topCard.GetComponent<Card>().val;

            //delete the respective hearts copy
            if(deck.GetComponent<DeckFill>().specialHearts[cardVal - 1] == topCard)
            {
                Destroy(deck.GetComponent<DeckFill>().specialHearts[(cardVal + 13) - 1]);
            }
            else
            {
                Destroy(deck.GetComponent<DeckFill>().specialHearts[(cardVal) - 1]);
            }
        }

        //delete the other ace of spade copies if an ace of spade was drawn
        else if (topCard.GetComponent<Card>().cardSuit == Card.Suit.SPADES && topCard.GetComponent<Card>().val == 1 && destroyedAces == false)
        {
            for(int i = 0; i < 3; i++)
            {
                if(deck.GetComponent<DeckFill>().specialAces[i] != topCard)
                    Destroy(deck.GetComponent<DeckFill>().specialAces[i]);
            }

            destroyedAces = true;
        }

        //move the card over to the appropriate player in travel_time with spin and spin speed
        topCard.GetComponent<Card>().MoveTo(position_to_send, travel_time, spin, spinSpeed);

        return topCard;
    }

    //draw the top 2 cards from the deck, and animate them moving  to the player areas
    public void Deal_Cards()
    {
        //move cards to discard if the round has already been scored
        if (scored)
        {
            P1_card.GetComponent<Card>().MoveTo(discardPile.transform.position, 1f, true, Random.Range(1f, 8f));
            P2_card.GetComponent<Card>().MoveTo(discardPile.transform.position, 1f, true, Random.Range(1f, 8f));

            scored = false;
        }

        //make the deal button un interactable until the cards are judged
        dealCardsBtn.interactable = false;

        //stop the coroutines currently running in this script
        StopAllCoroutines();
        

        //make sure the system "states" are reset
        doneWaiting_flip = false;
        doneWaiting_judge = false;

        //only deal the top if there are cards left in the deck
        if (deck.transform.childCount > 0)
        {
            P1_card = Deal_Top(P1_area.transform.position, 30f, Random.Range(1f, 8f), true);
            P2_card = Deal_Top(P2_area.transform.position, 30f, Random.Range(1f, 8f), true);
            Reveal_Cards();
        }
        else
        {
            //destroy discard pile and remake deck to "reshuffle" discard pile
            Debug.Log("Reshuffle deck from discard pile");
            dealCardsBtn.gameObject.SetActive(false);
            reshuffleDeckBtn.gameObject.SetActive(true);
        }

        
    }

    //reshuffle the deck by deleting whatever is in the discard pile and recreate it
    public void ReshuffleDeck()
    {
        //clear out the discard pile
        foreach(Transform child in discardPile.transform)
        {
            Destroy(child.gameObject);
        }

        //recreate the deck
        deck.GetComponent<DeckFill>().CreateDeck();

        //swap presence of the deal and reshuffle buttons
        reshuffleDeckBtn.gameObject.SetActive(false);
        dealCardsBtn.gameObject.SetActive(true);
    }

    //wait for a given amount of time before flipping cards over and judging values
    IEnumerator WaitTime_ThenFlip(float time)
    {
        //timer to track how much time has passed before flipping
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //flip the cards and judge them according to the < and > operators of the cards
        doneWaiting_flip = true;
        P1_card.GetComponent<Card>().flip();
        P2_card.GetComponent<Card>().flip();

        Judge();
    }

    //wait for a specific amount of time before judging the cards and affecting the scores
    IEnumerator WaitTime_ThenJudge(float time)
    {
        float elapsedTime = 0f;

        //wait given amount of time before actually flipping the cards over
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        doneWaiting_judge = true;

        //get the result for judging the cards
        int result = JudgeCards(P1_card.GetComponent<Card>(), P2_card.GetComponent<Card>());

        if (result == 1)
            P1_score++;
        else if (result == 2)
            P2_score++;

        //update score displays
        p1.text = P1_score.ToString();
        p2.text = P2_score.ToString();
        scored = true;
        dealCardsBtn.interactable = true;
    }

    //flip the cards after waiting for the cards to actually flip if in the 
    public void Reveal_Cards()
    {
        if (doneWaiting_flip == false)
        {
            StartCoroutine(WaitTime_ThenFlip(2f));
        }
    }

    //judges the cards after they've been flipped
    public void Judge()
    {
        if (doneWaiting_flip)
        {
            Debug.Log("In judge: done waiting for flip - " + doneWaiting_flip);
            StartCoroutine(WaitTime_ThenJudge(0.5f));
        }
    }

    //compare two cards against each other and return the player who got the higher value
    //returns the number of the player with the higher card value
    //return -1 if incomparable
    public int JudgeCards(Card p1, Card p2)
    {
        if (p1 > p2)
            return 1;
        else if (p2 > p1)
            return 2;

        return -1;
        
    }

}
