using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Rocket : Bullet
{
    Rigidbody rigidbody;
    float velocity;
    public override void Init(float dmg, float velocity, float lifeTime)//�Ҹ��� ������, �ӵ�, �����ð� �ʱ�ȭ.
    {
        this.baseDmg = dmg;
        this.lifeTime = lifeTime;
        rigidbody = GetComponent<Rigidbody>();
        this.velocity = velocity;
    }

    private void FixedUpdate()
    {
        if(lifeTime > 4.5f)
        {
            rigidbody.AddForce(this.transform.forward * velocity * 2, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))//��ź�� ����� �±װ� Enemy�� ��� �������� ����
        {
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg);
        }
        ProjectileDestroy(collision.contacts[0].point);//�浹 ��ġ�� �Ѱܼ� ���� ó���� �ϴ� ����.
    }
    void ProjectileDestroy(Vector3 hitPosition)
    {
        EffectManager.Instance.ExplosionEffectGenerate(hitPosition, 2);
        SplashDamage(hitPosition, 10);

        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }

    void SplashDamage(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius);//��� �ݶ��̴� ����

        foreach (Collider hit in colliders)//�ݶ��̴� ��ȸ
        {
            Rigidbody targetRigidbody;
            if(hit.gameObject.TryGetComponent<Rigidbody>(out targetRigidbody))
            {
                targetRigidbody.AddForce((hit.transform.position - center).normalized * 3, ForceMode.VelocityChange);
            }

            if (hit.gameObject.CompareTag("Enemy"))
            {
                hit.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg * (radius - (center - hit.transform.position).magnitude) / radius);//���� ����. �Ÿ��� ���� �������� ���������� ����
            }
        }
    }
}
