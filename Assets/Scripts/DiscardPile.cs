using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    public float stack_modifier = 0.01f;    //modifier for the cards to show up higher than others
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("On trigger enter");

        //if card collides with this, have it stop moving/spinning and add it to the pile
        if(collision.gameObject.name.Contains("Card"))
        {
            collision.gameObject.GetComponent<Card>().StopAllCoroutines();
            collision.gameObject.transform.SetParent(transform);
            stack_modifier += 0.01f;

            //turn off the added card's colliders 
            collision.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            Debug.Log("Object name: " + collision.gameObject.name);
        }
    }
}
