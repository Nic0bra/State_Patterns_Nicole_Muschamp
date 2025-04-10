using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] TMP_Text currentAmmoText;
    [SerializeField] TMP_Text maxAmmoText;

    FPSController player;

    //Editing this script for health

    //References and variables
    [SerializeField] UnityEvent OnTakeHit;
    [SerializeField] GameObject redCanvas;
    [SerializeField] int maxHealth = 100;
    float currentHealth;



    // Start is called before the first frame update
    void Start()
    {
        redCanvas.SetActive(false);
        player = FindObjectOfType<FPSController>();

        //reset health turn off screen flash and update health bar
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    //Take damage
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        //Keep health from going below zero
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //Trigger red flash
        OnTakeHit.Invoke();
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Debug.Log("Stop hitting yourself, you died");
        }
    }

    //Trigger Flash
    public void TriggerRedFlash()
    {
        redCanvas.SetActive(true);
        Invoke(nameof(EndRedFlash), 0.1f);
    }

    //End flash
    void EndRedFlash()
    {
        redCanvas.SetActive(false);
    }
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    //Listen for damage event
    public void ApplyDamageFromEvent(float damage)
    {
        TakeDamage(damage);
    }
}
