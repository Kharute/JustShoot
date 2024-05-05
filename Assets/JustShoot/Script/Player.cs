using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : SceneSingleton<Player>
{
    [SerializeField] Animator animator;

    Vector2 moveVector = Vector2.zero;
    Vector2 moveVectorTarget;
    float moveSpeed = 1;
    bool isFire = false;
    bool isZoom = false;
    Vector2 mouseDeltaPos = Vector2.zero;
    float senst;//ī�޶� ����
    int controlWeaponIndex;

    CharacterController cc;

    [SerializeField] GameObject tpsVCamRoot;
    [SerializeField] CinemachineVirtualCamera tpsVCam;   
    [SerializeField] Transform weaponPoint;

    [Header("UsingWeapons")]
    [SerializeField] List<Weapon> weaponsList;
    //[SerializeField] int[] usingWeaponsIndex = new int[3];
    [SerializeField] List<Weapon> usingWeapons;
    [SerializeField] Weapon controlweapon;

    //float fireDelay = 0;
    //float delayCount = 0.1f;
    //int shell = 100;

    bool isReload = false;
    bool isActive = false;

    public PlayerCombat combat = new PlayerCombat();
    PlayerCombatData data = new PlayerCombatData();

    // CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        for (int i = 0; i < weaponPoint.childCount; i++)
        {
            Weapon weapon;
            if(weaponPoint.GetChild(i).TryGetComponent<Weapon>(out weapon))
            {
                weaponsList.Add(weapon);
            }
        }
        tpsVCamRoot.transform.parent = null;

        WeaponSelect(0, 1, 2);//���� �ڵ�. ���� ���� UI�� �����Ǹ� �� �޼��� ȣ���� ������ ��
    }

    /// <summary>
    /// ���� �����ϴ� �޼���. ������ �Ű������� 0���� 7������ �ߺ����� �ʴ� int���� �����ϸ� ��. �� �޼��尡 �ݵ�� ����Ǿ�� ĳ���Ͱ� �����̱� ������.
    /// </summary>
    /// <param name="weaponIndex1"></param>
    /// <param name="weaponIndex2"></param>
    /// <param name="weaponIndex3"></param>
    public void WeaponSelect(int weaponIndex1, int weaponIndex2, int weaponIndex3)
    {
        usingWeapons.Add(weaponsList[weaponIndex1]);
        usingWeapons.Add(weaponsList[weaponIndex2]);
        usingWeapons.Add(weaponsList[weaponIndex3]);

        isActive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        combat.Init(transform, 100f);
        //tpsCmc = tpsVCam.GetComponent<CinemachineVirtualCamera>();        
        cc = GetComponent<CharacterController>();
        //impulseSource = GetComponent<CinemachineImpulseSource>();

        SetCamType(false);

        controlWeaponIndex = 0;
        WeaponChange(controlWeaponIndex);

        combat.OnDamaged += HitAnimPlay;
        combat.OnDead += DieAnimPlay;

        senst = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            MoveOrder();//�̵�  
            RotateOrder();//ĳ���� �� �ѱ� ȸ��

            WeaponSelect();
            //GunFire();//���� ���
            //Debug.Log(tpsVCam.transform.position);

            SetCombatData();
        }
        else
        {
            DeadCamMove();
        }
    }
    private void LateUpdate()
    {
        if (isActive)
        {
            CamRotate();//ī�޶� ȸ��
        }
    }
    private void MoveOrder()
    {        
        moveVector = Vector2.Lerp(moveVector, moveVectorTarget * moveSpeed, Time.deltaTime * 5);

        Vector3 moveVector3 = new Vector3(moveVector.x * 0.5f, Physics.gravity.y, moveVector.y);
        //moveVector3 = this.transform.rotation * moveVector3;
        
        cc.Move(this.transform.rotation * moveVector3 * Time.deltaTime);

        animator.SetFloat("X_Speed", moveVector.x);
        animator.SetFloat("Y_Speed", moveVector.y);
    }
    void CamRotate()
    {
        tpsVCamRoot.transform.position = this.transform.position + new Vector3(0, 1.5f, 0);

        Vector3 camAngle = tpsVCamRoot.transform.rotation.eulerAngles;

        
        if(isZoom)
        {
            mouseDeltaPos *= 0.2f * senst;
        }
        else
        {
            mouseDeltaPos *= 0.4f * senst;
        }

        float x = camAngle.x - mouseDeltaPos.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 25f);
        }
        else
        {
            x = Mathf.Clamp(x, 345f, 361f);
        }

        tpsVCamRoot.transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDeltaPos.x, camAngle.z);
        mouseDeltaPos *= 0.9f;
    }
    void DeadCamMove()
    {
        tpsVCam.transform.localPosition += new Vector3(1, 1, -1) * Time.deltaTime;
        tpsVCam.transform.LookAt(this.transform.position);
    }
    void RotateOrder()
    {
        Vector3 direction = (tpsVCam.transform.forward).normalized;
        
        Quaternion rotationWeapon = Quaternion.LookRotation(direction);
        rotationWeapon = Quaternion.Euler(rotationWeapon.eulerAngles.x, this.transform.rotation.eulerAngles.y, rotationWeapon.eulerAngles.z);
        weaponPoint.rotation = Quaternion.Slerp(weaponPoint.rotation, rotationWeapon, Time.deltaTime * controlweapon.Operability() * 0.4f);

        direction = new Vector3(direction.x, 0, direction.z);

        Quaternion rotationBody = Quaternion.LookRotation(direction);
        //rotationBody = Quaternion.Euler(0, rotationBody.eulerAngles.y, 0);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotationBody, Time.deltaTime * controlweapon.Operability() * 0.4f);
    }
    void SetCamType(bool isFps)
    {
        if (isFps)
        {
            tpsVCam.Priority = 9;
        }
        else
        {
            tpsVCam.Priority = 11;
        }
    }
    void WeaponSelect()
    {
        if (isReload == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                WeaponChange(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                WeaponChange(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                WeaponChange(2);
            }
        }
    }
    void WeaponChange(int index)
    {
        if(usingWeapons.Count > index)
        {
            for (int i = 0; i < usingWeapons.Count; i++)
            {
                if(index == i)
                {
                    usingWeapons[i].gameObject.SetActive(true);
                    controlweapon = usingWeapons[i];
                    controlWeaponIndex = index;
                }
                else
                {
                    usingWeapons[i].gameObject.SetActive(false);
                }
            }
        }
    }
    void GunFire()
    {
        if(isFire)//��ư ��Ŭ�� �Է��� �����Ǿ��� ��� (��Ŭ�� �ϴ� ���� ���)
        {
            //fireDelay = 0;

            //GameObject bulletIst = ObjectPoolManager.Instance.DequeueObject(bullet);
            //bulletIst.transform.position = firePoint.position;
            //bulletIst.transform.rotation = firePoint.rotation;

            //bulletIst.GetComponent<Rigidbody>().velocity = bulletIst.transform.forward * 500;

            //EffectManager.Instance.FireEffectGenenate(firePoint.position, firePoint.rotation);
            ////impulseSource.GenerateImpulse(this.transform.position);
            //mouseDeltaPos = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 3f));

            //animator.SetTrigger("Fire");
            //shell--;

            //WeaponŬ�������� �߻� ó���ϴ� ������ ����
        }
    }
    void Reload()
    {
        animator.SetTrigger("Reload");
        animator.SetFloat("ReloadSpeed", 4/controlweapon.GetReloadTime());
        controlweapon.ReloedStart();
        SFXManager.Instance.ReloadSoundOn(this.transform.position);
        StartCoroutine(ReloadEnd());
    }
    public void Recoil(float recoli)
    {
        mouseDeltaPos = new Vector2(Random.Range(-recoli, recoli), Random.Range(recoli, recoli*3)) * 0.12f;
    }
    IEnumerator ReloadEnd()
    {
        yield return new WaitForSeconds(controlweapon.GetReloadTime());
        isReload = false;
        controlweapon.ReloedEnd();
        //shell += 100;
        //shell = Mathf.Clamp(shell, 0, 101);
    }

    public void TakeDamage(float dmg)
    {
        combat.TakeDamage(dmg);
        if(isActive && combat.IsDead())
        {
            isActive = true;
            SetCamType(false);
        }
    }


    void OnMove(InputValue inputValue)//WASD ����
    {
        moveVectorTarget = inputValue.Get<Vector2>();//��ǲ ���� �޾ƿ�
        //moveVectorTarget = inputMovement;
        //Debug.Log(inputMovement);
    }
    void OnSprint(InputValue inputValue)
    {        
        float value = inputValue.Get<float>();
        moveSpeed = (value * 4) + 1;
        //Debug.Log(isClick);
    }
    void OnLeftClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        if (isActive)
        {
            float isClick = inputValue.Get<float>();

            if (isClick == 1)//������ ��
            {
                isFire = true;
            }
            else//�� ��
            {
                isFire = false;
            }
            controlweapon.SetTrigger(isFire);
        }
    }
    void OnRightClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        if (isActive)
        {
            float isClick = inputValue.Get<float>();

            if (isClick == 1)//������ ��
            {
                isZoom = true;
            }
            else//�� ��
            {
                isZoom = false;
            }
            SetCamType(isZoom);
        }
    }
    void OnAim(InputValue inputValue)
    {
        mouseDeltaPos = inputValue.Get<Vector2>();//��ǲ ���� �޾ƿ�        
    }
    void OnReload(InputValue inputValue)
    {
        if (isActive)
        {
            float isClick = inputValue.Get<float>();

            if (!isReload)
            {
                //Debug.Log(isClick);
                isReload = true;
                Reload();
            }
        }
    }

    void SetCombatData()
    {
        data.playerMaxHp = combat.GetMaxHp();
        data.playerCurHp = combat.GetHp();
        data.controlWeaponName = controlweapon.gameObject.name;
        data.controlWeaponIndex = controlWeaponIndex;
        data.cwMaxMag = controlweapon.MagazineBulletCount();
        data.cwCurMag = controlweapon.bullet;
        data.killCount = combat.GetKillCount();
    }

    public PlayerCombatData GetCombatData()
    {
        return data;
    }


    //combat�� �̺�Ʈ ���
    private void HitAnimPlay()
    {
        animator.SetTrigger("Hit");
    }
    private void DieAnimPlay()
    {
        animator.SetTrigger("Die");
    }
}

public class PlayerCombatData//������ ������ Ŭ����
{
    public float playerMaxHp;//�ִ� ü��
    public float playerCurHp;//���� ü��
    public string controlWeaponName;//������� ���� �̸�
    public int controlWeaponIndex;//������� ���� �ε���
    public int cwMaxMag;//������� ���� �ִ� ��ź��
    public int cwCurMag;//������� ���� ���� ��ź��
    public int killCount;//ų��
}