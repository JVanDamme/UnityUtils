using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class Random : MonoBehaviour
{
	uint N = 16384;
	uint ITERATIONS = 16384 * 1000;
	uint MIN;
	uint MAX;

	// Use this for initialization
	void Start ()
	{
		uint num = 100;
		float mean = 0f;

		/*ResetMinMax();
		for (int i = 0; i < num; ++i)
		{
			mean = mean + SystemRandom(); 
		}	
		mean = mean / (float) num;
		Debug.Log("Normal deviation mean: " + mean + ", iterations: " + num);
		Debug.Log("Min: " + MIN + ", Max: " + MAX);*/

		/*ResetMinMax();
		mean = 0f;
		for (int i = 0; i < num; ++i)
		{
			mean = mean + SystemCryptographyRandom(); 
		}	
		mean = mean / (float) num;
		Debug.Log("Normal deviation mean: " + mean + ", iterations: " + num);
		Debug.Log("Min: " + MIN + ", Max: " + MAX);*/

		/*ResetMinMax();
		mean = 0f;
		for (int i = 0; i < num; ++i)
		{
			mean = mean + UnityRandom(); 
		}	
		mean = mean / (float) num;
		Debug.Log("Normal deviation mean: " + mean + ", iterations: " + num);
		Debug.Log("Min: " + MIN + ", Max: " + MAX);*/

		ResetMinMax();
		mean = 0f;
		for (int i = 0; i < num; ++i)
		{
			mean = mean + WELL512Random(); 
		}	
		mean = mean / (float) num;
		Debug.Log("Normal deviation mean: " + mean + ", iterations: " + num);
		Debug.Log("Min: " + MIN + ", Max: " + MAX);
	}

	void ResetMinMax()
	{
		MIN = 2000;
		MAX = 0;
	}

	void GetMinMax(ushort[] array)
	{
		for (var i = 0; i < array.Length; ++i)
		{
			if (array[i] < MIN && array[i] != 0) MIN = array[i];	
			if (array[i] > MAX) MAX = array[i];
		}	
	}

	int CryptoServiceProviderSeed()
	{
		RNGCryptoServiceProvider random = new RNGCryptoServiceProvider ();

		byte[] seed = new byte[4];
		random.GetBytes (seed);

		return (int) (seed [0]) << 24 | (int) (seed [1]) << 16 | (int) (seed [1]) << 8 | seed [0];
	}


	float UnityRandom()
	{
		//UnityEngine.Random.InitState(Int) = Initializes the random number generator state with a seed.
		//UnityEngine.Random.Range(float, float) =

		int seed = CryptoServiceProviderSeed ();
		UnityEngine.Random.InitState(seed);

		ushort[] array = new ushort[N];
		for (int i = 0; i < ITERATIONS; ++i)
		{
			int index = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float) N-1));
			array[index] += 1;
		}

		GetMinMax(array);

		float mean = Mean(array, N);
		return Deviation(array, mean, N);
	}

	float WELL512Random()
	{
		int seed = CryptoServiceProviderSeed ();
		WELL512.init_well(seed);

		ushort[] array = new ushort[N];
		for (int i = 0; i < ITERATIONS; ++i)
		{
			int well = (int) WELL512.well512 ();

			int index = 0;
			if (well != int.MinValue) index = System.Math.Abs(well) % (int) N;
			array[index] += 1;
		}

		GetMinMax(array);

		float mean = Mean(array, N);
		return Deviation(array, mean, N);
	}

	float SystemRandom()
	{
		// Random()	= Initializes a new instance of the Random class, using a time-dependent default seed value.
		// Random(Int32) = Initializes a new instance of the Random class, using the specified seed value

		byte MASK = 63;
		ushort SHIFT = 8;

		byte[] randomArray = new byte[ITERATIONS*2];
		System.Random random = new System.Random();
		random.NextBytes (randomArray);

		ushort[] array = new ushort[N];
		for (int i = 0; i < randomArray.Length-1; i = i+2)
		{
			int rshort = (int) (randomArray[i] & MASK);
			int index = (rshort << SHIFT) | randomArray[i+1];
			array[index] += 1;
		}

		GetMinMax(array);

		float mean = Mean(array, N);
		return Deviation(array, mean, N);
	}

	float SystemCryptographyRandom()
	{
		//RandomNumberGenerator() = Initializes a new instance of RandomNumberGenerator.
		//GetBytes(Byte[]) = When overridden in a derived class, fills an array of bytes with a cryptographically
		// strong random sequence of values.

		byte MASK = 63;
		ushort SHIFT = 8;

		byte[] randomArray = new byte[ITERATIONS*2];
		RNGCryptoServiceProvider random = new RNGCryptoServiceProvider ();
		random.GetBytes (randomArray);

		ushort[] array = new ushort[N];
		for (int i = 0; i < randomArray.Length-1; i = i+2)
		{
			int rshort = (int) (randomArray[i] & MASK);
			int index = (rshort << SHIFT) | randomArray[i+1];
			array[index] += 1;
		}

		GetMinMax(array);

		float mean = Mean(array, N);
		return Deviation(array, mean, N);
	}

	float Deviation(ushort[] array, float mean, uint n)
	{
		float deviation = 0;
		for (int i = 0; i < n; i++)
		{
			deviation += Mathf.Pow(array[i] - mean, 2); 
		}
		deviation = Mathf.Sqrt(deviation / (float) (n - 1));
		return deviation;
	}

	float Mean(ushort[] array, uint n)
	{
		float mean = 0;
		for (int i = 0; i < n; i++)
		{
			mean += array[i]; 
		}	
		mean = mean / (float) n;
		return mean;
	}
}
