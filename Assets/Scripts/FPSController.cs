using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class FPSController : MonoBehaviour
{
    //References
    [SerializeField] PlayerHUD playerHUD;
    // references
    CharacterController controller;
    [SerializeField] GameObject cam;
    [SerializeField] Transform gunHold;
    [SerializeField] Gun initialGun;

    // stats
    [SerializeField] float movementSpeed = 2.0f;
    [SerializeField] float lookSensitivityX = 1.0f;
    [SerializeField] float lookSensitivityY = 1.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;

    // private variables
    Vector3 origin;
    Vector3 velocity;
    bool grounded;
    float xRotation;
    List<Gun> equippedGuns = new List<Gun>();
    int gunIndex = 0;
    Gun currentGun = null;

    //Input systems
    Vector2 moveInput;
    bool jump = false;
    bool sprint = false;
    Vector2 lookInput;

    // properties
    public GameObject Cam { get { return cam; } }


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // start with a gun
        if (initialGun != null)
            AddGun(initialGun);

        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Look();

        // always go back to "no velocity"
        // "velocity" is for movement speed that we gain in addition to our movement (falling, knockback, etc.)
        Vector3 noVelocity = new Vector3(0, velocity.y, 0);
        velocity = Vector3.Lerp(velocity, noVelocity, 5 * Time.deltaTime);
    }

    void Movement()
    {
        grounded = controller.isGrounded;

        if (grounded && velocity.y < 0)
        {
            velocity.y = -1;// -0.5f;
        }


        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * movementSpeed * (GetSprint() ? 2 : 1) * Time.deltaTime);

        if (jump && grounded)
        {
            velocity.y += Mathf.Sqrt(jumpForce * -1 * gravity);
            jump = false;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void OnJump()
    {
        Debug.Log("Player jumped");
        if (grounded)
            jump = true;
    }
    void OnMove(InputValue move)
    {
        Debug.Log("Player moved");
        moveInput = move.Get<Vector2>();
    }

    void Look()
    {
        float lookX = lookInput.x * lookSensitivityX * Time.deltaTime;
        float lookY = lookInput.y * lookSensitivityY * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }

    public void OnLook(InputValue mouseLook)
    {
        Debug.Log("Player looked");
        lookInput = mouseLook.Get<Vector2>();
    }
    void OnChangeGun(InputValue wheel)
    {
        Debug.Log("Player changed gun");
        if (equippedGuns.Count == 0)
            return;

        if (wheel.Get<float>() > 0)
        {
            gunIndex++;
            if (gunIndex > equippedGuns.Count - 1)
                gunIndex = 0;

            EquipGun(equippedGuns[gunIndex]);
        }

        else if (wheel.Get<float>() < 0)
        {
            gunIndex--;
            if (gunIndex < 0)
                gunIndex = equippedGuns.Count - 1;

            EquipGun(equippedGuns[gunIndex]);
        }
    }

    /*void FireGun()
    {
        // don't fire if we don't have a gun
        if (currentGun == null)
            return;

        // pressed the fire button
        if (GetPressFire())
        {
            currentGun?.AttemptFire();
        }

        // holding the fire button (for automatic)
        else if (GetHoldFire())
        {
            if (currentGun.AttemptAutomaticFire())
                currentGun?.AttemptFire();
        }

        // pressed the alt fire button
        if (GetPressAltFire())
        {
            currentGun?.AttemptAltFire();
        }
    }*/

    void OnShoot()
    {
        Debug.Log("Player shot");
        if (currentGun == null)
            return;

        currentGun?.AttemptFire();
    }

    void EquipGun(Gun g)
    {
        // disable current gun, if there is one
        currentGun?.Unequip();
        currentGun?.gameObject.SetActive(false);

        // enable the new gun
        g.gameObject.SetActive(true);
        g.transform.parent = gunHold;
        g.transform.localPosition = Vector3.zero;
        currentGun = g;

        g.Equip(this);
    }

    // public methods

    public void AddGun(Gun g)
    {
        // add new gun to the list
        equippedGuns.Add(g);

        // our index is the last one/new one
        gunIndex = equippedGuns.Count - 1;

        // put gun in the right place
        EquipGun(g);
    }

    public void IncreaseAmmo(int amount)
    {
        currentGun.AddAmmo(amount);
    }

    public void Respawn()
    {
        transform.position = origin;
    }

    // Input methods

    /*bool GetPressFire()
    {
        return Input.GetButtonDown("Fire1");
    }

    bool GetHoldFire()
    {
        return Input.GetButton("Fire1");
    }

    bool GetPressAltFire()
    {
        return Input.GetButtonDown("Fire2");
    }

    Vector2 GetPlayerMovementVector()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    Vector2 GetPlayerLook()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }*/
    void OnSprint(InputValue shift)
    {
        Debug.Log("Player sprinted");
        sprint = shift.isPressed;
    }
    bool GetSprint()
    {
        return sprint;
    }
    

    // Collision methods

    // Character Controller can't use OnCollisionEnter :D thanks Unity
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Damager damager = hit.gameObject.GetComponent<Damager>();
        if (damager != null)
        {
            var collisionPoint = hit.collider.ClosestPoint(transform.position);
            var knockbackAngle = (transform.position - collisionPoint).normalized;
            velocity = (20 * knockbackAngle);

            //Apply damage
            if (playerHUD != null)
            {
                //Pass damage
                playerHUD.TakeDamage(damager.damageAmount);
            }
        }
        if (hit.gameObject.GetComponent<Damager>())
        {
            var collisionPoint = hit.collider.ClosestPoint(transform.position);
            var knockbackAngle = (transform.position - collisionPoint).normalized;
            velocity = (20 * knockbackAngle);
        }

        if (hit.gameObject.GetComponent<KillZone>())
        {
            Respawn();
        }
    }
}
