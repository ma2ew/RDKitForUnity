using UnityEngine;
using GraphMolWrap;
using Unity.Mathematics;
using AtomData;

namespace RDKitForUnityHelpers
{
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.Mathematics; // Needed for float4x4 and float4
    /// <summary>
    /// The general class that both the Desktop and VR molecule inherit from.
    /// </summary>
    public abstract class RDKFUMoleculeParent
    {
        public List<Vector3> AtomPositions = new List<Vector3>(); 
        public List<int> AtomElements = new List<int>(); 
        public Dictionary<int, List<Matrix4x4>> AtomMatrices = new Dictionary<int, List<Matrix4x4>>(); 
        public List<float4x4> BondMatrices = new List<float4x4>(); 
        public List<float4> BondColors = new List<float4>(); 
        public GameObject MoleculeRoot; 
        public List<Bond.BondType> BondOrders = new List<Bond.BondType>(); 

        public Dictionary<Color, MaterialPropertyBlock> BondMPBs = new Dictionary<Color, MaterialPropertyBlock>(); 
        public Dictionary<Color, MaterialPropertyBlock> AtomMPBs = new Dictionary<Color, MaterialPropertyBlock>(); 

        public Dictionary<Color, List<Matrix4x4>> WorldAtomMatrices = new Dictionary<Color, List<Matrix4x4>>(); 
        public Dictionary<Color, List<Matrix4x4>> WorldBondMatrices = new Dictionary<Color, List<Matrix4x4>>(); 
        public Matrix4x4 oldltw; 
        public RWMol rdkmol;
    }

    /// <summary>
    /// The class used for desktop molecules.
    /// </summary>
    public class RDKFUMolecule: RDKFUMoleculeParent
    { 

    }

    /// <summary>
    /// The class used for VR molecules.
    /// </summary>
    public class RDKFUMoleculeVR: RDKFUMoleculeParent
    {

        public string Name;
        public string SMILES;
        public LineRenderer LineRenderer;

    }
    /// <summary>
    /// Class used for keeping up with molecules fed to the GPU Instancing pipeline.
    /// </summary>
    public class RKDFURenderFunctions
    {
        /// <summary>
        /// Renderlist for Desktop and VRmolecules. The GPU Instancing function iterates through this list per molecule.
        /// While a renderlist exists for both Desktop and VR molecules, it is highly advisable to stick with ECS rendering on the Desktop molecules.
        /// </summary>
        public static List<RDKFUMoleculeParent> MoleculeRenderList  = new List<RDKFUMoleculeParent>();

        public static void AddToMoleculeRenderList(RDKFUMoleculeParent molecule)
        {
            MoleculeRenderList.Add(molecule);
        }
        public static void RemoveFromMoleculeRenderList(RDKFUMoleculeParent molecule)
        {
            MoleculeRenderList.Remove(molecule);
        }


    }
}
namespace RDKitForUnity
{
    public partial class RDKitUnityFuncs
    {
        /// <summary>
        /// Gamma color correction function. Implemented for float4s used in Jobs.
        /// </summary>
        /// <param name="gammaColor"></param>
        /// <returns></returns>
        public static float4 GammaToLinear(float4 gammaColor)
        {
            return new float4(
                Mathf.GammaToLinearSpace(gammaColor.x),
                Mathf.GammaToLinearSpace(gammaColor.y),
                Mathf.GammaToLinearSpace(gammaColor.z),
                gammaColor.w // alpha unchanged
            );
        }
        /// <summary>
        /// Function to generate bonds for a molecule. Currently handles single, double, and triple bonds.
        /// </summary>
        /// <param name="molecule">The molecule object in question. Can be either VR or Desktop, as the function operates on the parent class.</param>
        /// <param name="mode">The render mode currently in use (bondline, ball+stick, etc.) </param>
        /// <param name="bondRadius">User-derived radius parameter.</param>
        partial void BondGen(RDKitForUnityHelpers.RDKFUMoleculeParent molecule, RDKitUnityFuncs.RenderMode mode, float bondRadius)
        {
            // Normal #1 for sp2 alignment w/ respect to double bond neighbors.
            Vector3 firstplaneNormal;
            // Normal #1 for sp2 alignment w/ respect to double bond neighbors. This plane normal is necessary as the first normal is perpendicular to the sp2 plane.
            Vector3 planeNormal;
            var bonds = molecule.rdkmol.getBonds();
            // Generates bonds without showing bond order.
            if (mode == RenderMode.SIMPLEBALLSTICK || mode == RenderMode.SIMPLEBONDLINE)
            {
                foreach (Bond bond in bonds)
                    {
                        Bond.BondType bondOrder = bond.getBondType();
                        molecule.BondOrders.Add(bondOrder);
                        uint firstAtomIdx = bond.getBeginAtomIdx(); uint secondAtomIdx = bond.getEndAtomIdx();
                        var firstAtom = bond.getBeginAtom(); var secondAtom = bond.getEndAtom();

                        int i = (int)firstAtomIdx;
                        int j = (int)secondAtomIdx;

                        Vector3 a = molecule.AtomPositions[i];
                        Vector3 b = molecule.AtomPositions[j];
                        // Midpoint coordinates between the two atoms
                        Vector3 mid = (a + b) * 0.5f;
                        // Unit vector for direction of the bond
                        Vector3 dir = (b - a).normalized;
                        // Overall rotation quaternion for the bond
                        Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir);

                        // NORMAL SINGLE + COORDINATE/DATIVE BOND
                        void AddBondSegment(Vector3 p1, Vector3 p2, int element, float bondRadius)
                        {
                            var result = (Element)element-1; 
                            // Centers the half-bond segment between the atom and the overall bond midpoint
                            Vector3 midSeg = (p1 + p2) * 0.5f;
                            float length = Vector3.Distance(p1, p2);
                            Vector3 scale = new Vector3(bondRadius, length * 0.5f, bondRadius);
                            molecule.BondMatrices.Add(Matrix4x4.TRS(midSeg, rot, scale));
                            molecule.BondColors.Add(result == Element.C ? (float4)(Vector4)Color.white : (float4)(Vector4)AtomicRadiiAndColors.ElementColor(result));
                        }

                        // Add the base simple bond segments
                        AddBondSegment(a, mid, molecule.AtomElements[i], bondRadius);
                        AddBondSegment(b, mid, molecule.AtomElements[j], bondRadius); 
                    }
            }
            // Generates bonds that factor in bond order.
            else if (mode == RenderMode.BALLSTICK || mode == RenderMode.BONDLINE)
            {
                foreach (Bond bond in bonds)
                {
                    Bond.BondType bondOrder = bond.getBondType();
                    molecule.BondOrders.Add(bondOrder);
                    uint firstAtomIdx = bond.getBeginAtomIdx(); uint secondAtomIdx = bond.getEndAtomIdx();
                    var firstAtom = bond.getBeginAtom(); var secondAtom = bond.getEndAtom();

                    int i = (int)firstAtomIdx;
                    int j = (int)secondAtomIdx;

                    Vector3 a = molecule.AtomPositions[i];
                    Vector3 b = molecule.AtomPositions[j];
                    Vector3 mid = (a + b) * 0.5f;
                    Vector3 dir = (b - a).normalized;
                    Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir);

                    // NORMAL SINGLE + COORDINATE/DATIVE BOND
                    void AddBondSegment(Vector3 p1, Vector3 p2, int element, float bondRadius)
                    {
                            var result = (Element)element-1; 

                        Vector3 midSeg = (p1 + p2) * 0.5f;
                        float length = Vector3.Distance(p1, p2);
                        Vector3 scale = new Vector3(bondRadius, length * 0.5f, bondRadius);
                        molecule.BondMatrices.Add(Matrix4x4.TRS(midSeg, rot, scale));
                        molecule.BondColors.Add(result == Element.C ? (float4)(Vector4)Color.white : (float4)(Vector4)AtomicRadiiAndColors.ElementColor(result));
                    }

                    // Add the base single bond segments
                    if (bondOrder == Bond.BondType.SINGLE || bondOrder == Bond.BondType.DATIVE)
                    {
                        AddBondSegment(a, mid, molecule.AtomElements[i], bondRadius);
                        AddBondSegment(b, mid, molecule.AtomElements[j], bondRadius); 
                    }

                    // DOUBLE BOND OFFSET HANDLING
                    /* Separate logic from the single bonds has to be done here to make the loaded cylinders look neat.
                    Many double bond systems are completely flat geometrically, so this is used to make the two cylinders
                    properly lie in the same plane. due to reliance on one neighbor position, edge cases like 
                    buckminsterfullerene might slightly fail, but it is barely noticeable.*/

                    if (bondOrder == Bond.BondType.DOUBLE)
                    {
                        var aNeighbors = molecule.rdkmol.getAtomNeighbors(firstAtom);
                        var bNeighbors = molecule.rdkmol.getAtomNeighbors(secondAtom);

                        // Pick the first neighbor not in the target double bond system
                        Vector3? aNeighborPos = null;
                        foreach (var n in aNeighbors)
                            if (n.getIdx() != secondAtomIdx) { aNeighborPos = molecule.AtomPositions[(int)n.getIdx()]; break; }

                        // If atom 1 has no neighbors, find the neighbors for atom 2
                        Vector3? bNeighborPos = null;
                        foreach (var n in bNeighbors)
                            if (n.getIdx() != firstAtomIdx) { bNeighborPos = molecule.AtomPositions[(int)n.getIdx()]; break; }

                        if ((aNeighborPos.HasValue && bNeighborPos.HasValue) || aNeighborPos.HasValue)
                        {
                            // Finding the vector going from atom A to the selected neighbor of atom A
                            // Double cross product to create a 90 degree angle with the double bond in the sp2 plane
                            firstplaneNormal = Vector3.Cross(dir, (aNeighborPos.Value - a)).normalized;
                            planeNormal = Vector3.Cross(dir, firstplaneNormal).normalized;
                        }
                        else if (bNeighborPos.HasValue)
                        {
                            firstplaneNormal = Vector3.Cross(dir, (bNeighborPos.Value - b)).normalized;
                            planeNormal = Vector3.Cross(dir, firstplaneNormal).normalized;
                        }
                        else
                        {

                            // Fallback: arbitrary perpendicular
                            planeNormal = Vector3.Cross(Vector3.right, dir);
                            if (planeNormal.sqrMagnitude < 1e-3f)
                                planeNormal = Vector3.Cross(Vector3.up, dir);

                            planeNormal.Normalize();
                        }

                        /* Offset cylinders along plane normal determined from second cross product
                        Two cross products are done to ensure 90 degree angle between the bond and the new vector
                        The reason I don't just take the neighbor minus the target element is because the bond will be lopsided
                        */
                        float offset = bondRadius;
                        Vector3 offsetVec = planeNormal * offset;
                        // Add parallel cylinders (above and below the bond axis)
                        AddBondSegment(a + offsetVec, mid + offsetVec, molecule.AtomElements[i], bondRadius * 0.75f);
                        AddBondSegment(b + offsetVec, mid + offsetVec, molecule.AtomElements[j], bondRadius * 0.75f);

                        AddBondSegment(a - offsetVec, mid - offsetVec, molecule.AtomElements[i], bondRadius * 0.75f);
                        AddBondSegment(b - offsetVec, mid - offsetVec, molecule.AtomElements[j], bondRadius * 0.75f);
                    }
                    // Combination of both single and arbitrary plane-based double bond logic, as it generally doesn't need to be aligned to any specific plane.
                    if (bondOrder == Bond.BondType.TRIPLE)
                    {
                        planeNormal = Vector3.Cross(Vector3.right, dir);
                        if (planeNormal.sqrMagnitude < 1e-3f)
                            planeNormal = Vector3.Cross(Vector3.up, dir);
                        planeNormal.Normalize();
                        float offset = bondRadius;
                        Vector3 offsetVec = planeNormal * offset;
                        // Add parallel cylinders (above and below the bond axis)
                        AddBondSegment(a + offsetVec, mid + offsetVec, molecule.AtomElements[i], bondRadius * 0.75f);
                        AddBondSegment(b + offsetVec, mid + offsetVec, molecule.AtomElements[j], bondRadius * 0.75f);
                        AddBondSegment(a, mid, molecule.AtomElements[i], bondRadius * 0.75f);
                        AddBondSegment(b, mid, molecule.AtomElements[j], bondRadius * 0.75f); 
                        AddBondSegment(a - offsetVec, mid - offsetVec, molecule.AtomElements[i], bondRadius * 0.75f);
                        AddBondSegment(b - offsetVec, mid - offsetVec, molecule.AtomElements[j], bondRadius * 0.75f);
                    }                
                }

            }
        }
    }
}