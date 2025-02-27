using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace autoClicker
{
    public class ClickerList
    {
        public Dictionary<String, List<ClickParameters>> Items { get; set; } = new Dictionary<String, List<ClickParameters>>();
        public Dictionary<String, ClickParameters> WindowParams { get; set; } = new Dictionary<String, ClickParameters>();
        string FilePathData;
        string FilePathWindowParam;

        public ClickerList(string filePath)
        {
            this.FilePathData = filePath + ".txt";
            this.FilePathWindowParam = filePath + "WindowParam.txt";
            try
            {
                LoadFromFile();
            }
            catch (Exception ex)
            {
            }

        }

        public void SaveToFile()
        {
            Serialize(Items, File.Open(FilePathData, FileMode.Create));
            Serialize(WindowParams, File.Open(FilePathWindowParam, FileMode.Create));
        }

        public void LoadFromFile()
        {
            Items = Deserialize<Dictionary<String, List<ClickParameters>>>(File.Open(FilePathData, FileMode.Open));
            WindowParams = Deserialize<Dictionary<String, ClickParameters>>(File.Open(FilePathWindowParam, FileMode.Open));
        }

        public List<String> AllKeys()
        {
            if (Items.Count == 0)
            {
                return null;
            }
            LoadFromFile();
            return Items.Keys.ToList();
        }
        public static void Serialize<Object>(Object dictionary, Stream stream)
        {
            try
            {
                using (stream)
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, dictionary);
                }
            }
            catch (IOException)
            {
            }
        }

        public static Object Deserialize<Object>(Stream stream) where Object : new()
        {
            Object ret = CreateInstance<Object>();
            try
            {
                using (stream)
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    ret = (Object)bin.Deserialize(stream);
                }
            }
            catch (IOException)
            {
            }
            return ret;
        }
        public static Object CreateInstance<Object>() where Object : new()
        {
            return (Object)Activator.CreateInstance(typeof(Object));
        }

        internal bool PointInScope(Point? point, ClickParameters windowParam)
        {
            if (point == null) return false;
            else if (point.Value.X < 0) return false;
            else if (point.Value.Y < 0) return false;
            else if (point.Value.X > windowParam.Start.Value.X) return false;
            else if (point.Value.Y > windowParam.Start.Value.Y) return false;
            else return true;
        }
    }
}
