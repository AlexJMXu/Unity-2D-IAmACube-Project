using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform target;

    private Enemy enemy;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float elapsedTime;


    // following player 
    private const float movementDistance = 1f;
    public float moveSpeed = 4;
    public float rotationSpeed = 5;
    public float maxDist = 5;   

    // randomly moving
    private float latestDirectionChangeTime;
    private readonly float directionChangeTime = 1f;
    private float characterVelocity = 1f;
    private Vector2 movementDirection;
    private Vector2 movementPerSecond;

    // use case - player collision - wait 30sec before following the player again 
    private float timerAfterPlayerCollision = 30;
    private bool playerCollision = false;
    private bool follow = true;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        latestDirectionChangeTime = 0f;
    }

    void RandomDirection()
    {
        movementDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        movementPerSecond = movementDirection * characterVelocity;
    }

    void RandomPatrol()
    {
        System.Random rnd = new System.Random();
        int random = rnd.Next(1, 4);
        switch (random)
        {
            case 1:
                SetDestination(-movementDistance, -movementDistance/2);
                break;
            case 2:
                SetDestination(-movementDistance, movementDistance/2);
                break;
            case 3:
                SetDestination(movementDistance, -movementDistance/2);
                break;
            case 4:
                SetDestination(movementDistance, movementDistance/2);
                break;
        }
    }

    void FollowPlayer()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void Update()
    {    
        if (enemy.enemyState == Enemy.EnemyState.Moving) return;
        else RandomPatrol();

        /*
        transform.LookAt(target);
        bool nearPlayer = follow && Vector3.Distance(transform.position, target.position) <= maxDist;
        if (playerCollision)
            timerAfterPlayerCollision -= Time.deltaTime;

        if (timerAfterPlayerCollision < 0)
        {
            follow = true;
            playerCollision = false;
            timerAfterPlayerCollision = 30;
        }

        if (nearPlayer)
            FollowPlayer();
        else
            RandomPatrol();
            */
    }

    IEnumerator PlayerFreeze(PlayerController player)
    {
        playerCollision = true;
        yield return new WaitForSeconds(10);
        player.canMove = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            follow = false;

            PlayerController player = collider.GetComponent<PlayerController>();

            // freeze player
            player.canMove = false;
            StartCoroutine(PlayerFreeze(player));
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (enemy.enemyState == Enemy.EnemyState.Idle) return;

        if (elapsedTime >= 0.5f)
        {
            enemy.animator.SetBool("isMoving", false);
        }

        if (elapsedTime >= 1f)
        {
            enemy.enemyState = Enemy.EnemyState.Idle;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            enemy.associatedTransform.position = Vector3.Lerp(startPos, targetPos, elapsedTime);
        }
    }

    private void SetDestination(float xDest, float yDest)
    {
        enemy.animator.SetBool("isMoving", true);
        enemy.enemyState = Enemy.EnemyState.Moving;
        startPos = enemy.associatedTransform.position;
        targetPos = enemy.associatedTransform.position + new Vector3(-xDest, -yDest, 0f);
        elapsedTime = 0f;
    }
}

