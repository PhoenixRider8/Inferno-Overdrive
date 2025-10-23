using System.Collections;
using UnityEngine;

public class ParticleEndTime : MonoBehaviour
{
    public ParticleSystem ps;
    public float endTime = 3f; // seconds

    void Start()
    {
        StartCoroutine(StopAfterTime());
    }

    IEnumerator StopAfterTime()
    {
        yield return new WaitForSeconds(endTime);
        ps.Stop(); // stops emission but particles already emitted will continue
    }
}
