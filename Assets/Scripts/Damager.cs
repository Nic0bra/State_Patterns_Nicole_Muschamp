using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    //Damage dealt
    public float damageAmount = 5f;
    //Event triggered to deal damage
    public UnityEvent<float> OnDamageDealt;

    //On collision funtion for damage cube
    private void Start()
    {
        if (OnDamageDealt == null)
        {
            //Initialize event if null
            OnDamageDealt = new UnityEvent<float>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            //Trigger the damage event
            OnDamageDealt.Invoke(damageAmount);
        }
    }

}
