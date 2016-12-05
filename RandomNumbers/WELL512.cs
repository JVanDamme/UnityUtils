using System.Collections;
using System;

public class WELL512
{
	private static uint[] state = new uint[16];
	private static int index = -1;

	public static void init_well(int seed)
	{
		seed = Math.Abs(seed);
		index = seed & 15;
		var rand = new System.Random(seed);
		uint u = (uint)rand.Next(1 << 30);
		for (int i = 0; i < 15; i++)
		{
			state[i] = (uint)rand.Next(1 << 30) << 2 | u & 3u;
			state[i] ^= (uint)rand.Next(1 << 30);
			state[i] ^= (uint)rand.Next(1 << 30);
			u >>= 2;
		}
		state[15] = (uint)rand.Next(1 << 30) << 2 | (uint)rand.Next(4);
		state[15] ^= (uint)rand.Next(1 << 30);
		state[15] ^= (uint)rand.Next(1 << 30);
	}

	public static uint well512()
	{
		uint a, b, c, d;
		a = state[index];
		c = state[index + 13 & 15];
		b = a ^ c ^ (a << 16) ^ (c << 15);
		c = state[index + 9 & 15];
		c ^= c >> 11;
		a = state[index] = b ^ c;
		d = a ^ ((a << 5) & 0xda442d24u);
		index = index + 15 & 15;
		a = state[index];
		state[index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);
		return state[index];
	}

	private static ulong buf = 0;              // 64 random bits buffer
	private static int av = 0;                 // available bits in buf
	private static uint g_u = 0;               // copy of u
	private static int g_nd = 0;               // nd (needed) bits = highest set bit of u
	private static int g_s = 0;                // shift
	private static bool ones = false;          // true if u = 10, 110, 1110, ...

	private static uint well_uint(uint u)
	{
		uint x; int s;
		if (u == 0) return 0;
		if (u != g_u)
		{
			g_u = u;
			g_nd = nob(u);
			g_s = 64 - g_nd;
			ones = (u + 1 & u + 2) == 0;
		}
		L0: if (av < g_nd)
		{
			av += 32;
			buf <<= 32;
			buf |= well512();
		}
		x = (uint)buf << g_s >> g_s;
		buf >>= g_nd;
		av -= g_nd;
		if (x <= u) return x;
		if (ones) goto L0;
		s = nob(x ^ u) - 1;
		av += s;
		buf <<= s;
		buf ^= x;
		goto L0;
	}

	private static int nob(uint u)
	{
		return
			u < 1u << 16 ? u < 1u << 08 ? u < 1u << 04 ? u < 1u << 02 ? u < 1u << 01 ? 01 : 02 :
			u < 1u << 03 ? 03 : 04 :
			u < 1u << 06 ? u < 1u << 05 ? 05 : 06 :
			u < 1u << 07 ? 07 : 08 :
			u < 1u << 12 ? u < 1u << 10 ? u < 1u << 09 ? 09 : 10 :
			u < 1u << 11 ? 11 : 12 :
			u < 1u << 14 ? u < 1u << 13 ? 13 : 14 :
			u < 1u << 15 ? 15 : 16 :
			u < 1u << 24 ? u < 1u << 20 ? u < 1u << 18 ? u < 1u << 17 ? 17 : 18 :
			u < 1u << 19 ? 19 : 20 :
			u < 1u << 22 ? u < 1u << 21 ? 21 : 22 :
			u < 1u << 23 ? 23 : 24 :
			u < 1u << 28 ? u < 1u << 26 ? u < 1u << 25 ? 25 : 26 :
			u < 1u << 27 ? 27 : 28 :
			u < 1u << 30 ? u < 1u << 29 ? 29 : 30 :
			u < 1u << 31 ? 31 : 32;
	}
}
