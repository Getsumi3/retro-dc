using UnityEngine;
using System.Collections;
public enum NPC_EnemyState{IDLE_STATIC,IDLE_ROAMER,IDLE_PATROL,INSPECT,ATTACK,FIND_WEAPON,KNOCKED_OUT,DEAD,NONE}
public enum NPC_WeaponType{KNIFE,RIFLE,SHOTGUN, LASER}
public class NPC_Enemy : LivingEntity {
    public float damage = 1;
    public float enemySpeed = 12;
    public float inspectTimeout; //Once the npc reaches the destination, how much time unitl in goes back.
	public UnityEngine.AI.NavMeshAgent navMeshAgent;
	public Animator npcAnimator;

    private Loot loot;
    public GameObject proyectilePrefab;
    public GameObject beamPrefab;
    delegate void InitState();
	delegate void UpdateState();
	delegate void EndState();
	InitState _initState;
	InitState _updateState;
	InitState _endState;
	public NPC_WeaponType weaponType=NPC_WeaponType.KNIFE;
	public NPC_EnemyState idleState=NPC_EnemyState.IDLE_ROAMER;
	NPC_EnemyState currentState=NPC_EnemyState.NONE;
	Vector3 targetPos,startingPos;
    public LayerMask hitTestLayer;
	float weaponRange;
	public Transform weaponPivot;
    public Transform laserPivot;
    float weaponActionTime,weaponTime; //cast time and reload time.
	int hashSpeed;
    public NPC_PatrolNode patrolNode;
    public AudioClip shootSound;
    public bool randomizeWeapon = false;

    public bool isBoss = false;
    public GameObject exitPortal;
    // Use this for initialization

    protected override void Start () {
        base.Start();
        if (isBoss)
        {
            GameManager.isBossRoom = true;
        }
        loot = GetComponent<Loot>();
		startingPos = transform.position;
		hashSpeed = Animator.StringToHash ("Speed");
		SetWeapon (weaponType);
		SetState (idleState);
    }

    void SetWeapon(NPC_WeaponType newWeapon){
        npcAnimator.SetTrigger("WeaponChange");
		npcAnimator.SetInteger ("WeaponType", (int)weaponType);
		switch (weaponType) {
			case NPC_WeaponType.KNIFE:
				weaponRange=2.5f;
				weaponActionTime=0.53f;
				weaponTime=0.53f;
			break;
			case NPC_WeaponType.RIFLE:
				weaponRange=20.0f;
				weaponActionTime=0.90f/2;
				weaponTime=0.975f/2;
			break;
		case NPC_WeaponType.SHOTGUN:
			weaponRange=20.0f;
			weaponActionTime=1.80f/2;
			weaponTime=0.95f;
			break;
        case NPC_WeaponType.LASER:
            weaponRange = 100.0f;
            weaponActionTime = 1.60f;
            weaponTime = 3f;
            break;
        }
	}
	// Update is called once per frame
	void Update () {
		_updateState ();

		npcAnimator.SetFloat (hashSpeed, navMeshAgent.velocity.magnitude);
    }
	public void SetState(NPC_EnemyState newState){
		if (currentState != newState) {
			if(_endState!=null)
				_endState();
			switch(newState){
				case NPC_EnemyState.IDLE_STATIC:  _initState=StateInit_IdleStatic; 	_updateState=StateUpdate_IdleStatic; 	_endState=StateEnd_IdleStatic; 	break;				
				case NPC_EnemyState.IDLE_ROAMER:  _initState=StateInit_IdleRoamer; 	_updateState=StateUpdate_IdleRoamer; 	_endState=StateEnd_IdleRoamer; 	break;			
				case NPC_EnemyState.IDLE_PATROL:  _initState=StateInit_IdlePatrol; 	_updateState=StateUpdate_IdlePatrol; 	_endState=StateEnd_IdlePatrol; 	break;			
				case NPC_EnemyState.INSPECT:  _initState=StateInit_Inspect; 	_updateState=StateUpdate_Inspect; 	_endState=StateEnd_Inspect; 	break;			
				case NPC_EnemyState.ATTACK:  _initState=StateInit_Attack; 	_updateState=StateUpdate_Attack; 	_endState=StateEnd_Attack; 	break;			
			}
			_initState();			
			currentState=newState;					
		}
	}

	void UpdateSensors(){
		
	}

	///////////////////////////////////////////////////////// STATE: IDLE STATIC


	void StateInit_IdleStatic(){
        npcAnimator.SetTrigger("Idle");
        navMeshAgent.SetDestination (startingPos);
        //navMeshAgent.Resume ();
        navMeshAgent.isStopped = false;
        AudioManager.instance.PlaySound("enemy_idle", transform.position);
    }
	void StateUpdate_IdleStatic(){	
	
	}
	void StateEnd_IdleStatic(){	
	}
	///////////////////////////////////////////////////////// STATE: IDLE PATROL
	
	
	void StateInit_IdlePatrol(){
		navMeshAgent.speed = 6.0f;
        npcAnimator.SetTrigger("Run");
        npcAnimator.ResetTrigger("Idle");
        navMeshAgent.SetDestination (patrolNode.GetPosition ());
        AudioManager.instance.PlaySound("enemy_idle", transform.position);
    }
	void StateUpdate_IdlePatrol(){	
		if (HasReachedMyDestination ()) {
			patrolNode=patrolNode.nextNode;
			navMeshAgent.SetDestination (patrolNode.GetPosition ());
		}
		
	}
	void StateEnd_IdlePatrol(){	
	}

	///////////////////////////////////////////////////////// STATE: IDLE ROAMER


	Misc_Timer idleTimer=new Misc_Timer();
	Misc_Timer idleRotateTimer=new Misc_Timer();
	bool idleWaiting,idleMoving;
	void StateInit_IdleRoamer(){
        navMeshAgent.speed = 7.0f;
        idleTimer.StartTimer (Random.Range (2.0f, 4.0f));
		RandomRotate ();
		AdvanceIdle ();
		idleWaiting = false;
		idleMoving = true;
        npcAnimator.ResetTrigger("Idle");
        npcAnimator.SetTrigger("Run");
        AudioManager.instance.PlaySound("enemy_idle", transform.position);
    }
    void StateUpdate_IdleRoamer(){	
	
		idleTimer.UpdateTimer ();
	
		if (idleMoving) {
            npcAnimator.SetTrigger("Run");
            npcAnimator.ResetTrigger("Idle");
            if (HasReachedMyDestination ()) {
				AdvanceIdle();

			}
		} else if(idleWaiting){
			idleRotateTimer.UpdateTimer ();
            npcAnimator.SetTrigger("Idle");
            npcAnimator.ResetTrigger("Run");
            if (	idleRotateTimer.IsFinished()){
				RandomRotate();
				idleRotateTimer.StartTimer(Random.Range(1.5f,3.25f));
			}
		
		}
		if (idleTimer.IsFinished ()) {
			if(idleMoving){
                //navMeshAgent.Stop();
                navMeshAgent.isStopped = true;
                float waitTime=Random.Range (2.5f,6.5f);
				float randomTurnTime=waitTime/2.0f;
				idleRotateTimer.StartTimer (randomTurnTime);
				idleTimer.StartTimer (waitTime);

			
			}
			else if(idleWaiting){
				idleTimer.StartTimer (Random.Range (2.0f, 4.0f));

				AdvanceIdle();
			}

			idleMoving=!idleMoving;
			idleWaiting=!idleMoving;

		}

	}
	void StateEnd_IdleRoamer(){	
	}

	void AdvanceIdle(){

		RaycastHit hit = new RaycastHit ();
		Physics.Raycast (transform.position, transform.forward*5.0f, out hit,50.0f,hitTestLayer);
        //Debug.DrawRay (transform.position, transform.forward, Color.red);
        
        if (hit.distance < 3.0f) {
			Vector3 dir =  hit.point-transform.position;
			Vector3 reflectedVector = Vector3.Reflect (dir,hit.normal);	
			Physics.Raycast (transform.position,reflectedVector, out hit,50.0f,hitTestLayer);
		}

        //navMeshAgent.Resume();
        navMeshAgent.isStopped = false;
		navMeshAgent.SetDestination (hit.point);
    }
	///////////////////////////////////////////////////////// STATE: INSPECT
	Misc_Timer inspectTimer = new Misc_Timer ();
	Misc_Timer inspectTurnTimer = new Misc_Timer ();
	bool inspectWait;
	void StateInit_Inspect(){
        navMeshAgent.speed = enemySpeed;
		//navMeshAgent.Resume ();
        navMeshAgent.isStopped = false;

        npcAnimator.SetTrigger("Run");
        npcAnimator.ResetTrigger("Idle");
        inspectTimer.StopTimer ();
		inspectWait = false;

    }
	void StateUpdate_Inspect(){	


		if (HasReachedMyDestination () && !inspectWait) {
            npcAnimator.SetTrigger("Idle");
            npcAnimator.ResetTrigger("Run");
            inspectWait =true;
			inspectTimer.StartTimer (2.0f);
			inspectTurnTimer.StartTimer(1.0f);
        }
		navMeshAgent.SetDestination (targetPos);
		RaycastHit hit = new RaycastHit ();
		Physics.Raycast (transform.position,transform.forward, out hit,weaponRange,hitTestLayer);

		if (hit.collider != null && hit.collider.tag == "Player") {
			SetState(NPC_EnemyState.ATTACK);
		}
		if (inspectWait) {
			inspectTimer.UpdateTimer ();
			inspectTurnTimer.UpdateTimer();
			if (inspectTurnTimer.IsFinished ()) {
				RandomRotate ();
				inspectTurnTimer.StartTimer (Random.Range (0.5f, 1.25f));
			}
			if (inspectTimer.IsFinished ())
				SetState (idleState);
		}
    }
	void StateEnd_Inspect(){	
	}

	///////////////////////////////////////////////////////// STATE: ATTACK
	Misc_Timer attackActionTimer=new Misc_Timer();
	bool actionDone;
	void StateInit_Attack(){

        //navMeshAgent.Stop ();
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
		npcAnimator.SetTrigger ("Attack");
        //npcAnimator.ResetTrigger("Run");

        CancelInvoke ("AttackAction");
		Invoke ("AttackAction", weaponActionTime);
		attackActionTimer.StartTimer (weaponTime);

		actionDone = false;
	}
	void StateUpdate_Attack(){	
		attackActionTimer.UpdateTimer ();
		if (!actionDone && attackActionTimer.IsFinished ()) {
			EndAttack();

			actionDone=true;
		}
	}
	void StateEnd_Attack(){	
		
    }
	void EndAttack(){
		SetState (NPC_EnemyState.INSPECT);
	}
	void AttackAction(){

        if (randomizeWeapon)
        {
            RandomWeapon();
        }

        switch (weaponType) {
			case NPC_WeaponType.KNIFE:
			RaycastHit[] hits=Physics.SphereCastAll (weaponPivot.position,2.0f, weaponPivot.forward);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null && hit.collider.tag == "Player")
                    {
                        hit.collider.GetComponent<LivingEntity>().TakeHit(damage);
                    }

                }
                AudioManager.instance.PlaySound("sword_attack", transform.position);

			break;
			case NPC_WeaponType.RIFLE:
				GameObject bullet=GameObject.Instantiate(proyectilePrefab, weaponPivot.position,weaponPivot.rotation) as GameObject;
                bullet.GetComponent<Proyectile_Simple>().damage = damage;
                bullet.transform.Rotate(0,Random.Range(-7.5f,7.5f),0);
                AudioManager.instance.PlaySound(shootSound, transform.position);
                break;
			case NPC_WeaponType.SHOTGUN:
                for (int i=0;i<5;i++){
					GameObject birdshot=GameObject.Instantiate(proyectilePrefab, weaponPivot.position,weaponPivot.rotation) as GameObject;
                    birdshot.GetComponent<Proyectile_Simple>().damage = damage;
                    birdshot.transform.Rotate(0,Random.Range(-15,15),0);
				}
                AudioManager.instance.PlaySound(shootSound, transform.position);
                break;
            case NPC_WeaponType.LASER:
                GameObject beam = GameObject.Instantiate(beamPrefab, laserPivot.position, laserPivot.rotation) as GameObject;
                beam.GetComponent<Proyectile_Simple>().damage = damage;
                //bullet.transform.Rotate(0, Random.Range(-7.5f, 7.5f), 0);
                beam.transform.parent = laserPivot;
                AudioManager.instance.PlaySound(shootSound, transform.position);
                break;
        }
	}
	////////////////////////// MISC FUNCTIONS //////////////////////////

	void RandomRotate(){
		float randomAngle =Random.Range (45, 180);
		float randomSign = Random.Range (0, 2);
		if (randomSign == 0)
			randomAngle *= -1;

		transform.Rotate (0, randomAngle, 0);
	}
    void RandomWeapon()
    {
        int randomWeapon = Random.Range(1, 4);
        weaponType = (NPC_WeaponType)randomWeapon;
        SetWeapon(weaponType);
        print("WEAPON INDEX: " + (int)weaponType);
    }
    /*float randomMoveInnerRadius=0.5f, randomMoveOuterRadius=10.0f;
	private Vector3 GetRandomPoint(){	
		Vector3 newPos;
		//do{
			newPos=Random.insideUnitSphere * randomMoveOuterRadius;
		//}while(newPos.x <randomMoveInnerRadius && newPos.y<randomMoveInnerRadius);
		Vector3 finalPos = transform.position + newPos;

		return finalPos;
	}*/
    public bool HasReachedMyDestination(){
		float dist = Vector3.Distance (transform.position, navMeshAgent.destination);
		if ( dist<= 1.5f) {
			return 	true;
		}
		
		return false;
	}
	////////////////////////// PUBLIC FUNCTIONS //////////////////////////
	public void SetAlertPos(Vector3 newPos){
		if (idleState != NPC_EnemyState.IDLE_STATIC) {
			SetTargetPos(newPos);
		}
	}
	public void SetTargetPos(Vector3 newPos){
		targetPos = newPos;
		if (currentState != NPC_EnemyState.ATTACK ) {
			SetState (NPC_EnemyState.INSPECT);
		}
	}

    public override void TakeHit(float damage)
    {
        
        base.TakeHit(damage);
        Damage();
    }

    public void Damage(){
        if (health <= 0)
        {
            
            AudioManager.instance.PlaySound("enemy_death", transform.position);
            navMeshAgent.velocity = Vector3.zero;
            //navMeshAgent.Stop ();
            npcAnimator.SetTrigger("Dead");
            npcAnimator.StopPlayback();

            GameManager.AddScore(100);
                        
            npcAnimator.transform.parent = null;
            Vector3 pos = npcAnimator.transform.position;
            //pos.y = 0.2f;
            npcAnimator.transform.position = pos;

            loot.DropItem(pos);
            GameManager.AddToEnemyCount();
            if (isBoss)
            {
                GameManager.AddScore(9999);
                exitPortal.SetActive(true);
                EnemySpawner[] enemySpawners = FindObjectsOfType<EnemySpawner>();
                foreach (EnemySpawner spawner in enemySpawners)
                {
                    spawner.enabled = false;
                }
                NPC_Enemy[] enemies = FindObjectsOfType<NPC_Enemy>();
                foreach (NPC_Enemy _Enemy in enemies)
                {
                    Destroy(_Enemy.gameObject);
                }
                PlayerPrefs.SetFloat("starting health", PlayerPrefs.GetFloat("starting health") + 5);
                GameManager.isBossRoom = false;
            }
            Destroy(npcAnimator.gameObject, 3f);
            Destroy(gameObject);
        }
	}


}
