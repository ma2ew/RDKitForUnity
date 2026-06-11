using UnityEngine;
using RDKitForUnity;
public class Testrun : MonoBehaviour
{
    public RenderingData renderingData;
    public Mesh bondmesh;
    public Material bondmaterial;
    public string test;
    RDKitUnityFuncs _funcs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _funcs = new RDKitUnityFuncs();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _funcs.GenerateBaseMolecule(renderingData,$"Assets/{test}",mode: renderingData.renderMode, showHydrogens: true);

        }
    }
}
