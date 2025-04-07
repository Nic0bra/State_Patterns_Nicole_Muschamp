using TMPro;
using UnityEngine;

public class AmmoDisplayUpdater : MonoBehaviour
{
    //References to Unity object
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text maxAmmoText;

    //Display the updated ammo count
    public void UpdateAmmoText(int ammo)
    {
        ammoText.text = ammo.ToString();
    }
}
