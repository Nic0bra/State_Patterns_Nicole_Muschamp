using UnityEngine;

public class ShootEffects : MonoBehaviour
{
    //References
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioSource gunShotSound;

    public void PlayEffects()
    {
        //Play muzzle flash
        if(muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        //play gun shot sound
        if(gunShotSound != null)
        {
            gunShotSound.Play();
        }

    }

}
