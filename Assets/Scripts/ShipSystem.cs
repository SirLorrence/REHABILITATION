using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShipSystem : MonoBehaviour
{
    AudioSource audioSrcEffect;
    AudioSource audioSrcDamage;
    public AudioClip damageHealthSfx;
    public AudioClip damageShieldSfx;
    public AudioClip shieldRechargeSfx;
    int health = 100;
    float shield = 100;
    bool shieldDamaged = false;
    public static bool shipDestroyed = false;
    IEnumerator respawnRoutine = null;
    public GameObject explosionPrefab;
    public GameObject ShipModel;
    public SphereCollider shipCollider;
    public Image healthFillAmt;
    public Image shieldFillAmt;
    public Image flashDamage;
    bool flash;
    float flashTimer = 0.1f;
    float flashTime;
    public float respawnSpeed;
    public float repairTime;
    public float repairTimer;
    public float repairSpeed = 5f;
    bool startRepair = false;
    public Image repairScreen;
    public Text repairText;

    void Start()
    {
        flashTime = flashTimer;
        repairTime = repairTimer;
        repairText.text = null;
        audioSrcEffect = GameObject.Find("GameControl/AudioSystem/PlayerEffects").GetComponent<AudioSource>();
        audioSrcDamage = GameObject.Find("GameControl/AudioSystem/PlayerDamage").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShipDamage(10);
        }
        if (flash)
        {
            if (flashTime > 0)
            {
                flashTime -= Time.deltaTime;
                flashDamage.enabled = true;
            }
            else if (flashTime < 0)
            {
                flashDamage.enabled = false;
                flashTime = flashTimer;
                flash = false;

            }

        }
        if (startRepair)
        {
            if(repairTime > 0)
            {
                repairTime -= Time.deltaTime;
                repairScreen.color = new Color(0, 0, 1, 0);
                repairText.color = new Color(1, 1, 1, 0);
                repairText.text = null;
            }
            else if(repairTime < 0)
            {
                if (!audioSrcEffect.isPlaying)
                {
                    audioSrcEffect.clip = shieldRechargeSfx;
                    audioSrcEffect.Play();
                }
                repairScreen.color = new Color(0, 0, 1, Mathf.PingPong(Time.time, 1));
                repairText.text = "Repairing!";
                repairText.color = new Color(1, 1, 1, Mathf.PingPong(Time.time, 1));
                shield += Time.deltaTime * repairSpeed;
                if(shield > 0)
                    shieldDamaged = false;
                if (shield > 99)
                {
                    repairText.color = new Color(1, 1, 1, 0);
                    repairText.text = null;
                    repairScreen.color = new Color(0, 0, 1, 0);
                    shield = 100;
                    startRepair = false;
                    repairTime = repairTimer;
                }
            }
        }
        handler();
    }

    void handler()
    {
        healthFillAmt.fillAmount = Map(health, 0, 100, 0, 1);
        shieldFillAmt.fillAmount = Map(shield, 0, 100, 0, 1);
        
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    public void ShipDamage(int amt)
    {
        
        startRepair = true;
        repairTime = repairTimer;
        flash = true;
        if (!shieldDamaged)
        {
            audioSrcDamage.PlayOneShot(damageShieldSfx);
            flashDamage.color = new Color(1, 1, 1, 0.2f);
            shield -= amt;
            if(shield < 0)
            {
                shield = 0;
                shieldDamaged = true;
            }
        }
        else
        {
            audioSrcDamage.PlayOneShot(damageHealthSfx);
            flashDamage.color = new Color(1, 0, 0, 0.2f);
            health -= amt;
            if(health < 0)
            {
                health = 0;
                shipDestroyed = true;
                CharacterController charCtrl = GetComponent<CharacterController>();
                charCtrl.ResetVelocity();
                Explode();
                if (respawnRoutine != null)
                    StopCoroutine(Respawn());
                respawnRoutine = Respawn();
                StartCoroutine(respawnRoutine);
            }

        }
    }
    public void Explode()
    {
        Transform explosionObjectPool = GameObject.Find("GameControl/ObjectPool/Explosions").transform;
        GameObject prefab = Instantiate(explosionPrefab, transform.position, Quaternion.identity, explosionObjectPool) as GameObject;
        Destroy(prefab, 5f);
    }
    public void ShipRecovery(int amt)
    {
        health += amt;
        if (health >= 100)
        {
            health = 100;
        }

    }
    public IEnumerator Respawn()
    {
        ShipModel.SetActive(false);
        shipCollider.enabled = false;
        yield return new WaitForSeconds(respawnSpeed);
        ShipModel.SetActive(true);
        shipCollider.enabled = true;
        shipDestroyed = false;
        health = 100;
        shield = 100;
        shieldDamaged = false;
    }
}
