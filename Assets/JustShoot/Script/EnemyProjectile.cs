using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Bullet
{
    ParticleSystem ps;
    Rigidbody erb;
    private void Awake()
    {
        ps = transform.GetComponentInChildren<ParticleSystem>();
        erb = GetComponent<Rigidbody>();
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
            Player.Instance.TakeDamage(baseDmg);
        }
        StartCoroutine(ProjectileDestroy(collision.contacts[0].point, type));//�浹 ��ġ�� �Ѱܼ� ���� ó���� �ϴ� ����.
    }
    IEnumerator ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.HitEffectGenenate(hitPosition, type);//��ź ����Ʈ �߻�
        erb.velocity = Vector3.zero;
        ps.Stop();

        yield return new WaitForSeconds(3f);
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);//�� ������Ʈ �ı�
    }
}
