using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMonster : Unit
{
    [SerializeField]
    private float lives = 2.0F;

    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    public override void ReceiveDamage()
    {
        lives--;
        if (lives == 0) Die();
       
    }
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.GetComponent<Bullet>();

        if (bullet)
        {
            ReceiveDamage();
        }
    }
	
}
