﻿
using UnityEngine;

public class SimplexNoise
{
    private const float Div6 = 1.0f / 6.0f;
    private const float Div3 = 1.0f / 3.0f;

    /* Unit vectors for gradients to points on cube,equal distances apart (ie vector from center to the middle of each side */
    private static readonly int[][] Grad3 = new[]
        {
            new[] { 1, 1, 0 }, new[] { -1, 1, 0 }, new[] { 1, -1, 0 }, new[] { -1, -1, 0 },
            new[] { 1, 0, 1 }, new[] { -1, 0, 1 }, new[] { 1, 0, -1 }, new[] { -1, 0, -1 },
            new[] { 0, 1, 1 }, new[] { 0, -1, 1 }, new[] { 0, 1, -1 }, new[] { 0, -1, -1 }
        };

    //0..255, randomized
    private static readonly int[] P =
        {
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103,
            30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197,
            62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20,
            125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231,
            83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102,
            143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
            196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226,
            250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
            58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70,
            221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224,
            232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12,
            191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199,
            106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
            222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

    private static readonly int[] Perm = new int[512];

    private static readonly float Sqrt3 = Mathf.Sqrt(3);
    private static readonly float F2 = 0.5f * (Sqrt3 - 1.0f);
    private static readonly float G2 = (3.0f - Sqrt3) * Div6;

    static SimplexNoise()
    {
        for (int i = 0; i < 512; i++)
        {
            Perm[i] = P[i & 255];
        }

        Singleton = new SimplexNoise();
    }

    public static SimplexNoise Singleton { get; private set; }

    public float Noise01(float x, float y)
    {
        return (Noise(x, y) + 1) * 0.5f;
    }

    public float MultiNoise(int octaves, float x, float y)
    {
        float value = 0.0f;
        float mul = 1;
        for (int i = 0; i < octaves; i++)
        {
            value += Noise((x + 10) * mul, (y + 15) * mul) / mul;

            mul *= 2;
        }
        return value;
    }

    public float MultiNoise01(int octaves, float x, float y)
    {
        return (MultiNoise(octaves, x, y) + 1.0f) * 0.5f;
    }

    public float RidgedMulti(int octaves, float x, float y)
    {
        float value = 0.0f;
        float mul = 1;
        for (int i = 0; i < octaves; i++)
        {
            float added = Noise(x * mul, y * mul) / mul;
            value += Mathf.Abs(added);

            mul *= 2.18387276f;
        }
        return value;
    }

    public float Noise(float xin, float yin)
    {
        float n0, n1, n2; // Noise contributions from the three corners

        // Skew the input space to a square to determine which simplex cell we're in

        float s = (xin + yin) * F2; // Hairy factor for 2D
        int i = FastFloor(xin + s);
        int j = FastFloor(yin + s);

        float t = (i + j) * G2;
        float x0p = i - t; // Unskew the cell origin back to (x,y) space
        float y0p = j - t;
        float x0 = xin - x0p; // The x,y distances from the cell origin
        float y0 = yin - y0p;

        // For the 2D case, the simplex shape is an equilateral triangle.
        // Determine which simplex we are in.
        int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
        if (x0 > y0)
        {
            // lower triangle, XY order: (0,0)->(1,0)->(1,1)
            i1 = 1;
            j1 = 0;
        }
        else
        {
            i1 = 0;
            j1 = 1;
        } // upper triangle, YX order: (0,0)->(0,1)->(1,1)

        // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
        // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
        // c = (3-sqrt(3))/6

        float x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
        float y1 = y0 - j1 + G2;
        float x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
        float y2 = y0 - 1.0f + 2.0f * G2;

        // Work out the hashed gradient indices of the three simplex corners
        int ii = i & 255;
        int jj = j & 255;
        int gi0 = Perm[ii + Perm[jj]] % 12;
        int gi1 = Perm[ii + i1 + Perm[jj + j1]] % 12;
        int gi2 = Perm[ii + 1 + Perm[jj + 1]] % 12;

        // Calculate the contribution from the three corners
        float t0 = 0.5f - x0 * x0 - y0 * y0;
        if (t0 < 0)
        {
            n0 = 0.0f;
        }
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * Dot(Grad3[gi0], x0, y0); // (x,y) of grad3 used for 2D gradient
        }
        float t1 = 0.5f - x1 * x1 - y1 * y1;
        if (t1 < 0)
        {
            n1 = 0.0f;
        }
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * Dot(Grad3[gi1], x1, y1);
        }
        float t2 = 0.5f - x2 * x2 - y2 * y2;
        if (t2 < 0)
        {
            n2 = 0.0f;
        }
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * Dot(Grad3[gi2], x2, y2);
        }
        // Add contributions from each corner to get the final noise value.
        // The result is scaled to return values in the interval [-1,1].
        return 70.0f * (n0 + n1 + n2);
    }

    public float Multi01(int octaves, float x, float y, float z)
    {
        return (Multi(octaves, x, y, z) + 1) * 0.5f;
    }

    public float Multi(int octaves, float x, float y, float z)
    {
        float value = 0.0f;
        float mul = 1;
        for (int i = 0; i < octaves; i++)
        {
            float added = Noise(x * mul, y * mul, z * mul) / mul;
            value += added;
            mul *= 2;
        }
        return value;
    }

    public float Noise01(float x, float y, float z)
    {
        // Noise  is in the range -1 to +1
        float val = Noise(x, y, z);
        return (val + 1) * 0.5f;
    }

    public float Noise01(Vector3 pos)
    {
        // Noise  is in the range -1 to +1
        float val = Noise(pos.x, pos.y, pos.z);
        return (val + 1) * 0.5f;
    }

    public float NoiseRange(Vector3 pos, float min, float max)
    {
        return Mathf.Lerp(min, max, Noise01(pos));
    }

    public float RidgedMulti(int octaves, float x, float y, float z)
    {
        float value = 0.0f;
        float mul = 1;
        for (int i = 0; i < octaves; i++)
        {
            float added = Noise(x * mul, y * mul, z * mul) / mul;
            value += Mathf.Abs(added);
            mul *= 2;
        }
        return value;
    }

    public float Noise(float xin, float yin, float zin)
    {
        float n0, n1, n2, n3; // Noise contributions from the four corners
                                // Skew the input space to determine which simplex cell we're in

        float s = (xin + yin + zin) * Div3; // Very nice and simple skew factor for 3D
        int i = FastFloor(xin + s);
        int j = FastFloor(yin + s);
        int k = FastFloor(zin + s);

        float t = (i + j + k) * Div6;
        float ax0 = i - t; // Unskew the cell origin back to (x,y,z) space
        float ay0 = j - t;
        float az0 = k - t;
        float x0 = xin - ax0; // The x,y,z distances from the cell origin
        float y0 = yin - ay0;
        float z0 = zin - az0;
        // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
        // Determine which simplex we are in.
        int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
        int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
        if (x0 >= y0)
        {
            if (y0 >= z0)
            {
                i1 = 1;
                j1 = 0;
                k1 = 0;
                i2 = 1;
                j2 = 1;
                k2 = 0;
            }
            else if (x0 >= z0)
            {
                i1 = 1;
                j1 = 0;
                k1 = 0;
                i2 = 1;
                j2 = 0;
                k2 = 1;
            }
            else
            {
                i1 = 0;
                j1 = 0;
                k1 = 1;
                i2 = 1;
                j2 = 0;
                k2 = 1;
            }
        }
        else
        {
            // x0<y0
            if (y0 < z0)
            {
                i1 = 0;
                j1 = 0;
                k1 = 1;
                i2 = 0;
                j2 = 1;
                k2 = 1;
            }
            else if (x0 < z0)
            {
                i1 = 0;
                j1 = 1;
                k1 = 0;
                i2 = 0;
                j2 = 1;
                k2 = 1;
            }
            else
            {
                i1 = 0;
                j1 = 1;
                k1 = 0;
                i2 = 1;
                j2 = 1;
                k2 = 0;
            }
        }
        // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
        // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
        // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
        // c = 1/6.
        float x1 = x0 - i1 + Div6; // Offsets for second corner in (x,y,z) coords
        float y1 = y0 - j1 + Div6;
        float z1 = z0 - k1 + Div6;
        float x2 = x0 - i2 + 2.0f * Div6; // Offsets for third corner in (x,y,z) coords
        float y2 = y0 - j2 + 2.0f * Div6;
        float z2 = z0 - k2 + 2.0f * Div6;
        float x3 = x0 - 1.0f + 3.0f * Div6; // Offsets for last corner in (x,y,z) coords
        float y3 = y0 - 1.0f + 3.0f * Div6;
        float z3 = z0 - 1.0f + 3.0f * Div6;
        // Work out the hashed gradient indices of the four simplex corners
        int ii = i & 255;
        int jj = j & 255;
        int kk = k & 255;
        int gi0 = Perm[ii + Perm[jj + Perm[kk]]] % 12;
        int gi1 = Perm[ii + i1 + Perm[jj + j1 + Perm[kk + k1]]] % 12;
        int gi2 = Perm[ii + i2 + Perm[jj + j2 + Perm[kk + k2]]] % 12;
        int gi3 = Perm[ii + 1 + Perm[jj + 1 + Perm[kk + 1]]] % 12;
        // Calculate the contribution from the four corners
        float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
        if (t0 < 0)
        {
            n0 = 0.0f;
        }
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * Dot(Grad3[gi0], x0, y0, z0);
        }
        float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
        if (t1 < 0)
        {
            n1 = 0.0f;
        }
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * Dot(Grad3[gi1], x1, y1, z1);
        }
        float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
        if (t2 < 0)
        {
            n2 = 0.0f;
        }
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * Dot(Grad3[gi2], x2, y2, z2);
        }
        float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
        if (t3 < 0)
        {
            n3 = 0.0f;
        }
        else
        {
            t3 *= t3;
            n3 = t3 * t3 * Dot(Grad3[gi3], x3, y3, z3);
        }
        // Add contributions from each corner to get the final noise value.
        // The result is scaled to stay just inside [-1,1]
        return 32.0f * (n0 + n1 + n2 + n3);
    }

    private static int FastFloor(float x)
    {
        // This method is a *lot* faster than using (int)Math.floor(x)
        // This comment is regards to C, 
        return x > 0 ? (int)x : (int)x - 1;
    }

    private static float Dot(int[] g, float x, float y)
    {
        return g[0] * x + g[1] * y;
    }

    private static float Dot(int[] g, float x, float y, float z)
    {
        return g[0] * x + g[1] * y + g[2] * z;
    }
}
