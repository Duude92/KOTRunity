using UnityEditor;
using System.Collections.Generic;
using WWUtils.Audio;
using UnityEngine;
class Soundfiles : MonoBehaviour
{
    public List<AudioClip> wavs = new List<AudioClip>();

    public void Add(string name, int dataSize, byte[] data)
    {
        //return; //FIXME: Wav почему то не работает, но и пофигу
        WAV wav = new WAV(data);
        AudioClip audioClip = AudioClip.Create(name, wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
        audioClip.SetData(wav.LeftChannel, 0);

        wavs.Add(audioClip);
        //Debug.Log(name+ ": "+wav.ToString());
    }
    public void Add(string name, int startIndex, int dataSize, byte[] data)
    {
        byte[] newData = new byte[dataSize];
        System.Array.Copy(data, startIndex, newData, 0, dataSize);
        WAV wav = new WAV(data);
        AudioClip audioClip = AudioClip.Create(name, wav.SampleCount, wav.ChannelCount, wav.Frequency, false);
        audioClip.SetData(wav.LeftChannel, 0);

        wavs.Add(audioClip);
        //Debug.Log(name+ ": "+wav.ToString());
    }

}
