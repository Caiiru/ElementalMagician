using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBall_Projectile_Script : MonoBehaviour
{
    public Vector3 direction;
    public float projectileSpeed;
    private int _damage;
    private Element _element;
    private Transform parentTransform;

    public void CreateProjectile(Vector3 startPos, Vector3 direction, int damage,Element _element, Transform parent)
    {
        this.direction = direction;
        transform.position = startPos;
        _damage = damage;
        this._element = _element;
        parentTransform = parent;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
        transform.GetComponent<Rigidbody2D>().AddForce(direction * projectileSpeed,ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.GetComponent<Entity>())
        {
            if (other.transform.GetComponent<Entity>() == GameManager.GetInstance().GetPlayerEntity())
            {
                other.GetComponent<Entity>().takeDamage(_damage,_element);
            }
        }

        if (other.transform != parentTransform)
        {
            Destroy(this.gameObject);
        }
    }
}
