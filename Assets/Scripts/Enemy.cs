using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

//Define states
enum EnemyStates
{
    WANDER,
    PURSUE,
    ATTACK,
    RECOVERY
}

public class Enemy : MonoBehaviour
{
    //Enemy States
    [SerializeField] EnemyStates enemyState;
    public Rigidbody Rigidbody { get; private set; }
    public CharacterController playerBod;
    Vector3 origin;
    [SerializeField] Transform player;
    [SerializeField] AudioSource rawrSound;

    //Enemy variables
    [SerializeField] float wanderRange = 10f;
    [SerializeField] float playerSightRange = 15f;
    [SerializeField] float playerAttackRange = 2f;
    [SerializeField] float recoveryTime = .5f;
    float currentStateElapsed;
    [SerializeField] NavMeshAgent agent;
    public float damageAmount = 10f;
    public UnityEvent<float> OnDamageDealt;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerBod = player.GetComponent<CharacterController>();
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentStateElapsed += Time.deltaTime;

        switch (enemyState)
        {
            case EnemyStates.WANDER:
                UpdateWander();
                break;
            case EnemyStates.PURSUE:
                UpdatePursue();
                break;
            case EnemyStates.ATTACK:
                UpdateAttack();
                break;
            case EnemyStates.RECOVERY:
                UpdateRecovery();
                break;
        }
    }
    
    //Wander within random position within the wander range
    void UpdateWander()
    {
        Debug.Log("Wandering");


        if(currentStateElapsed > 2f || !agent.hasPath)
        {
            //Wander to a random position
            Vector3 randomDirection = Random.insideUnitSphere * wanderRange;
            //Keep Jeff grounded
            randomDirection.y = 0;
            NavMeshHit hit;

            if(NavMesh.SamplePosition(transform.position + randomDirection, out hit, wanderRange,1))
            {
                agent.SetDestination(hit.position);
            }
            currentStateElapsed = 0f;
            //Switch to pursue if player comes into sight range
            if (Vector3.Distance(transform.position, player.position) < playerSightRange)
            {
                enemyState = EnemyStates.PURSUE;
                currentStateElapsed = 0f;
            }
        }
    }

    //Move towards player
    void UpdatePursue()
    {
        Debug.Log("Pursuing");

        agent.isStopped = false;
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        //Attack the player if within attack range
        if (distance <= playerAttackRange)
        {
            enemyState = EnemyStates.ATTACK;
            currentStateElapsed = 0f;
        }
        //If player is out of sight go back to wander
        else if (distance > playerSightRange)
        {
            enemyState = EnemyStates.WANDER;
            currentStateElapsed = 0f;
        }
    }
    //Lunge towards player and apply damage
    void UpdateAttack()
    {
        Debug.Log("Attacking");
        //Stop moving
        agent.isStopped = true;

        rawrSound?.Play();

        //Lunge by adding force
        Vector3 direction = (player.position - transform.position).normalized;
        agent.Move(direction * 1.5f);

        if (Vector3.Distance(transform.position, player.position) < 1.2f)
        {
            //Simulating damage until I check that code is working properly
            Debug.Log("Enemy hit player");

            OnDamageDealt?.Invoke(damageAmount);
            KnockPlayerBack(direction);
        }

        Debug.Log("Jeff recovers after hitting player");
        //Go to recovery state
        enemyState = EnemyStates.RECOVERY;
        currentStateElapsed = 0f;
    }

    //Do nothing for a time
    void UpdateRecovery()
    {
        Debug.Log("Jeff is recovering after attacking");
        if (currentStateElapsed > recoveryTime)
        {
            //Continue with agent Jeff
            agent.isStopped = false;
            enemyState = EnemyStates.WANDER;
            currentStateElapsed = 0f;
        }
    }


    /*public void RecoverFromDamage(Vector3 knockback)
    {
        Debug.Log("Jeff is hurt from damage");
        agent.isStopped = true;
        //Enter recovery after being shot
        enemyState = EnemyStates.RECOVERY;
        currentStateElapsed = 0f;
        //
        Rigidbody.AddForce(knockback,ForceMode.Impulse);
    }*/
    public void KnockEnemyBack(Vector3 knockback)
    {
        GetComponent<Rigidbody>().AddForce(knockback, ForceMode.Impulse);
    }
    public void KnockPlayerBack(Vector3 direction)
    {
        if (playerBod != null)
        {
            Vector3 knockback = -direction * 5f;
            playerBod.Move(knockback * Time.deltaTime);
        }
    }

    public void Respawn()
    {
        transform.position = origin;
        agent.ResetPath();
        agent.isStopped = false;
        enemyState = EnemyStates.WANDER;
        currentStateElapsed = 0f;
    }
}
