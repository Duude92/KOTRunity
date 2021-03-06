using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            textureNames = new List<string>();
            matNames = new List<string>();
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
        List<Texture2D> textures = new List<Texture2D>();

        var mats = objTarget.GetComponent<Materials>();
        mats.material = new List<string>();
        mats.maths = new List<Material>();

        var texts = objTarget.GetComponent<Texturefiles>();
        texts.textures = new List<Texture2D>();
        texts.Texturenames = new List<string>();

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



            mats.maths.Add(item);
            int tPos = int.MinValue;

            string mName = item.name;
            tPos = mName.IndexOf(" tex ");

            if (tPos == -1)
            {
                tPos = mName.IndexOf(" ttx ");

                if (tPos == -1)
                {
                    tPos = mName.IndexOf(" itx ");

                }
            }

            if (!item.mainTexture)
            {
                matNames.Add($"col{i}");
            }
            else if (tPos != -1)    //если использован материал из импорченого .res файла
            {
                textures.Add((Texture2D)item.mainTexture);
                mName = mName.Substring(0, tPos + 4);
                mName += $" {i} ";
                mName += item.name.Substring(tPos + 8, item.name.Length - tPos - 8);
                matNames.Add(mName);

            }
            else
            {
                textures.Add((Texture2D)item.mainTexture);

                string[] splits = item.mainTexture.name.Split('\\');
                string texName = splits[splits.Length - 1];
                texName = texName.Split('.')[0];
                item.mainTexture.name = $"txr\\{texName}.txr";



                string matName = splits[splits.Length - 1];
                matName = matName.Split('.')[0];
                matNames.Add($"mat_{matName} tex {i}");
            }
            i++;
        }
        StreamWriter file = new StreamWriter($"Assets/{target.name}.pro");
        file.WriteLine($"TEXTUREFILES {textures.Count}");
        foreach (var item in textures)
        {
            file.WriteLine(item.name);
            texts.Texturenames.Add(item.name);
        }
        file.WriteLine($"MATERIALS {matNames.Count}");
        foreach (var item in matNames)
        {
            file.WriteLine(item);
        }
        file.Close();
        AssetDatabase.Refresh();
        mats.material = matNames;
        texts.textures = textures;

        return;
    }
    void OnEnable()
    {
        objTarget = target as Resourcex;

        matNames = new List<string>();
        textureNames = new List<string>();
    }
}

