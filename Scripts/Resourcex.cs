using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using WWUtils.Audio;


public class Resourcex : MonoBehaviour {

	public FileInfo file;
	public void StartRes () {
		BinaryReader br = new BinaryReader(File.Open(file.FullName,FileMode.Open));
		byte[] resource = new byte[file.Length];
		br.Read(resource,0,(int)file.Length);
		br.Close();
		int pos = 0;
		bool exit = false;
		while ((pos<file.Length)&&(!exit))
		{
				string ID = GetLine(resource, ref pos);
				string[] spl = new string[2];
				spl = ID.Split(' ');
				switch(spl[0])
				{
					case "PALETTEFILES":
					{
						int num = int.Parse(spl[1]);
						var pals = gameObject.AddComponent<Palettefiles>();
						List<Color32> colors = new List<Color32>();

						for (int i = 0;i<num;i++)
						{
							string name = GetLine(resource, ref pos);
							int DataSize0 = System.BitConverter.ToInt32(resource,pos);
							pos+=4;
							char[] plm = new char[4];
							System.Array.Copy(resource,pos,plm,0,4);
							pos+=4;
							DataSize0-=4;
							uint DataSize1 = (uint)System.BitConverter.ToInt32(resource,pos);
							pos+=4;
							DataSize0-=4;
							while (DataSize0>0)
							{
								byte[] id = new byte[4];
								System.Array.Copy(resource,pos,id,0,4);
								string idS = System.Text.Encoding.UTF8.GetString(id);
								pos+=4;
								DataSize0-=4;
								switch (idS)
								{
									case "PALT":
									{
										uint DataSize2 = (uint)System.BitConverter.ToInt32(resource,pos);
										pos+=4;
										DataSize0-=4;
										for (int j = 0;j<768;j+=3)
										{
											colors.Add(new Color32(resource[pos+j],resource[pos+j+1],resource[pos+j+2],255));	
										}
										pos+=768;
										DataSize0-=768;
										pals.colors = colors;
										break;

									}
									case "OPAC": //TODO
									{
										uint DataSize2 = (uint)System.BitConverter.ToInt32(resource,pos);
										pos+=4;
										DataSize0-=4;
										int Width = System.BitConverter.ToInt32(resource,pos);
										int Height = System.BitConverter.ToInt32(resource,pos+4);
										pos+=8;
										DataSize0-=8;
										int Size = System.BitConverter.ToInt32(resource,pos);
										pos+=4;
										DataSize0-=4;
										Debug.Log("OPAC "+Width+" "+Height+" "+Size);



										pos+=Width*Height*Size;
										DataSize0-=Width*Height*Size;






										break;
									}
									case "INTE": //TODO
									{
										uint DataSize2 = (uint)System.BitConverter.ToInt32(resource,pos);
										pos+=4;
										DataSize0-=4;
										int Width = System.BitConverter.ToInt32(resource,pos);
										int Height = System.BitConverter.ToInt32(resource,pos+4);
										pos+=8;
										DataSize0-=8;
										int Size = System.BitConverter.ToInt32(resource,pos);
										pos+=4;
										DataSize0-=4;
										Debug.Log("INTE "+Width+" "+Height+" "+Size);

										pos+=Height*Size;
										DataSize0-=Height*Size;
										

										break;
									}
									default:
									{
										Debug.LogError("unknown palette id");
										Debug.Log(pos+" D "+DataSize0);
										pos+=4;
										DataSize0-=4;
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
						for (int i = 0;i<num;i++)
						{
							string name = GetLine(resource, ref pos);
							uint DataSize = (uint)System.BitConverter.ToInt32(resource,pos);
							pos+=4;
							byte[] RawData = new byte[DataSize];
							//br.ReadBytes(4);
							byte[] newData = new byte[(int)DataSize];
							System.Array.Copy(resource,pos,newData,0,DataSize);
							pos+=(int)DataSize;
							snds.Add(name,(int)DataSize, newData);

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
						for (int i = 0; i<num;i++)
						{
							string name = GetLine(resource, ref pos);

							int SectionSize = System.BitConverter.ToInt32(resource,pos);
							pos+=4;
							byte[] ident = new byte[4];
							System.Array.Copy(resource,pos,ident,0,4);
							//br.Read(ident,0,4);
							if (System.Text.Encoding.UTF8.GetString(ident) == "MASK")
							{
								byte[] buffer = new byte[SectionSize];
								System.Array.Copy(resource,pos,buffer,0,SectionSize);
								pos+=SectionSize;
								msk.AddMsk(name,buffer);
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
						for (int i = 0; i<num;i++)
						{

							string name = GetLine(resource, ref pos);
							
							int SectionSize = System.BitConverter.ToInt32(resource,pos); //= br.ReadInt32();  
							pos+=4;
							byte[] buff = new byte[18];
							System.Array.Copy(resource,pos,buff,0,18);
							/*byte  IDLength = br.ReadByte();
							byte  ColorMapType = br.ReadByte();
							byte  ImageType = br.ReadByte();
							short   FirstIndexEntry = br.ReadInt16();
							short   ColorMapLength = br.ReadInt16();
							byte  ColorMapEntrySize = br.ReadByte();



							short   XOrigin = br.ReadInt16();
							short   YOrigin = br.ReadInt16();*/
							pos+=12;
							short   Width = System.BitConverter.ToInt16(resource,pos);// br.ReadInt16();
							pos+=2;
							short   Height = System.BitConverter.ToInt16(resource,pos);// br.ReadInt16();
							pos+=2;
							/*byte  PixelDepth = br.ReadByte();
							byte  ImageDescriptor = br.ReadByte();*/
							pos+=2;
							int format = 0;
							if (System.BitConverter.ToInt32(resource,pos) == 1179012940)
							{

								//byte[] Loff;

								pos+=4;
								//int LineOffset = br.ReadInt32();  
								pos+=4;
								int SizeImage = System.BitConverter.ToInt32(resource,pos);//br.ReadInt32();  
								pos+=4;
								//Debug.Log(SizeImage);

								// TO TEXTURES CLASS:	Texture2D tex = new Texture2D(Width,Height,TextureFormat.RG16,false);

								int newsize = pos + 2 * Width * Height;

								byte[] rawIm = new byte[SizeImage-30];

								System.Array.Copy(resource,pos,rawIm,0,SizeImage-30);
								pos +=SizeImage-30;
								/*
								for (int j = 0; j<SizeImage-30;j++)
								{
									rawIm[j] = br.ReadByte();
								}*/





								byte[] ident = new byte[4];
								System.Array.Copy(resource,pos,ident,0,4);
								//br.Read(ident,0,4);

								if (System.Text.Encoding.UTF8.GetString(ident) == "LVMP")
								{
									pos+=4;

									//SectionSize = br.ReadInt32();
									//int Type = br.ReadInt32();
									pos+=8;
									int Width1 = System.BitConverter.ToInt32(resource,pos); //br.ReadInt32();
									pos+=4;
									int Height1 = System.BitConverter.ToInt32(resource,pos); //br.ReadInt32();
									pos+=4;
									//int HS = br.ReadInt32();
									pos+=4;

									//Debug.Log(br.BaseStream.Position);
									
									while (Width1>=1)
									{
										for (int j = Width1*Height1;j>0;j--)
										{
											//br.BaseStream.Seek(2,SeekOrigin.Current);
											pos+=2;
										}

										Width1 = Width1/2;
										Height1 = Height1/2;
									}


									//br.BaseStream.Seek(2,SeekOrigin.Current);

									//br.BaseStream.Seek(4,SeekOrigin.Current);//pfrm
									pos+=6;
									int x = System.BitConverter.ToInt32(resource,pos); //br.ReadInt32();
									pos+=4;
									//br.Read(); 
									pos+=1;
									format = resource[pos];
									pos+=1;
									//br.BaseStream.Seek(x+2,SeekOrigin.Current);//pfrm
									pos+=x+2;



									//br.ReadInt32();
									pos+=4;
								}
								else
								{
									pos+=4;

									int x = System.BitConverter.ToInt32(resource,pos); //br.ReadInt32();

									pos+=4;
									//br.Read(); 
									pos+=1;
									format = resource[pos];
									pos+=1;

									//br.BaseStream.Seek(x+6,SeekOrigin.Current);//pfrm
									pos+=x+6;

								}
								//Debug.LogWarning(pos);
								
								texs.addTex(name,Width,Height,rawIm,format);
							}
							else
							{
								/*byte[] palette = new byte[256];
								System.Array.Copy(resource,pos,palette,0,256);
								pos+=256;*/

								byte[] rawIm = new byte[SectionSize-18+18];
								System.Array.Copy(buff,rawIm,18);
								System.Array.Copy(resource,pos,rawIm,18,SectionSize-18);

								pos+=SectionSize-18;

								texs.addTex(name,Width,Height,rawIm,0);

							}
						}
						break;
					}
					case "COLORS":
					{
						Colors colorsComp = gameObject.AddComponent<Colors>();
						int num = int.Parse(spl[1]);
						for (int i = 0; i<num;i++)
						{
							colorsComp.addString(GetLine(resource,ref pos));
						}						
						break;
					}
					case "MATERIALS":
					{
						Materials mats = gameObject.AddComponent<Materials>();
						int num = int.Parse(spl[1]);

						for (int i = 0; i<num;i++)
						{
							mats.addMat(GetLine(resource,ref pos));
						}						
						break;
					}
					case "SOUNDS":
					{
						Sounds snds = gameObject.AddComponent<Sounds>();
						int num = int.Parse(spl[1]);

						for (int i = 0; i<num;i++)
						{
							snds.Sound.Add(GetLine(resource,ref pos));
						}
						break;
					}
					default:
					{
						Debug.Log("Unknown id :"+ID);
						exit = true;
						break;
					}
				}
		}
		
	}
	
	
	// Update is called once per frame
	void Update () {
		
	}

	string GetLine(BinaryReader br)
	{
		string result;
		List<byte> chArr = new List<byte>();
		byte b = new byte();
		for(;;)
		{
			b = br.ReadByte();
			if (b!=0)
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
	string GetLine(byte[] br,ref int pos)
	{
		string result;
		List<byte> chArr = new List<byte>();
		while (br[pos]!=0)
		{
			chArr.Add(br[pos]);
			pos++;
		}
		result = System.Text.Encoding.UTF8.GetString(chArr.ToArray());
		pos++;
		return result;
	}

}



public class Colors : MonoBehaviour {
	public List<string> color = new List<string>();
	int x;
	//public string[] color = new string[num];
	public void addString(string abs)
	{
		color.Add(abs);
	}
}

class Materials : MonoBehaviour {
	public List<string> material = new List<string>();
	public List<Material> maths = new List<Material>();

	public void addMat(string abs)
	{
		material.Add(abs);
		string[] spl = abs.Split(' ');
		var ma = new Material(GameManager.TC);

		
		//tex.




		switch (spl[1])
		{
			case "col":
			{
				
				ma.color = GameManager.common.GetComponent<Palettefiles>().colors[int.Parse(spl[2])-1];
				break;
			}
			case "tex":
			{
				var tex = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2])-1];
				ma = new Material(GameManager.TCu);
				ma.mainTexture = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2])-1];
				break;
			}
			case "ttx":
			{
				var tex = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2])-1];

				ma = new Material(GameManager.TC);
				ma.mainTexture = gameObject.GetComponent<Texturefiles>().textures[int.Parse(spl[2])-1];
				//Debug.LogWarning(spl[1]+", "+spl[4]);
				if (spl.Length>4)
				if (spl[4] == "col")
				{
					//Debug.LogWarning(spl[5]);
					var colors = GameManager.common.GetComponent<Palettefiles>().colors;
					ma.SetColor("_TransparentColor",colors[int.Parse(spl[5])-1]);
					//ma.SetColor("_TransparentColor",new Color32(255,255,255,255));
				}
				break;
			}
			default:
			{
				break;
			}
		}
		maths.Add(ma);

		//ma.mainTexture = int.Parse(spl[1])
	}
	public int FindIndexByString(string name)
	{
		int a = 65535;
		int i = 0;
		List<string> material1 = gameObject.GetComponent<Materials>().material;
		foreach (string mat in material1)
		{
			//Debug.Log(name.Replace("\0",string.Empty));
			//Debug.LogWarning(mat.Split(' ')[0]);
			if (name.Replace("\0",string.Empty) == mat.Split(' ')[0])
			{
				a = i;
			}
			i++;
		}
		return a; 
	}
}
class Texturefiles : MonoBehaviour {
	public List<string> Texturenames = new List<string>();
	public List<Texture2D> textures = new List<Texture2D>();
	//public Dictionary<string,Texture2D> textures = new Dictionary<string,Texture2D>();
	public void addTex(string name,short width, short height, byte[] rawIm, int format)
	{
		TextureFormat tf;
		bool r8 = false;
		byte[] raw2 = null;
		switch (format)
		{
			case 248:
			{
				tf = TextureFormat.RGB565;
				break;
			}
			case 15:
			{
				//rawIm = Array
				tf = TextureFormat.ARGB4444;
				break;
			}
			case 0:
			{
				tf = TextureFormat.RGB24;
				int pos = 18;
				//byte [] colors = new byte[768];
				ColorsRGB[] colors = new ColorsRGB[256];

				for (int i = 0;i<256;i++)
				{
					colors[i].R = rawIm[pos];
					colors[i].G = rawIm[pos+1];
					colors[i].B = rawIm[pos+2];
					pos+=3;
				}
				raw2 = new byte[width*height*3];
				for (int i = 0;i<width*height;i++)
				{
					raw2[i*3] = colors[rawIm[pos]].B;
					raw2[i*3+1] = colors[rawIm[pos]].G;
					raw2[i*3+2] = colors[rawIm[pos]].R;
					pos++;
				}
				
				r8 = true;
				break;
			}
			default:
			{
				Debug.LogError("Unknown texture type: "+format+",  "+name);
				tf = TextureFormat.RGB565;
				break;
			}
		}
		var tex = new Texture2D(width,height,tf,false);
		
		
		
		if(r8)
		{
			byte[] row = new byte[width*3];
			int pos = 0;
			for (int i = 0; i<height;i++)
			{
				System.Array.Reverse(raw2,pos,width*3);
				pos+=width*3;
			}
			System.Array.Reverse(raw2);
			tex.LoadRawTextureData(raw2);
			
		}
		else
		{
			tex.LoadRawTextureData(rawIm);
		}
		#if UNITY_EDITOR
			tex.alphaIsTransparency = true;
		#endif
		tex.Apply();

		Texturenames.Add(name);
		textures.Add(tex);
	}
}

public struct ColorsRGB
{
	public byte R,G,B;
}
class Palettefiles : MonoBehaviour {
	public List<Color32> colors = new List<Color32>();

	
}

class Soundfiles : MonoBehaviour {
	public List<AudioClip> wavs = new List<AudioClip>();

	public void Add(string name,int dataSize, byte[] data)
	{
		return; //FIXME: Wav почему то не работает, но и пофигу
		WAV wav = new WAV(data);
 		AudioClip audioClip = AudioClip.Create(name,wav.SampleCount,wav.ChannelCount,wav.Frequency,false);
		audioClip.SetData(wav.LeftChannel,0);

		wavs.Add(audioClip);
		//Debug.Log(name+ ": "+wav.ToString());
	}	
	public void Add(string name,int startIndex, int dataSize, byte[] data)
	{
		byte[] newData = new byte[dataSize];
		System.Array.Copy(data,startIndex,newData,0,dataSize);
		WAV wav = new WAV(data);
		AudioClip audioClip = AudioClip.Create(name,wav.SampleCount,wav.ChannelCount,wav.Frequency,false);
		audioClip.SetData(wav.LeftChannel,0);

		wavs.Add(audioClip);
		//Debug.Log(name+ ": "+wav.ToString());
	}

}

class Backfiles : MonoBehaviour {

}

class Maskfiles : MonoBehaviour {
	public List<string> masks = new List<string>();
	public List<Texture2D> textures = new List<Texture2D>();
	public List<Material> maths = new List<Material>();
	public void AddMsk(string name, byte[] rawIm)
	{
		masks.Add(name);
		int pos = 4;

		short width = System.BitConverter.ToInt16(rawIm,pos);
		pos+=2;
		short height = System.BitConverter.ToInt16(rawIm,pos);
		pos+=2;

		BinaryWriter bw = new BinaryWriter(File.OpenWrite(@"d:\file"));
		bw.Write(rawIm);
		bw.Close();

		ColorsRGB[] colors = new ColorsRGB[256];

		for (int i = 0;i<256;i++)
		{
			colors[i].R = rawIm[pos];
			colors[i].G = rawIm[pos+1];
			colors[i].B = rawIm[pos+2];
			pos+=3;
		}

		byte[] raw2 = new byte[width*height];
		int pos2 = 0;

		for (;pos<rawIm.Length;)
		//while(pos2<rawIm.Length-776)
		{
			byte number = rawIm[pos];
			//Debug.Log(pos+" "+i);

			if (number<=127)
			{
				System.Array.Copy(rawIm,pos+1,raw2,pos2,number);
				pos2+=number;
				pos+=number+1;
			}
			else
			{
				pos2+=number-128;
				pos++;
			}


			/*raw2[i*3] = colors[rawIm[pos]].R;
			raw2[i*3+1] = colors[rawIm[pos]].G;
			raw2[i*3+2] = colors[rawIm[pos]].B;*/
		}
		byte[] raw3 = new byte[raw2.Length*3];
		for (int i = 0;i<raw2.Length;i++)
		{
			raw3[i*3] = colors[raw2[i]].R;
			raw3[i*3+1] = colors[raw2[i]].G;
			raw3[i*3+2] = colors[raw2[i]].B;
			pos++;
		}

		/*BinaryWriter bw2 = new BinaryWriter(File.OpenWrite(@"d:\file2"));
		bw2.Write(raw2);
		bw2.Close();*/
		pos = 0;
		for (int i = 0; i<height;i++)
		{
			System.Array.Reverse(raw3,pos,width*3);
			pos+=width*3;
		}
		System.Array.Reverse(raw3);
		var tex = new Texture2D(width,height,TextureFormat.RGB24,false);
		
		tex.LoadRawTextureData(raw3);
		tex.Apply();

		textures.Add(tex);

	}

}

class Sounds : MonoBehaviour {
	public List<string> Sound = new List<string>();
}