using System;
using UnityEngine;
using UnityEditor;
using PSG;

/// <summary>
/// 
/// Saving generated meshes.
/// 
/// Meshes and objects will appear in PSG/ directories by default.
/// To load, simply drag object to scene or load it by script.
/// 
/// Watch out for overwritting saved assets!
/// 
/// </summary>
[CustomEditor(typeof(MeshBase), true)]
public class SaveMeshEditor : Editor
{
    //standard override
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshBase targetScript = (MeshBase)target;
        //save MeshFilter's content
        if (GUILayout.Button("Save Mesh Only"))
        {
            SaveMeshToFile(targetScript.C_MF.sharedMesh, targetScript.name);
        }
        //save GameObject
        if (GUILayout.Button("Save Prefab"))
        {
            SavePrefabToFile(targetScript, targetScript.name);
        }
        //rebuild mesh from its values
        if (GUILayout.Button("Rebuild"))
        {
            targetScript.BuildMesh();
        }
    }

    //save MeshFilter's mesh
    private void SaveMeshToFile(Mesh mesh, string name)
    {
        CheckFolders("Saved meshes");

        //make a copy of Mesh to prevent sharing it among other MeshFilters
        Mesh meshCopy = Instantiate(mesh);
        try
        {
            AssetDatabase.CreateAsset(meshCopy, "Assets/PSG/Saved meshes/" + name + ".asset");
            Debug.Log("Mesh \"" + name + ".asset\" saved succesfully at PSG/Saved meshes");
        }
        catch (Exception e)
        {
            Debug.LogError("PSG::Mesh Generation failed! ("+e+")");
        }
    }

    //save entire GameObject
    private void SavePrefabToFile(MeshBase meshBase, string name)
    {
        CheckFolders("Saved prefabs");

        //mesh and it's material need to be saved too
        SaveMeshToFile(meshBase.C_MF.sharedMesh, name + "'mesh");

        try
        {
            PrefabUtility.CreatePrefab("Assets/PSG/Saved prefabs/" + name + ".prefab", meshBase.gameObject);
            Debug.Log("Prefab \"" + name + ".prefab\" saved succesfully at PSG/Saved prefabs");
        }
        catch (Exception e)
        {
            Debug.LogError("Saving prefab error! "+e);
        }
    }

    //save material if necessary
    private void SaveMaterial(Material material, string name)
    {
        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(material)))
        {
            AssetDatabase.CreateAsset(material, "Assets/PSG/Saved meshes/" + name + "'s material.asset");
        }
    }
    
    //utility: check if folder exists in PSG directory
    private void CheckFolders(string targetFolder)
    {
        const string savePath = "Assets/PSG";

        //check for PSG
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            AssetDatabase.CreateFolder("Assets", "PSG");
        }

        //check for {targetFolder}
        if(!AssetDatabase.IsValidFolder("Assets/PSG/"+ targetFolder))
        {
            AssetDatabase.CreateFolder(savePath, targetFolder);
        }
    }

}