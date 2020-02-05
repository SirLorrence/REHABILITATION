using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class CharacterController : MonoBehaviour
{
    [Header("Ship Settings")]
    public AudioClip ShipEngineSfx;
    public float RotateSpeed;
    public float SnappingSpeed;
    public float MoveSpeed;
    public float StrafeSpeed;
    public float moveVelocity;
    public float strafeVelocity;
    public float GamepadSensitivity;
    public float decelerationSpeed = 20f;
    public float shakeScreenStrength;
    public float rotationlerp;
    public float increaseSpeed;
    public float decreaseSpeed;
    public Image fuelFillAmt;
    float fuel = 100;
    public Transform[] bulletEmitters;
    AudioSource audioSource;
    Transform bulletObjectPool;
    bool shootRight;
    public float fireRate = 0.13f;
    bool isSnapping;
    public AudioClip shootFx;
    AudioSource audioSrc;
    public GameObject bulletPrefab;
    Rigidbody rb;
    Transform playerTransform;
    public static Vector2 moveAxis;
    public static Vector2 lookAxis;
    Vector3 StrafeAbscissaX;
    Vector3 StrafeOrdinateY;
    Vector3 StrafeApplicateZ;
    Vector3 currentVelocity;
    Vector3 oppositeVelocity;
    float nextFire;
    float OrdinateMovement;
    float SensitivityMultiplier = 50f;
    bool Locked;
    bool isBoosting;
    bool isMoving;
    float returnVel;
    [Space]
    [Header("Ship Effect")]
    public float lerpAmt = 1;
    public float lerpSpeed = 1;
    public GameObject shipModel;
    float MoveOnX;
    float MoveOnY;
    Vector3 defaultPos = Vector3.zero;
    Vector3 newPos;
    Vector3 originalPos;
    bool resetCamera;
    bool isShake = false;
    bool isLooking = false;
    Vector3 defaultRot;
    Vector3 newRot;
    bool isRefueling;
    bool outOfFuel = false;
    public ParticleSystem[] psSystems;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultPos = transform.localPosition;
        returnVel = moveVelocity;
        originalPos = Camera.main.transform.localPosition;
        audioSource = GameObject.Find("GameControl/AudioSystem/PlayerEngine").GetComponent<AudioSource>();
        audioSource.volume = 0.3f;
    }
   
    void Update()
    {
        handler();

        if (isRefueling && fuel < 100)
        {
            outOfFuel = false;
            fuel += Time.deltaTime * increaseSpeed; 
        }
        else if (fuel > 99 && isRefueling)
        {
            fuel = 100;
            isRefueling = false;
        }


        if (isShake)
        {
            ShakeScreen(shakeScreenStrength);
            resetCamera = true;
        }
        else if (!isShake && resetCamera)
        {
            Camera.main.transform.localPosition = originalPos;
            resetCamera = false;
        }
        currentVelocity = rb.velocity;
        oppositeVelocity = -currentVelocity;
        if (Gamepad.current != null)
        {
            if (Gamepad.current.leftTrigger.isPressed && Time.time > nextFire && !ShipSystem.shipDestroyed || Gamepad.current.rightTrigger.isPressed && Time.time > nextFire && !ShipSystem.shipDestroyed)
            {
                audioSrc = GetComponent<AudioSource>();
                if (moveAxis.x > 0.1f || moveAxis.x < 0.1f || moveAxis.y > 0.1f || moveAxis.y < 0.1f)
                    audioSrc.pitch = 1.1f;
                else
                {
                    audioSrc.pitch = 1;

                }
                audioSrc.PlayOneShot(shootFx);
                nextFire = Time.time + fireRate;
                Shoot();
            }
            if (Gamepad.current.buttonSouth.isPressed && !outOfFuel)
            {
                if (fuel > 0)
                {
                    if(!audioSource.isPlaying)
                        audioSource.PlayOneShot(ShipEngineSfx);
                    foreach (ParticleSystem psSystem in psSystems)
                    {
                        Vector3 shapeSize = new Vector3(0, 0.1f, 1);
                        var shape = psSystem.shape;
                        shape.scale = shapeSize;
                    }
                    moveVelocity = returnVel * 10;
                    fuel -= Time.deltaTime * decreaseSpeed;
                    isRefueling = false;
                    isShake = true;
                }
                else if (fuel < 0)
                {
                    
                    foreach (ParticleSystem psSystem in psSystems)
                    {
                        Vector3 shapeSize = new Vector3(0, 0.1f, 0);
                        var shape = psSystem.shape;
                        shape.scale = shapeSize;
                    }
                    fuel = 0;
                    outOfFuel = true;
                    moveVelocity = returnVel;
                    isShake = false;
                }

            }
            else if (!Gamepad.current.buttonSouth.isPressed)
            {
                
                foreach (ParticleSystem psSystem in psSystems)
                {
                    Vector3 shapeSize = new Vector3(0, 0.1f, 0);
                    var shape = psSystem.shape;
                    shape.scale = shapeSize;
                }
                isRefueling = true;
                moveVelocity = returnVel;
                isShake = false;
            }
        }
        float LookX = lookAxis.x; // -1.0..1.0 ??
        float LookY = lookAxis.y; // -1.0..1.0 ??


        //Vector3 newRotation = shipModel.transform.eulerAngles;
        //newRot = new Vector3(defaultRot.x + lookAxis.x, defaultRot.y + lookAxis.y, defaultRot.z);
        //newRotation.x = Mathf.MoveTowards(newRotation.x, newRot.x * 15, rotationlerp * Time.deltaTime);
        //newRotation.y = Mathf.MoveTowards(newRotation.y, newRot.y * 15, rotationlerp * Time.deltaTime);
        //shipModel.transform.eulerAngles = newRotation;
        if (!ShipSystem.shipDestroyed)
        {
            Vector3 newRotation = shipModel.transform.localEulerAngles;
            newRotation.x = lookAxis.y * -20; // -15.0..15.0 degrees 
            newRotation.y = lookAxis.x * 20; // -15.0..15.0 degrees
            shipModel.transform.localEulerAngles = newRotation;
        }

        if (!ShipSystem.shipDestroyed)
        {
            if (LookX > 0)
            {

                transform.Rotate(Vector3.up * LookX * GamepadSensitivity * SensitivityMultiplier * Time.deltaTime, Space.Self);
                //if (!isMoving)
                //    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
            if (LookX < 0)
            {
                transform.Rotate(Vector3.down * -LookX * GamepadSensitivity * SensitivityMultiplier * Time.deltaTime, Space.Self);
                //if (!isMoving)
                //    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
            if (LookY > 0)
            {
                transform.Rotate(Vector3.left * LookY * GamepadSensitivity * SensitivityMultiplier * Time.deltaTime, Space.Self);
                //if (!isMoving)
                //    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
            }
            if (LookY < 0)
            {
                transform.Rotate(Vector3.right * -LookY * GamepadSensitivity * SensitivityMultiplier * Time.deltaTime, Space.Self);
                //if (!isMoving)
                //    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);

            }
        }
        float AbscissaMovement = moveAxis.x;
        float ApplicateMovement = moveAxis.y;

        StrafeAbscissaX = (AbscissaMovement * transform.right);
        StrafeOrdinateY = (transform.forward);
        StrafeApplicateZ = (ApplicateMovement * transform.up);

       

        if (moveAxis.x < 0.1f && moveAxis.y < 0.1f && moveAxis.x > -0.1f && moveAxis.y > -0.1f)
            isMoving = false;
        else
            isMoving = true;

        if (!isMoving && !ShipSystem.shipDestroyed)
        {
            rb.AddForce(StrafeOrdinateY * MoveSpeed * Time.deltaTime);
            if (rb.velocity.magnitude > moveVelocity)
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveVelocity);
        }
       
        if (Gamepad.current.rightShoulder.isPressed)
        {
            isSnapping = false;
            transform.Rotate(Vector3.forward * RotateSpeed * Time.deltaTime, Space.Self);
        }
        else if (Gamepad.current.leftShoulder.isPressed)
        {
            isSnapping = false;
            transform.Rotate(Vector3.back * RotateSpeed * Time.deltaTime, Space.Self);
        }
        else if (!Gamepad.current.leftTrigger.isPressed || !Gamepad.current.rightTrigger.isPressed)
        {
            isSnapping = true;
        }



        if (isSnapping && !ShipSystem.shipDestroyed)
        {
            float Axisz = transform.rotation.eulerAngles.z;

            // 0-45------------------------------------------------------------------------------------
            if (Axisz > 4f && Axisz < 41f)
            {
                transform.Rotate(Vector3.back * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }

            // 45-90-----------------------------------------------------------------------------------
            if (Axisz > 49f && Axisz < 86f)
            {
                transform.Rotate(Vector3.forward * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }

            // 90-135----------------------------------------------------------------------------------
            if (Axisz > 94f && Axisz < 131f)
            {
                transform.Rotate(Vector3.back * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }

            // 135 - 180-------------------------------------------------------------------------------
            if (Axisz > 139f && Axisz < 176f)
            {
                transform.Rotate(Vector3.forward * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }

            // 180-225---------------------------------------------------------------------------------
            if (Axisz > 184f && Axisz < 221f)
            {
                transform.Rotate(Vector3.back * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }

            // 225-270---------------------------------------------------------------------------------
            if (Axisz > 229f && Axisz < 266f)
            {
                transform.Rotate(Vector3.forward * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }

            // 270-315---------------------------------------------------------------------------------
            if (Axisz > 274f && Axisz < 311f)
            {
                transform.Rotate(Vector3.back * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }

            // 315-360---------------------------------------------------------------------------------
            if (Axisz > 319f && Axisz < 356f)
            {
                transform.Rotate(Vector3.forward * SnappingSpeed * Time.deltaTime, Space.Self);
                Locked = true;
            }
        }


    }
    public void ShakeScreen(float Strength)
    {
        Camera.main.transform.localPosition = originalPos + Random.insideUnitSphere * Strength;
    }
    private void LateUpdate()
    {
        if (isMoving)
        {
            MoveOnX = moveAxis.x * lerpAmt;
            MoveOnY = moveAxis.y * lerpAmt;
        }
        else
        {
            MoveOnX = lookAxis.x * lerpAmt;
            MoveOnY = lookAxis.y * lerpAmt;
        }
        newPos = new Vector3(defaultPos.x + MoveOnX, defaultPos.y + MoveOnY, defaultPos.z);
        shipModel.transform.localPosition = Vector3.MoveTowards(shipModel.transform.localPosition, newPos, lerpSpeed * Time.deltaTime);


    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
       

        if (isMoving && !ShipSystem.shipDestroyed)
        {
            
            rb.AddForce(StrafeAbscissaX * StrafeSpeed * Time.deltaTime * 2f);
            rb.AddForce(StrafeApplicateZ * StrafeSpeed * Time.deltaTime * 2.0f);


            if (rb.velocity.magnitude > moveVelocity)
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, strafeVelocity);
        }
       
        
    }
    void handler()
    {
        fuelFillAmt.fillAmount = Map(fuel, 0, 100, 0, 1);
    }
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
    void Shoot()
    {
        bulletObjectPool = GameObject.Find("GameControl/ObjectPool/Bullets").transform;
        if(shootRight)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletEmitters[0].transform.position, transform.rotation, bulletObjectPool.transform) as GameObject;
            Destroy(bullet.gameObject, 15f);
            shootRight = false;
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletEmitters[1].transform.position, transform.rotation, bulletObjectPool.transform) as GameObject;
            Destroy(bullet.gameObject, 15f);
            shootRight = true;
          
        }
        
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveAxis = ctx.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookAxis = ctx.ReadValue<Vector2>();
    }
    public void ResetVelocity()
    {
        rb.velocity = new Vector3(0, 0, 0);
    }
}
