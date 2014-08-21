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
    public enum ManageExistingObject
    {
        Ignore,
        Overwrite,
        Extend
    }
    public enum ManageExistingKey
    {
        Ignore,
        Overwrite
    }
    public enum ManageNotExistingKey
    {
        Ignore,
        Create
    }

    [PluginInfo(Name = "SetObject", Category = "Raw", Version = "Object", AutoEvaluate = true)]
    public class RawSetObjectNode : IPluginEvaluate
    {
        [Input("Dictionary")]
        public Pin<RodWrap> FDictionary;

        [Input("Streams")]
        public ISpread<ISpread<Stream>> FStreamIn;
        [Input("Key")]
        public ISpread<ISpread<string>> FKey;
        [Input("Name")]
        public ISpread<string> FName;
        [Input("Debug")]
        public ISpread<string> FDebug;
        [Input("Set", IsBang = true)]
        public ISpread<bool> FSet;
        [Input("Reset Age")]
        public ISpread<bool> FAgeReset;

        [Input("Manage Existing Object", DefaultEnumEntry = "Extend")]
        public IDiffSpread<ManageExistingObject> FExistObjMan;
        [Input("Manage Not-Existing Object", DefaultEnumEntry = "Create")]
        public IDiffSpread<ManageNotExistingKey> FNotExistObjMan;
        [Input("Manage Existing Key", DefaultEnumEntry = "Overwrite")]
        public IDiffSpread<ManageExistingKey> FExistKeyMan;
        [Input("Manage Not-Existing Key", DefaultEnumEntry = "Create")]
        public IDiffSpread<ManageNotExistingKey> FNotExistKeyMan;

        private void Create(int i, int currSpreadMax)
        {
            RawObject temp = new RawObject(FDictionary[0]);
            temp.Name = FName[i];
            temp.Debug = FDebug[i];

            for (int j = 0; j < currSpreadMax; j++)
            {
                FStreamIn[i][j].Position = 0;
                bool contains = temp.Fields.ContainsKey(FKey[i][j]);
                if (contains)
                {
                    if (FExistKeyMan[i] == ManageExistingKey.Overwrite)
                    {
                        temp.Fields[FKey[i][j]].Flush();
                        temp.Fields[FKey[i][j]].SetLength(FStreamIn[i][j].Length);
                        temp.Fields[FKey[i][j]].Position = 0;
                        FStreamIn[i][j].CopyTo(temp.Fields[FKey[i][j]]);
                    }
                }
                else
                {
                    Stream newstream = new MemoryStream();
                    newstream.SetLength(FStreamIn[i][j].Length);
                    newstream.Position = 0;
                    FStreamIn[i][j].CopyTo(newstream);
                    newstream.Position = 0;
                    temp.Fields.Add(FKey[i][j], newstream);
                }
            }
            FDictionary[0].Objects.Add(FName[i], temp);
        }

        private void Extend(int i, int currSpreadMax)
        {
            RawObject temp = FDictionary[0].Objects[FName[i]];
            if (FAgeReset[i]) temp.Age.Restart();
            if (FDebug[i].Length > 0) temp.Debug = FDebug[i];

            for (int j = 0; j < currSpreadMax; j++)
            {
                FStreamIn[i][j].Position = 0;
                bool contains = temp.Fields.ContainsKey(FKey[i][j]);
                if (contains)
                {
                    if (FExistKeyMan[i] == ManageExistingKey.Overwrite)
                    {
                        temp.Fields[FKey[i][j]].Flush();
                        temp.Fields[FKey[i][j]].SetLength(FStreamIn[i][j].Length);
                        temp.Fields[FKey[i][j]].Position = 0;
                        FStreamIn[i][j].CopyTo(temp.Fields[FKey[i][j]]);
                    }
                }
                else
                {
                    if (FNotExistKeyMan[i] == ManageNotExistingKey.Create)
                    {
                        Stream newstream = new MemoryStream();
                        newstream.SetLength(FStreamIn[i][j].Length);
                        newstream.Position = 0;
                        FStreamIn[i][j].CopyTo(newstream);
                        newstream.Position = 0;
                        temp.Fields.Add(FKey[i][j], newstream);
                    }
                }
            }
        }

        private void Overwrite(int i, int currSpreadMax)
        {
            RawObject temp = FDictionary[0].Objects[FName[i]];
            foreach (KeyValuePair<string, Stream> kvp in temp.Fields) kvp.Value.Dispose();
            temp.Fields.Clear();

            if (FAgeReset[i]) temp.Age.Restart();
            if (FDebug[i].Length > 0) temp.Debug = FDebug[i];

            for (int j = 0; j < currSpreadMax; j++)
            {
                FStreamIn[i][j].Position = 0;
                bool contains = temp.Fields.ContainsKey(FKey[i][j]);
                if (contains)
                {
                    if (FExistKeyMan[i] == ManageExistingKey.Overwrite)
                    {
                        temp.Fields[FKey[i][j]].Flush();
                        temp.Fields[FKey[i][j]].SetLength(FStreamIn[i][j].Length);
                        temp.Fields[FKey[i][j]].Position = 0;
                        FStreamIn[i][j].CopyTo(temp.Fields[FKey[i][j]]);
                    }
                }
                else
                {
                    Stream newstream = new MemoryStream();
                    newstream.SetLength(FStreamIn[i][j].Length);
                    newstream.Position = 0;
                    FStreamIn[i][j].CopyTo(newstream);
                    newstream.Position = 0;
                    temp.Fields.Add(FKey[i][j], newstream);
                }
            }
        }
        public void Evaluate(int spreadMax)
        {
            if (FDictionary.IsConnected)
            {
                for (int i = 0; i < FName.SliceCount; i++)
                {
                    if (FSet[i])
                    {
                        int currSpreadMax = Math.Max(FStreamIn[i].SliceCount, FKey[i].SliceCount);
                        switch (FExistObjMan[i])
                        {
                            case ManageExistingObject.Ignore:
                                if (!FDictionary[0].Objects.ContainsKey(FName[i])) Create(i, currSpreadMax);
                                break;

                            case ManageExistingObject.Extend:
                                if (FDictionary[0].Objects.ContainsKey(FName[i])) Extend(i, currSpreadMax);
                                else if (FNotExistObjMan[i] == ManageNotExistingKey.Create) Create(i, currSpreadMax);
                                break;

                            case ManageExistingObject.Overwrite:
                                if (FDictionary[0].Objects.ContainsKey(FName[i])) Overwrite(i, currSpreadMax);
                                else if (FNotExistObjMan[i] == ManageNotExistingKey.Create) Create(i, currSpreadMax);
                                break;
                        }
                    }
                }
            }
        }
    }

    [PluginInfo(Name = "UpdateObject", Category = "Raw", Version = "Object", AutoEvaluate = true)]
    public class RawUpdateObjectNode : IPluginEvaluate
    {
        [Input("Object")]
        public Pin<RawObject> FObject;

        [Input("Streams")]
        public ISpread<ISpread<Stream>> FStreamIn;
        [Input("Key")]
        public ISpread<ISpread<string>> FKey;
        [Input("Write From")]
        public ISpread<ISpread<int>> FFrom;
        [Input("Update", IsBang = true)]
        public ISpread<bool> FSet;
        [Input("Reset Age")]
        public ISpread<bool> FAgeReset;

        [Input("Shrink Size")]
        public ISpread<bool> FShrink;
        [Input("Manage Not-Existing", DefaultEnumEntry = "Ignore")]
        public IDiffSpread<ManageNotExistingKey> FNotExistMan;

        Stream FBuffer = new MemoryStream();
        byte[] FBBuffer = new byte[1];

        public void Evaluate(int spreadMax)
        {
            int sprmx =
                Math.Max(FStreamIn.SliceCount,
                Math.Max(FFrom.SliceCount, FKey.SliceCount)
            );
            if (FObject.IsConnected)
            {
                for (int i = 0; i < sprmx; i++)
                {
                    if (FSet[i])
                    {
                        RawObject temp = FObject[i];
                        if (FAgeReset[i]) temp.Age.Restart();
                        int csprmx =
                            Math.Max(FStreamIn[i].SliceCount,
                            Math.Max(FFrom[i].SliceCount, FKey[i].SliceCount)
                        );
                        for (int j = 0; j < csprmx; j++)
                        {
                            FStreamIn[i][j].Position = 0;
                            bool contains = temp.Fields.ContainsKey(FKey[i][j]);
                            if (contains)
                            {
                                FBuffer.SetLength(FStreamIn[i][j].Length);
                                FBuffer.Position = 0;
                                FStreamIn[i][j].CopyTo(FBuffer);
                                FBuffer.Position = 0;

                                temp.Fields[FKey[i][j]].Position = FFrom[i][j];
                                for (int k = 0; k < FStreamIn[i][j].Length; k++)
                                {
                                    FBuffer.Read(FBBuffer, 0, 1);
                                    temp.Fields[FKey[i][j]].WriteByte(FBBuffer[0]);
                                }

                                if (FShrink[i] && ((FStreamIn[i][j].Length + FFrom[i][j]) < temp.Fields[FKey[i][j]].Length))
                                {
                                    temp.Fields[FKey[i][j]].SetLength(FStreamIn[i][j].Length + FFrom[i][j]);
                                }
                            }
                            else
                            {
                                if (FNotExistMan[i] == ManageNotExistingKey.Create)
                                {
                                    Stream newstream = new MemoryStream();
                                    newstream.SetLength(FStreamIn[i][j].Length);
                                    newstream.Position = 0;
                                    FStreamIn[i][j].CopyTo(newstream);
                                    newstream.Position = 0;
                                    temp.Fields.Add(FKey[i][j], newstream);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [PluginInfo(Name = "GetKey", Category = "Raw", Version = "Object")]
    public class RawGetKeyNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Object")]
        public Pin<RawObject> FObject;
        [Input("Key")]
        public ISpread<ISpread<string>> FKey;

        [Output("Streams")]
        public ISpread<ISpread<Stream>> FStream;
        #endregion fields & pins

        Stream FBuffer = new MemoryStream();

        public void Evaluate(int spreadMax)
        {
            if (FObject.IsConnected)
            {
                FStream.SliceCount = FObject.SliceCount;
                for (int i = 0; i < FObject.SliceCount; i++)
                {
                    RawObject temp = FObject[i];
                    FStream[i].SliceCount = 0;
                    for (int j = 0; j < FKey[i].SliceCount; j++)
                    {
                        bool contains = temp.Fields.ContainsKey(FKey[i][j]);
                        if (contains)
                        {
                            FStream[i].Add(temp.Fields[FKey[i][j]]);
                        }
                    }
                }
            }
            else FStream.SliceCount = 0;
        }
    }

    [PluginInfo(Name = "Info", Category = "Raw", Version = "Object")]
    public class ObjectRawInfoNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Object")]
        public Pin<RawObject> FObject;

        [Output("Streams")]
        public ISpread<ISpread<Stream>> FStream;
        [Output("Keys")]
        public ISpread<ISpread<string>> FKeys;
        [Output("Name")]
        public ISpread<string> FName;
        [Output("Debug")]
        public ISpread<string> FDebug;
        [Output("To be removed")]
        public ISpread<bool> FRemove;
        [Output("Age")]
        public ISpread<double> FAge;
        #endregion fields & pins

        Stream FBuffer = new MemoryStream();

        public void Evaluate(int spreadMax)
        {
            if (FObject.IsConnected)
            {
                FStream.SliceCount = FObject.SliceCount;
                FKeys.SliceCount = FObject.SliceCount;
                FName.SliceCount = FObject.SliceCount;
                FDebug.SliceCount = FObject.SliceCount;
                FRemove.SliceCount = FObject.SliceCount;
                FAge.SliceCount = FObject.SliceCount;

                for (int i = 0; i < FObject.SliceCount; i++)
                {
                    RawObject temp = FObject[i];
                    FStream[i].SliceCount = 0;
                    FKeys[i].SliceCount = 0;

                    FName[i] = temp.Name;
                    FDebug[i] = temp.Debug;
                    FAge[i] = temp.Age.Elapsed.TotalSeconds;
                    FRemove[i] = temp.Remove;

                    foreach (KeyValuePair<string, Stream> kvp in temp.Fields)
                    {
                        FStream[i].Add(kvp.Value);
                        FKeys[i].Add(kvp.Key);
                    }
                }
            }
            else
            {
                FStream.SliceCount = 0;
                FKeys.SliceCount = 0;
                FName.SliceCount = 0;
                FDebug.SliceCount = 0;
                FRemove.SliceCount = 0;
                FAge.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "Remove", Category = "Raw", Version = "Object", AutoEvaluate=true)]
    public class ObjectRawRemoveNode : IPluginEvaluate
    {
        [Input("Object")]
        public ISpread<RawObject> FObject;

        [Input("Remove", IsBang = true)]
        public ISpread<bool> FRemove;
        [Input("Auto Remove")]
        public ISpread<bool> FAuto;
        [Input("Remove Age", DefaultValue = 2.0)]
        public ISpread<double> FAge;

        public void Evaluate(int spreadMax)
        {
            for (int i = 0; i < FObject.SliceCount;i++ )
            {
                if (FRemove[i]) FObject[i].Remove = true;
                if (FAuto[i] && (FObject[i].Age.Elapsed.TotalSeconds > FAge[i])) FObject[i].Remove = true;
            }
        }
    }

    [PluginInfo(Name = "RemoveKey", Category = "Raw", Version = "Object", AutoEvaluate = true)]
    public class ObjectRawRemoveKeyNode : IPluginEvaluate
    {
        [Input("Object")]
        public ISpread<RawObject> FObject;

        [Input("Key")]
        public ISpread<ISpread<string>> FKey;
        [Input("Remove", IsBang = true)]
        public ISpread<ISpread<bool>> FRemove;

        public void Evaluate(int spreadMax)
        {
            for (int i = 0; i < FObject.SliceCount; i++)
            {
                for(int j=0; j<FKey[i].SliceCount; j++)
                {
                    if (FRemove[i][j] && FObject[i].Fields.ContainsKey(FKey[i][j]))
                        FObject[i].DisposeKey(FKey[i][j]);
                }
            }
        }
    }

    [PluginInfo(Name = "CopyObject", Category = "Raw", Version = "Object", AutoEvaluate = true)]
    public class ObjectRawCopyNode : IPluginEvaluate
    {
        [Input("Object")]
        public ISpread<RawObject> FObject;

        [Input("Destination Dictionary")]
        public ISpread<RodWrap> FDestDict;
        [Input("Copy", IsBang = true)]
        public ISpread<bool> FCopy;

        public void Evaluate(int spreadMax)
        {
            for (int i = 0; i < FObject.SliceCount; i++)
            {
                if (FCopy[i])
                {
                    FDestDict[0].Objects.Add(FObject[i].Name, FObject[i].DeepCopy(FDestDict[0]));
                }
            }
        }
    }
}
