using System;

namespace SimpleTaskListDemo
{
    [Serializable()]
    public class ProtectedFileObject
    {
        private Object fileData;
        private byte[] hash;

        public ProtectedFileObject(Object inFD, byte[] inHash)
        {
            fileData = inFD;
            hash = inHash;
        }
        public Object FileData
        {
            get => fileData;
        }
        public byte[] Hash
        {
            get => hash;
        }
    }
}