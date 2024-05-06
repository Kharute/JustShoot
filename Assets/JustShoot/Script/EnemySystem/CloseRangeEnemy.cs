using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CloseRangeEnemy : BaseEnemy
{
    public enum State
    {
        IDLE, TRACE, ATTACK, DEAD
    }
    public State state = State.IDLE;

    public float traceDistance = 9999f;
    public float attackDistance = 1.5f;
    public float aimRotateSpeed = 30f;

    public float maxHp = 100f;

    protected override void Awake()
    {
        base.Awake();
        combat = new EnemyCombat(transform, maxHp);

        statemachine = gameObject.AddComponent<Statemachine>();
        statemachine.AddState(State.IDLE, new IdleState(this));
        statemachine.AddState(State.TRACE, new TraceState(this));
        statemachine.AddState(State.ATTACK, new AttackState(this));
        statemachine.AddState(State.DEAD, new DeadState(this));
        statemachine.InitState(State.IDLE);

        agent.destination = playerTrf.position;
    }
    protected override void Start()
    {
        base.Start();
        StartCoroutine(CheckEnemyState());
    }
    protected virtual IEnumerator CheckEnemyState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);

            float distance = Vector3.Distance(playerTrf.position, enemyTrf.position);
            if (distance <= attackDistance)
            {
                statemachine.ChangeState(State.ATTACK);
                state = State.ATTACK;
            }
            else if (distance <= traceDistance)
            {
                statemachine.ChangeState(State.TRACE);
                state = State.TRACE;
            }
            else
            {
                statemachine.ChangeState(State.IDLE);
                state = State.IDLE;
            }
        }
        statemachine.ChangeState(State.DEAD);
        state = State.DEAD;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        audioSource.clip = traceSFX;
        audioSource.Play();
    }

    //�� ���� �ִϸ��̼ǿ��� �����
    public void OnAimationAttak()
    {
        audioSource.PlayOneShot(attackSFX);
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
            player.TakeDamage(10f);
            int type = 1;
            Vector3 hitPosition = player.transform.position + Vector3.up;
            EffectManager.Instance.HitEffectGenenate(hitPosition, type);//��ź ����Ʈ �߻�
        }
    }
    class BaseEnemyState : BaseState
    {
        protected CloseRangeEnemy owner;
        public BaseEnemyState(CloseRangeEnemy owner)
        {
            this.owner = owner;
        }
    }

    class IdleState : BaseEnemyState
    {
        public IdleState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.isStopped = true;
            owner.animator.SetBool(owner.hashTrace, false);
        }
    }

    class TraceState : BaseEnemyState
    {
        public TraceState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.agent.SetDestination(owner.playerTrf.position);

            owner.agent.isStopped = false;
            owner.animator.SetBool(owner.hashTrace, true);
            owner.animator.SetBool(owner.hashAttack, false);
        }
        public override void Update()
        {
            owner.agent.SetDestination(owner.playerTrf.position);
            bool moving = owner.agent.velocity.magnitude >= .01f;
            if (moving)
            {
                owner.animator.SetBool(owner.hashMoving, true);
            }
            else
            {
                owner.animator.SetBool(owner.hashMoving, false);
            }

        }
    }

    class AttackState : BaseEnemyState
    {
        public AttackState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetBool(owner.hashAttack, true);
        }
        public override void Update()
        {
            bool moving = owner.agent.velocity.magnitude >= .01f;
            if (moving)
            {
                owner.animator.SetBool(owner.hashMoving, true);
            }
            else
            {
                owner.animator.SetBool(owner.hashMoving, false);
            }

            Vector3 pos = owner.transform.position;
            Vector3 target = owner.playerTrf.position;
            Vector3 desiredDir = -pos + target;
            desiredDir = new Vector3(desiredDir.x, 0f, desiredDir.z);
            desiredDir = desiredDir.normalized;
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, Quaternion.LookRotation(desiredDir), Time.deltaTime * owner.aimRotateSpeed);
        }
    }
    class DeadState : BaseEnemyState
    {
        public DeadState(CloseRangeEnemy owner) : base(owner) { }

        public override void Enter()
        {
            owner.animator.SetTrigger(owner.hashDead);
            owner.animator.SetBool(owner.hashIsDead, true);
            owner.agent.isStopped = true;
            owner.audioSource.Stop();
            owner.audioSource.PlayOneShot(owner.deadSFX);
        }
    }
}
