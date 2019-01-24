using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public enum PlayerWeaponType{KNIFE = 0, WAND = 1, STAFF = 2, ORB = 3}
public class PlayerBehavior : LivingEntity
{
	public Transform hitTestPivot;
    public int daggerDamage = 1;
    public GameObject daggerPref;
    float daggerAttackTime=0.4f;

    public Animator anim;

    public static int normalProjectile = 200;
    public static float specialProjectile = 100;
    public static int heavyProjectile = 10;

    PlayerWeaponType currentWeapon;
    int currenWeaponIndex = 0;
    float wheelThreshold = 0.09f;

    Misc_Timer attackTimer= new Misc_Timer();
    [HideInInspector] public GunController gunController;
    // Use this for initialization
    void Awake() {
        gunController = GetComponent<GunController>();
    }
	protected override void Start ()
    {
        startingHealth = PlayerPrefs.GetFloat("starting health");

        base.Start();

		SetWeapon (PlayerWeaponType.KNIFE);
		attackTimer.StartTimer (0.1f);
	}

    // Update is called once per frame
    void Update() {

        InputController();

        attackTimer.UpdateTimer();
    }


    public void InputController()
    {
        switch (currentWeapon)
        {
            case PlayerWeaponType.KNIFE:
                if ((Input.GetButtonDown("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 1) && attackTimer.IsFinished())
                {
                    Attack();
                }
                break;
            case PlayerWeaponType.WAND:
                if ((Input.GetButton("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 1))
                {
                    gunController.OnTriggerHold();
                }
                if ((Input.GetButtonUp("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 0))
                {
                    gunController.OnTriggerRelease();
                }
                break;
            case PlayerWeaponType.STAFF:
                if ((Input.GetButton("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 1))
                {
                    gunController.OnTriggerHold();
                }
                if ((Input.GetButtonUp("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 0))
                {
                    gunController.OnTriggerRelease();
                }
                break;
            case PlayerWeaponType.ORB:
                if ((Input.GetButton("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 1))
                {
                    gunController.OnTriggerHold();
                }
                if ((Input.GetButtonUp("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 0))
                {
                    gunController.OnTriggerRelease();
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeapon(PlayerWeaponType.WAND);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeapon(PlayerWeaponType.STAFF);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetWeapon(PlayerWeaponType.ORB);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetWeapon(PlayerWeaponType.KNIFE);
        }

        currenWeaponIndex = (int)currentWeapon;
        float mouseScrollDir = CrossPlatformInputManager.GetAxisRaw("Mouse ScrollWheel");

        if ((mouseScrollDir > wheelThreshold) || CrossPlatformInputManager.GetButtonDown("ChangeWeaponDown"))
        {

            currenWeaponIndex--;
            if (currenWeaponIndex < 0)
            {
                currenWeaponIndex = 3;
            }
            currenWeaponIndex = Mathf.Clamp(currenWeaponIndex, 0, 3);
            SetWeapon((PlayerWeaponType)currenWeaponIndex);
        }
        else if ((mouseScrollDir < -wheelThreshold) || CrossPlatformInputManager.GetButtonDown("ChangeWeaponUp"))
        {
            currenWeaponIndex++;
            if (currenWeaponIndex > 3)
            {
                currenWeaponIndex = 0;
            }
            currenWeaponIndex = Mathf.Clamp(currenWeaponIndex, 0, 3);
            SetWeapon((PlayerWeaponType)currenWeaponIndex);
        }
    }

    public override void TakeHit(float damage)
    {
        
        base.TakeHit(damage);
        GameManager.UpdateHealth(health, startingHealth);
        DamagePlayer();
        //print("PlayerHP: " + health);
    }
    public void DamagePlayer(){
        if (health <= 0)
        {
            GameManager.RegisterPlayerDeath();
            gameObject.GetComponent<Collider>().enabled = false;
            //Vector3 pos = animator.transform.position;
            //pos.y = 0.2f;
            //animator.transform.position = pos;
        }
	}

	public void Attack(){
		switch (currentWeapon) {
			case PlayerWeaponType.KNIFE:							
				Invoke ("DoHitTest",0.2f);				
			break;
		}
		//animator.SetBool ("Attack", true);
		CancelInvoke ("AttackOver");
		Invoke ("AttackOver", daggerAttackTime);
		attackTimer.StartTimer (daggerAttackTime);

	}
	public void DoHitTest(){

		RaycastHit[] hits=Physics.SphereCastAll (hitTestPivot.position,2.0f, hitTestPivot.up);
		foreach(RaycastHit hit in hits){
			if (hit.collider!=null && hit.collider.tag == "Enemy") {
				RaycastHit forwarHit= new RaycastHit();
				Physics.Raycast(hitTestPivot.position,hit.transform.position-transform.position,out forwarHit);
				if (forwarHit.collider!=null && forwarHit.collider.tag == "Enemy") {
                    hit.collider.GetComponent<LivingEntity>().TakeHit(daggerDamage);
                }
			}
		}
        anim.SetTrigger("Melee");
        AudioManager.instance.PlaySound("sword_attack", transform.position);
        
	}
	void AttackOver(){
		//animator.SetBool ("Attack", false);
	}
	
	void SetWeapon(PlayerWeaponType weaponType){
		if (weaponType != currentWeapon) {
			currentWeapon = weaponType;
            //animator.SetTrigger ("WeaponChange");
            switch (weaponType)
            {
                case PlayerWeaponType.KNIFE:
                    daggerAttackTime = 0.5f;
                    daggerPref.SetActive(true);
                    GameManager.UpdateAmmo(new Color32(0, 0, 0, 0));
                    gunController.DisableAllGuns();
                break;
                case PlayerWeaponType.WAND:
                    gunController.EquipGun(0);
                    GameManager.UpdateAmmo(PlayerPrefs.GetInt("normal_ammo"), normalProjectile, "normal_ammo");
                    GameManager.UpdateAmmo(new Color32(40, 215, 105, 200));
                    daggerPref.SetActive(false);
                break;
                case PlayerWeaponType.STAFF:
                    gunController.EquipGun(1);
                    GameManager.UpdateAmmo(PlayerPrefs.GetInt("special_ammo"), specialProjectile, "special_ammo");
                    GameManager.UpdateAmmo(new Color32(255, 150, 0, 200));
                    daggerPref.SetActive(false);
                break;
                case PlayerWeaponType.ORB:
                    gunController.EquipGun(2);
                    GameManager.UpdateAmmo(PlayerPrefs.GetInt("heavy_ammo"), heavyProjectile, "heavy_ammo");
                    GameManager.UpdateAmmo(new Color32(185, 115, 230, 200));
                    daggerPref.SetActive(false);
                break;
            }
		}
		//GameManager.SelectWeapon (weaponType);
	}
}
