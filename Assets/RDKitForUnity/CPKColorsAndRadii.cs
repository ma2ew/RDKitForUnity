using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
namespace AtomData
{
    // Enum matching all supported elements
    public enum Element
    {
        H, HE, LI, BE, B, C, N, O, F, NE,
        NA, MG, AL, SI, P, S, CL, AR,
        K, CA, SC, TI, V, CR, MN, FE, CO, NI, CU, ZN,
        GA, GE, AS, SE, BR, KR,
        RB, SR, Y, ZR, NB, MO, TC, RU, RH, PD, AG, CD,
        IN, SN, SB, TE, I, XE,
        CS, BA,
        LA, CE, PR, ND, PM, SM, EU, GD, TB, DY,
        HO, ER, TM, YB, LU,
        HF, TA, W, RE, OS, IR, PT, AU, HG,
        PB, U,
        UNKNOWN
    }
    public static class AtomicRadiiAndColors 
    {
        public static float Radius(Element element)
        {
            switch (element)
            {
                // Calculated, not empirical
                case Element.H:  return 0.53f;
                case Element.HE: return 0.31f;
                case Element.LI: return 1.67f;
                case Element.BE: return 1.12f;
                case Element.B:  return 0.87f;
                case Element.C:  return 0.67f;
                case Element.N:  return 0.56f;
                case Element.O:  return 0.48f;
                case Element.F:  return 0.42f;
                case Element.NE: return 0.38f;
                case Element.NA: return 1.90f;
                case Element.MG: return 1.45f;
                case Element.AL: return 1.18f;
                case Element.SI: return 1.11f;
                case Element.P:  return 0.98f;
                case Element.S:  return 0.88f;
                case Element.CL: return 0.79f;
                case Element.AR: return 0.71f;
                case Element.K:  return 2.43f;
                case Element.CA: return 1.94f;
                case Element.SC: return 1.84f;
                case Element.TI: return 1.76f;
                case Element.V:  return 1.71f;
                case Element.CR: return 1.66f;
                case Element.MN: return 1.61f;
                case Element.FE: return 1.56f;
                case Element.CO: return 1.52f;
                case Element.NI: return 1.49f;
                case Element.CU: return 1.45f;
                case Element.ZN: return 1.42f;
                case Element.GA: return 1.36f;
                case Element.GE: return 1.25f;
                case Element.AS: return 1.14f;
                case Element.SE: return 1.03f;
                case Element.BR: return 0.94f;
                case Element.KR: return 0.88f;
                case Element.RB: return 2.65f;
                case Element.SR: return 2.19f;
                case Element.Y:  return 2.12f;
                case Element.ZR: return 2.06f;
                case Element.NB: return 1.98f;
                case Element.MO: return 1.90f;
                case Element.TC: return 1.83f;
                case Element.RU: return 1.78f;
                case Element.RH: return 1.73f;
                case Element.PD: return 1.69f;
                case Element.AG: return 1.65f;
                case Element.CD: return 1.61f;
                case Element.IN: return 1.56f;
                case Element.SN: return 1.45f;
                case Element.SB: return 1.33f;
                case Element.TE: return 1.23f;
                case Element.I:  return 1.15f;
                case Element.XE: return 1.08f;
                case Element.CS: return 2.98f;
                case Element.BA: return 2.53f;

                // Lanthanides
                case Element.LA: return 2.15f;
                case Element.CE: return 2.04f;
                case Element.PR: return 2.03f;
                case Element.ND: return 2.01f;
                case Element.PM: return 1.99f;
                case Element.SM: return 1.98f;
                case Element.EU: return 1.98f;
                case Element.GD: return 1.96f;
                case Element.TB: return 1.94f;
                case Element.DY: return 1.92f;
                case Element.HO: return 1.92f;
                case Element.ER: return 1.89f;
                case Element.TM: return 1.90f;
                case Element.YB: return 1.87f;
                case Element.LU: return 1.87f;

                // Heavy transition metals
                case Element.HF: return 2.08f;
                case Element.TA: return 2.00f;
                case Element.W:  return 1.93f;
                case Element.RE: return 1.88f;
                case Element.OS: return 1.85f;
                case Element.IR: return 1.80f;
                case Element.PT: return 1.77f;
                case Element.AU: return 1.74f;
                case Element.HG: return 1.71f;
                case Element.PB: return 1.54f;
                case Element.U:  return 1.86f;

                default: return 1f;
            }
        }

    public static float4 ElementColor(Element element) 
    { 
        switch (element) 
        { 
            case Element.H: return new float4(1.000f, 1.000f, 1.000f, 1.000f); 
            case Element.HE: return new float4(0.851f, 1.000f, 1.000f, 1.000f); 
            case Element.LI: return new float4(0.800f, 0.502f, 1.000f, 1.000f); 
            case Element.BE: return new float4(0.761f, 1.000f, 0.000f, 1.000f); 
            case Element.B: return new float4(1.000f, 0.710f, 0.710f, 1.000f); 
            case Element.C: return new float4(0.565f, 0.565f, 0.565f, 1.000f); 
            case Element.N: return new float4(0.188f, 0.314f, 0.973f, 1.000f); 
            case Element.O: return new float4(1.000f, 0.051f, 0.051f, 1.000f); 
            case Element.F: return new float4(0.565f, 0.878f, 0.314f, 1.000f); 
            case Element.NE: return new float4(0.702f, 0.890f, 0.961f, 1.000f); 
            case Element.NA: return new float4(0.671f, 0.361f, 0.949f, 1.000f); 
            case Element.MG: return new float4(0.541f, 1.000f, 0.000f, 1.000f); 
            case Element.AL: return new float4(0.749f, 0.651f, 0.651f, 1.000f); 
            case Element.SI: return new float4(0.941f, 0.784f, 0.627f, 1.000f); 
            case Element.P: return new float4(1.000f, 0.502f, 0.000f, 1.000f); 
            case Element.S: return new float4(1.000f, 1.000f, 0.188f, 1.000f); 
            case Element.CL: return new float4(0.122f, 0.941f, 0.122f, 1.000f); 
            case Element.AR: return new float4(0.502f, 0.820f, 0.890f, 1.000f); 
            case Element.K: return new float4(0.561f, 0.251f, 0.831f, 1.000f); 
            case Element.CA: return new float4(0.239f, 1.000f, 0.000f, 1.000f); 
            case Element.SC: return new float4(0.902f, 0.902f, 0.902f, 1.000f); 
            case Element.TI: return new float4(0.749f, 0.761f, 0.780f, 1.000f); 
            case Element.V: return new float4(0.651f, 0.651f, 0.671f, 1.000f); 
            case Element.CR: return new float4(0.541f, 0.600f, 0.780f, 1.000f); 
            case Element.MN: return new float4(0.612f, 0.478f, 0.780f, 1.000f); 
            case Element.FE: return new float4(0.878f, 0.400f, 0.200f, 1.000f); 
            case Element.CO: return new float4(0.941f, 0.565f, 0.627f, 1.000f); 
            case Element.NI: return new float4(0.314f, 0.816f, 0.314f, 1.000f); 
            case Element.CU: return new float4(0.784f, 0.502f, 0.200f, 1.000f); 
            case Element.ZN: return new float4(0.490f, 0.502f, 0.690f, 1.000f); 
            case Element.GA: return new float4(0.761f, 0.561f, 0.561f, 1.000f); 
            case Element.GE: return new float4(0.400f, 0.561f, 0.561f, 1.000f); 
            case Element.AS: return new float4(0.741f, 0.502f, 0.890f, 1.000f); 
            case Element.SE: return new float4(1.000f, 0.631f, 0.000f, 1.000f); 
            case Element.BR: return new float4(0.651f, 0.161f, 0.161f, 1.000f); 
            case Element.KR: return new float4(0.361f, 0.722f, 0.820f, 1.000f); 
            case Element.RB: return new float4(0.439f, 0.180f, 0.690f, 1.000f); 
            case Element.SR: return new float4(0.000f, 1.000f, 0.000f, 1.000f); 
            case Element.Y: return new float4(0.580f, 1.000f, 1.000f, 1.000f); 
            case Element.ZR: return new float4(0.580f, 0.878f, 0.878f, 1.000f); 
            case Element.NB: return new float4(0.451f, 0.761f, 0.788f, 1.000f); 
            case Element.MO: return new float4(0.329f, 0.710f, 0.710f, 1.000f); 
            case Element.TC: return new float4(0.231f, 0.620f, 0.620f, 1.000f); 
            case Element.RU: return new float4(0.141f, 0.561f, 0.561f, 1.000f); 
            case Element.RH: return new float4(0.039f, 0.490f, 0.549f, 1.000f); 
            case Element.PD: return new float4(0.000f, 0.412f, 0.522f, 1.000f); 
            case Element.AG: return new float4(0.753f, 0.753f, 0.753f, 1.000f); 
            case Element.CD: return new float4(1.000f, 0.851f, 0.561f, 1.000f); 
            case Element.IN: return new float4(0.651f, 0.459f, 0.451f, 1.000f); 
            case Element.SN: return new float4(0.400f, 0.502f, 0.502f, 1.000f); 
            case Element.SB: return new float4(0.620f, 0.388f, 0.710f, 1.000f); 
            case Element.TE: return new float4(0.831f, 0.478f, 0.000f, 1.000f); 
            case Element.I: return new float4(0.580f, 0.000f, 0.580f, 1.000f); 
            case Element.XE: return new float4(0.259f, 0.620f, 0.690f, 1.000f); 
            case Element.CS: return new float4(0.341f, 0.090f, 0.561f, 1.000f); 
            case Element.BA: return new float4(0.000f, 0.788f, 0.000f, 1.000f); 
            case Element.LA: case Element.CE: case Element.PR: case Element.ND: 
            case Element.PM: case Element.SM: case Element.EU: case Element.GD: 
            case Element.TB: case Element.DY: case Element.HO: case Element.ER: 
            case Element.TM: case Element.YB: case Element.LU: 
                return new float4(0.439f, 0.831f, 1.000f, 1.000f); 
            case Element.HF: case Element.TA: case Element.W: case Element.RE: 
            case Element.OS: case Element.IR: case Element.PT: case Element.AU: 
            case Element.HG: 
                return new float4(0.722f, 0.722f, 0.816f, 1.000f); 
            case Element.PB: return new float4(0.341f, 0.349f, 0.380f, 1.000f); 
            case Element.U: return new float4(0.000f, 0.561f, 1.000f, 1.000f); 
            default: return new float4(1.0f, 0.0f, 1.0f, 1.0f); // Pure Magenta alpha 1
        } 
        }

        // Optional helper if parsing from strings is still needed
        public static Element Parse(string symbol)
        {
            if (System.Enum.TryParse(symbol.ToUpperInvariant(), out Element result))
                return result;

            return Element.UNKNOWN;
        }
    }
}