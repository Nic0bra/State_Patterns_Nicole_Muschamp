using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// gun base class
public class Gun : MonoBehaviour
{
    //Event for ammo change
    public UnityEvent<int> OnAmmoChanged = new UnityEvent<int>();

    //Event for fire gun camera shake
    public UnityEvent OnFire = new UnityEvent();

    protected FPSController player;

    // references
    [SerializeField] protected Transform gunBarrelEnd;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Animator anim;

    // stats
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float timeBetweenShots = 0.1f;
    [SerializeField] protected bool isAutomatic = false;

    // private variables
    protected int ammo;
    protected float elapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        ammo = maxAmmo;
        OnAmmoChanged?.Invoke(ammo);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        /* cheat code to refill ammo
        if (Input.GetKeyDown(KeyCode.R))
        {
            AddAmmo(999);
        }*/
    }

    public virtual void Equip(FPSController p)
    {
        player = p;
    }

    public virtual void Unequip() { }

    public bool AttemptAutomaticFire()
    {
        if (!isAutomatic)
            return false;

        return true;
    }

    public virtual bool AttemptFire()
    {
        if (ammo <= 0)
        {
            return false;
        }

        if (elapsed < timeBetweenShots)
        {
            return false;
        }

        Debug.Log("Bang");
        Instantiate(bulletPrefab, gunBarrelEnd.transform.position, gunBarrelEnd.rotation);
        anim.SetTrigger("shoot");
        timeBetweenShots = 0;
        ammo -= 1;
        OnAmmoChanged.Invoke(ammo);
        OnFire.Invoke();


        return true;
    }

    public virtual bool AttemptAltFire()
    {
        return false;
    }

    public virtual void AddAmmo(int amount)
    {
        ammo += amount;
        ammo = Mathf.Clamp(ammo, 0, maxAmmo);
        OnAmmoChanged?.Invoke(ammo);
    }
}
