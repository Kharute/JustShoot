using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeEnemy : Enemy
{
    private void Start()
    {
        Debug.Assert(player != null);// �÷��̾ ���̸� ���
    }

    //�� ���� �ִϸ��̼ǿ��� �����
    public void CloseDamageToPlayer()
    {
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
}
