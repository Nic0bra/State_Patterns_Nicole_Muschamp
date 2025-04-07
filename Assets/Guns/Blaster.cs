using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : Gun
{
    [SerializeField] GameObject bleedEffect;
    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(3, 100, 2, 5, Bleed); // version without special effect
        //b.GetComponent<Projectile>().Initialize(1, 100, 2, 5, DoThing); // version with special effect

        anim.SetTrigger("shoot");
        elapsed = 0;
        ammo -= 1;

        return true;
    }

    //Make bleed
    void Bleed(HitData data)
    {
        Vector3 impactLocation = data.location;

        //Add particle affect upon impact location
        Instantiate(bleedEffect, impactLocation, Quaternion.identity);
    }
}
