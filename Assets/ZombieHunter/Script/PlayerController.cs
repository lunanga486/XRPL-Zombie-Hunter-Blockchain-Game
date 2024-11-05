using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public enum GunHandlerState { AVAILABLE, SWAPPING, RELOADING, EMPTY }
public enum ShootingMethob { SingleShoot, AutoShoot}
public enum WEAPON_STATE { MELEE, GUN}
[RequireComponent(typeof(PlayerSpineHelper))]
public class PlayerController : MonoBehaviour, ICanTakeDamage
{
    [Header("SET UP")]

    public Animator anim;
    public float moveSpeed = 10;
    public float limitAbovePos = -0.8f;
    public float limitBelowPos = -3.65f;
    [ReadOnly] public float limitLeft;
    [ReadOnly] public float limitRight;
    public AudioClip soundHurt, soundDie;

    [Header("BLOCK LAYER")]
    public LayerMask blockWayLayerMask;

    [Header("HEALTH")]
    [Range(0, 5000)]
    public int health = 100;
    public Vector2 healthBarOffset = new Vector2(0, 1.5f);

    float currentHealth;

    [Header("GRENADE")]
    public GameObject grenade;
    public Transform throwPoint;

    [Header("WEAPONS")]
    [ReadOnly] public WEAPON_STATE weaponState;
    [ReadOnly] public GunHandlerState GunState;
    [ReadOnly] public GunTypeID gunTypeID;
    public LayerMask targetLayer;
    float lastTimeShooting = -999;
    public AudioClip throwGrenadeSound;
    bool allowShooting = true;
    protected HealthBarEnemyNew healthBar;
    PlayerSpineHelper playerSpineHelper;
    public bool isFacingRight { get { return transform.rotation.eulerAngles.y == 0; } }

    void Start()
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        GameManager.Instance.Player = this;
        gunTypeID = GunManager.Instance.getGunID();
        SetGun(gunTypeID);
        GunManager.Instance.ResetGunBullet();

        playerSpineHelper = GetComponent<PlayerSpineHelper>();
        playerMeleeWeapon = GetComponent<PlayerMeleeWeapon>();

        currentHealth = health;
        var healthBarObj = (HealthBarEnemyNew)Resources.Load("HealthBar", typeof(HealthBarEnemyNew));
        healthBar = (HealthBarEnemyNew)Instantiate(healthBarObj, healthBarOffset, Quaternion.identity);

        healthBar.Init(transform, (Vector3)healthBarOffset);
    }

    private void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.Player = this;
    }
    public Vector2 finalSpeed;

    public void GetLimitHorizontal()
    {
        limitLeft = Camera.main.ViewportToWorldPoint(Vector3.zero).x + 0.5f;
        limitRight = Camera.main.ViewportToWorldPoint(Vector3.right).x - 0.5f;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        GetInput();

        finalSpeed = input * moveSpeed * Time.deltaTime;

        GetLimitHorizontal();
        if (finalSpeed.x > 0 && transform.position.x >= limitRight)
            finalSpeed.x = 0;
        else if (finalSpeed.x < 0 && transform.position.x <= limitLeft)
            finalSpeed.x = 0;

        if ((finalSpeed.x > 0 && !isFacingRight) || (finalSpeed.x < 0 && isFacingRight))
            Flip();

       

        if (!isWayBlocked())
        {
            transform.Translate(finalSpeed, Space.World);

            if(transform.position.y > limitAbovePos)
            {
                transform.position = new Vector3(transform.position.x, limitAbovePos, transform.position.z);
            }else if (transform.position.y < limitBelowPos)
            {
                transform.position = new Vector3(transform.position.x, limitBelowPos, transform.position.z);
            }
        }

        healthBar.transform.localScale = new Vector2(transform.localScale.x > 0 ? Mathf.Abs(healthBar.transform.localScale.x) : -Mathf.Abs(healthBar.transform.localScale.x), healthBar.transform.localScale.y);
        AnimSetFloat("speed", input.magnitude);
    }

    bool isWayBlocked()
    {
        return Physics2D.Raycast(transform.position, input, 0.2f, blockWayLayerMask);
    }

    Vector2 input;
    void GetInput()
    {
        input = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal") + Input.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical") + Input.GetAxis("Vertical"));

        if (gunTypeID.shootingMethob == ShootingMethob.SingleShoot)
        {
            if (CrossPlatformInputManager.GetButtonDown("Shoot"))
            {
                Shoot();
            }
        }

        else if (gunTypeID.shootingMethob == ShootingMethob.AutoShoot)
        {
            if (CrossPlatformInputManager.GetButton("Shoot"))
            {
                Shoot();
            }
        }


        if (CrossPlatformInputManager.GetButtonDown("Melee"))
        {
            MeleeAttack();
        }
    }

    void Flip()
    {
        transform.rotation = Quaternion.Euler(0, isFacingRight ? 180 : 0, 0);
    }

    public void AnimSetTrigger(string name)
    {
        anim.SetTrigger(name);
    }

    public void AnimSetSpeed(float value)
    {
        if (anim)
            anim.speed = value;
    }

    public void AnimSetFloat(string name, float value)
    {
        anim.SetFloat(name, value);
    }

    public void AnimSetBool(string name, bool value)
    {
        anim.SetBool(name, value);
    }

    public void SetState(GunHandlerState state)
    {
        GunState = state;
    }

    private void SetAvailabeAfterSwap()
    {

        SetState(GunHandlerState.AVAILABLE);
        Debug.Log("SetAvailabeAfterSwap");
        CheckBulletRemain();
    }

    public void Shoot()
    {
        if (weaponState == WEAPON_STATE.MELEE)
        {
            SetGun(GunManager.Instance.getGunID());
            return;
        }

        if (!allowShooting || gunTypeID.bullet <= 0)
            return;

        if (Time.time < (lastTimeShooting + gunTypeID.rate))
            return;

        if (GunState != GunHandlerState.AVAILABLE)
            return;

        lastTimeShooting = Time.time;
        gunTypeID.bullet--;
        AnimSetTrigger("shoot");
        for (int i = 0; i < gunTypeID.maxBulletPerShoot; i++)
        {
            StartCoroutine(FireCo());
        }

        if (gunTypeID.shellFX)
        {
            Vector2 shellPos = gunTypeID.shellPoint.position;
            var _tempFX = SpawnSystemHelper.GetNextObject(gunTypeID.shellFX, true);
            _tempFX.transform.position = shellPos;
        }

        SoundManager.PlaySfx(gunTypeID.soundFire, gunTypeID.soundFireVolume);
        SoundManager.PlaySfx(gunTypeID.shellSound, gunTypeID.shellSoundVolume);

        CancelInvoke("CheckBulletRemain");
        Invoke("CheckBulletRemain", gunTypeID.rate);

        if (gunTypeID.dualShot)
            Invoke("ShootSecondGun", gunTypeID.fireSecondGunDelay);
    }

    void ShootSecondGun()
    {
        //gunTypeID.bullet--;
        //SubtractBullet(1);
        for (int i = 0; i < gunTypeID.maxBulletPerShoot; i++)
        {
            StartCoroutine(FireCo());
        }
        SoundManager.PlaySfx(gunTypeID.soundFire, gunTypeID.soundFireVolume);
        SoundManager.PlaySfx(gunTypeID.shellSound, gunTypeID.shellSoundVolume);
    }

    public IEnumerator FireCo()
    {
        
        yield return null;

        var _dir = (isFacingRight ? Vector2.right : Vector2.left) + new Vector2(0, Random.Range(-(1f - gunTypeID.accuracy), (1f - gunTypeID.accuracy)));
        RaycastHit2D hit = Physics2D.Raycast(playerSpineHelper.GetFireWorldPoint() + (isFacingRight ? Vector2.left : Vector2.right), _dir, 100, targetLayer);

        if (gunTypeID.muzzleTracerFX)
        {
            var _tempFX = SpawnSystemHelper.GetNextObject(gunTypeID.muzzleTracerFX, true);
            _tempFX.transform.position = playerSpineHelper.GetFireWorldPoint();
            _tempFX.transform.right = _dir;
        }

        if (gunTypeID.muzzleFX)
        {
            var _muzzle = SpawnSystemHelper.GetNextObject(gunTypeID.muzzleFX, playerSpineHelper.GetFireWorldPoint(), true);

            _muzzle.transform.right = (isFacingRight ? Vector2.right : Vector2.left);
            //_muzzle.transform.parent = firePoint;
        }

        if (hit)
        {
            var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (takeDamage != null)
            {
                var finalDamage = (int)(Random.Range(gunTypeID.minPercentAffect * 0.01f, 1f) * gunTypeID.UpgradeRangeDamage);

                takeDamage.TakeDamage(finalDamage, Vector2.zero, hit.point, gameObject);
            }
        }

        if (gunTypeID.reloadPerShoot)
        {
            StartCoroutine(ReloadGunSub());
        }
    }

    void CheckBulletRemain()
    {
        if (gunTypeID.bullet <= 0)
        {
            GunManager.Instance.NextGun();
        }
    }

    public void ReloadGun()
    {
        SetState(GunHandlerState.RELOADING);
        //SoundManager.PlaySfx (soundReload, soundReloadVolume);
        AnimSetTrigger("reload");
        AnimSetBool("reloading", true);
        Invoke("ReloadComplete", gunTypeID.reloadTime);

        SoundManager.PlaySfx(gunTypeID.reloadSound, gunTypeID.reloadSoundVolume);
    }

    IEnumerator ReloadGunSub()
    {
        SetState(GunHandlerState.RELOADING);
        AnimSetBool("isReloadPerShootNeeded", true);

        yield return new WaitForSeconds(gunTypeID.reloadTime);

        SetState(GunHandlerState.AVAILABLE);
        AnimSetBool("isReloadPerShootNeeded", false);
    }

    public void ReloadComplete()
    {
        lastTimeShooting = Time.time;
        AnimSetBool("reloading", false);
        SetState(GunHandlerState.AVAILABLE);
    }

    public void ThrowGrenade()
    {
        var obj = (GameObject)SpawnSystemHelper.GetNextObject(grenade, false);
        SoundManager.PlaySfx(throwGrenadeSound);
        obj.GetComponent<Grenade>().SetDirection(isFacingRight);
        obj.transform.position = throwPoint.position;
        obj.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var isCollectItem = (ICanCollect)collision.GetComponent(typeof(ICanCollect));
        if (isCollectItem != null)
        {
            isCollectItem.Collect();
        }
    }

    public void SetGun(GunTypeID gunID)
    {
        weaponState = WEAPON_STATE.GUN;
        anim.runtimeAnimatorController = gunID.animatorOverride;
        gunTypeID = gunID;
        AnimSetTrigger("swap-gun");
        allowShooting = false;
        SoundManager.PlaySfx(SoundManager.Instance.swapGun);
        Invoke("AllowShooting", 0.3f);
    }

    void AllowShooting()
    {
        allowShooting = true;
    }

    #region MELEE ATTACK
    PlayerMeleeWeapon playerMeleeWeapon;

    public void SetMelee()
    {
        weaponState = WEAPON_STATE.MELEE;
        anim.runtimeAnimatorController = playerMeleeWeapon.animatorOverride;
        SoundManager.PlaySfx(playerMeleeWeapon.soundSwap);
        AnimSetTrigger("swap-gun");
    }

    public void MeleeAttack()
    {
        if (weaponState == WEAPON_STATE.GUN)
        {
            SetMelee();
            return;
        }
        if (Time.time > (playerMeleeWeapon.lastAttackTime + playerMeleeWeapon.rate))
        {
            playerMeleeWeapon.lastAttackTime = Time.time;
            AnimSetTrigger("melee-attack");
            
            Invoke("MeleeCheckEnemy", playerMeleeWeapon.delayToSync);
        }
    }

    void MeleeCheckEnemy()
    {
        SoundManager.PlaySfx(playerMeleeWeapon.soundAttack);
        RaycastHit2D hit = Physics2D.CircleCast(playerMeleeWeapon.checkPoint.position, playerMeleeWeapon.radiusCheck, Vector2.zero, 0, targetLayer);

        if (hit)
        {
            var takeDamage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (takeDamage != null)
            {
                var finalDamage = (int)(Random.Range(0.8f, 1f) * playerMeleeWeapon.damage);

                takeDamage.TakeDamage(finalDamage, new Vector2(5,0), hit.point, gameObject);
            }
        }
    }

    public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null, WEAPON_EFFECT forceEffect = WEAPON_EFFECT.NONE)
    {
        currentHealth -= damage;
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);
        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
            AnimSetTrigger("dead");
            SoundManager.PlaySfx(soundDie);
        }
        else
        {
            AnimSetTrigger("hurt");
            SoundManager.PlaySfx(soundHurt);
        }
    }
    #endregion

    public void AddHearth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, health);
        if (healthBar)
            healthBar.UpdateValue(currentHealth / (float)health);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, limitAbovePos, 0));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, limitBelowPos, 0));
    }
}