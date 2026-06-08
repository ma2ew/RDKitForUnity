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
    public class RDKFUMolecule
    {

            public List<Vector3> AtomPositions = new List<Vector3>();
            public List<string> AtomElements = new List<string>();
            public Dictionary<string, List<Matrix4x4>> AtomMatrices = new Dictionary<string, List<Matrix4x4>>();
            public List<float4x4> BondMatrices = new List<float4x4>();
            public List<float4> BondColors = new List<float4>();
            public GameObject MoleculeRoot;
            public List<Bond.BondType> BondOrders = new List<Bond.BondType>();
            public Dictionary<Color, MaterialPropertyBlock> BondMPBs = new Dictionary<Color, MaterialPropertyBlock>(); // Updated
            public Dictionary<Color, MaterialPropertyBlock> AtomMPBs = new Dictionary<Color, MaterialPropertyBlock>(); // Updated
            public Dictionary<Color, List<Matrix4x4>> WorldAtomMatrices = new Dictionary<Color, List<Matrix4x4>>(); // Added
            public Dictionary<Color, List<Matrix4x4>> WorldBondMatrices = new Dictionary<Color, List<Matrix4x4>>(); // Added
            public RWMol rdkmol;
            public Matrix4x4 oldltw;
            public List<Matrix4x4> FlatAtomMatrices = new List<Matrix4x4>();
            public List<Color>     FlatAtomColors   = new List<Color>();
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