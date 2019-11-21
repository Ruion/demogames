using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Sirenix.OdinInspector;

    /// Generic Data class
    public static class Data{
       
        /// <summary>Save Generic Data.
        /// <para>Save file as Object in streaming assets Path. <see cref="UnityEngine.Application.streamingAssetsPath"/> for more information.</para>
        /// </summary>
        public static bool SaveData(System.Object data,string fileName){ return Save(data,Application.streamingAssetsPath+"/"+fileName); }
        /// <summary>Save Generic Data.
        /// <para>Save file as Object in custom Path.</para>
        /// </summary>
        public static bool Save(System.Object data, string pathFileName){
           
            FileStream file;
           
            try{ file = File.Create(pathFileName); }
            catch { return false; }
           
            BinaryFormatter bf = new BinaryFormatter();
           
            try{ bf.Serialize(file,data); Debug.Log("Save Data success"); }
            catch {
               
                file.Close();
                File.Delete(pathFileName);
                return false;
               
            }
           
            file.Close();
            return true;
           
        }
       
        /// <summary>Load Generic Data.
        /// <para>Load file as Object from streaming assets Path. <see cref="Application.streamingAssetsPath+fileName"/> for more information.</para>
        /// </summary>
        public static System.Object LoadData(string fileName){ return Load(Application.streamingAssetsPath+"/"+fileName); }
        /// <summary>Load Generic Data.
        /// <para>Load file as Object from custom Path.</para>
        /// </summary>
        public static System.Object Load(string pathFileName){
           
            if(!File.Exists(pathFileName)) return null;
           
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(pathFileName,FileMode.Open);
           
            System.Object data;
           
            try{ data = bf.Deserialize(file); }
            catch {
               
                file.Close();
                return null;
               
            }
           
            file.Close();
            return data;
           
        }
       
    }

[System.Serializable]
public struct DataSettings{
    public string fileName;
    public string extension;

    [HideInInspector]
    public string fileFullName;
}