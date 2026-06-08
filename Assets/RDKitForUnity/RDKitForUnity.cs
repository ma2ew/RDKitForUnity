using UnityEngine;
using GraphMolWrap;
using System.IO;
using System.Collections.Generic;
using AtomData;
using System;
using System.Collections;
using RDKitForUnityHelpers;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using System.Threading;
namespace RDKitForUnity
{
    public class RDKitUnityFuncs
    {
        private RWMol rdkmol;
        public enum RenderMode
        {
            BONDLINE,
            LINE,
            SIMPLEBONDLINE,
            SIMPLELINE
        }
        public void GenerateBaseMolecule(RenderingData renderingData, string filename, bool genconformer = false, bool showHydrogens = false, RenderMode mode = RenderMode.BONDLINE, float coordinateScale = .1f, float atomRadius = 0.055f, float bondRadius = 0.0085f)
        {
           // Define some basic lists and the RDKit RWMol
            var positionList = new List<Vector3>();
            var elementList = new List<string>();
            Debug.Log("hi");
            try
            {
            rdkmol = RWMol.MolFromPDBFile(filename);
            }
            catch
            {
            rdkmol = RWMol.MolFromPDBFile(filename,false);
            }

            Debug.Log(rdkmol);
            
            if (rdkmol == null)
            {
                Debug.LogError($"RDKit failed to load molecule file: {filename}");
            }
            // Address flags for things such as generating 3D conformers and showing hydrogens
            if (genconformer == true)
            {
                RDKFuncs.addHs(rdkmol);
                DistanceGeom.EmbedMolecule(rdkmol);
                ForceField.MMFFOptimizeMolecule(rdkmol,"MMFF94s");
            }
            else if (showHydrogens == true)
            {
                RDKFuncs.addHs(rdkmol);
                DistanceGeom.EmbedMolecule(rdkmol);
            }

            // Name and define the positions of the elements in the molecule
            var atomVect = rdkmol.getAtoms();
            //renderingData.buffersize += (atomVect.Count*2)+2;
            Debug.Log(renderingData.buffersize);
            foreach (var k in atomVect)
            {
                string element = k.getSymbol();
                var positions = rdkmol.getConformer().getAtomPos(k.getIdx());
                Vector3 pos = new Vector3((float)positions.x, (float)positions.y, (float)positions.z) * coordinateScale;
                positionList.Add(pos);
                elementList.Add(element);
            }
            rdkmol.Kekulize();

            // Establish the basic components of the molecule class
            var molecule = new RDKFUMolecule
            {
                rdkmol = rdkmol,
                AtomPositions = positionList,
                AtomElements = elementList,
                AtomMatrices = new Dictionary<string, List<Matrix4x4>>(),
                BondMatrices = new List<float4x4>(),
                BondColors = new List<float4>()
            };

            // Creating atom matrices for GPU instancing
            for (int i = 0; i < molecule.AtomPositions.Count; i++)
            {
                string e = molecule.AtomElements[i];
                System.Enum.TryParse<Element>(e, true, out Element result);

                if (!molecule.AtomMatrices.ContainsKey(e))
                    molecule.AtomMatrices[e] = new List<Matrix4x4>();
                molecule.AtomMatrices[e].Add(Matrix4x4.TRS(molecule.AtomPositions[i], Quaternion.identity, Vector3.one * (atomRadius*AtomicRadiiAndColors.Radius(result))));
            }

            // Bond logic for creating clean and accurate bonds
            Vector3 firstplaneNormal;
            Vector3 planeNormal;
            var bonds = rdkmol.getBonds();
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
                        void AddBondSegment(Vector3 p1, Vector3 p2, string element, float bondRadius)
                        {
                                        System.Enum.TryParse<Element>(element, true, out Element result);

                            Vector3 midSeg = (p1 + p2) * 0.5f;
                            float length = Vector3.Distance(p1, p2);
                            Vector3 scale = new Vector3(bondRadius, length * 0.5f, bondRadius);
                            molecule.BondMatrices.Add(Matrix4x4.TRS(midSeg, rot, scale));
                            molecule.BondColors.Add(element == "C" ? (float4)(Vector4)Color.white : (float4)(Vector4)AtomicRadiiAndColors.ElementColor(result));
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
                    void AddBondSegment(Vector3 p1, Vector3 p2, string element, float bondRadius)
                    {
                                    System.Enum.TryParse<Element>(element, true, out Element result);

                        Vector3 midSeg = (p1 + p2) * 0.5f;
                        float length = Vector3.Distance(p1, p2);
                        Vector3 scale = new Vector3(bondRadius, length * 0.5f, bondRadius);
                        molecule.BondMatrices.Add(Matrix4x4.TRS(midSeg, rot, scale));
                        molecule.BondColors.Add(element == "C" ? (float4)(Vector4)Color.white : (float4)(Vector4)AtomicRadiiAndColors.ElementColor(result));
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
        molecule.MoleculeRoot = new GameObject();
        molecule.MoleculeRoot.transform.position = Vector3.zero;
        molecule.MoleculeRoot.transform.rotation = Quaternion.identity;
        molecule.oldltw = Matrix4x4.zero;
        RKDFURenderFunctions.AddToMoleculeRenderList(molecule);
        NativeList<Element> elementdata = new NativeList<Element>(molecule.AtomPositions.Count, Allocator.TempJob); 
        NativeList<float3> positiondata = new NativeList<float3>(molecule.AtomPositions.Count, Allocator.TempJob);
        NativeArray<float4x4> bondMatrices = new NativeArray<float4x4>(molecule.BondMatrices.Count, Allocator.TempJob);
        NativeList<float4> bondColors = new NativeList<float4>(molecule.BondMatrices.Count, Allocator.TempJob);
        for (int i = 0; i < molecule.AtomPositions.Count; i++)
        {
            string e = molecule.AtomElements[i];
                                    System.Enum.TryParse<Element>(e, true, out Element result);
            if (!molecule.AtomMatrices.ContainsKey(e))

                molecule.AtomMatrices[e] = new List<Matrix4x4>();

            Matrix4x4 mat = Matrix4x4.TRS(
                molecule.AtomPositions[i],
                Quaternion.identity,
                Vector3.one * (atomRadius * AtomicRadiiAndColors.Radius(result)));

            molecule.AtomMatrices[e].Add(mat);
            positiondata.Add((float3)molecule.AtomPositions[i]);
            elementdata.Add(result);
            // NEW — also push into flat lists for BRG
            molecule.FlatAtomMatrices.Add(mat);
            molecule.FlatAtomColors.Add((Color)(Vector4)AtomicRadiiAndColors.ElementColor(result));
        }
        for (int i = 0; i < molecule.BondMatrices.Count; i++)
        {
            bondMatrices[i] = molecule.BondMatrices[i];
            bondColors.Add(molecule.BondColors[i]);
        }

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var atomprefab = entityManager.CreateEntityQuery(typeof(EntityPrefabComponent));
        var x = atomprefab.ToEntityArray(Allocator.TempJob);
        var y = entityManager.GetComponentData<EntityPrefabComponent>(x[1]);
        var z = entityManager.GetComponentData<EntityPrefabComponent>(x[0]);
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        AtomPlotterJob atomPlotterJob = new AtomPlotterJob
        {
            atomEntity = y.Prefab,
            ECB = ecb.AsParallelWriter(),
            location = positiondata.AsArray(),
            element = elementdata.AsArray()
        
        };
        BondPlotterJob bondPlotterJob = new BondPlotterJob
        {
            bondEntity = z.Prefab,
            ECB = ecb.AsParallelWriter(),
            transformdata = bondMatrices,
            element = bondColors.AsArray()

        };
        JobHandle handle = atomPlotterJob.Schedule(molecule.AtomPositions.Count, 16);
        JobHandle handle2 = bondPlotterJob.Schedule(molecule.BondMatrices.Count, 16, handle);
        handle.Complete();
        handle2.Complete();
        ecb.Playback(entityManager);
        bondMatrices.Dispose();
        bondColors.Dispose();
        positiondata.Dispose();
        elementdata.Dispose();
        x.Dispose(); // Remember to dispose of the ToEntityArray persistent allocation as well!
        ecb.Dispose();
    }
    }
}

