using UnityEngine;
using System.Collections;
using System.Threading;
using System;

[ExecuteInEditMode]
public class Bezier : MonoBehaviour
{
	UnityEngine.Random random = new UnityEngine.Random();

	[Header("Curves")]
	public RectTransform[] rectPoints;
	public int curveSamples;
	public int curveLanes;
	public float laneOffset;

	[Header("Particles")]
	public GameObject[] objParticlePrefab;
	public float timeBetweenParticles;
	public float particleTimeTravel;
	public int totalCastParticles;

	[Header("Settings")]
	public bool threading;

	// Particles
	RectTransform[] rectParticles;
	Vector2[] positionParticles;
	float[] timeParticle;
	bool[] activeParticle;

	// Hidden vars
	Vector2[] points2D;

	// Shared vars
	float travelTimeMultiplier;
	float timeToNextParticle;
	int maxShownParticles;
	int lastActiveCookie;
	int numCastParticles;

	// Frame Threading
	Job job;

	void Start()
	{
		Format ();

		ParticlesWarmUp();
	}

	#region WarmUp
	void Format()
	{
		if (curveLanes <= 0) curveLanes = 1;
		if (curveLanes >= 3) curveLanes = 3;

		points2D = new Vector2 [rectPoints.Length];

		for (int i = 0; i < rectPoints.Length; ++i)
		{
			Vector3 refer = rectPoints [i].position;
			points2D[i] = new Vector2 (refer.x, refer.y);
		}
	}

	void ParticlesWarmUp()
	{
		// Variables.
		maxShownParticles = Mathf.CeilToInt(particleTimeTravel / timeBetweenParticles);
		travelTimeMultiplier = 1f / particleTimeTravel;

		// Buffers
		positionParticles = new Vector2[maxShownParticles];
		rectParticles = new RectTransform[maxShownParticles];
		activeParticle = new bool[maxShownParticles];
		timeParticle = new float[maxShownParticles];
	}
	#endregion

	#region Objects
	public void CastParticles()
	{
		// Particles instantiation.
		for (int i = 0; i < maxShownParticles; ++i)
		{
			GameObject particle = Instantiate(objParticlePrefab[i % 2]) as GameObject;

			// Particle properties.
			rectParticles[i] = particle.GetComponent<RectTransform>();
			rectParticles[i].transform.SetParent(this.gameObject.transform);
			rectParticles[i].gameObject.SetActive (false);
			timeParticle[i] = 0f;
		}

		// First cookie properties.
		rectParticles[0].gameObject.SetActive(true);
		rectParticles[0].localPosition = Formula(0f);
		activeParticle[0] = true;

		// Start casting data.
		numCastParticles = 1;
		lastActiveCookie = 0;

		// Multithreading.
		job = new Job(Frame);
		FrameThread.Instance.AddJob(job);
	}
	#endregion

	#region FrameUpdate
	void Frame(Job job, float deltaTime)
	{
		// Threaded delta time.
		timeToNextParticle += deltaTime;

		// Cast particle if is the time.
		bool castParticle = timeToNextParticle >= timeBetweenParticles;
		bool excededNumParticles = numCastParticles > totalCastParticles;

		if (castParticle && !excededNumParticles)
		{
			// Particles index.
			numCastParticles += 1;
			lastActiveCookie += 1;

			if (lastActiveCookie == maxShownParticles) {
				lastActiveCookie = 0;
			}

			// Reset particle new properties.
			activeParticle [lastActiveCookie] = true;
			timeParticle [lastActiveCookie] = 0.0f;
				timeToNextParticle = 0.0f;
			}	

			// Particle position calculus.
			int numActiveParticles = 0;

			for (int i = 0; i < maxShownParticles; ++i)
			{
				if (activeParticle [i])
				{
					numActiveParticles += 1;

					// Particle time.
					float t = timeParticle [i];

					// Particle lane offset.
					float offset = 0;
					int mod = (i % curveLanes);
					if (mod == 1) offset = QuartOffset (t, laneOffset);
					if (mod == 2) offset = QuartOffset (t, -laneOffset);

					// Particle new position.
					Vector2 posParticle = Formula (t);
					posParticle.x += offset;

					positionParticles[i] = posParticle;
					timeParticle[i] += deltaTime * travelTimeMultiplier;

					// On particle end point arrival.
					if (timeParticle [i] >= 1.0f) {
						activeParticle [i] = false;
						numCastParticles += 1;
					}
				}
			}

		// Going out.
		job.completed = (numActiveParticles == 0);
	}

	void Update()
	{
		if (job != null)
		{
			for (int i = 0; i < maxShownParticles; ++i)
			{
				if (activeParticle[i])
				{
					rectParticles[i].localPosition = positionParticles[i];
				}
				rectParticles [i].gameObject.SetActive(activeParticle [i]);
			}

			if (job.completed)
			{
				for (int i = 0; i < maxShownParticles; ++i) {
					Destroy(rectParticles [i].gameObject);
				}

				job = null;
			}
		}
	}
	#endregion

	#region Gizmos
	void OnDrawGizmos ()
	{
		Format ();

		// Draw points and lines.
		Gizmos.color = Color.black;
		for (int i = 0; i < rectPoints.Length-1; ++i)
		{
			Gizmos.DrawLine(rectPoints[i].position, rectPoints[i+1].position);
		}

		// Draw the curve.
		float t_offset = 1f / (float) curveSamples;
		float t_accum = 0f;

		for (int i = 0; i < curveSamples; ++i)
		{	
			Vector2 position = Formula(t_accum);

			DrawProbe (position, t_accum, 0f, Color.white);
			if (curveLanes >= 1) DrawProbe (position, t_accum, -laneOffset, Color.red);
			if (curveLanes >= 2) DrawProbe (position, t_accum, laneOffset, Color.blue);

			t_accum += t_offset;
		}
	}

	void DrawProbe(Vector2 position, float t, float xOffset, Color color)
	{
		Gizmos.color = color;
		position.x += QuartOffset(t, xOffset);
		Gizmos.DrawSphere (position, 10f);
	}
	#endregion

	#region Maths
	Vector2 Formula(float t)
	{
		int n = rectPoints.Length - 1;
		Vector2 result = Vector2.zero;
		float one_t = 1f - t;

		for (int i = 0; i <= n; ++i)
		{
			float t_pows = Mathf.Pow (t, (float) i) * Mathf.Pow (one_t, (float) (n - i));
			float binomial = BinomialCoefficient (n, i);

			float partial_res = binomial * t_pows;
			result.x += points2D[i].x * partial_res;
			result.y += points2D[i].y * partial_res;
		}
			
		return result;
	}

	float ExponentialOffset(float t, float offset)
	{
		return -offset * Mathf.Pow(2, 10 * (t - 1)) + offset;
	}

	float QuartOffset(float t, float offset)
	{
		return -offset * t * t * t * t + offset;
	}

	float BinomialCoefficient(int n, int k)
	{
		if (k == 0 || n == k) return 1f;
		return Factorial (n) / (Factorial (k) * Factorial (n - k));
	}

	float Factorial(int n)
	{
		if (n <= 1) return 1;

		int res = 1;

		for (int i = n; i > 1; --i) {
			res *= i;
		}

		return (float) res;
	}
	#endregion
}