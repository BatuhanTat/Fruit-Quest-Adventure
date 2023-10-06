using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarAnimation : MonoBehaviour
{
    [SerializeField, Range(0, 100f)] public float scalePercent = 5f;
    [SerializeField, Range(0.1f, 1)] public float timeAnimation = 0.2f;
    [SerializeField] private AnimationCurve animationCurve = new AnimationCurve();

    private Vector3 _initialScale;


    public void PlayStarAnimation(GameObject star)
    {
        _initialScale = star.transform.localScale;

        var myTransform = transform;
        myTransform.localScale = _initialScale;
        var target = _initialScale + (_initialScale * (scalePercent / 100f));
        //StartCoroutine(_Scale(myTransform, target, 0, timeAnimation / 2, animationCurve));

        StartCoroutine(_Scale(myTransform, target, 0, timeAnimation / 2, animationCurve, () =>
         {
             // This callback will be called when the first scale animation is complete.
             StartCoroutine(_Scale(myTransform, _initialScale, 0, timeAnimation / 2, animationCurve, null));
         }));
    }

    private static IEnumerator _Scale(Transform scaleThis, Vector3 toThis, float delay, float time,
            AnimationCurve curve, System.Action onComplete = null)
    {
        yield return new WaitForSeconds(delay);
        var passed = 0f;
        var initScale = scaleThis.localScale;
        while (passed < time)
        {
            passed += Time.deltaTime;
            var normalized = passed / time;
            var rate = curve.Evaluate(normalized);
            scaleThis.localScale = Vector3.LerpUnclamped(initScale, toThis, rate);
            yield return null;
        }
        // If an onComplete action is provided, call it after the scale animation is complete.
        onComplete?.Invoke();
    }
}
