using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Range(0.25f, 50f)]
    public float speed = 1f;

    [System.NonSerialized]
    public float range = 12;
    protected float _distance = 0;

    [Range(1, 100)]
    public int damageFactor = 5;

    [Tooltip("VFX game object when hit to wall etc.")]
    public GameObject impactVFX;
    private int _impactId;

    protected AudioSource _audioSource;
    protected RangeWeapon _shooter;
    //protected Rigidbody rigidBody;

    //protected ParticleSystem vFXInstance;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (impactVFX != null)
        {
            _impactId = impactVFX.GetInstanceID();
            ObjectPool.GetOrInitPool(impactVFX);
        }

        //rigidBody = GetComponent<Rigidbody>();
        //rigidBody.detectCollisions = false;

        //vFXInstance = Instantiate(explosionVFX);
        //vFXInstance.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (_audioSource != null)
            _audioSource.Play();
        _distance = 0;
    }

    private void FixedUpdate()
    {
        transform.Translate(0, 0, speed * Time.fixedDeltaTime, transform);
        _distance += speed * Time.fixedDeltaTime;

        if (_distance > range)
            ObjectPool.Release(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        {

            if (impactVFX != null)
            {
                //var rotat = collision.transform.Direction(transform.position);
                //rotat.y -= 90;
                ObjectPool.GetInstance(_impactId, collision.contacts[0].point);
            }

            ObjectPool.Release(gameObject);
        }
    }
}