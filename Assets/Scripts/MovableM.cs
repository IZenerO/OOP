﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovableM : DefaultMonster
{
    [SerializeField]
    private float speed = 2.0F;

    [SerializeField]
    private Bullet bullet;

    private SpriteRenderer sprite;
    private Vector3 direction;


    public override void ReceiveDamage()
    {
        Die();        
    }

    protected override void Awake()
    {
        bullet = Resources.Load<Bullet>("Bullet");
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Start()
    {
        direction = transform.right;
    }

    protected override void Update()
    {
        Move();
    }
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if(unit && unit is Character)
        {
            if (Mathf.Abs(unit.transform.position.x - transform.position.x) < 0.6F) ReceiveDamage();
            else unit.ReceiveDamage();
        }
    }
    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5F + transform.right * direction.x * 0.5F, 0.1F);
        if (colliders.Length > 0 && colliders.All(x => !x.GetComponent<Character>())) direction *= -1.0F;
        Collider2D[] colliders1 = Physics2D.OverlapCircleAll(transform.position + transform.up * -0.5F + transform.right * direction.x * 0.5F, 0.1F);
        if (colliders1.Length < 1) direction *= -1.0F;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }
}
