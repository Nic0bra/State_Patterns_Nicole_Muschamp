using UnityEngine;

public class Shotgun : Gun
{
    [SerializeField] GameObject bleedEffect;

    [SerializeField] float damage = 10;
    [SerializeField] float velocity = 15;
    [SerializeField] float life = 1;
    [SerializeField] float force = 20;
    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(damage, velocity, life, force, Bleed);

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
