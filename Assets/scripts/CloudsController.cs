using UnityEngine;

public class CloudsController : MonoBehaviour
{

    [SerializeField] private Transform[] _clouds;

    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _rightLimit = 10f;
    [SerializeField] private float _leftLimit = -10f;
    private void Update()
    {
        for (int i = 0; i < _clouds.Length; i++)
        {
            Vector3 pos = _clouds[i].localPosition;

            pos.x += _speed * Time.deltaTime;

            if (pos.x > _rightLimit)
            {
                pos.x = _leftLimit;
            }

            _clouds[i].localPosition = pos;
        }
    }
}
