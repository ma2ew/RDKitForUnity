using System.Collections.Generic;
using UnityEngine;
using GraphMolWrap;
using System.IO;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
namespace RDKitForUnityHelpers
{
    
    // Class for standard Molecules
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; // Needed for float4x4 and float4

public class RDKFUMolecule 
{ 
    public List<Vector3> AtomPositions = new List<Vector3>(); 
    public List<string> AtomElements = new List<string>(); 
    public Dictionary<string, List<Matrix4x4>> AtomMatrices = new Dictionary<string, List<Matrix4x4>>(); 
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
    public List<Matrix4x4> FlatAtomMatrices = new List<Matrix4x4>(); 
    public List<Color> FlatAtomColors = new List<Color>();

    // --- FIXED: Consistent Naming From First Response ---
    
    /// <summary> GPU matrix data arrays for bonds, matching Step 1 naming. </summary>
    public Dictionary<Color, ComputeBuffer> GPUDataBuffers = new Dictionary<Color, ComputeBuffer>();

    /// <summary> GPU draw call arguments for bonds, matching Step 1 naming. </summary>
    public Dictionary<Color, GraphicsBuffer> ArgsBuffers = new Dictionary<Color, GraphicsBuffer>();
    public RWMol rdkmol;

    /// <summary>
    /// Safely cleans up GPU resources to prevent VRAM memory leaks.
    /// </summary>
    public void ReleaseBuffers()
    {
        if (GPUDataBuffers != null)
        {
            foreach (var buffer in GPUDataBuffers.Values) buffer?.Release();
            GPUDataBuffers.Clear();
        }
        if (ArgsBuffers != null)
        {
            foreach (var buffer in ArgsBuffers.Values) buffer?.Release();
            ArgsBuffers.Clear();
        }
    }
}


    // Class for VR Molecules
    public class RDKFUMoleculeVR
    {
            public List<Vector3> AtomPositions = new List<Vector3>();
            public List<string> AtomElements = new List<string>();
            public Dictionary<string, List<Matrix4x4>> AtomMatrices = new Dictionary<string, List<Matrix4x4>>();
            public List<Matrix4x4> BondMatrices = new List<Matrix4x4>();
            public List<Color> BondColors = new List<Color>();
            public GameObject MoleculeRoot;
            public LineRenderer LineRenderer;
            public List<Bond.BondType> BondOrders = new List<Bond.BondType>();
            public Dictionary<Color, MaterialPropertyBlock> BondMPBs = new Dictionary<Color, MaterialPropertyBlock>(); // Updated
            public Dictionary<Color, MaterialPropertyBlock> AtomMPBs = new Dictionary<Color, MaterialPropertyBlock>(); // Updated
            public Matrix4x4 oldltw;
            public Dictionary<Color, List<Matrix4x4>> WorldAtomMatrices = new Dictionary<Color, List<Matrix4x4>>(); // Added
            public Dictionary<Color, List<Matrix4x4>> WorldBondMatrices = new Dictionary<Color, List<Matrix4x4>>(); // Added
            public RWMol rdkmol;
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