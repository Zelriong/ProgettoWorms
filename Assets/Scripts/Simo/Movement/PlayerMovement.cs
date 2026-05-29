using System;
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
    [SerializeField] private Animator animator;

    [Header("Movimento")]
    [SerializeField] float movementSpeed;
    [SerializeField] private float moveEnergy;
    private float currentEnergy;
    [SerializeField] float jumpForce;

    [Header("Mira")]
    [SerializeField] GameObject aim;
    private float rotationValue;
    [SerializeField] float rotationSpeed;
    [SerializeField] Image aimCharge;
    [SerializeField] float chargeSpeed;

    [Header("Sparo")]
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject spawnPoint;
    public float power;
    private float minPower;
    [SerializeField] float maxPower = 250;
    public Vector2 launchDirection;


    [Header("SFX")]
    [SerializeField] AudioClip _charging;
    [SerializeField] AudioClip[] jumping;
    [SerializeField] AudioClip walk;
    [SerializeField] AudioClip[] shooting;
    [SerializeField] AudioClip[] wormSelected;

    //Vector2 moveDirection;

    float isJumping;
    bool charging;
    private bool canShoot;

    public LayerMask mask;

    public float direction;
    float cd = 0;
    GameState state;

    public static event Action onPlayerMove;

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

        gameObject.transform.position = controlledWorm.transform.position;

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
            else
            {
                rotationValue = 0f;
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
            else if (aim.transform.rotation.eulerAngles.z <= 90.5f)
                aim.transform.rotation = Quaternion.Euler(0, 0, 90.5f);
        }
        else if (direction == -1f)
        {
            if (aim.transform.rotation.eulerAngles.z <= 180.5f)
                aim.transform.rotation = Quaternion.Euler(0, 0, 180.5f);
            else if (aim.transform.rotation.eulerAngles.z >= 269.5f)
                aim.transform.rotation = Quaternion.Euler(0, 0, 269.5f);
        }

        // if (aim.transform.rotation.eulerAngles.z >= 179.5f)
        //     aim.transform.rotation = Quaternion.Euler(0, 0, 179.5f);
        // else if (aim.transform.rotation.eulerAngles.z <= 0.5f)
        //     aim.transform.rotation = Quaternion.Euler(0, 0, 0.5f);
    }

    private void RegulateForce()
    {
        //power = Mathf.Clamp(power, minPower, maxPower);
        power += chargeSpeed * Time.deltaTime;
        // if (InputManager.IsMoving(out Vector2 moveDirection))
        // {
        //     if (moveDirection.y <= -0.1f)
        //     {
        //         power -= chargeSpeed * Time.deltaTime;
        //     }
        //     else if (moveDirection.y >= 0.1f)
        //     {
        //         power += chargeSpeed * Time.deltaTime;
        //     }
        // }
        aimCharge.fillAmount = power / maxPower;
        if (power >= maxPower)
            StopShooting();
    }

    private void FixedUpdate()
    {
        if (InputManager.IsMoving(out Vector2 moveDirection))
        {
            if (IsGrounded() && state == GameState.moving && currentEnergy > 0f) Move(moveDirection);
        }
        else if (IsGrounded())
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsTakingDamage", false);
        }
        else
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsMoving", false);
        }
    }

    void Move(Vector2 moveDirection)
    {
        animator.SetBool("IsMoving", true);
        onPlayerMove?.Invoke();
        currentEnergy -= Time.fixedDeltaTime;
        
        cd += Time.fixedDeltaTime;

        if (cd > walk.length/2) //cooldown per evitare che le clip "clippino"
        {
            SoundFXManager.instance.PlaySoundFXClip(walk, transform, 1f);
            cd = 0f;
        }
        //rb.linearVelocity = new Vector2(moveDirection.x * movementSpeed * Time.fixedDeltaTime, 0f);

        Vector2 horMove = new Vector2(moveDirection.x, 0f);
        rb.MovePosition(rb.position + horMove * (movementSpeed * Time.fixedDeltaTime));
        
        //animator.SetBool("IsJumping", false);
    }

    private bool IsGrounded()
    {
        /*bool isGrounded*/
        ;
        RaycastHit2D rayHit = Physics2D.CapsuleCast(capsuleCollider2d.bounds.center,
            capsuleCollider2d.bounds.size,
            0f, 0f, Vector2.down, 0.15f, mask);

        //Collider2D colliders = Physics2D.OverlapCircle(boxCollider2d.bounds.center, 1f, 6);

        //if (rayHit == null) return false;

        //else return true;a

        return rayHit;
    }

    void Jump()
    {
        if (!IsGrounded() || state != GameState.moving || currentEnergy <= 0f)
            return;
        rb.AddForce(new Vector2(0.8f * direction, 1f) * jumpForce, ForceMode2D.Impulse);
        int rand = UnityEngine.Random.Range(0, jumping.Length);
        SoundFXManager.instance.PlaySoundFXClip(jumping[rand], transform, 1f);
        //animator.SetBool("IsJumping", true);
    }

    //on right click start
    private void IsShooting()
    {

        if (IsGrounded() && state == GameState.moving && canShoot)
        {
            state = GameState.aiming;
            aim.transform.position = controlledWorm.transform.position;
            aim.SetActive(true);
            //StartCoroutine(Shooting());
            int rand = UnityEngine.Random.Range(0, shooting.Length);
            SoundFXManager.instance.PlaySoundFXClip(shooting[rand], transform, 1f);
        }
    }

    //on right click end
    private void CancelShooting()
    {
        if (state != GameState.aiming) return;

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
        SoundFXManager.instance.PlaySoundFXClip(_charging, transform, 1f);
        //StartCoroutine(Shoot());
    }

    //on left click end
    private void StopShooting()
    {
        if (state != GameState.shooting) return;

        //charging = false;
        aim.SetActive(false);
        aimCharge.gameObject.SetActive(false);

        launchDirection = (spawnPoint.transform.position - controlledWorm.transform.position).normalized;

        Instantiate(bullet, spawnPoint.transform.position, aim.transform.rotation);

        state = GameState.moving; //poi da cambiare in "fineTurno"

        power = 0f;
        canShoot = false;
    }

    private void ChangeWormRef(GameObject worm)
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsTakingDamage", false);
        }
        
        controlledWorm = worm;
        if (worm.TryGetComponent(out Rigidbody2D wormRB))
            rb = wormRB;
        if (worm.TryGetComponent(out CapsuleCollider2D wormCollider))
            capsuleCollider2d = wormCollider;

        SpriteRenderer wormSprite = worm.GetComponentInChildren<SpriteRenderer>();
        wormSpriteObj = wormSprite.gameObject;

        currentEnergy = moveEnergy;
        canShoot = true;

        int rand = UnityEngine.Random.Range(0, wormSelected.Length);
        SoundFXManager.instance.PlaySoundFXClip(wormSelected[rand] , transform, 1f);

        animator = worm.GetComponentInChildren<Animator>();
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
