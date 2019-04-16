using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour {
    Rigidbody2D rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, -1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground" || collision.gameObject.tag == "piece") {
            rb.Sleep();
        }
    }
}
