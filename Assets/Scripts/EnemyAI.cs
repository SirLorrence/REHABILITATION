using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    enum EnemyState { Patrol, Attack }
    // 0, 1
    AudioSource audioSource;
    public AudioClip enemyShot;
    public AudioClip enemyHit;
    public float patrolSpeed;
    public float attackSpeed;
    public Transform[] PLocations;
    public GameObject bulletPrefab;
    public Transform[] bulletEmitters;
    public GameObject explosionPrefab;
    public int enemyScorePoints = 100;
    public int enemyScoreMultiplier = 1;
    public float fireRate;
    Transform player;
    float nextFire;
    static bool wait = false;
    static int moveLocation;
    static EnemyState state = EnemyState.Attack; // attack by default
    bool playerInShootingRange; // true = shoot at player; false = don't shoot at player
    public float awarenessRange = 100.0f; // in units; shoot at player if this close or closer
    public float shootWithinAngleRange = 15.0f; // shoot if aim at player within this angle
    bool shootRight = false;
    bool isDead = false;
    public int enemyHealth = 100;







    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("current state: " + state);
        audioSource = GameObject.Find("GameControl/AudioSystem/Enemy").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) // state is the input/current number
        {
            // enemystate.patrol is 0
            case EnemyState.Patrol:
                //Patrol();
                break;

            // enemystate.attack is 1
            case EnemyState.Attack:
                Attack();
                break;

            default:
                Patrol();
                break;
        }
    }
    public void DamageEnemy(int amt)
    {
        audioSource = GameObject.Find("GameControl/AudioSystem/Enemy").GetComponent<AudioSource>();
        enemyHealth -= amt;
        audioSource.clip = enemyHit;
        if (!audioSource.isPlaying)
            audioSource.Play();
        if (enemyHealth < 0 && !isDead)
        {
            enemyHealth = 0;

            Explode();
            SphereCollider col = GetComponent<SphereCollider>();
            col.enabled = false;
            ScoreManager scoreManager = GameObject.Find("GameControl/ScoreManager").GetComponent<ScoreManager>();
            scoreManager.AddScore(enemyScorePoints);
            scoreManager.AddMultiplier(enemyScoreMultiplier);
            Destroy(gameObject, .4f);
            isDead = true;
        }
    }
    public void Explode()
    {
        Transform explosionObjPool = GameObject.Find("GameControl/ObjectPool/Explosions").transform;
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity, explosionObjPool) as GameObject;
        Destroy(explosion, 5f);
    }
    void Patrol()
    {
        //if (wait == false)
        //{
        //    moveLocation = Random.Range(0, PLocations.Length);
        //    Debug.Log(moveLocation);
        //    wait = true;
        //}
        //if (wait == true)
        //{
        //    float dist = Vector3.Distance(PLocations[moveLocation].transform.position, transform.position);
        //    transform.position = Vector3.MoveTowards(transform.position, PLocations[moveLocation].transform.position, dist * patrolSpeed * Time.deltaTime);
        //    transform.LookAt(PLocations[moveLocation]);

        //    if (dist < 1)
        //    {
        //        Debug.Log("Made it");
        //        wait = false;
        //    }
        //}
    }

    void Attack()
    {
        float playerDist;
        Debug.Log("Entered Attack Mode");

        

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {

            // move towards player
            player = GameObject.FindGameObjectWithTag("Player").transform;

            playerDist = Vector3.Distance(player.transform.position, transform.position);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, playerDist * patrolSpeed * Time.deltaTime);
            transform.LookAt(player);

            // shoot script stuff here
            if (PlayerInShootingRange() && !ShipSystem.shipDestroyed)
            {
                if (Time.time > nextFire)
                {
                    Shoot();
                    nextFire = Time.time + fireRate;
                }
            }
        }


    }
    public void Shoot()
    {
        Transform enemyBulletObjectPool = GameObject.Find("GameControl/ObjectPool/EnemyBullets").transform;
       
        audioSource.PlayOneShot(enemyShot);
        if (shootRight)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletEmitters[0].transform.position, transform.rotation, enemyBulletObjectPool.transform) as GameObject;
            Destroy(bullet.gameObject, 15f);
            shootRight = false;
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletEmitters[1].transform.position, transform.rotation, enemyBulletObjectPool.transform) as GameObject;
            Destroy(bullet.gameObject, 15f);
            shootRight = true;

        }
 
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            state = EnemyState.Attack;
        }
    }

    bool PlayerInShootingRange()
    {
        // shoot at player?
        // yes, if 1) close to player and 2) aiming at player.
        bool playerInShootingRange = false; // assume not

        // 1) close to player?
        bool closeToPlayer = false; // assume not
        if (Vector3.Distance(gameObject.transform.position, player.position) <= awarenessRange)
        {
            closeToPlayer = true;
        }
        
        // 2) aiming at player?
        bool aimingAtPlayer = false; // assume not
        Vector3 enemyToPlayerDirection = player.position - gameObject.transform.position;
        Vector3 enemyDirection = gameObject.transform.forward;
        if (Vector3.Angle(enemyToPlayerDirection, enemyDirection) < shootWithinAngleRange)
        {
            aimingAtPlayer = true;
        }
        
        if (closeToPlayer && aimingAtPlayer) playerInShootingRange = true;

        return (playerInShootingRange);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, awarenessRange);
    }
}
