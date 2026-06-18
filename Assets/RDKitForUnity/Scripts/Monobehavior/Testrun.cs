using UnityEngine;
using RDKitForUnity;
using UnityEngine.InputSystem;
public class Testrun : MonoBehaviour
{
    public RenderingData renderingData;
    public Mesh templateBondMesh;
    public Mesh atomMesh;
    public Material bondMaterial;
    public string test;
    RDKitUnityFuncs _funcs;
    GPUInstancing _instancing;
    Mesh bondMesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _funcs = new RDKitUnityFuncs();
        _instancing = new GPUInstancing();
        bondMesh = new Mesh();
        Vector3[] meshverts = templateBondMesh.vertices;
        for (int i = 0; i < meshverts.Length; i++)
        {
            meshverts[i] = Vector3.Scale(meshverts[i], new Vector3(.4f,2f,.4f));
        }
        bondMesh.vertices = meshverts;
        bondMesh.triangles = templateBondMesh.triangles;
        bondMesh.RecalculateBounds();
        bondMesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        _instancing.DrawMolecule(bondMaterial, bondMesh, atomMesh);
        if (Input.GetKeyDown(KeyCode.P))
        {
            _funcs.GenerateBaseMolecule(renderingData,$"Assets/{test}",mode: renderingData.renderMode, showHydrogens: true);

        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            _funcs.GenerateBaseMolecule(renderingData,$"Assets/{test}",mode: renderingData.renderMode, showHydrogens: true, vr: true);
        }
    }
}
