using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Grenade : Bullet
{
    [SerializeField]GameObject fragPrf; //���� ������
    private void OnCollisionEnter(Collision collision)
    {
        int type = 0;
        //if (collision.gameObject.CompareTag("Enemy"))//��ź�� ����� �±װ� Enemy�� ���, ����Ʈ�� ��Ƣ��� ����Ʈ�� ����
        //{
        //    type = 1;
        //}

        //���⿡ ���� �������� ������ ����� �߰��� ��.
        ProjectileDestroy(collision.contacts[0].point, type);//�浹 ��ġ�� �Ѱܼ� ���� ó���� �ϴ� ����.
    }
    void ProjectileDestroy(Vector3 hitPosition, int type)
    {
        EffectManager.Instance.ExplosionEffectGenerate(hitPosition, 1);

        for (int i = 0; i < 100; i++)
        {
            GameObject frag = ObjectPoolManager.Instance.DequeueObject(fragPrf);
            frag.transform.position = hitPosition + new Vector3(0, 1, 0);
            frag.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            frag.GetComponent<Bullet>().Init(baseDmg * 0.01f, 100, 0.2f);
        }

        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }
}
