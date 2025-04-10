using UnityEngine;

public class ShootEffects : MonoBehaviour
{
    //References
    [SerializeField] AudioSource gunShotSound;

    public void PlayEffects()
    {
        //play gun shot sound
        if (gunShotSound != null)
        {
            gunShotSound.Play();
        }

    }

}
