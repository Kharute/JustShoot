using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : SceneSingleton<SFXManager>
{
    public GameObject explosion;
    public GameObject reload;

    public void SoundOn(Vector3 position, GameObject prf)//����� ������ҽ��� ��� �������� �Ű������� �޾Ƽ� position���� �����Ŵ.
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(prf);

        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;

        AudioSource audioSource = item.GetComponent<AudioSource>();
        audioSource.Play();
        float onTime = audioSource.clip.length;

        StartCoroutine(EnqueueObject(item, onTime));
    }
    public void SoundOnAttach(Transform parent, GameObject prf)//����� ������ҽ��� ��� �������� �Ű������� �޾Ƽ� position���� �����Ŵ.
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(prf);

        item.transform.SetParent(parent, false);

        AudioSource audioSource = item.GetComponent<AudioSource>();
        audioSource.Play();
        float onTime = audioSource.clip.length;

        StartCoroutine(EnqueueObject(item, onTime));
    }

    public void ExplosionSoundOn(Vector3 position)//�Ŵ����� �̸� ĳ���س��� �������� �Ű������� �Ѱܼ� position���� �����Ŵ.
    {
        SoundOn(position, explosion);
    }
    public void ReloadSoundOn(Vector3 position)
    {
        SoundOn(position, reload);
    }

    IEnumerator EnqueueObject(GameObject item, float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
}
