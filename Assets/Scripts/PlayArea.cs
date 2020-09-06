using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public Card currentCard;    //the current card in this play area

    public void OnTriggerEnter(Collider collision)
    {
        //when a card collides with this, stop all card movement/rotation
        if(collision.gameObject.name.Contains("Card"))
        {
            Debug.Log("Collided with play area");
            
            currentCard = collision.gameObject.GetComponent<Card>();
            currentCard.StopAllCoroutines();
        }
    }


}
