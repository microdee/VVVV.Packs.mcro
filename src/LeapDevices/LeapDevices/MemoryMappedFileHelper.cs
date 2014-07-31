using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace MemoryMappedFileHelper
{
    public static class Helper
    {
        public static byte[] ConvertFloatToByteArray(float input)
        {
            byte[] ret = new byte[4];// a single float is 4 bytes/32 bits
            ret = BitConverter.GetBytes(input);
            return ret;
        }
        public static float ConvertByteArrayToFloat(byte[] bytes)
        {
            if(bytes.Length != 4) return 0;

            float output = BitConverter.ToSingle(bytes, 0);

            return output;
        }
        public static float ReadFloat(this MemoryMappedFile mmf)
        {
            using (var stream = mmf.CreateViewStream())
            {
                byte[] ret = new byte[4];
                for (int i = 0; i < 4; i++)
                    ret[i] = (byte)stream.ReadByte();
                float fret = Helper.ConvertByteArrayToFloat(ret);
                return fret;
            }
        }

        public static void WriteFloat(this MemoryMappedFile mmf, float input)
        {
            using (var stream = mmf.CreateViewStream())
            {
                byte[] ret = Helper.ConvertFloatToByteArray(input);

                for (int i = 0; i < 4; i++)
                    stream.WriteByte(ret[i]);
            }
        }
    }
}
