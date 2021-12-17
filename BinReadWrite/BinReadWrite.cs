using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinReadWrite
{
    public class BinReadWrite
    {
        private String filepath;
        private Object fileData;

        public BinReadWrite()
        {
            filepath = "";
        }
        public BinReadWrite(String inFP, Object inFD)
        {
            filepath = inFP;
            fileData = inFD;
        }
        public String Filepath
        {
            get => filepath;
            // Directory must exist, file doesn't need to.
            set => filepath = (Directory.Exists(Path.GetDirectoryName(value))) ? value : filepath;
        }
        public Object FileData
        {
            get => fileData;
            set => fileData = (value != null) ? value : fileData;
        }
        public bool readFile()
        {
            fileData = staticReadFile(filepath);
            return (FileData != null);
        }
        public bool writeFile()
        {
            return staticWriteFile(filepath, fileData);
        }
        public static Object staticReadFile(String inFP)
        {
            Object loadData = null;

            try
            {
                Stream stream = File.Open(inFP, FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                loadData = bf.Deserialize(stream);

                stream.Close();
            }
            catch (IOException ex) { }

            return loadData;
        }
        public static bool staticWriteFile(String inFP, Object inFD)
        {
            bool isSuccess = false;

            try
            {
                Stream stream = File.Open(inFP, FileMode.OpenOrCreate);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, inFD);

                stream.Close();
                isSuccess = true;
            }
            catch (IOException ex) { }

            return isSuccess;
        }
    }
}
