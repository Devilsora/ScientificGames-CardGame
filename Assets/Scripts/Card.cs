using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //constants for Min/Max card value
    public const int MIN_CARD_VALUE = 1;
    public const int MAX_CARD_VALUE = 13;

    //suit value of a given card
    public enum Suit
    {
        CLUBS = 0,
        DIAMONDS,
        HEARTS,
        SPADES,
        MAX_SUITS

    }

    [SerializeField]
    Suit _cardSuit;   //the suit of the card
    
    //accessible value of the suit
    public Suit cardSuit
    {
        get { return _cardSuit; }

        set
        {
            if (value < Suit.MAX_SUITS)
                _cardSuit = value;
        }
    }

    [SerializeField]
    int _val;  //the value of the card (Aces = 1, Jacks = 11, Queens = 12, Kings = 13)
    
    //accessable value of the card
    public int val 
    {
        get { return _val; }

        set
        {
            if (value >= MIN_CARD_VALUE && value <= MAX_CARD_VALUE)
                _val = value;
        }

    }
    
    //front and back of the card
    public GameObject front;    //obj for the card front, contains sprite renderer to render image
    public GameObject back;     //obj for the card back, contains sprite renderer to render image
    
    //overloaded comparator functions for card values
    public static bool operator >(Card lhs, Card rhs)
    {
        bool status = false;

        if (lhs.val > rhs.val)
        {
            status = true;
        }
        else if (lhs.val == rhs.val)
        {
            if ((int)lhs.cardSuit > (int)rhs.cardSuit)
            {
                status = true;
            }
        }

        return status;
    }

    public static bool operator <(Card lhs, Card rhs)
    {
        bool status = false;

        if (lhs.val < rhs.val)
        {
            status = true;
        }
        else if (lhs.val == rhs.val)
        {
            if ((int)lhs.cardSuit < (int)rhs.cardSuit)
            {
                status = true;
            }
        }

        return status;
    }

    //state checking functions to determine card orientation
    public bool isFaceUp()
    {
        if (transform.localRotation.eulerAngles.x == -90)
            return true;

        return false;
    }

    public bool isFaceDown()
    {
        if (transform.localRotation.eulerAngles.y == 90)
            return true;

        return false;
    }

    //moves a card from a location to another within a given time
    //can optionally add rotation to it as it moves with a given rotation speed
    public void MoveTo(Vector3 location, float time, bool rot = false, float rotate_speed = 0)
    {
        Debug.Log("Calling move to");
        transform.SetParent(null);
        StartCoroutine(MoveToPos(location, time));

        if(rot)
        {
            StartCoroutine(rotate(rotate_speed));
        }
        
    }

    //changes the name of the card (mostly used for debugging purposes)
    public void ChangeName()
    {
        gameObject.name += cardSuit + "  " + val;
    }

    //couroutine that manages the actual moving of the card
    IEnumerator MoveToPos(Vector3 location, float time)
    {
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(transform.position, location, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    //couroutine that manages the actual rotation of the card
    public IEnumerator rotate(float rot_speed)
    {
        while (true)
        {
            transform.Rotate(Vector3.forward * rot_speed);
            yield return new WaitForEndOfFrame();
        }
    }


    //couroutine that manages the rotation of the card, but in the special case that the user wants to rotate it
    //360 deg. instead of some other amount
    public IEnumerator rotate_full(float duration)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, endRotation, transform.eulerAngles.z);
    }

    //"flips" the card over, in reality more of a sprite swap
    public void flip()
    {
        //swap the sprites
        Sprite temp = front.GetComponent<SpriteRenderer>().sprite;

        front.GetComponent<SpriteRenderer>().sprite = back.GetComponent<SpriteRenderer>().sprite;
        back.GetComponent<SpriteRenderer>().sprite = temp;

    }

    public void OnDestroy()
    {
        Debug.Log(gameObject.name + " was destroyed");
    }
}
