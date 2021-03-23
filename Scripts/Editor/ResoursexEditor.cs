using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(Resourcex))]
public class ResoursexEditor : Editor
{
    Resourcex objTarget;
    List<string> matNames = new List<string>();
    List<string> textureNames = new List<string>();

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Export texture Assets"))
        {
            Texturefiles tf = objTarget.GetComponent<Texturefiles>();
            for (int i = 0; i < tf.textures.Count; i++)
            {
                var texture = tf.textures[i];
                texture.name = tf.Texturenames[i];
                string[] splits = texture.name.Split('\\'); //Уберем txr директорию
                splits = splits[splits.Length - 1].Split(' '); // Уберем лишние пробелы после .txr
                splits = splits[0].Split('.');//Убираем расширение txr или msk
                texture.name = splits[0] + ".asset"; //
                AssetDatabase.CreateAsset(texture, $"Assets/Textures/{texture.name}");
            }
        }
        if (GUILayout.Button("Export materials"))
        {
            objTarget = target as Resourcex;
            
            Materials tf = objTarget.GetComponent<Materials>();
            for (int i = 0; i < tf.material.Count; i++)
            {
                var material = tf.maths[i];
                material.name = tf.material[i];
                string[] splits = material.name.Split(' '); // Оставляем только имя материала
                material.name = splits[0] + ".mat"; //
                AssetDatabase.CreateAsset(material, $"Assets/Materials/{material.name}");
            }
        }
        if (GUILayout.Button("Prepare materials & textures"))
        {
            PrepareMatNTexNames(textureNames, matNames);
        }
        if (GUILayout.Button("Export res"))
        {

        }
    }
    void PrepareMatNTexNames(List<string> textureNames, List<string> matNames)
    {

        MeshRenderer[] meshRenderers = ((Resourcex)target).GetComponentsInChildren<MeshRenderer>();
        List<Material> materials = new List<Material>();
        foreach (var item in meshRenderers) //чистим список от повторяющихся материалов
        {
            foreach (var mat in item.sharedMaterials)
            {
                if (!materials.Contains(mat))
                {
                    materials.Add(mat);
                }
            }
        }
        int i = 0;
        foreach (var item in materials) //Первичная обработка имени материала, если отсутствует текстура - значит цвет (col), 
                                        //если текстура из ресурсов дб2 (содержит txr) - добавляем как есть
                                        //если текстура не содержит txr (текстура внешняя, не содержится в ресурсах) - обновляем имя для дальнейшего экспорта
        {
            if (!item)
                continue; //FIXME: заплатка из-за импорта
            Debug.Log(item.name);
            if (item.mainTexture)
            {
                if (item.mainTexture.name.Contains("txr\\"))
                {
                    textureNames.Add(item.mainTexture.name);
                }
                else
                {
                    string[] splits = item.mainTexture.name.Split('\\');
                    string texName = splits[splits.Length - 1];
                    texName = texName.Split('.')[0];
                    textureNames.Add($"txr\\{texName}.txr");
                }
            }
            else
            {
                textureNames.Add($"col{i}");
                i++;
            }
        }
        i = 0;
        foreach (var item in textureNames)
        {
            if (item.Contains("col"))
            {
                matNames.Add($"mat_{item} col 0");
            }
            else
            {
                string[] splits = item.Split('\\');
                string matName = splits[splits.Length - 1];
                matName = matName.Split('.')[0];
                matNames.Add($"mat_{matName} tex {i}");
                i++;

            }
            Debug.Log(matNames[matNames.Count - 1]);
        }
    }
    void OnEnable()
    {
        objTarget = target as Resourcex;

        matNames = new List<string>();
        textureNames = new List<string>();
    }
}

