using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isActive = false;
    private float MinPitch = 0.9f;
    private float MaxPitch = 1.1f;
    private float currentGrindingVolume = 0f;
    private const float grindPitch = 1f;
    private const float spatialBlend = 1f;

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

        float slidingSpeed = Vector3
            .ProjectOnPlane(collision.relativeVelocity, contactPoint.normal)
            .magnitude;
        float targetVolume = 0f;

        if (slidingSpeed > 0.01f)
        {
            targetVolume = Mathf.InverseLerp(0.01f, 10f, slidingSpeed);
            // Debug.Log($"Sliding speed: {slidingSpeed:F2} m/s, Volume: {volume:F2}");
        }

        currentGrindingVolume = Mathf.Lerp(
            currentGrindingVolume,
            targetVolume,
            Time.fixedDeltaTime * 10f
        );

        if (grindingAudioSource == null)
        {
            grindingAudioSource = SoundFXManager.Instance.PlayContinuousSound(
                "Grinding",
                currentGrindingVolume,
                grindPitch,
                contactPoint.point,
                grindingAudioSource,
                spatialBlend
            );
        }
        else
        {
            SoundFXManager.Instance.UpdateContinuousSound(
                grindingAudioSource,
                currentGrindingVolume,
                grindPitch,
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
