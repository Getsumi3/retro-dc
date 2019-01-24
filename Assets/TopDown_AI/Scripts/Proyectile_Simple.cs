using UnityEngine;
using System.Collections;

public class Proyectile_Simple : MonoBehaviour {
    public enum CollisionTarget {PLAYER,ENEMIES }
    public CollisionTarget collisionTarget;
	public float lifeTime=3.0f;
	public float speed=1.5f;
    [HideInInspector]public float damage;

	bool hitTest=true;
	bool moving;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Start () {
		moving = true;
		Destroy (gameObject, lifeTime);
	}


	void Update () {

        if (moving)
        {
            transform.Translate(Vector3.forward * speed);
        }



    }

    private void OnTriggerEnter(Collider other)
    {
        if (collisionTarget == CollisionTarget.PLAYER && other.gameObject.tag == "Player")
        {
            DamageTarget(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision){
        if (collisionTarget== CollisionTarget.PLAYER  && collision.gameObject.tag == "Player") {
            DamageTarget(collision.gameObject);
            
        }
        else if (collisionTarget == CollisionTarget.ENEMIES && collision.gameObject.tag == "Enemy") {
            DamageTarget(collision.gameObject);            
        }
        else if(collision.gameObject.tag == "Finish"){ //This is to detect if the proyectile collides with the world, i used this tag because it is standard in Unity (To prevent asset importing issues)
            DestroyProyectile();
        }
        


    }

    void DamageTarget(GameObject collision)
    {
        IDamageable damageableObject = collision.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage);
        }
    }

	void DestroyProyectile(){
	
		/*hitTest=false;
		gameObject.GetComponent<Rigidbody> ().isKinematic = true;
		gameObject.GetComponent<Collider> ().enabled = false;
		moving = false;*/
		Destroy (gameObject);	
	}

}

