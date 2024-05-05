using UnityEngine;

//Todo �̸���Ģ ����
[System.Serializable]
public class EnemyCombat : Combat
{
    public EnemyCombat() 
    {
        OnDamagedWDamage += SpawnDamageNumberUi;
    }

    private void SpawnDamageNumberUi(float damage)
    {
        EffectManager.Instance.DamageNumberUiGenerate(_owner.transform.position , damage);
    }
}