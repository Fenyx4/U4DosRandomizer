﻿using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public static class SeamlessSimplexNoise
    {
        //https://ronvalstar.nl/creating-tileable-noise-maps
        //https://github.com/Mindwerks/pyplatec/blob/master/platec_src/simplexnoise.cpp
        public static float[,] simplexnoise(long seed, int width, int height, float roughness, float scale = 2.0f, float loBound = 0.0f, float hiBound = 1.0f)
        {
            float[,] result = new float[width, height];
            float ka = 256 / seed;
            float kb = seed * 567 % 256;
            float kc = (seed * seed) % 256;
            float kd = (567 - seed) % 256;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float fNX = x / (float)width; // we let the x-offset define the circle
                    float fNY = y / (float)height; // we let the x-offset define the circle
                    float fRdx = fNX * 2 * MathF.PI; // a full circle is two pi radians
                    float fRdy = fNY * 2 * MathF.PI; // a full circle is two pi radians
                    float fYSin = (float)Math.Sin(fRdy);
                    float fRdsSin = 1.0f;
                    float noiseScale = 0.593F;
                    float a = fRdsSin * MathF.Sin(fRdx);
                    float b = fRdsSin * MathF.Cos(fRdx);
                    float c = fRdsSin * MathF.Sin(fRdy);
                    float d = fRdsSin * MathF.Cos(fRdy);
                    float v = scaled_octave_noise_4d(64.0f,
                            roughness,
                            scale,
                            loBound,
                            hiBound,
                            ka + a * noiseScale,
                            kb + b * noiseScale,
                            kc + c * noiseScale,
                            kd + d * noiseScale);
                    result[x, y] = v;
                }
            }

            return result;
        }

        // 4D Scaled Multi-octave Simplex noise.
        //
        // Returned value will be between loBound and hiBound.
        private static float scaled_octave_noise_4d(float octaves, float persistence, float scale, float loBound, float hiBound, float x, float y, float z, float w)
        {
            return octave_noise_4d(octaves, persistence, scale, x, y, z, w) * (hiBound - loBound) / 2 + (hiBound + loBound) / 2;
        }

        // 4D Multi-octave Simplex noise.
        //
        // For each octave, a higher frequency/lower amplitude function will be added to the original.
        // The higher the persistence [0-1], the more of each succeeding octave will be added.
        private static float octave_noise_4d(float octaves, float persistence, float scale, float x, float y, float z, float w)
        {
            float total = 0;
            float frequency = scale;
            float amplitude = 1;

            // We have to keep track of the largest possible amplitude,
            // because each octave adds more, and we need a value in [-1, 1].
            float maxAmplitude = 0;

            for (int i = 0; i < octaves; i++)
            {
                total += raw_noise_4d(x * frequency, y * frequency, z * frequency, w * frequency) * amplitude;

                frequency *= 2;
                maxAmplitude += amplitude;
                amplitude *= persistence;
            }

            return total / maxAmplitude;
        }

        // 4D Scaled Simplex raw noise.
        //
        // Returned value will be between loBound and hiBound.
        private static float scaled_raw_noise_4d(float loBound, float hiBound, float x, float y, float z, float w)
        {
            return raw_noise_4d(x, y, z, w) * (hiBound - loBound) / 2 + (hiBound + loBound) / 2;
        }

        private static int fastfloor(float x) { return x > 0 ? (int)x : (int)x - 1; }
        private static float dot(int[] g, float x, float y, float z, float w) { return g[0] * x + g[1] * y + g[2] * z + g[3] * w; }

        // A lookup table to traverse the simplex around a given point in 4D.
        private static int[] simplex = new int[64 * 4] {
            0,1,2,3,0,1,3,2,0,0,0,0,0,2,3,1,0,0,0,0,0,0,0,0,0,0,0,0,1,2,3,0,
    0,2,1,3,0,0,0,0,0,3,1,2,0,3,2,1,0,0,0,0,0,0,0,0,0,0,0,0,1,3,2,0,
    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    1,2,0,3,0,0,0,0,1,3,0,2,0,0,0,0,0,0,0,0,0,0,0,0,2,3,0,1,2,3,1,0,
    1,0,2,3,1,0,3,2,0,0,0,0,0,0,0,0,0,0,0,0,2,0,3,1,0,0,0,0,2,1,3,0,
    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    2,0,1,3,0,0,0,0,0,0,0,0,0,0,0,0,3,0,1,2,3,0,2,1,0,0,0,0,3,1,2,0,
    2,1,0,3,0,0,0,0,0,0,0,0,0,0,0,0,3,1,0,2,0,0,0,0,3,2,0,1,3,2,1,0
        };

        // Permutation table.The same list is repeated twice.
        private static int[] perm = new int[] {
            151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
            8,99,37,240,21,10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,
            35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,
            134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,
            55,46,245,40,244,102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208, 89,
            18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,
            250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,
            189,28,42,223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,
            172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,
            228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,
            107,49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,

            151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
            8,99,37,240,21,10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,
            35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,
            134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,
            55,46,245,40,244,102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208, 89,
            18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,
            250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,
            189,28,42,223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,
            172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,
            228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,
            107,49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
            138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
        };

        // The gradients are the midpoints of the vertices of a hypercube.
        private static int[][] grad4 = new int[][] {
            new int[] {0,1,1,1},  new int[] {0,1,1,-1},  new int[] {0,1,-1,1}, new int[]  {0,1,-1,-1},
            new int[] {0,-1,1,1}, new int[] {0,-1,1,-1}, new int[] {0,-1,-1,1}, new int[] {0,-1,-1,-1},
            new int[] {1,0,1,1},  new int[] {1,0,1,-1},  new int[] {1,0,-1,1},  new int[] {1,0,-1,-1},
            new int[] {-1,0,1,1}, new int[] {-1,0,1,-1}, new int[] {-1,0,-1,1}, new int[] {-1,0,-1,-1},
            new int[] {1,1,0,1},  new int[] {1,1,0,-1},  new int[] {1,-1,0,1},  new int[] {1,-1,0,-1},
            new int[] {-1,1,0,1}, new int[] {-1,1,0,-1}, new int[] {-1,-1,0,1}, new int[] {-1,-1,0,-1},
            new int[] {1,1,1,0},  new int[] {1,1,-1,0},  new int[] {1,-1,1,0},  new int[] {1,-1,-1,0},
            new int[] {-1,1,1,0}, new int[] {-1,1,-1,0}, new int[] {-1,-1,1,0}, new int[] {-1,-1,-1,0}
        };

        private static float F4 = (MathF.Sqrt(5.0F) - 1.0F) / 4.0F;
        private static float G4 = (5.0F - MathF.Sqrt(5.0F)) / 20.0F;

        // 4D raw Simplex noise
        private static float raw_noise_4d(float x, float y, float z, float w)
        {
            // The skewing and unskewing factors are hairy again for the 4D case
            //float F4 = (MathF.Sqrt(5.0F) - 1.0F) / 4.0F;
            //float G4 = (5.0F - MathF.Sqrt(5.0F)) / 20.0F;
            float n0, n1, n2, n3, n4; // Noise contributions from the five corners

            // Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
            float s = (x + y + z + w) * F4; // Factor for 4D skewing
            int i = fastfloor(x + s);
            int j = fastfloor(y + s);
            int k = fastfloor(z + s);
            int l = fastfloor(w + s);
            float t = (i + j + k + l) * G4; // Factor for 4D unskewing
            float X0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
            float Y0 = j - t;
            float Z0 = k - t;
            float W0 = l - t;

            float x0 = x - X0; // The x,y,z,w distances from the cell origin
            float y0 = y - Y0;
            float z0 = z - Z0;
            float w0 = w - W0;

            // For the 4D case, the simplex is a 4D shape I won't even try to describe.
            // To find out which of the 24 possible simplices we're in, we need to
            // determine the magnitude ordering of x0, y0, z0 and w0.
            // The method below is a good way of finding the ordering of x,y,z,w and
            // then find the correct traversal order for the simplex we're in.
            // First, six pair-wise comparisons are performed between each possible pair
            // of the four coordinates, and the results are used to add up binary bits
            // for an integer index.
            int c1 = (x0 > y0) ? 32 : 0;
            int c2 = (x0 > z0) ? 16 : 0;
            int c3 = (y0 > z0) ? 8 : 0;
            int c4 = (x0 > w0) ? 4 : 0;
            int c5 = (y0 > w0) ? 2 : 0;
            int c6 = (z0 > w0) ? 1 : 0;
            int c = c1 + c2 + c3 + c4 + c5 + c6;
            c <<= 2;

            int i1, j1, k1, l1; // The integer offsets for the second simplex corner
            int i2, j2, k2, l2; // The integer offsets for the third simplex corner
            int i3, j3, k3, l3; // The integer offsets for the fourth simplex corner

            // simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
            // Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
            // impossible. Only the 24 indices which have non-zero entries make any sense.
            // We use a thresholding to set the coordinates in turn from the largest magnitude.
            // The number 3 in the "simplex" array is at the position of the largest coordinate.
            // The number 2 in the "simplex" array is at the second largest coordinate.
            // The number 1 in the "simplex" array is at the second smallest coordinate.
            i1 = simplex[c] >= 3 ? 1 : 0;
            i2 = simplex[c] >= 2 ? 1 : 0;
            i3 = simplex[c++] >= 1 ? 1 : 0;

            j1 = simplex[c] >= 3 ? 1 : 0;
            j2 = simplex[c] >= 2 ? 1 : 0;
            j3 = simplex[c++] >= 1 ? 1 : 0;


            k1 = simplex[c] >= 3 ? 1 : 0;
            k2 = simplex[c] >= 2 ? 1 : 0;
            k3 = simplex[c++] >= 1 ? 1 : 0;


            l1 = simplex[c] >= 3 ? 1 : 0;
            l2 = simplex[c] >= 2 ? 1 : 0;
            l3 = simplex[c] >= 1 ? 1 : 0;

            // The fifth corner has all coordinate offsets = 1, so no need to look that up.


            float x1 = x0 - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
            float y1 = y0 - j1 + G4;
            float z1 = z0 - k1 + G4;
            float w1 = w0 - l1 + G4;
            float x2 = x0 - i2 + 2.0F * G4; // Offsets for third corner in (x,y,z,w) coords
            float y2 = y0 - j2 + 2.0F * G4;
            float z2 = z0 - k2 + 2.0F * G4;
            float w2 = w0 - l2 + 2.0F * G4;
            float x3 = x0 - i3 + 3.0F * G4; // Offsets for fourth corner in (x,y,z,w) coords
            float y3 = y0 - j3 + 3.0F * G4;
            float z3 = z0 - k3 + 3.0F * G4;
            float w3 = w0 - l3 + 3.0F * G4;
            float x4 = x0 - 1.0F + 4.0F * G4; // Offsets for last corner in (x,y,z,w) coords
            float y4 = y0 - 1.0F + 4.0F * G4;
            float z4 = z0 - 1.0F + 4.0F * G4;
            float w4 = w0 - 1.0F + 4.0F * G4;

            // Work out the hashed gradient indices of the five simplex corners
            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;
            int ll = l & 255;
            int gi0 = perm[ii + perm[jj + perm[kk + perm[ll]]]] % 32;
            int gi1 = perm[ii + i1 + perm[jj + j1 + perm[kk + k1 + perm[ll + l1]]]] % 32;
            int gi2 = perm[ii + i2 + perm[jj + j2 + perm[kk + k2 + perm[ll + l2]]]] % 32;
            int gi3 = perm[ii + i3 + perm[jj + j3 + perm[kk + k3 + perm[ll + l3]]]] % 32;
            int gi4 = perm[ii + 1 + perm[jj + 1 + perm[kk + 1 + perm[ll + 1]]]] % 32;

            // Calculate the contribution from the five corners
            float t0 = 0.6F - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 < 0)
            {
                n0 = 0.0F;
            }
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * dot(grad4[gi0], x0, y0, z0, w0);
            }

            float t1 = 0.6F - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0)
            {
                n1 = 0.0F;
            }
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * dot(grad4[gi1], x1, y1, z1, w1);
            }

            float t2 = 0.6F - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0)
            {
                n2 = 0.0F;
            }
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * dot(grad4[gi2], x2, y2, z2, w2);
            }

            float t3 = 0.6F - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0)
            {
                n3 = 0.0F;
            }
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * dot(grad4[gi3], x3, y3, z3, w3);
            }

            float t4 = 0.6F - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0)
            {
                n4 = 0.0F;
            }
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * dot(grad4[gi4], x4, y4, z4, w4);
            }

            // Sum up and scale the result to cover the range [-1,1]
            return 27.0F * (n0 + n1 + n2 + n3 + n4);
        }
    }
}
