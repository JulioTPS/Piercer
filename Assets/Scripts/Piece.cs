using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isActive = false;
    private float MinPitch = 0.9f;
    private float MaxPitch = 1.1f;
    private const float GRIND_PITCH = 1f;
    private const float SPATIAL_BLEND = 1f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing on the Piece object.");
        }
    }

    private AudioSource grindingAudioSource = null;

    private void Update() { }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];
        float volume = Mathf.InverseLerp(
            3f,
            600f,
            contactPoint.impulse.magnitude / Time.fixedDeltaTime
        );
        float pitch = Random.Range(MinPitch, MaxPitch);
        // Debug.Log(
        //     $"Collision impulse: {contactPoint.impulse.magnitude / Time.fixedDeltaTime}, Volume: {volume}"
        // );
        SoundFXManager.Instance.PlaySFX("Impact", contactPoint.point, volume, pitch);
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];

        Vector3 velocityAtContact = rb.GetPointVelocity(contactPoint.point);

        float slidingSpeed = Vector3
            .ProjectOnPlane(velocityAtContact, contactPoint.normal)
            .magnitude;
        float targetVolume = 0f;

        if (slidingSpeed > 0.01f)
        {
            targetVolume = Mathf.InverseLerp(0.01f, 10f, slidingSpeed);
            // Debug.Log($"Sliding speed: {slidingSpeed:F2} m/s, Volume: {volume:F2}");
        }

        if (grindingAudioSource == null)
        {
            grindingAudioSource = SoundFXManager.Instance.PlayContinuousSound(
                "Grinding",
                targetVolume,
                GRIND_PITCH,
                contactPoint.point,
                grindingAudioSource,
                SPATIAL_BLEND
            );
        }
        else
        {
            SoundFXManager.Instance.UpdateContinuousSound(
                grindingAudioSource,
                targetVolume,
                GRIND_PITCH,
                contactPoint.point
            );
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        SoundFXManager.Instance.StopContinuousSound(grindingAudioSource);
        grindingAudioSource = null;
    }

    void OnDestroy()
    {
        if (grindingAudioSource != null)
        {
            SoundFXManager.Instance.StopContinuousSound(grindingAudioSource);
            grindingAudioSource = null;
        }
    }
}
