using UnityEngine;
using System.Collections;

public class Quaternions : MonoBehaviour
{
	public GameObject fan;

	Quaternion xAxis90;
	Quaternion zAxis45p;
	Quaternion zAxis45n;

	float accuTimeMSsign = 1f;
	float accuTimeMS;
	float accuTimeS;

	void Start()
	{
		xAxis90 = RotateAboutVector (new Vector3 (1f, 0f, 0f), -90f);
		zAxis45p = RotateAboutVector (new Vector3 (0f, 0f, 1f), 45f);
		zAxis45n = RotateAboutVector (new Vector3 (0f, 0f, 1f), -45f);
	}

	void Update()
	{
		// Y axis rotation quaternion
		accuTimeS += Time.deltaTime * 1000f;
		Quaternion rotation = RotateAboutVector (new Vector3 (0f, 1f, 0f), accuTimeS);

		// Time for spherical interpolation
		accuTimeMS = accuTimeMS + accuTimeMSsign * Time.deltaTime;

		if (accuTimeMS > 1f || accuTimeMS < 0f)
		{
			accuTimeMSsign = accuTimeMSsign * -1f;
			accuTimeMS = Mathf.Clamp01 (accuTimeMS);	
		}

		// First the X axis is rotated 90 degrees to face the camera. Then
		// it is performed a spherical interpolation around Z axis, which
		// is in reality the new Y Axis. Finally the true Y axis, is rotated
		// to move the fan, no matter what happens with other axis.
		fan.transform.rotation = xAxis90 * Slerp(zAxis45p, zAxis45n, accuTimeMS) * rotation;
	}

	// A quaternion is 4th dimensional array that is an extension from complex numbers.
	// Quaternions are representes in the form: q = [s, v] = [s, xi + yj + zk].
	// Unity Quaternion class takes the scalar in its fourth position.  
	public static Quaternion RotateAboutVector(Vector3 vector, float degrees)
	{
		vector = vector.normalized;

		// Because the multiplication by a quaternion and its inverse,
		// the degrees have to be the half.
		degrees = degrees * 0.5f;

		// Sin & cos are functions which works with radians.
		// First we have to convert degrees in radians.
		float radians = degrees * Mathf.Deg2Rad;

		float radianSin = Mathf.Sin (radians);
		float radianCos = Mathf.Cos (radians);

		// [ cos (alpha / 2) , sin (alpha / 2) * v ]
		// This formula is the same as the rotors of complex numbers.
		Quaternion result;
		result.w = radianCos;
		result.x = radianSin * vector.x;
		result.y = radianSin * vector.y;
		result.z = radianSin * vector.z;

		return result;
	}

	// Spherical interpolation between two unit norm quaternions.
	public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
	{
		// Clamp because we don't want more than 1 or less than 0.
		t = Mathf.Clamp (t, 0f, 1f);

		// Alpha is the angle between both quaternions
		float alpha = Mathf.Acos(a.x*b.x + a.y*b.y + a.z*b.z + a.w*b.w);
		float sinAlpha = Mathf.Sin (alpha);

		float firstScalar = (Mathf.Sin (1f - t) * alpha) / sinAlpha;
		float secondScalar = (Mathf.Sin (t) * alpha) / sinAlpha;

		a = QuaternionScalarMultiply (a, firstScalar);
		b = QuaternionScalarMultiply (b, secondScalar);

		return QuaternionsAdd(a, b);
	}

	public static Quaternion QuaternionsAdd(Quaternion a, Quaternion b)
	{
		return new Quaternion (a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
	}

	public static Quaternion QuaternionScalarMultiply(Quaternion a, float f)
	{
		return new Quaternion (a.x * f, a.y * f, a.z * f, a.w * f);
	}
}
