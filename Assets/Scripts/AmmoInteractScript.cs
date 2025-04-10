using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class AmmoInteractScript : MonoBehaviour
{
    //References
    [SerializeField] TMP_Text interactiveText;
    [SerializeField] FPSController player;
    public UnityEvent OnEnterZone = new UnityEvent();
    public UnityEvent OnExitZone = new UnityEvent();
    public UnityEvent OnEPress = new UnityEvent();
    bool isInZone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Hides the interactive on start
        interactiveText.enabled = false;
    }

    //Checks if the player has entered the interactive zone
    private void OnTriggerEnter(Collider other)
    {
        EnterInteractiveZone(other);
    }
    //Checks if player has left the interactive zone
    private void OnTriggerExit(Collider other)
    {
        ExitInteractiveZone(other);
    }

    //Shows text if player enters the interactive zone
    void EnterInteractiveZone(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            isInZone = true;
            //Display the prompt if true
            interactiveText.enabled = true;
            OnEnterZone.Invoke();
        }
    }

    //Hides text if player left interacitve zone
    void ExitInteractiveZone(Collider other)
    {
        if (other.gameObject == player.gameObject)
        {
            //Hide the prompt if false
            interactiveText.enabled = false;
            OnExitZone.Invoke();
        }
    }

    private void Update()
    {
        //Check if they player is in the zone and pressed the button
        if (isInZone && Input.GetButtonDown("Interact"))
        {
            OnEPress.Invoke();
        }
    }
}
