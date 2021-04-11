using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

public class Resourcex : MonoBehaviour
{
    public FileInfo file;
    public void StartRes(AssetImportContext assetObj = null)
    {
        if (!File.Exists(file.FullName))
        {
            Debug.LogError("RESOURCE FILE NOT FOUND FOR SCENE");
            return;
        }
        BinaryReader br = new BinaryReader(File.Open(file.FullName, FileMode.Open));
        byte[] resource = new byte[file.Length];
        br.Read(resource, 0, (int)file.Length);
        br.Close();
        int pos = 0;
        bool exit = false;
        while ((pos < file.Length) && (!exit))
        {
            string ID = GetLine(resource, ref pos);
            string[] spl = new string[2];
            spl = ID.Split(' ');
            switch (spl[0])
            {
                case "PALETTEFILES":
                    {
                        int num = int.Parse(spl[1]);
                        var pals = gameObject.AddComponent<Palettefiles>();
                        List<Color32> colors = new List<Color32>();

                        for (int i = 0; i < num; i++)
                        {
                            string name = GetLine(resource, ref pos);
                            int DataSize0 = System.BitConverter.ToInt32(resource, pos);
                            pos += 4;
                            char[] plm = new char[4];
                            System.Array.Copy(resource, pos, plm, 0, 4);
                            pos += 4;
                            DataSize0 -= 4;
                            uint DataSize1 = (uint)System.BitConverter.ToInt32(resource, pos);
                            pos += 4;
                            DataSize0 -= 4;
                            while (DataSize0 > 0)
                            {
                                byte[] id = new byte[4];
                                System.Array.Copy(resource, pos, id, 0, 4);
                                string idS = System.Text.Encoding.UTF8.GetString(id);
                                pos += 4;
                                DataSize0 -= 4;
                                switch (idS)
                                {
                                    case "PALT":
                                        {
                                            uint DataSize2 = (uint)System.BitConverter.ToInt32(resource, pos);
                                            pos += 4;
                                            DataSize0 -= 4;
                                            for (int j = 0; j < 768; j += 3)
                                            {
                                                colors.Add(new Color32(resource[pos + j], resource[pos + j + 1], resource[pos + j + 2], 255));
                                            }
                                            pos += 768;
                                            DataSize0 -= 768;
                                            pals.colors = colors;
                                            break;

                                        }
                                    case "OPAC": //TODO
                                        {
                                            uint DataSize2 = (uint)System.BitConverter.ToInt32(resource, pos);
                                            pos += 4;
                                            DataSize0 -= 4;
                                            int Width = System.BitConverter.ToInt32(resource, pos);
                                            int Height = System.BitConverter.ToInt32(resource, pos + 4);
                                            pos += 8;
                                            DataSize0 -= 8;
                                            int Size = System.BitConverter.ToInt32(resource, pos);
                                            pos += 4;
                                            DataSize0 -= 4;
                                            Debug.Log("OPAC " + Width + " " + Height + " " + Size);



                                            pos += Width * Height * Size;
                                            DataSize0 -= Width * Height * Size;






                                            break;
                                        }
                                    case "INTE": //TODO
                                        {
                                            uint DataSize2 = (uint)System.BitConverter.ToInt32(resource, pos);
                                            pos += 4;
                                            DataSize0 -= 4;
                                            int Width = System.BitConverter.ToInt32(resource, pos);
                                            int Height = System.BitConverter.ToInt32(resource, pos + 4);
                                            pos += 8;
                                            DataSize0 -= 8;
                                            int Size = System.BitConverter.ToInt32(resource, pos);
                                            pos += 4;
                                            DataSize0 -= 4;
                                            Debug.Log("INTE " + Width + " " + Height + " " + Size);

                                            pos += Height * Size;
                                            DataSize0 -= Height * Size;


                                            break;
                                        }
                                    default:
                                        {
                                            Debug.LogError("unknown palette id");
                                            Debug.Log(pos + " D " + DataSize0);
                                            pos += 4;
                                            DataSize0 -= 4;
                                            break;
                                        }

                                }

                            }

                        }
                        break;
                    }
                case "SOUNDFILES":
                    {
                        var snds = gameObject.AddComponent<Soundfiles>();
                        int num = int.Parse(spl[1]);
                        for (int i = 0; i < num; i++)
                        {
                            string name = GetLine(resource, ref pos);
                            uint DataSize = (uint)System.BitConverter.ToInt32(resource, pos);
                            pos += 4;
                            byte[] RawData = new byte[DataSize];
                            //br.ReadBytes(4);
                            byte[] newData = new byte[(int)DataSize];
                            System.Array.Copy(resource, pos, newData, 0, DataSize);
                            pos += (int)DataSize;
                            snds.Add(name, (int)DataSize, newData);

                        }
                        break;
                    }
                case "BACKFILES":
                    {
                        var bckf = gameObject.AddComponent<Backfiles>();
                        if (int.Parse(spl[1]) > 0)
                        {
                            Debug.Log("BACKFILES");
                        }
                        break;
                    }
                case "MASKFILES":
                    {
                        int num = int.Parse(spl[1]);
                        var msk = gameObject.AddComponent<Maskfiles>();
                        for (int i = 0; i < num; i++)
                        {
                            string name = GetLine(resource, ref pos);

                            int SectionSize = System.BitConverter.ToInt32(resource, pos);
                            pos += 4;
                            byte[] ident = new byte[4];
                            System.Array.Copy(resource, pos, ident, 0, 4);
                            //br.Read(ident,0,4);
                            if (System.Text.Encoding.UTF8.GetString(ident) == "MASK")
                            {
                                byte[] buffer = new byte[SectionSize];
                                System.Array.Copy(resource, pos, buffer, 0, SectionSize);
                                pos += SectionSize;
                                msk.AddMsk(name, buffer, assetObj);
                            }
                            else
                            {
                                Debug.Log("unknown msk");
                            }

                        }
                        break;
                    }
                case "TEXTUREFILES":
                    {

                        int num = int.Parse(spl[1]);
                        var texs = gameObject.AddComponent<Texturefiles>();
                        for (int i = 0; i < num; i++)
                        {

                            string name = GetLine(resource, ref pos);

                            int SectionSize = System.BitConverter.ToInt32(resource, pos); //= br.ReadInt32();  
                            pos += 4;
                            byte[] buff = new byte[18];
                            System.Array.Copy(resource, pos, buff, 0, 18);
                            pos += 12;
                            short Width = System.BitConverter.ToInt16(resource, pos);// br.ReadInt16();
                            pos += 2;
                            short Height = System.BitConverter.ToInt16(resource, pos);// br.ReadInt16();
                            pos += 2;
                            pos += 2;
                            int format = 0;
                            if (System.BitConverter.ToInt32(resource, pos) == 1179012940)
                            {

                                //byte[] Loff;

                                pos += 4;
                                pos += 4;
                                int SizeImage = System.BitConverter.ToInt32(resource, pos);//br.ReadInt32();  
                                pos += 4;
                                int newsize = pos + 2 * Width * Height;

                                byte[] rawIm = new byte[SizeImage - 30];

                                System.Array.Copy(resource, pos, rawIm, 0, SizeImage - 30);
                                pos += SizeImage - 30;





                                byte[] ident = new byte[4];
                                System.Array.Copy(resource, pos, ident, 0, 4);

                                if (System.Text.Encoding.UTF8.GetString(ident) == "LVMP")
                                {
                                    pos += 4;

                                    //SectionSize = br.ReadInt32();
                                    //int Type = br.ReadInt32();
                                    pos += 8;
                                    int Width1 = System.BitConverter.ToInt32(resource, pos); //br.ReadInt32();
                                    pos += 4;
                                    int Height1 = System.BitConverter.ToInt32(resource, pos); //br.ReadInt32();
                                    pos += 4;
                                    //int HS = br.ReadInt32();
                                    pos += 4;

                                    //Debug.Log(br.BaseStream.Position);

                                    while (Width1 >= 1)
                                    {
                                        for (int j = Width1 * Height1; j > 0; j--)
                                        {
                                            //br.BaseStream.Seek(2,SeekOrigin.Current);
                                            pos += 2;
                                        }

                                        Width1 = Width1 / 2;
                                        Height1 = Height1 / 2;
                                    }


                                    //br.BaseStream.Seek(2,SeekOrigin.Current);

                                    //br.BaseStream.Seek(4,SeekOrigin.Current);//pfrm
                                    pos += 6;
                                    int x = System.BitConverter.ToInt32(resource, pos); //br.ReadInt32();
                                    pos += 4;
                                    //br.Read(); 
                                    pos += 1;
                                    format = resource[pos];
                                    pos += 1;
                                    //br.BaseStream.Seek(x+2,SeekOrigin.Current);//pfrm
                                    pos += x + 2;



                                    //br.ReadInt32();
                                    pos += 4;
                                }
                                else
                                {
                                    pos += 4;

                                    int x = System.BitConverter.ToInt32(resource, pos); //br.ReadInt32();

                                    pos += 4;
                                    //br.Read(); 
                                    pos += 1;
                                    format = resource[pos];
                                    pos += 1;

                                    //br.BaseStream.Seek(x+6,SeekOrigin.Current);//pfrm
                                    pos += x + 6;

                                }

                                texs.addTex(name, Width, Height, rawIm, format, assetObj);
                            }
                            else
                            {
                                /*byte[] palette = new byte[256];
								System.Array.Copy(resource,pos,palette,0,256);
								pos+=256;*/

                                byte[] rawIm = new byte[SectionSize - 18 + 18];
                                System.Array.Copy(buff, rawIm, 18);
                                System.Array.Copy(resource, pos, rawIm, 18, SectionSize - 18);

                                pos += SectionSize - 18;

                                texs.addTex(name, Width, Height, rawIm, 0, assetObj);

                            }
                        }
                        break;
                    }
                case "COLORS":
                    {
                        Colors colorsComp = gameObject.AddComponent<Colors>();
                        int num = int.Parse(spl[1]);
                        for (int i = 0; i < num; i++)
                        {
                            colorsComp.addString(GetLine(resource, ref pos));
                        }
                        break;
                    }
                case "MATERIALS":
                    {
                        Materials mats = gameObject.AddComponent<Materials>();
                        int num = int.Parse(spl[1]);

                        for (int i = 0; i < num; i++)
                        {
                            mats.addMat(GetLine(resource, ref pos), assetObj);
                        }
                        break;
                    }
                case "SOUNDS":
                    {
                        Sounds snds = gameObject.AddComponent<Sounds>();
                        int num = int.Parse(spl[1]);

                        for (int i = 0; i < num; i++)
                        {
                            snds.Sound.Add(GetLine(resource, ref pos));
                        }
                        break;
                    }
                default:
                    {
                        Debug.Log("Unknown id :" + ID);
                        exit = true;
                        break;
                    }
            }
        }

    }


    // Update is called once per frame
    void Update()
    {

    }

    string GetLine(BinaryReader br)
    {
        string result;
        List<byte> chArr = new List<byte>();
        byte b = new byte();
        for (; ; )
        {
            b = br.ReadByte();
            if (b != 0)
            {
                chArr.Add(b);
            }
            else
            {
                break;
            }

        }
        result = System.Text.Encoding.UTF8.GetString(chArr.ToArray());

        return result;
    }
    string GetLine(byte[] br, ref int pos)
    {
        string result;
        List<byte> chArr = new List<byte>();
        while (br[pos] != 0)
        {
            chArr.Add(br[pos]);
            pos++;
        }
        result = System.Text.Encoding.UTF8.GetString(chArr.ToArray());
        pos++;
        return result;
    }

}


public struct ColorsRGB
{
    public byte R, G, B;
}
