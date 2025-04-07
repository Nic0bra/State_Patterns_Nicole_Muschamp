using UnityEngine;

public class Duplicate : Gun
{
    [SerializeField] GameObject poofEffect;
    [SerializeField] GameObject enemyJeff;
    [SerializeField] int minDuplicates = 1;
    [SerializeField] int maxDuplicates = 5;
    [SerializeField] float destroyTime;
    public override bool AttemptFire()
    {
        if (!base.AttemptFire())
            return false;

        var b = Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        b.GetComponent<Projectile>().Initialize(3, 100, 2, 5, Pooficate);

        anim.SetTrigger("shoot");
        elapsed = 0;
        ammo -= 1;

        return true;
    }

    //Make Pooficate
    void Pooficate(HitData data)
    {
        //Not working so we debugging
        Debug.Log("Pooficate triggered on: " + data.target.name);
        Vector3 impactLocation = data.location;

        //Add particle affect upon impact location
        Instantiate(poofEffect, impactLocation, Quaternion.identity);

        //Get Jeff instead of Damageable
        Transform hitTransform = data.target.transform;
        while (hitTransform.parent != null)
        {
            //Check if it's Jeff
            if (hitTransform.CompareTag("Jeff"))
                break;
            hitTransform = hitTransform.parent;
        }

        //Check if Jeff is found
        if (hitTransform.CompareTag("Jeff"))
        {
            //Check if it worked
            Debug.Log("Jeff hit, duplicate");

            //Generate a random number between min and max duplicates
            int numOfDuplicates = Random.Range(minDuplicates, maxDuplicates + 1);

            //Duplicate the Jeffs
            for (int i = 0; i < numOfDuplicates; i++)
            {
                //Offset duplicates
                Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));

                GameObject duplicate = Instantiate(enemyJeff, impactLocation + randomOffset, Quaternion.identity);

                //Destroy Duplicates 
                Destroy(duplicate, destroyTime);
            }
        }
        else
        {
            Debug.Log("Jeff not found. " + hitTransform.name + "was hit.");
        }
    }
}

