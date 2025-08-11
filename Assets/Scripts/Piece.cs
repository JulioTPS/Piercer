using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private bool isActive = false;
    private const float MIN_PITCH = 0.9f;
    private const float MAX_PITCH = 1.1f;
    private const float GRIND_PITCH = 1f;
    private const float SPATIAL_BLEND = 1f;
    private float currentGrindingVolume = 0f;
    private float startGrindingThreshold = 0.2f;
    private AudioSource grindingAudioSource = null;
    private Rigidbody rb;
    private bool isImpacting = false;
    private float continuousSoundDelay = 0.5f;
    private float timer = 0f;
    private float startGrindingSound = 0f;
    private float targetGrindingSound = 0f;
    private Vector3 lastContactPoint;
    private float grindingLoudSpeed = 30f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing on the Piece object.");
        }
    }

    private void Start() { }

    private void Update()
    {
        if (!isActive)
            return;

        timer += Time.deltaTime;

        if (timer >= continuousSoundDelay)
        {
            startGrindingSound = currentGrindingVolume;
        }

        currentGrindingVolume = Mathf.Lerp(
            startGrindingSound,
            targetGrindingSound,
            Mathf.Sqrt(timer / continuousSoundDelay)
        );

        if (grindingAudioSource == null)
        {
            grindingAudioSource = SoundFXManager.Instance.PlayContinuousSound(
                "Grinding",
                currentGrindingVolume,
                GRIND_PITCH,
                lastContactPoint,
                grindingAudioSource,
                SPATIAL_BLEND
            );
        }
        else
        {
            SoundFXManager.Instance.UpdateContinuousSound(
                grindingAudioSource,
                currentGrindingVolume,
                GRIND_PITCH,
                lastContactPoint
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive || isImpacting)
            return;
        isImpacting = true;
        ContactPoint contactPoint = collision.contacts[0];
        float volume = Mathf.InverseLerp(
            3f,
            100f,
            Mathf.Sqrt(contactPoint.impulse.magnitude / Time.fixedDeltaTime)
        );
        float pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        StartCoroutine(PlayImpactSound("Impact", contactPoint, volume, pitch));
    }

    private IEnumerator PlayImpactSound(
        string clipKey,
        ContactPoint contactPoint,
        float volume,
        float pitch
    )
    {
        float delay = SoundFXManager.Instance.PlaySFX(clipKey, contactPoint.point, volume, pitch);
        yield return new WaitForSeconds(delay);
        isImpacting = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isActive)
            return;

        ContactPoint contactPoint = collision.contacts[0];
        lastContactPoint = collision.contacts[0].point;

        Vector3 velocityAtContact = rb.GetPointVelocity(contactPoint.point);

        float slidingSpeed = Vector3
            .ProjectOnPlane(velocityAtContact, contactPoint.normal)
            .magnitude;

        if (slidingSpeed > startGrindingThreshold)
        {
            targetGrindingSound = Mathf.InverseLerp(
                startGrindingThreshold,
                grindingLoudSpeed,
                slidingSpeed
            );
        }
        else
        {
            targetGrindingSound = 0f;
        }
    }

    private void OnCollisionExit()
    {
        targetGrindingSound = 0f;
        lastContactPoint = transform.position;
    }

    void OnDestroy()
    {
        if (grindingAudioSource != null)
        {
            SoundFXManager.Instance.StopContinuousSound(grindingAudioSource);
            grindingAudioSource = null;
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (!isActive && grindingAudioSource != null)
        {
            SoundFXManager.Instance.StopContinuousSound(grindingAudioSource);
            grindingAudioSource = null;
        }
    }
}
