using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isActive = false;
    private float collisionMinPitch = 0.5f;
    private float collisionMaxPitch = 0.6f;
    private float grindingMinPitch = 0.3f;
    private float grindingMaxPitch = 0.4f;

    private void Update() { }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];
        float pitch = Random.Range(collisionMinPitch, collisionMaxPitch);
        float volume = Mathf.InverseLerp(
            3f,
            600f,
            contactPoint.impulse.magnitude / Time.fixedDeltaTime
        );
        // Debug.Log(
        //     $"Collision impulse: {contactPoint.impulse.magnitude / Time.fixedDeltaTime}, Volume: {volume}"
        // );
        SoundFXManager.Instance.PlaySFX("PieceHit", contactPoint.point, volume, pitch);
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];

        Vector3 relativeVelocity = collision.relativeVelocity;
        Vector3 surfaceVelocity = Vector3.ProjectOnPlane(relativeVelocity, contactPoint.normal);
        float slidingSpeed = surfaceVelocity.magnitude;
        if (slidingSpeed > 0.1f)
        {
            float volume = Mathf.InverseLerp(0.1f, 15f, slidingSpeed);

            float pitch = Random.Range(grindingMinPitch, grindingMaxPitch);

            Debug.Log($"Sliding speed: {slidingSpeed:F2} m/s, Volume: {volume:F2}");

            SoundFXManager.Instance.PlaySFX("Grinding", contactPoint.point, volume, pitch);
        }
    }
}
