using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField] float maxHp = 100;
    public UnityEvent<Vector3> OnHit;
    [SerializeField] GameObject damageNumberPrefab;

    float currentHp;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentHp = maxHp;
    }

    public void Hit(Vector3 knockback, float damageAmount)
    {
        //owner.ApplyKnockback(knockback);
        OnHit.Invoke(knockback);//, damageAmount);
        TakeDamage(damageAmount);
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if(damageNumberPrefab != null)
        {
            var damageNumber = Instantiate(damageNumberPrefab,transform.position, Quaternion.identity);

            DamageNumber dn = damageNumber.GetComponent<DamageNumber>();
            if(dn != null)
            {
                dn.SetNumber(amount);
            }
        }

        if (currentHp < 0)
        {
            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Respawn();
            }

        }
    }
}
