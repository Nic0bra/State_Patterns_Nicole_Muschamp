using UnityEngine;

public class Automatic : Gun
{
    [SerializeField] GameObject bleedEffect;

    [SerializeField] float damage = 1;
    [SerializeField] float velocity = 200;
    [SerializeField] float life = 2;
    [SerializeField] float force = 1;
    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(damage, velocity, life, force, Bleed); // version without special effect

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
