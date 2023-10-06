using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarAnimation : MonoBehaviour
{
	[SerializeField, Range(0, 100f)] public float scalePercent = 5f;
	[SerializeField, Range(0.1f, 1)] public float timeAnimation = 0.2f;
	[SerializeField] private AnimationCurve animationCurve = new AnimationCurve();

	private Vector3 _initialScale;


	public void StartAnimation(GameObject start)
    {

    }
}
