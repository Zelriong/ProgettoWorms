using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    moving,
    aiming,
    shooting,
    jumping,
}
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject controlledWorm;
    private GameObject wormSpriteObj;
    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider2d;
    private Vector3 wormSpriteScale;
    
    [Header("Movimento")]
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;

    [Header("Mira")]
    [SerializeField] GameObject aim;
    private float rotationValue;
    [SerializeField] float rotationSpeed;
    [SerializeField] Image aimCharge;
    [SerializeField] float chargeSpeed;

    [Header("Sparo")]
    [SerializeField] GameObject bullet;
    Transform spawnPoint;
    public float power;
    private float minPower;
    [SerializeField] float maxPower = 250;
    
    //Vector2 moveDirection;

    float isJumping;
    bool charging;

    public LayerMask mask;

    float direction;
    
    GameState state;

    private void Awake()
    {
        ChangeWormRef(controlledWorm);
    }

    private void OnEnable()
    {
        InputManager.OnJump += Jump;
        InputManager.OnAiming += IsShooting;
        InputManager.OnCancel += CancelShooting;
        InputManager.OnCharging += Shot;
        InputManager.OnShooting += StopShooting;

        TurnManager.onWormRefChanged += ChangeWormRef;
    }

    private void OnDisable()
    {
        InputManager.OnJump -= Jump;
        InputManager.OnAiming -= IsShooting;
        InputManager.OnCancel -= CancelShooting;
        InputManager.OnCharging -= Shot;
        InputManager.OnShooting -= StopShooting;

        TurnManager.onWormRefChanged += ChangeWormRef;
    }

    private void Start()
    {
        state = GameState.moving;
        
        aim.SetActive(false);
        aimCharge.gameObject.SetActive(false);
        
        direction = 1;

        minPower = maxPower / 10;
        
        wormSpriteScale = new Vector3(wormSpriteObj.transform.localScale.x, 
                    wormSpriteObj.transform.localScale.y, 
                    wormSpriteObj.transform.localScale.z);
    }
    
    private void Update()
    {
        //isJumping = InputManager.instance.inputs.Player.Jump.ReadValue<float>();

        if (InputManager.IsMoving(out Vector2 moveDirection))
        {
            if (moveDirection.x < 0) { direction = -1; } // se ci sportiamo a sx si gira a sx
            
            else if (moveDirection.x > 0) { direction = 1; } // se ci giriamo a dx si gira a dx
        }

        //if (moveDirection.x != 0) { anim.SetBool("IsMoving", true); }
        //else { anim.SetBool("IsMoving", false); }

        wormSpriteObj.transform.localScale = new Vector3(wormSpriteScale.x * direction, wormSpriteScale.y, wormSpriteScale.z);    //la scale va in base alla direction

        transform.localScale = new Vector3(direction, 
                                transform.localScale.y, 
                                transform.localScale.z);
        
        //runs only when right click is held
        if (state == GameState.aiming)
        {
            RegulateAim();
        }
        
        //runs only when left click is held
        if (state == GameState.shooting)
        {
            RegulateForce();
        }
    }
    
    private void RegulateAim()
    {
        if (InputManager.IsMoving(out Vector2 moveDirection))
        {
            if (moveDirection.y <= -0.1f)
            {
                rotationValue = -rotationSpeed * Time.deltaTime;
            }
            else if (moveDirection.y >= 0.1f)
            {
                rotationValue = rotationSpeed * Time.deltaTime;
            }
        }
        else
        {
            rotationValue = 0f;
        }
        
        Vector3 rotateZ = new Vector3(0f, 0f, rotationValue);
        aim.transform.Rotate(rotateZ);
        
        //Debug.Log(aim.transform.rotation.eulerAngles.z);

        //locks rotation from going over 180 degrees in one direction
        if (direction == 1f)
        {
            if (aim.transform.rotation.eulerAngles.z >= 179.5f)
                aim.transform.rotation = Quaternion.Euler(0, 0, 179.5f);
            else if (aim.transform.rotation.eulerAngles.z <= 0.5f)
                aim.transform.rotation = Quaternion.Euler(0, 0, 0.5f);
        }
        else if (direction == -1f)
        {
            if (aim.transform.rotation.eulerAngles.z >= 359.5f)
                aim.transform.rotation = Quaternion.Euler(0, 0, 359.5f);
            else if (aim.transform.rotation.eulerAngles.z <= 180.5f)
                aim.transform.rotation = Quaternion.Euler(0, 0, 180.5f);
        }
        
        // if (aim.transform.rotation.eulerAngles.z >= 179.5f)
        //     aim.transform.rotation = Quaternion.Euler(0, 0, 179.5f);
        // else if (aim.transform.rotation.eulerAngles.z <= 0.5f)
        //     aim.transform.rotation = Quaternion.Euler(0, 0, 0.5f);
    }

    private void RegulateForce()
    {
        power = Mathf.Clamp(power, minPower, maxPower); 
        if (InputManager.IsMoving(out Vector2 moveDirection))
        {
            if (moveDirection.y <= -0.1f)
            {
                power -= chargeSpeed * Time.deltaTime;
            }
            else if (moveDirection.y >= 0.1f)
            {
                power += chargeSpeed * Time.deltaTime;
            }
        }
        aimCharge.fillAmount = power / maxPower;
    }

    private void FixedUpdate()
    {
        if (IsGrounded() && state == GameState.moving) Move();
    }
    
    void Move()
    {
        if (InputManager.IsMoving(out Vector2 moveDirection))
        {
            //rb.linearVelocity = new Vector2(moveDirection.x * movementSpeed * Time.fixedDeltaTime, 0f);
            
            Vector2 horMove = new Vector2(moveDirection.x, 0f);
            rb.MovePosition(rb.position + horMove * (movementSpeed * Time.fixedDeltaTime));
        }
    }

    private bool IsGrounded()
    {
        /*bool isGrounded*/
        ;
        RaycastHit2D rayHit = Physics2D.CapsuleCast(capsuleCollider2d.bounds.center,
            capsuleCollider2d.bounds.size, 
            0f, 0f, Vector2.down, 0.1f, mask);

        //Collider2D colliders = Physics2D.OverlapCircle(boxCollider2d.bounds.center, 1f, 6);

        //if (rayHit == null) return false;

        //else return true;a

        return rayHit;
    }

    void Jump()
    {
        if (!IsGrounded())
            return;
        rb.AddForce(new Vector2(0.8f * direction, 1f) * jumpForce, ForceMode2D.Impulse);
    }

    //on right click start
    private void IsShooting()
    {
        
        if (IsGrounded() && state == GameState.moving)
        {
            state = GameState.aiming;
            aim.transform.position = controlledWorm.transform.position;
            aim.SetActive(true);
            //StartCoroutine(Shooting());
        }
    }

    //on right click end
    private void CancelShooting()
    {
        aim.SetActive(false);
        aimCharge.gameObject.SetActive(false);
        state = GameState.moving;
    }

    //on left click start
    private void Shot()
    {
        if (state != GameState.aiming) return;
        
        //charging = true;
        state = GameState.shooting;
        aimCharge.gameObject.SetActive(true);
        //StartCoroutine(Shoot());
    }

    //on left click end
    private void StopShooting()
    {
        if (state != GameState.shooting) return;
        
        //charging = false;
        aim.SetActive(false);
        aimCharge.gameObject.SetActive(false);
        
        Instantiate(bullet, spawnPoint.position, spawnPoint.rotation, transform);

        state = GameState.moving; //poi da cambiare in "fineTurno"

        power = 0f;
    }

    private void ChangeWormRef(GameObject worm)
    {
        controlledWorm = worm;
        if (worm.TryGetComponent(out Rigidbody2D wormRB))
            rb = wormRB;
        if (worm.TryGetComponent(out CapsuleCollider2D wormCollider))
            capsuleCollider2d = wormCollider;
        
        SpriteRenderer wormSprite = worm.GetComponentInChildren<SpriteRenderer>();
        wormSpriteObj = wormSprite.gameObject;
    }

    // IEnumerator Shooting()
    // {
    //     while (power < 0)
    //     {
    //         Vector3 rotateZ = new Vector3(0, 0, moveDirection.y);
    //         aim.transform.Rotate(rotateZ);
    //
    //         if (state != GameState.shooting) yield return null;
    //
    //         break;
    //     }
    //     yield return null;
    // }


    // IEnumerator Shoot()
    // {
    //     if (state != GameState.shooting) yield return null;
    //
    //     // while (charging == true)
    //     // {
    //     //     if (power >= maxPower) break;
    //     //
    //     //     power += chargeSpeed * Time.deltaTime;
    //     //     aimCharge.fillAmount = power / maxPower;
    //     //
    //     //     //if (Input.GetMouseButtonUp(0))
    //     //     //{
    //     //     //    Instantiate(bullet, spawnPoint.position, spawnPoint.rotation, transform);
    //     //     //    aim.gameObject.SetActive(false);
    //     //     //    break;
    //     //     //}
    //     //     break;
    //     // }
    //
    //     aimCharge.gameObject.SetActive(false);
    //     
    //     Instantiate(bullet, spawnPoint.position, spawnPoint.rotation, transform);
    //
    //     state = GameState.moving; //poi da cambiare in "fineTurno"
    //     
    //     yield return new WaitForSeconds(2f);
    //
    //     power = 0f;
    //
    //
    //     StopAllCoroutines();
    //
    // }
    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawCube(new Vector3(boxCollider2d.bounds.center.x, boxCollider2d.bounds.center.y - 0.1f, boxCollider2d.bounds.center.z), boxCollider2d.bounds.size);
    //     Gizmos.color = Color.greenYellow;
    // }
}
