using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
[ScriptedImporter(1, "res")]
class ResCustomImport : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string rootName = ctx.assetPath;
        var split = rootName.Split('/');
        rootName = split[split.Length - 1];
        rootName = rootName.Split('.')[0];
        GameObject rootObject = new GameObject(rootName);
        Resourcex res = rootObject.AddComponent<Resourcex>();
        res.file = new System.IO.FileInfo(ctx.assetPath);
        res.StartRes();
        ctx.AddObjectToAsset(rootName, rootObject);
        ctx.SetMainObject(rootObject);
    }
}