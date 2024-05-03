using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Transform firePoint;//����ü ���� ��ġ
    [SerializeField] Transform fpsViewPoint;//1��Ī���� �� ����ī�޶� ��ġ    

    [Header("Spec")]
    [SerializeField] GameObject projectilePrf;//�߻��� ����ü ������
    [SerializeField] float rpm;//�д� �߻� �ӵ�
    [SerializeField] int fireCount;//1ȸ�� �߻� �ӵ�. 
    [SerializeField] float burstDelay;//1ȸ�� �������� �߻��ϰ� �� ���, �� �߰��� �ð� ����
    [SerializeField] int magazineBulletCount;//1źâ�� źȯ ��
    [SerializeField] float operability;//���� ���ۼ�. ��ġ�� Ŭ���� ĳ���� ȸ���� ����
    [SerializeField] float projectileVelocity;//�����ʼ�. m/s����
    [SerializeField] float dmg;//�ߴ� ������
    [SerializeField] float recoil;//�ݵ�. Ŭ���� ȭ���� ũ�� Ʀ

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
