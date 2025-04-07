using UnityEngine;

public class OofScript : MonoBehaviour
{
    //References
    [SerializeField] AudioSource oofSound;

    public void PlayOofed()
    {
        //play gun shot sound
        if (oofSound != null)
        {
            oofSound.Play();
        }

    }

}
