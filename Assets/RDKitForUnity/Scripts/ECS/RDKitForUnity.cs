using UnityEngine;
using GraphMolWrap;
using System.Collections.Generic;
using AtomData;
using RDKitForUnityHelpers;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using System.IO;
namespace RDKitForUnity
{

    public partial class RDKitUnityFuncs
    {        
        public enum RenderMode
        {
            BONDLINE,
            LINE,
            SIMPLEBONDLINE,
            SIMPLELINE
        }
        // Try to guess what molfile is being used
        private RWMol RDKitmolgen(string filename, bool sanitize = true)
        {
            switch (filename.Substring(filename.LastIndexOf('.')+1))
            {
                case "pdb":
                return RWMol.MolFromPDBFile(filename,sanitize);
                case "mol2":
                return RWMol.MolFromMol2File(filename,sanitize);
                case "mol":
                case "sdf":
                default:
                return RWMol.MolFromMolFile(filename,sanitize);
            }
        }
        partial void BondGen(RDKFUMoleculeParent molecule, RDKitUnityFuncs.RenderMode mode, float bondRadius);
        private RWMol rdkmol;
        // WIP
        public void GenerateBaseMolecule(RenderingData renderingData, string filename, bool genconformer = false, bool showHydrogens = false, RenderMode mode = RenderMode.BONDLINE, float coordinateScale = .1f, float atomRadius = 0.055f, float bondRadius = 0.0085f, bool vr = false)
        {
           // Define some basic lists and the RDKit RWMol
            var positionList = new List<Vector3>();
            var elementList = new List<int>();
            Debug.Log(filename.Substring(filename.LastIndexOf('.')+1));
            try
            {
            rdkmol = RDKitmolgen(filename);
            }
            catch
            {
            rdkmol = RDKitmolgen(filename,false);
            }

            Debug.Log(rdkmol);
            
            if (rdkmol == null)
            {
                Debug.LogError($"RDKit failed to load molecule file: {filename}");
                return;
            }
            // Address flags for things such as generating 3D conformers and showing hydrogens
            if (showHydrogens == true)
            {
                if (rdkmol.getNumAtoms() >= 200)
                {
                    Debug.LogWarning("WARNING: Adding hydrogens at this number of base atoms will cause a crash. Skipping showHydrogens section");
                }
                else
                {
                    RDKFuncs.addHs(rdkmol);
                    DistanceGeom.EmbedMolecule(rdkmol);
                    ForceField.MMFFOptimizeMolecule(rdkmol,"MMFF94s");                    
                }

            }

            // Name and define the positions of the elements in the molecule
            var atomVect = rdkmol.getAtoms();
            //renderingData.buffersize += (atomVect.Count*2)+2;
            Debug.Log(renderingData.buffersize);
            foreach (var k in atomVect)
            {
                int element = k.getAtomicNum();
                var positions = rdkmol.getConformer().getAtomPos(k.getIdx());
                Vector3 pos = new Vector3((float)positions.x, (float)positions.y, (float)positions.z) * coordinateScale;
                positionList.Add(pos);
                elementList.Add(element);
            }
            rdkmol.Kekulize();

            // Establish the basic components of the molecule class
            if (vr)
            {
            var molecule = new RDKFUMoleculeVR
            {
                rdkmol = rdkmol,
                AtomPositions = positionList,
                AtomElements = elementList,
                AtomMatrices = new Dictionary<int, List<Matrix4x4>>(),
                BondMatrices = new List<float4x4>(),
                BondColors = new List<float4>()
            };
                for (int i = 0; i < molecule.AtomPositions.Count; i++)
                {
                    var e = molecule.AtomElements[i];
                    var result = (Element)molecule.AtomElements[i]-1; 
                    if (!molecule.AtomMatrices.ContainsKey(e))

                        molecule.AtomMatrices[e] = new List<Matrix4x4>();

                    Matrix4x4 mat = Matrix4x4.TRS(
                        molecule.AtomPositions[i],
                        Quaternion.identity,
                        Vector3.one * (atomRadius * AtomicRadiiAndColors.Radius(result)));

                    molecule.AtomMatrices[e].Add(mat);

                }


            BondGen(molecule, mode, bondRadius);
        
            molecule.MoleculeRoot = new GameObject();
            molecule.MoleculeRoot.transform.position = Vector3.zero;
            molecule.MoleculeRoot.transform.rotation = Quaternion.identity;
            molecule.oldltw = Matrix4x4.zero;
            RKDFURenderFunctions.AddToMoleculeRenderList(molecule);                
            }
            else
            {
                var molecule = new RDKFUMolecule
                {
                    rdkmol = rdkmol,
                    AtomPositions = positionList,
                    AtomElements = elementList,
                    AtomMatrices = new Dictionary<int, List<Matrix4x4>>(),
                    BondMatrices = new List<float4x4>(),
                    BondColors = new List<float4>()
                };


                BondGen(molecule, mode, bondRadius);
                


                NativeArray<float4x4> bondMatrices = new NativeArray<float4x4>(molecule.BondMatrices.Count, Allocator.TempJob);
                NativeArray<float4> bondColors = new NativeArray<float4>(molecule.BondMatrices.Count, Allocator.TempJob);
                for (int i = 0; i < molecule.BondMatrices.Count; i++)
                {
                    bondMatrices[i] = molecule.BondMatrices[i];
                    bondColors[i] = GammaToLinear(molecule.BondColors[i]);
                }

                var ecbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
                var atomEcb = ecbSystem.CreateCommandBuffer().AsParallelWriter();                
                var bondEcb = ecbSystem.CreateCommandBuffer().AsParallelWriter();                  
                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var atomprefab = entityManager.CreateEntityQuery(typeof(AtomPrototypeTag));
                var bondprefab = entityManager.CreateEntityQuery(typeof(BondPrototypeTag));
                var atomSingleton = atomprefab.GetSingletonEntity();
                var bondSingleton = bondprefab.GetSingletonEntity();
                var atomPrefabComponent = entityManager.GetComponentData<EntityPrefabComponent>(atomSingleton);
                var bondPrefabComponent = entityManager.GetComponentData<EntityPrefabComponent>(bondSingleton);
                if (mode == RenderMode.LINE || mode == RenderMode.SIMPLELINE)
                    {      
                        BondPlotterJob bondPlotterJob = new BondPlotterJob
                        {
                            bondEntity = bondPrefabComponent.Prefab,
                            ECB = bondEcb,
                            transformdata = bondMatrices,
                            element = bondColors

                        };
                        JobHandle handle = bondPlotterJob.Schedule(molecule.BondMatrices.Count, 16);
                        ecbSystem.AddJobHandleForProducer(handle);
                    }
                else
                {
                NativeArray<Element> elementdata = new NativeArray<Element>(molecule.AtomPositions.Count, Allocator.TempJob); 
                NativeArray<float3> positiondata = new NativeArray<float3>(molecule.AtomPositions.Count, Allocator.TempJob);
                for (int i = 0; i < molecule.AtomPositions.Count; i++)
                {
                    var e = molecule.AtomElements[i];
                    var result = (Element)e-1;
                    if (!molecule.AtomMatrices.ContainsKey(e))

                        molecule.AtomMatrices[e] = new List<Matrix4x4>();

                    Matrix4x4 mat = Matrix4x4.TRS(
                        molecule.AtomPositions[i],
                        Quaternion.identity,
                        Vector3.one * (atomRadius * AtomicRadiiAndColors.Radius(result)));

                    molecule.AtomMatrices[e].Add(mat);
                    positiondata[i] = (float3)molecule.AtomPositions[i];
                    elementdata[i] = result;
                }
                    AtomPlotterJob atomPlotterJob = new AtomPlotterJob
                    {
                        atomEntity = atomPrefabComponent.Prefab,
                        ECB = atomEcb,
                        location = positiondata,
                        element = elementdata
                    
                    };         
                    BondPlotterJob bondPlotterJob = new BondPlotterJob
                    {
                        bondEntity = bondPrefabComponent.Prefab,
                        ECB = bondEcb,
                        transformdata = bondMatrices,
                        element = bondColors

                    };
                    JobHandle atomHandle = atomPlotterJob.Schedule(molecule.AtomPositions.Count, 16);
                    JobHandle bondHandle = bondPlotterJob.Schedule(molecule.BondMatrices.Count, 16);
                    JobHandle combinedHandle = JobHandle.CombineDependencies(atomHandle, bondHandle);
                    ecbSystem.AddJobHandleForProducer(combinedHandle);                
                }
            }
        }
    }
}

