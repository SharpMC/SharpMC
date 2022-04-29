using System;

namespace SharpMC.Noise.Open
{
    internal class OpenSimplex : NoiseGen
    {
        private const double StretchConstant2D = -0.211324865405187;
        private const double SquishConstant2D = 0.366025403784439;
        private const double StretchConstant3D = -1.0 / 6;
        private const double SquishConstant3D = 1.0 / 3;
        private const double NormConstant2D = 47;
        private const double NormConstant3D = 103;
        private short[] _perm;
        private short[] _permGradIndex3D;

        private int Octaves { get; set; }
        private double Amplitude { get; set; }
        private double Persistance { get; set; }
        private double Frequency { get; set; }
        private double Lacunarity { get; set; }

        private OpenSimplex() : this(new Random().Next())
        {
        }

        private OpenSimplex(short[] perm)
        {
            _perm = perm;
            _permGradIndex3D = new short[256];
            for (var I = 0; I < 256; I++)
            {
                _permGradIndex3D[I] = (short) (perm[I] % (Gradients3D.Length / 3) * 3);
            }

            Octaves = 2;
            Amplitude = 2;
            Persistance = 1;
            Frequency = 1;
            Lacunarity = 2;
        }

        internal OpenSimplex(long seed)
        {
            Octaves = 2;
            Amplitude = 2;
            Persistance = 1;
            Frequency = 1;
            Lacunarity = 2;
            SetSeed(seed);
        }

        private void SetSeed(long seed)
        {
            _perm = new short[256];
            _permGradIndex3D = new short[256];
            var source = new short[256];
            for (short I = 0; I < 256; I++)
                source[I] = I;
            seed = seed * 6364136223846793005L + 1442695040888963407L;
            seed = seed * 6364136223846793005L + 1442695040888963407L;
            seed = seed * 6364136223846793005L + 1442695040888963407L;
            for (var I = 255; I >= 0; I--)
            {
                seed = seed * 6364136223846793005L + 1442695040888963407L;
                var r = (int) ((seed + 31) % (I + 1));
                if (r < 0)
                    r += I + 1;
                _perm[I] = source[r];
                _permGradIndex3D[I] = (short) (_perm[I] % (Gradients3D.Length / 3) * 3);
                source[r] = source[I];
            }
        }

        protected override double Value2D(double x, double y)
        {
            var total = 0.0;
            var frequency = Frequency;
            var amplitude = Amplitude;

            for (var I = 0; I < Octaves; I++)
            {
                total += GetValue2D(x * frequency, y * frequency) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Persistance;
            }
            return total;
        }

        private double GetValue2D(double x, double y)
        {
            //Place input coordinates onto grid.
            var stretchOffset = (x + y) * StretchConstant2D;
            var xs = x + stretchOffset;
            var ys = y + stretchOffset;

            //Floor to get grid coordinates of rhombus (stretched square) super-cell origin.
            var xsb = Floor(xs);
            var ysb = Floor(ys);

            //Skew out to get actual coordinates of rhombus origin. We'll need these later.
            var squishOffset = (xsb + ysb) * SquishConstant2D;
            var xb = xsb + squishOffset;
            var yb = ysb + squishOffset;

            //Compute grid coordinates relative to rhombus origin.
            var xins = xs - xsb;
            var yins = ys - ysb;

            //Sum those together to get a value that determines which region we're in.
            var inSum = xins + yins;

            //Positions relative to origin point.
            var dx0 = x - xb;
            var dy0 = y - yb;

            //We'll be defining these inside the next block and using them afterwards.
            double dxExt, dyExt;
            int xsvExt, ysvExt;
            double value = 0;

            //Contribution (1,0)
            var dx1 = dx0 - 1 - SquishConstant2D;
            var dy1 = dy0 - 0 - SquishConstant2D;
            var attn1 = 2 - dx1 * dx1 - dy1 * dy1;
            if (attn1 > 0)
            {
                attn1 *= attn1;
                value += attn1 * attn1 * Extrapolate2D(xsb + 1, ysb + 0, dx1, dy1);
            }

            //Contribution (0,1)
            var dx2 = dx0 - 0 - SquishConstant2D;
            var dy2 = dy0 - 1 - SquishConstant2D;
            var attn2 = 2 - dx2 * dx2 - dy2 * dy2;
            if (attn2 > 0)
            {
                attn2 *= attn2;
                value += attn2 * attn2 * Extrapolate2D(xsb + 0, ysb + 1, dx2, dy2);
            }
            if (inSum <= 1)
            {
                //We're inside the triangle (2-Simplex) at (0,0)
                var zins = 1 - inSum;
                if (zins > xins || zins > yins)
                {
                    //(0,0) is one of the closest two triangular vertices
                    if (xins > yins)
                    {
                        xsvExt = xsb + 1;
                        ysvExt = ysb - 1;
                        dxExt = dx0 - 1;
                        dyExt = dy0 + 1;
                    }
                    else
                    {
                        xsvExt = xsb - 1;
                        ysvExt = ysb + 1;
                        dxExt = dx0 + 1;
                        dyExt = dy0 - 1;
                    }
                }
                else
                {
                    //(1,0) and (0,1) are the closest two vertices.
                    xsvExt = xsb + 1;
                    ysvExt = ysb + 1;
                    dxExt = dx0 - 1 - 2 * SquishConstant2D;
                    dyExt = dy0 - 1 - 2 * SquishConstant2D;
                }
            }
            else
            {
                //We're inside the triangle (2-Simplex) at (1,1)
                var zins = 2 - inSum;
                if (zins < xins || zins < yins)
                {
                    //(0,0) is one of the closest two triangular vertices
                    if (xins > yins)
                    {
                        xsvExt = xsb + 2;
                        ysvExt = ysb + 0;
                        dxExt = dx0 - 2 - 2 * SquishConstant2D;
                        dyExt = dy0 + 0 - 2 * SquishConstant2D;
                    }
                    else
                    {
                        xsvExt = xsb + 0;
                        ysvExt = ysb + 2;
                        dxExt = dx0 + 0 - 2 * SquishConstant2D;
                        dyExt = dy0 - 2 - 2 * SquishConstant2D;
                    }
                }
                else
                {
                    //(1,0) and (0,1) are the closest two vertices.
                    dxExt = dx0;
                    dyExt = dy0;
                    xsvExt = xsb;
                    ysvExt = ysb;
                }
                xsb += 1;
                ysb += 1;
                dx0 = dx0 - 1 - 2 * SquishConstant2D;
                dy0 = dy0 - 1 - 2 * SquishConstant2D;
            }
            //Contribution (0,0) or (1,1)
            var attn0 = 2 - dx0 * dx0 - dy0 * dy0;
            if (attn0 > 0)
            {
                attn0 *= attn0;
                value += attn0 * attn0 * Extrapolate2D(xsb, ysb, dx0, dy0);
            }
            //Extra Vertex
            var attnExt = 2 - dxExt * dxExt - dyExt * dyExt;
            if (attnExt > 0)
            {
                attnExt *= attnExt;
                value += attnExt * attnExt * Extrapolate2D(xsvExt, ysvExt, dxExt, dyExt);
            }
            return value / NormConstant2D;
        }

        public override double Value3D(double x, double y, double z)
        {
            var total = 0.0;
            var frequency = Frequency;
            var amplitude = Amplitude;

            for (var I = 0; I < Octaves; I++)
            {
                total += GetValue3D(x * frequency, y * frequency, z * frequency) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Persistance;
            }
            return total;
        }

        private double GetValue3D(double x, double y, double z)
        {
            //Place input coordinates on simplectic honeycomb.
            var stretchOffset = (x + y + z) * StretchConstant3D;
            var xs = x + stretchOffset;
            var ys = y + stretchOffset;
            var zs = z + stretchOffset;

            //Floor to get simplectic honeycomb coordinates of rhombohedron (stretched cube) super-cell origin.
            var xsb = Floor(xs);
            var ysb = Floor(ys);
            var zsb = Floor(zs);

            //Skew out to get actual coordinates of rhombohedron origin. We'll need these later.
            var squishOffset = (xsb + ysb + zsb) * SquishConstant3D;
            var xb = xsb + squishOffset;
            var yb = ysb + squishOffset;
            var zb = zsb + squishOffset;

            //Compute simplectic honeycomb coordinates relative to rhombohedral origin.
            var xins = xs - xsb;
            var yins = ys - ysb;
            var zins = zs - zsb;

            //Sum those together to get a value that determines which region we're in.
            var inSum = xins + yins + zins;

            //Positions relative to origin point.
            var dx0 = x - xb;
            var dy0 = y - yb;
            var dz0 = z - zb;

            //We'll be defining these inside the next block and using them afterwards.
            double dxExt0, dyExt0, dzExt0;
            double dxExt1, dyExt1, dzExt1;
            int xsvExt0, ysvExt0, zsvExt0;
            int xsvExt1, ysvExt1, zsvExt1;
            double value = 0;
            if (inSum <= 1)
            {
                /*
                 * We're inside the tetrahedron (3-Simplex) at (0,0,0)
                 * Determine which two of (0,0,1), (0,1,0), (1,0,0) are closest.
                 */
                byte aPoint = 0x01;
                var aScore = xins;
                byte bPoint = 0x02;
                var bScore = yins;
                if (aScore >= bScore && zins > bScore)
                {
                    bScore = zins;
                    bPoint = 0x04;
                }
                else if (aScore < bScore && zins > aScore)
                {
                    aScore = zins;
                    aPoint = 0x04;
                }

                /*
                 * Now we determine the two lattice points not part of the tetrahedron that may contribute.
                 * This depends on the closest two tetrahedral vertices, including (0,0,0)
                 */
                var wins = 1 - inSum;
                if (wins > aScore || wins > bScore)
                {
                    //(0,0,0) is one of the closest two tetrahedral vertices.
                    var c = bScore > aScore
                        ? bPoint
                        : aPoint; //Our other closest vertex is the closest out of a and b.
                    if ((c & 0x01) == 0)
                    {
                        xsvExt0 = xsb - 1;
                        xsvExt1 = xsb;
                        dxExt0 = dx0 + 1;
                        dxExt1 = dx0;
                    }
                    else
                    {
                        xsvExt0 = xsvExt1 = xsb + 1;
                        dxExt0 = dxExt1 = dx0 - 1;
                    }

                    if ((c & 0x02) == 0)
                    {
                        ysvExt0 = ysvExt1 = ysb;
                        dyExt0 = dyExt1 = dy0;
                        if ((c & 0x01) == 0)
                        {
                            ysvExt1 -= 1;
                            dyExt1 += 1;
                        }
                        else
                        {
                            ysvExt0 -= 1;
                            dyExt0 += 1;
                        }
                    }
                    else
                    {
                        ysvExt0 = ysvExt1 = ysb + 1;
                        dyExt0 = dyExt1 = dy0 - 1;
                    }

                    if ((c & 0x04) == 0)
                    {
                        zsvExt0 = zsb;
                        zsvExt1 = zsb - 1;
                        dzExt0 = dz0;
                        dzExt1 = dz0 + 1;
                    }
                    else
                    {
                        zsvExt0 = zsvExt1 = zsb + 1;
                        dzExt0 = dzExt1 = dz0 - 1;
                    }
                }
                else
                {
                    //(0,0,0) is not one of the closest two tetrahedral vertices.
                    var c = (byte) (aPoint | bPoint); //Our two extra vertices are determined by the closest two.
                    if ((c & 0x01) == 0)
                    {
                        xsvExt0 = xsb;
                        xsvExt1 = xsb - 1;
                        dxExt0 = dx0 - 2 * SquishConstant3D;
                        dxExt1 = dx0 + 1 - SquishConstant3D;
                    }
                    else
                    {
                        xsvExt0 = xsvExt1 = xsb + 1;
                        dxExt0 = dx0 - 1 - 2 * SquishConstant3D;
                        dxExt1 = dx0 - 1 - SquishConstant3D;
                    }

                    if ((c & 0x02) == 0)
                    {
                        ysvExt0 = ysb;
                        ysvExt1 = ysb - 1;
                        dyExt0 = dy0 - 2 * SquishConstant3D;
                        dyExt1 = dy0 + 1 - SquishConstant3D;
                    }
                    else
                    {
                        ysvExt0 = ysvExt1 = ysb + 1;
                        dyExt0 = dy0 - 1 - 2 * SquishConstant3D;
                        dyExt1 = dy0 - 1 - SquishConstant3D;
                    }

                    if ((c & 0x04) == 0)
                    {
                        zsvExt0 = zsb;
                        zsvExt1 = zsb - 1;
                        dzExt0 = dz0 - 2 * SquishConstant3D;
                        dzExt1 = dz0 + 1 - SquishConstant3D;
                    }
                    else
                    {
                        zsvExt0 = zsvExt1 = zsb + 1;
                        dzExt0 = dz0 - 1 - 2 * SquishConstant3D;
                        dzExt1 = dz0 - 1 - SquishConstant3D;
                    }
                }

                //Contribution (0,0,0)
                var attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
                if (attn0 > 0)
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate3D(xsb + 0, ysb + 0, zsb + 0, dx0, dy0, dz0);
                }

                //Contribution (1,0,0)
                var dx1 = dx0 - 1 - SquishConstant3D;
                var dy1 = dy0 - 0 - SquishConstant3D;
                var dz1 = dz0 - 0 - SquishConstant3D;
                var attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
                }

                //Contribution (0,1,0)
                var dx2 = dx0 - 0 - SquishConstant3D;
                var dy2 = dy0 - 1 - SquishConstant3D;
                var dz2 = dz1;
                var attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
                }

                //Contribution (0,0,1)
                var dx3 = dx2;
                var dy3 = dy1;
                var dz3 = dz0 - 1 - SquishConstant3D;
                var attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate3D(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
                }
            }
            else if (inSum >= 2)
            {
                /*
                 * We're inside the tetrahedron (3-Simplex) at (1,1,1)
                 * Determine which two tetrahedral vertices are the closest, out of (1,1,0), (1,0,1), (0,1,1) but not (1,1,1).
                 */
                byte aPoint = 0x06;
                var aScore = xins;
                byte bPoint = 0x05;
                var bScore = yins;
                if (aScore <= bScore && zins < bScore)
                {
                    bScore = zins;
                    bPoint = 0x03;
                }
                else if (aScore > bScore && zins < aScore)
                {
                    aScore = zins;
                    aPoint = 0x03;
                }

                /*
                 * Now we determine the two lattice points not part of the tetrahedron that may contribute.
                 * This depends on the closest two tetrahedral vertices, including (1,1,1)
                 */
                var wins = 3 - inSum;
                if (wins < aScore || wins < bScore)
                {
                    //(1,1,1) is one of the closest two tetrahedral vertices.
                    var c = bScore < aScore
                        ? bPoint
                        : aPoint; //Our other closest vertex is the closest out of a and b.
                    if ((c & 0x01) != 0)
                    {
                        xsvExt0 = xsb + 2;
                        xsvExt1 = xsb + 1;
                        dxExt0 = dx0 - 2 - 3 * SquishConstant3D;
                        dxExt1 = dx0 - 1 - 3 * SquishConstant3D;
                    }
                    else
                    {
                        xsvExt0 = xsvExt1 = xsb;
                        dxExt0 = dxExt1 = dx0 - 3 * SquishConstant3D;
                    }

                    if ((c & 0x02) != 0)
                    {
                        ysvExt0 = ysvExt1 = ysb + 1;
                        dyExt0 = dyExt1 = dy0 - 1 - 3 * SquishConstant3D;
                        if ((c & 0x01) != 0)
                        {
                            ysvExt1 += 1;
                            dyExt1 -= 1;
                        }
                        else
                        {
                            ysvExt0 += 1;
                            dyExt0 -= 1;
                        }
                    }
                    else
                    {
                        ysvExt0 = ysvExt1 = ysb;
                        dyExt0 = dyExt1 = dy0 - 3 * SquishConstant3D;
                    }

                    if ((c & 0x04) != 0)
                    {
                        zsvExt0 = zsb + 1;
                        zsvExt1 = zsb + 2;
                        dzExt0 = dz0 - 1 - 3 * SquishConstant3D;
                        dzExt1 = dz0 - 2 - 3 * SquishConstant3D;
                    }
                    else
                    {
                        zsvExt0 = zsvExt1 = zsb;
                        dzExt0 = dzExt1 = dz0 - 3 * SquishConstant3D;
                    }
                }
                else
                {
                    //(1,1,1) is not one of the closest two tetrahedral vertices.
                    var c = (byte) (aPoint & bPoint); //Our two extra vertices are determined by the closest two.
                    if ((c & 0x01) != 0)
                    {
                        xsvExt0 = xsb + 1;
                        xsvExt1 = xsb + 2;
                        dxExt0 = dx0 - 1 - SquishConstant3D;
                        dxExt1 = dx0 - 2 - 2 * SquishConstant3D;
                    }
                    else
                    {
                        xsvExt0 = xsvExt1 = xsb;
                        dxExt0 = dx0 - SquishConstant3D;
                        dxExt1 = dx0 - 2 * SquishConstant3D;
                    }

                    if ((c & 0x02) != 0)
                    {
                        ysvExt0 = ysb + 1;
                        ysvExt1 = ysb + 2;
                        dyExt0 = dy0 - 1 - SquishConstant3D;
                        dyExt1 = dy0 - 2 - 2 * SquishConstant3D;
                    }
                    else
                    {
                        ysvExt0 = ysvExt1 = ysb;
                        dyExt0 = dy0 - SquishConstant3D;
                        dyExt1 = dy0 - 2 * SquishConstant3D;
                    }

                    if ((c & 0x04) != 0)
                    {
                        zsvExt0 = zsb + 1;
                        zsvExt1 = zsb + 2;
                        dzExt0 = dz0 - 1 - SquishConstant3D;
                        dzExt1 = dz0 - 2 - 2 * SquishConstant3D;
                    }
                    else
                    {
                        zsvExt0 = zsvExt1 = zsb;
                        dzExt0 = dz0 - SquishConstant3D;
                        dzExt1 = dz0 - 2 * SquishConstant3D;
                    }
                }

                //Contribution (1,1,0)
                var dx3 = dx0 - 1 - 2 * SquishConstant3D;
                var dy3 = dy0 - 1 - 2 * SquishConstant3D;
                var dz3 = dz0 - 0 - 2 * SquishConstant3D;
                var attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate3D(xsb + 1, ysb + 1, zsb + 0, dx3, dy3, dz3);
                }

                //Contribution (1,0,1)
                var dx2 = dx3;
                var dy2 = dy0 - 0 - 2 * SquishConstant3D;
                var dz2 = dz0 - 1 - 2 * SquishConstant3D;
                var attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 1, dx2, dy2, dz2);
                }

                //Contribution (0,1,1)
                var dx1 = dx0 - 0 - 2 * SquishConstant3D;
                var dy1 = dy3;
                var dz1 = dz2;
                var attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 1, dx1, dy1, dz1);
                }

                //Contribution (1,1,1)
                dx0 = dx0 - 1 - 3 * SquishConstant3D;
                dy0 = dy0 - 1 - 3 * SquishConstant3D;
                dz0 = dz0 - 1 - 3 * SquishConstant3D;
                var attn0 = 2 - dx0 * dx0 - dy0 * dy0 - dz0 * dz0;
                if (attn0 > 0)
                {
                    attn0 *= attn0;
                    value += attn0 * attn0 * Extrapolate3D(xsb + 1, ysb + 1, zsb + 1, dx0, dy0, dz0);
                }
            }
            else
            {
                //We're inside the octahedron (Rectified 3-Simplex) in between.
                double aScore;
                byte aPoint;
                bool aIsFurtherSide;
                double bScore;
                byte bPoint;
                bool bIsFurtherSide;

                //Decide between point (0,0,1) and (1,1,0) as closest
                var p1 = xins + yins;
                if (p1 > 1)
                {
                    aScore = p1 - 1;
                    aPoint = 0x03;
                    aIsFurtherSide = true;
                }
                else
                {
                    aScore = 1 - p1;
                    aPoint = 0x04;
                    aIsFurtherSide = false;
                }

                //Decide between point (0,1,0) and (1,0,1) as closest
                var p2 = xins + zins;
                if (p2 > 1)
                {
                    bScore = p2 - 1;
                    bPoint = 0x05;
                    bIsFurtherSide = true;
                }
                else
                {
                    bScore = 1 - p2;
                    bPoint = 0x02;
                    bIsFurtherSide = false;
                }

                //The closest out of the two (1,0,0) and (0,1,1) will replace the furthest out of the two decided above, if closer.
                var p3 = yins + zins;
                if (p3 > 1)
                {
                    var score = p3 - 1;
                    if (aScore <= bScore && aScore < score)
                    {
                        aPoint = 0x06;
                        aIsFurtherSide = true;
                    }
                    else if (aScore > bScore && bScore < score)
                    {
                        bPoint = 0x06;
                        bIsFurtherSide = true;
                    }
                }
                else
                {
                    var score = 1 - p3;
                    if (aScore <= bScore && aScore < score)
                    {
                        aPoint = 0x01;
                        aIsFurtherSide = false;
                    }
                    else if (aScore > bScore && bScore < score)
                    {
                        bPoint = 0x01;
                        bIsFurtherSide = false;
                    }
                }

                //Where each of the two closest points are determines how the extra two vertices are calculated.
                if (aIsFurtherSide == bIsFurtherSide)
                {
                    if (aIsFurtherSide)
                    {
                        //Both closest points on (1,1,1) side

                        //One of the two extra points is (1,1,1)
                        dxExt0 = dx0 - 1 - 3 * SquishConstant3D;
                        dyExt0 = dy0 - 1 - 3 * SquishConstant3D;
                        dzExt0 = dz0 - 1 - 3 * SquishConstant3D;
                        xsvExt0 = xsb + 1;
                        ysvExt0 = ysb + 1;
                        zsvExt0 = zsb + 1;

                        //Other extra point is based on the shared axis.
                        var c = (byte) (aPoint & bPoint);
                        if ((c & 0x01) != 0)
                        {
                            dxExt1 = dx0 - 2 - 2 * SquishConstant3D;
                            dyExt1 = dy0 - 2 * SquishConstant3D;
                            dzExt1 = dz0 - 2 * SquishConstant3D;
                            xsvExt1 = xsb + 2;
                            ysvExt1 = ysb;
                            zsvExt1 = zsb;
                        }
                        else if ((c & 0x02) != 0)
                        {
                            dxExt1 = dx0 - 2 * SquishConstant3D;
                            dyExt1 = dy0 - 2 - 2 * SquishConstant3D;
                            dzExt1 = dz0 - 2 * SquishConstant3D;
                            xsvExt1 = xsb;
                            ysvExt1 = ysb + 2;
                            zsvExt1 = zsb;
                        }
                        else
                        {
                            dxExt1 = dx0 - 2 * SquishConstant3D;
                            dyExt1 = dy0 - 2 * SquishConstant3D;
                            dzExt1 = dz0 - 2 - 2 * SquishConstant3D;
                            xsvExt1 = xsb;
                            ysvExt1 = ysb;
                            zsvExt1 = zsb + 2;
                        }
                    }
                    else
                    {
                        /*
                         * Both closest points on (0,0,0) side
                         * One of the two extra points is (0,0,0)
                         */
                        dxExt0 = dx0;
                        dyExt0 = dy0;
                        dzExt0 = dz0;
                        xsvExt0 = xsb;
                        ysvExt0 = ysb;
                        zsvExt0 = zsb;

                        //Other extra point is based on the omitted axis.
                        var c = (byte) (aPoint | bPoint);
                        if ((c & 0x01) == 0)
                        {
                            dxExt1 = dx0 + 1 - SquishConstant3D;
                            dyExt1 = dy0 - 1 - SquishConstant3D;
                            dzExt1 = dz0 - 1 - SquishConstant3D;
                            xsvExt1 = xsb - 1;
                            ysvExt1 = ysb + 1;
                            zsvExt1 = zsb + 1;
                        }
                        else if ((c & 0x02) == 0)
                        {
                            dxExt1 = dx0 - 1 - SquishConstant3D;
                            dyExt1 = dy0 + 1 - SquishConstant3D;
                            dzExt1 = dz0 - 1 - SquishConstant3D;
                            xsvExt1 = xsb + 1;
                            ysvExt1 = ysb - 1;
                            zsvExt1 = zsb + 1;
                        }
                        else
                        {
                            dxExt1 = dx0 - 1 - SquishConstant3D;
                            dyExt1 = dy0 - 1 - SquishConstant3D;
                            dzExt1 = dz0 + 1 - SquishConstant3D;
                            xsvExt1 = xsb + 1;
                            ysvExt1 = ysb + 1;
                            zsvExt1 = zsb - 1;
                        }
                    }
                }
                else
                {
                    //One point on (0,0,0) side, one point on (1,1,1) side
                    byte c1, c2;
                    if (aIsFurtherSide)
                    {
                        c1 = aPoint;
                        c2 = bPoint;
                    }
                    else
                    {
                        c1 = bPoint;
                        c2 = aPoint;
                    }

                    //One contribution is a permutation of (1,1,-1)
                    if ((c1 & 0x01) == 0)
                    {
                        dxExt0 = dx0 + 1 - SquishConstant3D;
                        dyExt0 = dy0 - 1 - SquishConstant3D;
                        dzExt0 = dz0 - 1 - SquishConstant3D;
                        xsvExt0 = xsb - 1;
                        ysvExt0 = ysb + 1;
                        zsvExt0 = zsb + 1;
                    }
                    else if ((c1 & 0x02) == 0)
                    {
                        dxExt0 = dx0 - 1 - SquishConstant3D;
                        dyExt0 = dy0 + 1 - SquishConstant3D;
                        dzExt0 = dz0 - 1 - SquishConstant3D;
                        xsvExt0 = xsb + 1;
                        ysvExt0 = ysb - 1;
                        zsvExt0 = zsb + 1;
                    }
                    else
                    {
                        dxExt0 = dx0 - 1 - SquishConstant3D;
                        dyExt0 = dy0 - 1 - SquishConstant3D;
                        dzExt0 = dz0 + 1 - SquishConstant3D;
                        xsvExt0 = xsb + 1;
                        ysvExt0 = ysb + 1;
                        zsvExt0 = zsb - 1;
                    }

                    //One contribution is a permutation of (0,0,2)
                    dxExt1 = dx0 - 2 * SquishConstant3D;
                    dyExt1 = dy0 - 2 * SquishConstant3D;
                    dzExt1 = dz0 - 2 * SquishConstant3D;
                    xsvExt1 = xsb;
                    ysvExt1 = ysb;
                    zsvExt1 = zsb;
                    if ((c2 & 0x01) != 0)
                    {
                        dxExt1 -= 2;
                        xsvExt1 += 2;
                    }
                    else if ((c2 & 0x02) != 0)
                    {
                        dyExt1 -= 2;
                        ysvExt1 += 2;
                    }
                    else
                    {
                        dzExt1 -= 2;
                        zsvExt1 += 2;
                    }
                }

                //Contribution (1,0,0)
                var dx1 = dx0 - 1 - SquishConstant3D;
                var dy1 = dy0 - 0 - SquishConstant3D;
                var dz1 = dz0 - 0 - SquishConstant3D;
                var attn1 = 2 - dx1 * dx1 - dy1 * dy1 - dz1 * dz1;
                if (attn1 > 0)
                {
                    attn1 *= attn1;
                    value += attn1 * attn1 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 0, dx1, dy1, dz1);
                }

                //Contribution (0,1,0)
                var dx2 = dx0 - 0 - SquishConstant3D;
                var dy2 = dy0 - 1 - SquishConstant3D;
                var dz2 = dz1;
                var attn2 = 2 - dx2 * dx2 - dy2 * dy2 - dz2 * dz2;
                if (attn2 > 0)
                {
                    attn2 *= attn2;
                    value += attn2 * attn2 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 0, dx2, dy2, dz2);
                }

                //Contribution (0,0,1)
                var dx3 = dx2;
                var dy3 = dy1;
                var dz3 = dz0 - 1 - SquishConstant3D;
                var attn3 = 2 - dx3 * dx3 - dy3 * dy3 - dz3 * dz3;
                if (attn3 > 0)
                {
                    attn3 *= attn3;
                    value += attn3 * attn3 * Extrapolate3D(xsb + 0, ysb + 0, zsb + 1, dx3, dy3, dz3);
                }

                //Contribution (1,1,0)
                var dx4 = dx0 - 1 - 2 * SquishConstant3D;
                var dy4 = dy0 - 1 - 2 * SquishConstant3D;
                var dz4 = dz0 - 0 - 2 * SquishConstant3D;
                var attn4 = 2 - dx4 * dx4 - dy4 * dy4 - dz4 * dz4;
                if (attn4 > 0)
                {
                    attn4 *= attn4;
                    value += attn4 * attn4 * Extrapolate3D(xsb + 1, ysb + 1, zsb + 0, dx4, dy4, dz4);
                }

                //Contribution (1,0,1)
                var dx5 = dx4;
                var dy5 = dy0 - 0 - 2 * SquishConstant3D;
                var dz5 = dz0 - 1 - 2 * SquishConstant3D;
                var attn5 = 2 - dx5 * dx5 - dy5 * dy5 - dz5 * dz5;
                if (attn5 > 0)
                {
                    attn5 *= attn5;
                    value += attn5 * attn5 * Extrapolate3D(xsb + 1, ysb + 0, zsb + 1, dx5, dy5, dz5);
                }

                //Contribution (0,1,1)
                var dx6 = dx0 - 0 - 2 * SquishConstant3D;
                var dy6 = dy4;
                var dz6 = dz5;
                var attn6 = 2 - dx6 * dx6 - dy6 * dy6 - dz6 * dz6;
                if (attn6 > 0)
                {
                    attn6 *= attn6;
                    value += attn6 * attn6 * Extrapolate3D(xsb + 0, ysb + 1, zsb + 1, dx6, dy6, dz6);
                }
            }

            //First extra vertex
            var attnExt0 = 2 - dxExt0 * dxExt0 - dyExt0 * dyExt0 - dzExt0 * dzExt0;
            if (attnExt0 > 0)
            {
                attnExt0 *= attnExt0;
                value += attnExt0 * attnExt0 * Extrapolate3D(xsvExt0, ysvExt0, zsvExt0, dxExt0, dyExt0, dzExt0);
            }

            //Second extra vertex
            var attnExt1 = 2 - dxExt1 * dxExt1 - dyExt1 * dyExt1 - dzExt1 * dzExt1;
            if (attnExt1 > 0)
            {
                attnExt1 *= attnExt1;
                value += attnExt1 * attnExt1 * Extrapolate3D(xsvExt1, ysvExt1, zsvExt1, dxExt1, dyExt1, dzExt1);
            }
            return value / NormConstant3D;
        }

        private double Extrapolate2D(int xs, int ys, double xd, double yd)
        {
            var index = _perm[(_perm[xs & 0xFF] + ys) & 0xFF] & 0x0E;
            return Gradients2D[index] * xd + Gradients2D[index + 1] * yd;
        }

        private double Extrapolate3D(int xs, int ys, int zs, double xd, double yd, double zd)
        {
            int index = _permGradIndex3D[(_perm[(_perm[xs & 0xFF] + ys) & 0xFF] + zs) & 0xFF];
            return Gradients3D[index] * xd + Gradients3D[index + 1] * yd + Gradients3D[index + 2] * zd;
        }

        private static readonly short[] Gradients2D =
        {
            5, 2, 2, 5,
            -5, 2, -2, 5,
            5, -2, 2, -5,
            -5, -2, -2, -5
        };

        private static readonly short[] Gradients3D =
        {
            -11, 4, 4, -4, 11, 4, -4, 4, 11,
            11, 4, 4, 4, 11, 4, 4, 4, 11,
            -11, -4, 4, -4, -11, 4, -4, -4, 11,
            11, -4, 4, 4, -11, 4, 4, -4, 11,
            -11, 4, -4, -4, 11, -4, -4, 4, -11,
            11, 4, -4, 4, 11, -4, 4, 4, -11,
            -11, -4, -4, -4, -11, -4, -4, -4, -11,
            11, -4, -4, 4, -11, -4, 4, -4, -11
        };
    }
}