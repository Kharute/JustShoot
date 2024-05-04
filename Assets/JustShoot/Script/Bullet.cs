using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected float baseDmg;
    protected float lifeTime;

    public virtual void Init(float dmg, float velocity, float lifeTime)//�Ҹ��� ������, �ӵ�, �����ð� �ʱ�ȭ.
    {
        this.baseDmg = dmg;
        this.lifeTime = lifeTime;
        GetComponent<Rigidbody>().velocity = this.transform.forward * velocity;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0 )
        {
            ObjectPoolManager.Instance.EnqueueObject(this.gameObject);//�� ������Ʈ �ı�
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;

        if (collision.gameObject.CompareTag("Enemy"))//��ź�� ����� �±װ� Enemy�� ���, ����Ʈ�� ��Ƣ��� ����Ʈ�� ����
        {
            type = 1;
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg);
        }
        else if (collision.gameObject.CompareTag("Player"))//��ź�� ����� �±װ� Player�� ���, ����Ʈ�� ��Ƣ��� ����Ʈ�� ����
        {
            type = 1;
            collision.gameObject.GetComponent<IDamagable>().TakeDamage(baseDmg);
        }
        ProjectileDestroy(collision.contacts[0].point, type);//�浹 ��ġ�� �Ѱܼ� ���� ó���� �ϴ� ����.
    }    
    void ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.HitEffectGenenate(hitPosition, type);//��ź ����Ʈ �߻�
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);//�� ������Ʈ �ı�
    }
}
