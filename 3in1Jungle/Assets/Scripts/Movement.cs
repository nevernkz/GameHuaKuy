using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public enum AIState
    {
        None,
        Idle,
        MoveAround,
        MoveToPlayer,
        Attack,
        TakeDamage,
        Dead,
    }


    public float damagePerHit = 20.0f;
    public float randomMoveRange = 3.0f;
    public Transform findingPlayerPoint;
    public StatusSystem statusSystem;


    

    private PlayerMovement playerTarget;
    public NavMeshAgent navAgent;
    public AIState aiState;
    private Animator animator;

    private AIState previousState;

    private void Start()
    {
        statusSystem = FindObjectOfType<StatusSystem>();

        navAgent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        ChangeState(AIState.Idle);

    }

    private void OnEnable()
    {
        statusSystem = this.GetComponent<StatusSystem>();

        statusSystem.OnTakeDamage += OnTakeDamage;
        statusSystem.OnDead += OnDead;
    }

    private void OnDisable()
    {
        statusSystem.OnTakeDamage -= OnTakeDamage;
        statusSystem.OnDead -= OnDead;
    }

    public void ChangeState(AIState toSetState)
    {
        if (aiState == toSetState)
        {
            return;
        }

        previousState = aiState;
        aiState = toSetState;

        switch(aiState)
        {
            case AIState.Idle:
                {
                    StartCoroutine(aiIdle());
                    break;
                }

            case AIState.MoveAround:
                {
                    StartCoroutine(aiMoveAround());

                    break;
                }

            case AIState.MoveToPlayer:
                {
                    StartCoroutine(aiMoveToPlayer());
                    break;
                }

            case AIState.Attack:
                {
                    StartCoroutine(aiAttack());
                    break;
                }

            case AIState.TakeDamage:
                {
                    StartCoroutine(aiTakeDamage());
                    break;
                }

            case AIState.Dead:
                {
                    StartCoroutine(aiDead());
                    break;
                }
        }

    }

    private bool IsPlayerFound()
    {
        if (playerTarget)
            return true;

        RaycastHit hit;
        Ray ray = new Ray();

        ray.origin = findingPlayerPoint.position;
        ray.direction = findingPlayerPoint.forward;

        bool isHit = Physics.Raycast(ray, out hit, 100);
        Debug.DrawLine(ray.origin, ray.origin + (ray.direction * 100), Color.red);
        if(isHit)
        {
            playerTarget = hit.collider.gameObject.GetComponent<PlayerMovement>();
            if(playerTarget)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator aiIdle()
    {
        animator.SetBool("IsMove", false);
        navAgent.speed = 0.0f;
        navAgent.velocity = Vector3.zero;

        float waitTimeIdle = Random.Range(1.0f, 3.0f);
        while (true)
        {
            waitTimeIdle -= Time.deltaTime;

            if (IsPlayerFound())
            {
                ChangeState(AIState.MoveToPlayer);
                break;
            }
            if (waitTimeIdle <= 0)
            {
                ChangeState(AIState.MoveAround);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator aiMoveAround()
    {
        Vector3 randomPos = Vector3.zero;

        randomPos.x = Random.Range(this.transform.position.x - randomMoveRange, this.transform.position.x + randomMoveRange);
        randomPos.y = this.transform.position.y;
        randomPos.z = Random.Range(this.transform.position.z - randomMoveRange, this.transform.position.z + randomMoveRange);

        animator.SetBool("IsMove", true);

        float randomSpeed = Random.Range(0.25f, 1.5f);
        navAgent.speed = randomSpeed;

        while (true)
        {
            Vector3 targetPos = randomPos;

            float distFromTarget = Vector3.Distance(targetPos, this.transform.position);

           
            if (distFromTarget > navAgent.stoppingDistance)
            {
                navAgent.SetDestination(targetPos);

                animator.SetFloat("Movement", navAgent.velocity.magnitude / 1.5f);

                if (IsPlayerFound())
                {
                    ChangeState(AIState.MoveToPlayer);
                    break;
                }
            }
            else
            {
                ChangeState(AIState.Idle);
                break;
            }

            yield return null;
        }
    }

    private IEnumerator aiMoveToPlayer()
    {
        animator.SetBool("IsMove", true);

        float randomSpeed = Random.Range(0.25f, 1.5f);
        navAgent.speed = randomSpeed;

        while (true)
        {
            float distFromTarget = Vector3.Distance(this.transform.position, playerTarget.transform.position);

            if (distFromTarget > navAgent.stoppingDistance)
            {
                navAgent.SetDestination(playerTarget.transform.position);

                animator.SetFloat("Movement", navAgent.velocity.magnitude / 1.5f);
            }
            else
            {
                ChangeState(AIState.Attack);
                break;
            }
            yield return null;
        }
    }

    private IEnumerator aiAttack()
    {
        statusSystem.playerHP -= 10;

        int randomIndexAtk = Random.Range(0, 2);

        animator.SetInteger("IndexAttack", randomIndexAtk);
    
        animator.SetTrigger("IsAttack");

        statusSystem.playerHP -= damagePerHit;



        yield return null;
    }

    private IEnumerator aiTakeDamage()
    {
        animator.SetTrigger("IsTakeDamage");
        float tempSpd = navAgent.speed;
        navAgent.speed = 0.0f;
        yield return new WaitForSeconds(0.5f);
        navAgent.speed = tempSpd;

        ChangeState(AIState.Idle);
        yield return null;
    }

    private IEnumerator aiDead()
    {
        animator.SetBool("IsDead", true);
        navAgent.speed = 0.0f;
        navAgent.velocity = Vector3.zero;
        Destroy(this.gameObject, 4.0f);

        yield return null;
    }

    public void OnTakeDamage(GameObject damageFrom, float inDamage)
    {
        //Debug.Log("Zombie take damage : " + inDamage);

        playerTarget = damageFrom.GetComponent<PlayerMovement>();

        if (statusSystem.IsAlive())
        {
            ChangeState(AIState.TakeDamage);
        }
    }

    public void OnDead()
    {
        StopAllCoroutines();
        ChangeState(AIState.Dead);
    }

}
