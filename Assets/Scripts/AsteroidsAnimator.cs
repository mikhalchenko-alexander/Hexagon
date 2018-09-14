using System.Collections;
using UnityEngine;

public class AsteroidsAnimator : MonoBehaviour {
    [SerializeField] private float _radius = 10;
    [SerializeField] private float _speed = 0.1f;
    private Vector2 _center;
    private Coroutine _animation;

    private void Start() {
        StartAnimation();
    }

    private void OnDestroy() {
        StopAnimation();
    }

    private void StopAnimation() {
        if (_animation != null) {
            StopCoroutine(_animation);
        }
    }

    public void StartAnimation() {
        _center = transform.position;
        _animation = StartCoroutine(Move());
    }

    private static Vector2 RandomDirection() {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private IEnumerator Move() {
        var destination = RandomDirection() * _radius + _center;

        while (true) {
            if (Vector2.Distance(transform.position, destination) > 0.2f) {
                transform.position = Vector2.MoveTowards(transform.position, destination, (_speed * Time.deltaTime));
            } else {
                destination = RandomDirection() * _radius + _center;
            }

            yield return null;
        }
    }

    private void OnDrawGizmos() {
#if UNITY_EDITOR
        Gizmos.DrawLine(_center - new Vector2(_radius, 0), _center + new Vector2(_radius, 0));
        Gizmos.DrawLine(_center - new Vector2(0, _radius), _center + new Vector2(0, _radius));
#endif
    }
}