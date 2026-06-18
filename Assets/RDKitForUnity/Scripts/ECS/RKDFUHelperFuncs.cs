using System.Collections.Generic;
using UnityEngine;
using GraphMolWrap;
using System.IO;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using AtomData;
using NUnit.Framework.Interfaces;
namespace RDKitForUnityHelpers
{
    
    // Class for standard Molecules
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; // Needed for float4x4 and float4
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
    
    // public RWMol rdkmol; // Commented out to compile cleanly without RDKit references
    public Matrix4x4 oldltw; 

    
    public RWMol rdkmol;
    }
public class RDKFUMolecule: RDKFUMoleculeParent
{ 

    /// <summary>
    /// Safely cleans up GPU resources to prevent VRAM memory leaks.
    /// </summary>

}

    // Class for VR Molecules
    public class RDKFUMoleculeVR: RDKFUMoleculeParent
    {

        public string Name;
        public string SMILES;
        public LineRenderer LineRenderer;

    }
    public class RKDFURenderFunctions
    {
        public static List<RDKFUMolecule> MoleculeRenderList  = new List<RDKFUMolecule>();
        public static List<RDKFUMoleculeVR> VRMoleculeRenderList  = new List<RDKFUMoleculeVR>();

        public static void AddToMoleculeRenderList(RDKFUMolecule molecule)
        {
            MoleculeRenderList.Add(molecule);
        }
        public static void AddToMoleculeRenderList(RDKFUMoleculeVR molecule)
        {
            VRMoleculeRenderList.Add(molecule);
        }
        public static void RemoveFromMoleculeRenderList(RDKFUMolecule molecule)
        {
            MoleculeRenderList.Remove(molecule);
        }
        public static void RemoveFromMoleculeRenderList(RDKFUMoleculeVR molecule)
        {
            VRMoleculeRenderList.Remove(molecule);
        }  

    }
}
namespace RDKitForUnity
{
    public partial class RDKitUnityFuncs
    {
        public static float4 GammaToLinear(float4 gammaColor)
        {
            return new float4(
                Mathf.GammaToLinearSpace(gammaColor.x),
                Mathf.GammaToLinearSpace(gammaColor.y),
                Mathf.GammaToLinearSpace(gammaColor.z),
                gammaColor.w // alpha unchanged
            );
        }
        partial void BondGen(RDKitForUnityHelpers.RDKFUMoleculeParent molecule, RDKitUnityFuncs.RenderMode mode, float bondRadius)
        {
            // Bond logic for creating clean and accurate bonds
            Vector3 firstplaneNormal;
            Vector3 planeNormal;
            var bonds = molecule.rdkmol.getBonds();
            if (mode == RenderMode.SIMPLEBONDLINE || mode == RenderMode.SIMPLELINE)
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

                        // Add the base simple bond segments
                        AddBondSegment(a, mid, molecule.AtomElements[i], bondRadius);
                        AddBondSegment(b, mid, molecule.AtomElements[j], bondRadius); 
                    }
            }
            else if (mode == RenderMode.BONDLINE || mode == RenderMode.LINE)
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