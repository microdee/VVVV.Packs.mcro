using System;
using System.IO;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using VVVV.Utils.Streams;
using VVVV.Utils.Linq;

using VVVV.Core.Logging;
using VVVV.ROD;

namespace VVVV.Nodes
{
    //// FILE ////
    /* (size)   (Offset)   (Desc)
     * header:
     *  100b           0b  arbitrary validation string
     *    4b         100b  Object Count (OC)
     * content:
     *               104b  Object
     *  
     * Contents start at 104b
     */

    //// OBJECT ////
    /* (size)   (Offset)          (Desc)
     * obj header:
     *   10b                  0b  arbitrary validation string
     *    4b                 10b  Object Name Length (ONL)
     *    4b                 14b  Object Debug Length (ODL)
     *    4b                 18b  Key Count (KC)
     *  ONLb                 22b  Object Name
     *  ODLb             22+ONLb  Object Debug
     * obj content:
     *               22+ONL+ODLb  Key
     * 
     * Contents start at 22+ONL+ODL
     */

    //// KEY ////
    /* (size)          (Offset) (Desc)
     * key header:
     *          10b        0b   arbitrary validation string
     *           4b       10b   Key Name Length (KNL)
     *           4b       14b   Key Length (KL)
     *         KNLb       18b   Key Name
     * key content:
     *           KL   18+KNLb   Key
     * 
     * Content starts at 18+KNLb length is KL[]-18-KNLb
     * Object/key offsets and lengths are zipped together [o0.offs, o0.len, o1.offs, o1.len, ... , oN.offs, oN.len]
     */
    public static class Helper
    {
        public static UInt32 ReadUInt32(this Stream input)
        {
            byte[] tmp = new byte[4];
            input.Read(tmp, 0, 4);
            return BitConverter.ToUInt32(tmp, 0);
        }
        public static string ReadASCII(this Stream input, int length)
        {
            byte[] tmp = new byte[length];
            input.Read(tmp, 0, length);
            return System.Text.Encoding.ASCII.GetString(tmp);
        }
        public static string ReadUTF8(this Stream input, int length)
        {
            byte[] tmp = new byte[length];
            input.Read(tmp, 0, length);
            return System.Text.Encoding.UTF8.GetString(tmp);
        }
        public static string ReadUnicode(this Stream input, int length)
        {
            byte[] tmp = new byte[length];
            input.Read(tmp, 0, length);
            return System.Text.Encoding.Unicode.GetString(tmp);
        }
        
        public static void WriteUInt32(this Stream input, uint data)
        {
            byte[] tmp = BitConverter.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteASCII(this Stream input, string data)
        {
            byte[] tmp = System.Text.Encoding.ASCII.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteUTF8(this Stream input, string data)
        {
            byte[] tmp = System.Text.Encoding.UTF8.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
        public static void WriteUnicode(this Stream input, string data)
        {
            byte[] tmp = System.Text.Encoding.Unicode.GetBytes(data);
            input.Write(tmp, 0, tmp.Length);
        }
    }
    [PluginInfo(Name = "Serialize", Category = "Raw", Version = "Object")]
    public class ObjectRawSerializeNode : IPluginEvaluate
    {
        [Input("Dictionary")]
        public ISpread<RodWrap> FDict;
        [Input("Serialize")]
        public ISpread<bool> FSerialize;

        [Output("Output")]
        public ISpread<Stream> FOut;

        Stream everything = new MemoryStream();

        // This string validates that the stream is an object dictionary
        string FileValidatorString =
            "gS3RN4QKvbezYRCfAiE5" +
            "6rEeszMlV0xv10eBVs45" +
            "j1iDdl0ijXZPt3T0ajAJ" +
            "CeW9WYmKH44vpc8XZ0u7" +
            "lCtIqkbHlVah0pMBSbFZ";
        byte[] FileValidator = new byte[100];

        // This string validates that positions are ok in the file
        string ObjectValidatorString = "j1iDdl0ijX";
        byte[] ObjectValidator = new byte[10];

        [ImportingConstructor]
        public ObjectRawSerializeNode()
        {
            this.FileValidator = System.Text.Encoding.ASCII.GetBytes(FileValidatorString);
            this.ObjectValidator = System.Text.Encoding.ASCII.GetBytes(ObjectValidatorString);
        }

        public void Evaluate(int spreadMax)
        {
            FOut.SliceCount = 1;
            
            if(FSerialize[0])
            {
                #region Construct File Header
                everything.Flush();
                everything.Position = 0;
                everything.Write(FileValidator, 0, FileValidator.Length);

                everything.WriteUInt32((UInt32)FDict[0].Objects.Count);
                #endregion Construct File Header

                foreach (KeyValuePair<string, RawObject> objkvp in FDict[0].Objects)
                {
                    #region Construct Object Header
                    everything.Write(ObjectValidator, 0, ObjectValidator.Length);

                    byte[] ObjName = System.Text.Encoding.Unicode.GetBytes(objkvp.Key);
                    byte[] ObjDebug = System.Text.Encoding.Unicode.GetBytes(objkvp.Value.Debug);

                    everything.WriteUInt32((UInt32)ObjName.Length);
                    everything.WriteUInt32((UInt32)ObjDebug.Length);
                    everything.WriteUInt32((UInt32)objkvp.Value.Fields.Count);

                    everything.Write(ObjName, 0, ObjName.Length);
                    everything.Write(ObjDebug, 0, ObjDebug.Length);
                    #endregion Construct Object Header

                    foreach (KeyValuePair<string, Stream> keykvp in objkvp.Value.Fields)
                    {
                        #region Construct Key Header
                        everything.Write(ObjectValidator, 0, ObjectValidator.Length);

                        byte[] KeyName = System.Text.Encoding.Unicode.GetBytes(keykvp.Key);

                        everything.WriteUInt32((UInt32)KeyName.Length);
                        everything.WriteUInt32((UInt32)keykvp.Value.Length);

                        everything.Write(KeyName, 0, KeyName.Length);
                        #endregion Construct Key Header

                        keykvp.Value.Position = 0;
                        keykvp.Value.CopyTo(everything);
                    }
                }
                FOut[0] = everything;
            }
        }
    }

    [PluginInfo(Name = "DeSerialize", Category = "Raw", Version = "Object", AutoEvaluate=true)]
    public class ObjectRawDeSerializeNode : IPluginEvaluate
    {
        [Input("Dictionary")]
        public ISpread<RodWrap> FDict;
        [Input("Input")]
        public ISpread<Stream> FInput;
        [Input("DeSerialize")]
        public ISpread<bool> FDeSerialize;

        [Input("Manage Existing Object", DefaultEnumEntry = "Extend")]
        public IDiffSpread<ManageExistingObject> FExistObjMan;
        [Input("Manage Not-Existing Object", DefaultEnumEntry = "Create")]
        public IDiffSpread<ManageNotExistingKey> FNotExistObjMan;
        [Input("Manage Existing Key", DefaultEnumEntry = "Overwrite")]
        public IDiffSpread<ManageExistingKey> FExistKeyMan;
        [Input("Manage Not-Existing Key", DefaultEnumEntry = "Create")]
        public IDiffSpread<ManageNotExistingKey> FNotExistKeyMan;

        [Output("Valid")]
        public ISpread<bool> FValid;

        Stream everything = new MemoryStream();
        RawObject NewObject;

        // This string validates that the stream is an object dictionary
        string FileValidatorString =
            "gS3RN4QKvbezYRCfAiE5" +
            "6rEeszMlV0xv10eBVs45" +
            "j1iDdl0ijXZPt3T0ajAJ" +
            "CeW9WYmKH44vpc8XZ0u7" +
            "lCtIqkbHlVah0pMBSbFZ";

        // This string validates that positions are ok in the file
        string ObjectValidatorString = "j1iDdl0ijX";

        public void Evaluate(int spreadMax)
        {
            if (FDeSerialize[0])
            {
                bool valid = true;
                FInput[0].Position = 0;
                string FVCompare = FInput[0].ReadASCII(100);

                if (FVCompare == FileValidatorString)
                {
                    uint ObjCount = FInput[0].ReadUInt32();
                    for (int i = 0; i < ObjCount; i++)
                    {
                        string OVCompare = FInput[0].ReadASCII(10);
                        if (OVCompare == ObjectValidatorString)
                        {
                            uint ObjNameLength = FInput[0].ReadUInt32();
                            uint ObjDebugLength = FInput[0].ReadUInt32();
                            uint KeyCount = FInput[0].ReadUInt32();
                            string ObjName = FInput[0].ReadUnicode((int)ObjNameLength);
                            string ObjDebug = FInput[0].ReadUnicode((int)ObjDebugLength);
                            //string ObjDebug = "";

                            bool ContainsObject = FDict[0].Objects.ContainsKey(ObjName);

                            if(ContainsObject)
                            {
                                NewObject = FDict[0].Objects[ObjName];
                                if(FExistObjMan[0] != ManageExistingObject.Ignore)
                                    NewObject.Debug = ObjDebug;
                            }
                            else
                            {
                                NewObject = new RawObject(FDict[0]);
                                NewObject.Name = ObjName;
                                NewObject.Debug = ObjDebug;
                            }

                            if ((FExistObjMan[0] != ManageExistingObject.Ignore) || (!ContainsObject))
                            {
                                if (ContainsObject && (FExistObjMan[0] == ManageExistingObject.Overwrite)) NewObject.Fields.Clear();
                                for (int j = 0; j < KeyCount; j++)
                                {
                                    OVCompare = FInput[0].ReadASCII(10);
                                    if (OVCompare == ObjectValidatorString)
                                    {
                                        uint KeyNameLength = FInput[0].ReadUInt32();
                                        uint KeyLength = FInput[0].ReadUInt32();
                                        string KeyName = FInput[0].ReadUnicode((int)KeyNameLength);

                                        Stream KeyContent = new MemoryStream();
                                        KeyContent.Position = 0;
                                        for (int k = 0; k < KeyLength; k++)
                                            KeyContent.WriteByte((byte)FInput[0].ReadByte());
                                            

                                        bool ContainsKey = NewObject.Fields.ContainsKey(KeyName);
                                        if(ContainsKey)
                                        {
                                            if(FExistKeyMan[0] == ManageExistingKey.Overwrite)
                                            {
                                                NewObject.Fields[KeyName].Position = 0;
                                                NewObject.Fields[KeyName].SetLength(KeyContent.Length);
                                                NewObject.Fields[KeyName].Position = 0;
                                                KeyContent.Position = 0;
                                                KeyContent.CopyTo(NewObject.Fields[KeyName]);
                                            }
                                        }
                                        else
                                        {
                                            if(FNotExistKeyMan[0] == ManageNotExistingKey.Create)
                                                NewObject.Fields.Add(KeyName, KeyContent);
                                        }
                                    }
                                    else valid = false;
                                }
                                if(!ContainsObject)
                                {
                                    if(FNotExistObjMan[0] == ManageNotExistingKey.Create)
                                        FDict[0].Objects.Add(ObjName, NewObject);
                                }
                            }
                        }
                        else valid = false;
                    }
                }
                else valid = false;
                FValid[0] = valid;
            }
        }
    }
}