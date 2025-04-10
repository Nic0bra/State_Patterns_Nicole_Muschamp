using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    Damager damagerReference;
    public Rigidbody Rigidbody { get; private set; }
    public CharacterController playerBod;
    Vector3 origin;
    [SerializeField] Transform player;

    //Enemy variables
    [SerializeField] float wanderRange = 10f;
    [SerializeField] float playerSightRange = 15f;
    [SerializeField] float playerAttackRange = 2f;
    [SerializeField] float recoveryTime = 2f;
    float currentStateElapsed = 0;
    [SerializeField] NavMeshAgent agent;
    public float damageAmount = 7f;
    public UnityEvent<float> OnDamageDealt;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        origin = transform.position;

        playerBod = player.GetComponent<CharacterController>();
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
        if(currentStateElapsed > 2f || !agent.hasPath)
        {
            //Wander to a random position
            Vector3 randomPoint = origin + (Random.insideUnitSphere * wanderRange);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRange, 1))
            {
                agent.SetDestination(hit.position);
            }
            currentStateElapsed = 0f;
        }
        //Switch to pursue if player comes into sight range
        if(Vector3.Distance(transform.position, player.position) < playerSightRange)
        {
            enemyState = EnemyStates.PURSUE;
            currentStateElapsed = 0f;
        }
    }

    //Move towards player
    void UpdatePursue()
    {
        agent.SetDestination(player.position);

        //Attack the player if within attack range
        if(Vector3.Distance(transform.position, player.position) < playerAttackRange)
        {
            enemyState = EnemyStates.ATTACK;
            currentStateElapsed = 0f;
        }
        //If player is out of sight go back to wander
        else if (Vector3.Distance(transform.position, player.position) > playerSightRange)
        {
            enemyState = EnemyStates.WANDER;
            currentStateElapsed = 0f;
        }
    }
    //Lunge towards player and apply damage
    void UpdateAttack()
    {
        //Stop moving
        agent.isStopped = true;

        //Lunge by adding force
        Vector3 direction = (player.position - transform.position).normalized;
        Rigidbody.AddForce(direction * 10f, ForceMode.VelocityChange);

        if (Vector3.Distance(transform.position, player.position) < 1f)
        {
            //Simulating damage until I check that code is working properly
            Debug.Log("Enemy hit player");
            if (OnDamageDealt == null)
            {
                //Initialize event if null
                OnDamageDealt = new UnityEvent<float>();
            }
        }

        //Apply Player knockback
        KnockPlayerBack(direction);

        //Go to recovery state
        enemyState = EnemyStates.RECOVERY;
        currentStateElapsed = 0f;
    }


    //Do nothing for a time
    void UpdateRecovery()
    {
        if(currentStateElapsed > recoveryTime)
        {
            //Continue with agent Jeff
            agent.isStopped = false;



            enemyState = EnemyStates.PURSUE;
            currentStateElapsed = 0f;
        }
    }

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
    }
}
