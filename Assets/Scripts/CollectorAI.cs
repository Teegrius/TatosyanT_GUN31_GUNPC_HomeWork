using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CollectorAI : MonoBehaviour
{
    public float searchRadius = 5f;
    public Text scoreText;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;
    private int score = 0;
    private float idleTimer = 0f;

    private enum State { Idle, Search, Collect }
    private State currentState = State.Idle;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = 5.5f;
        agent.angularSpeed = 720f;

        // Важные параметры для Starter Assets
        if (animator != null)
        {
            animator.SetBool("Grounded", true);
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 0f);
        }

        SetRandomDestination();
    }

    void Update()
    {
        // === КРИТИЧНО ДЛЯ АНИМАЦИИ ===
        if (animator != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed);
            animator.SetFloat("MotionSpeed", currentSpeed > 0.1f ? 1.0f : 0f);
        }

        switch (currentState)
        {
            case State.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer >= 5f)
                {
                    currentState = State.Search;
                    idleTimer = 0f;
                    SetRandomDestination();
                }
                break;

            case State.Search:
                LookForItem();
                if (!agent.hasPath || agent.remainingDistance < 1.5f)
                    SetRandomDestination();
                break;

            case State.Collect:
                if (currentTarget == null)
                {
                    currentState = State.Idle;
                    return;
                }

                agent.SetDestination(currentTarget.position);

                if (Vector3.Distance(transform.position, currentTarget.position) < 1.8f)
                {
                    Destroy(currentTarget.gameObject);
                    score++;
                    if (scoreText != null) scoreText.text = "Collected: " + score;

                    currentTarget = null;
                    currentState = State.Idle;
                    idleTimer = 0f;
                }
                break;
        }
    }

    void LookForItem()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, searchRadius);
        foreach (Collider col in items)
        {
            if (col.CompareTag("Collectible"))
            {
                currentTarget = col.transform;
                currentState = State.Collect;
                agent.stoppingDistance = 1.2f;
                return;
            }
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomPos = Random.insideUnitSphere * 20f + transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 30f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }
}