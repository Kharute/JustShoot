using UnityEngine;

//Todo �̸���Ģ ����
[System.Serializable]
public class EnemyCombat : Combat
{
    public EnemyCombat(Transform owner, float maxHp, bool defaultEffectOnDamaged = true) : base(owner, maxHp, defaultEffectOnDamaged)
    {
        OnDamagedWDamage += SpawnDamageNumberUi;
    }

    private void SpawnDamageNumberUi(float damage)
    {
        EffectManager.Instance.DamageNumberUiGenerate(_owner.transform.position , damage);
    }
}