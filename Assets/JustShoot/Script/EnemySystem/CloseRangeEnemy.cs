using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Todo: longrangeó�� �� ���� ����°� ��������
public class CloseRangeEnemy : Enemy, IDamagable
{
    private Combat combat = new Combat();
    protected override void Start()
    {
        base.Start();
        player = Player.Instance;
        Debug.Assert(player != null);// �÷��̾ ���̸� ���
        combat.Init(transform, 100f);

        combat.OnDead += Dead;
    }

    //�� ���� �ִϸ��̼ǿ��� �����
    public void OnAimationAttak()
    {
        //DealDamage
        float distance = Vector3.Distance(player.transform.position, transform.position);
        bool closeEnogh = distance <= attackDistance;

        Vector3 enemyToPlayerDir = (-transform.position + player.transform.position).normalized;
        //���� https://www.falstad.com/dotproduct/
        bool inAttackDirection = Vector3.Dot(transform.forward, enemyToPlayerDir) > .8f; // dot product �� ���� ���� ����� ���� ��ġ������ ������ ����ϸ� ������

        bool damagable = closeEnogh && inAttackDirection;

        if (damagable)//Todo: ���� �Ÿ� ����� �ٽ� �ϰ� ���� �� ����
        {
            //�÷��̾�� ������ �߰�
            Debug.Log("Player Damaged!!");
        }
    }

    public void TakeDamage(float damage)
    {
        combat.TakeDamage(damage);
    }

    private void Dead()
    {
        isDie = true;
    }
}
