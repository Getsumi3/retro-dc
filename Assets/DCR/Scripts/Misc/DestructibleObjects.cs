using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : MonoBehaviour {

    public Transform brokenPrefab;
    public int durability = 2;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Projectile")
        {
            durability--;
            if (durability <= 0)
            {
                Transform brokenGO = Instantiate(brokenPrefab, transform.position, transform.rotation);
                Destroy(brokenGO.gameObject, 5f);
                Destroy(gameObject);
            }
        }
    }
}
