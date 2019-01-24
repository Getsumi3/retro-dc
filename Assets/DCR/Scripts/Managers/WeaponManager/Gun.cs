using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public enum ProjectileType {NORMAL, SPECIAL, HEAVY }
public class Gun : MonoBehaviour
{
    public enum GunMode { Bullets, Laser }
    public GunMode gunMode;
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;

    public LineRenderer lineRenderer;
    public float laserDamage = 3f;
    public GameObject laserParticle;

    public Transform[] projectileSpawn;
    public Transform hitTestPivot;

    public Proyectile_Simple projectile;
    public float msBetweenShots = 100;
    public float projectileVelocity = 35;
    public int projectilePerBurst;
    public ProjectileType projectileType;
    public float projectileDamage = 1f;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3, 5);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    [Header("Effects")]
    public AudioClip shootAudio;

    MuzzleFlash muzzleflash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    float maxProjectile;
    [HideInInspector] public float projectilesRemainingInMag;
    string prefsKey;
    Color ammoTypeColor;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    void Start()
    {
        muzzleflash = GetComponent<MuzzleFlash>();

        switch (gunMode)
        {
            case GunMode.Bullets: break;
            case GunMode.Laser:
                lineRenderer.enabled = false;
                break;
        }

        shotsRemainingInBurst = projectilePerBurst;


        switch (projectileType)
        {
            case ProjectileType.NORMAL:
                prefsKey = "normal_ammo";
                projectilesRemainingInMag = PlayerPrefs.GetInt(prefsKey, PlayerBehavior.normalProjectile);
                maxProjectile = PlayerBehavior.normalProjectile;
                ammoTypeColor = new Color32(40, 215, 105, 200);
                break;
            case ProjectileType.SPECIAL:
                prefsKey = "special_ammo";
                projectilesRemainingInMag = PlayerPrefs.GetInt(prefsKey, (int)PlayerBehavior.specialProjectile);
                maxProjectile = PlayerBehavior.specialProjectile;
                ammoTypeColor = new Color32(255, 150, 0, 200);
                break;
            case ProjectileType.HEAVY:
                prefsKey = "heavy_ammo";
                projectilesRemainingInMag = PlayerPrefs.GetInt(prefsKey, PlayerBehavior.heavyProjectile);
                maxProjectile = PlayerBehavior.heavyProjectile;
                ammoTypeColor = new Color32(185, 115, 230, 200);
                break;
        }
    }

    void LateUpdate()
    {
        // animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (projectilesRemainingInMag == 0)
        {
            GameManager.UpdateAmmo(projectilesRemainingInMag, maxProjectile, prefsKey);
            //AudioManager.instance.PlaySound(noAmmoSound, transform.position);
        }
    }

    void Shoot()
    {

        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                if (projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Proyectile_Simple newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Proyectile_Simple;
                newProjectile.SetSpeed(projectileVelocity);
                newProjectile.damage = projectileDamage;


                GameManager.UpdateAmmo(projectilesRemainingInMag, maxProjectile, prefsKey);
                AlertEnemies();
            }
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    IEnumerator Laser()
    {
        lineRenderer.enabled = true;
        laserParticle.SetActive(true);
        while (Input.GetButton("Fire1") || CrossPlatformInputManager.GetAxisRaw("Fire1") == 1)
        {
            if (projectilesRemainingInMag <= 0)
            {
                break;
            }
            Ray ray = new Ray(lineRenderer.transform.position, lineRenderer.transform.forward);
            RaycastHit hit;
            lineRenderer.SetPosition(0, ray.origin);
            projectilesRemainingInMag -= 0.5f;
            GameManager.UpdateAmmo(projectilesRemainingInMag, maxProjectile, prefsKey);
            if (Physics.Raycast(ray, out hit, 100))
            {
                lineRenderer.SetPosition(1, hit.point);
                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(lineRenderer.transform.forward * 5, hit.point);
                    
                }
                DamageTarget(hit);
                
            }
            else
            {
                lineRenderer.SetPosition(1, ray.GetPoint(100));

            }
            AudioManager.instance.PlaySound(shootAudio, transform.position);
            yield return null;
            
        }
        lineRenderer.enabled = false;
        laserParticle.SetActive(false);
    }

    void DamageTarget(RaycastHit hit)
    {
        IDamageable damageableObject = hit.transform.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(laserDamage);
        }
    }

    void AlertEnemies()
    {
        RaycastHit[] hits = Physics.SphereCastAll(hitTestPivot.position, 20.0f, hitTestPivot.up);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<NPC_Enemy>().SetAlertPos(transform.position);
            }
        }
    }

    public void OnTriggerHold()
    {
        switch (gunMode)
        {
            case GunMode.Bullets: Shoot(); break;
            case GunMode.Laser: StopCoroutine("Laser"); StartCoroutine("Laser"); break;
        }
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = projectilePerBurst;
    }
}
