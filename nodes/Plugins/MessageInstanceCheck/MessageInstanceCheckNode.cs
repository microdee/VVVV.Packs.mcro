#region usings
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Diagnostics;
using VVVV.Packs.Message;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
    public class MessageInstance
    {
        public Stopwatch Age;
        public Stopwatch FilterAge;
        public string name;
        public Message message;

        public MessageInstance()
        {
            this.Age = new Stopwatch();
            this.Age.Start();
            this.FilterAge = new Stopwatch();
            this.FilterAge.Start();
            this.name = "";
        }
    }
    #region PluginInfo
    [PluginInfo(Name = "InstanceCheck", Category = "Message", Tags = "microdee")]
    #endregion PluginInfo
    public class JoinMessageInstanceCheckNode : IPluginEvaluate
    {
        #region fields & pins
        [Input("Name In")]
        public ISpread<string> FnameIn;
        [Input("Message In")]
        public ISpread<Message> FMessageIn;
        [Input("Max Age", DefaultValue = 2.0)]
        public ISpread<double> FMaxAge;
        [Input("Filter Below Age", DefaultValue = 0)]
        public ISpread<double> FFilterAge;

        [Output("Message")]
        public ISpread<Message> FMessageOut;
        [Output("Name Out")]
        public ISpread<string> FnameOut;
        [Output("Age")]
        public ISpread<double> FAge;
        [Output("FilterAge")]
        public ISpread<double> FFAge;

        Spread<MessageInstance> InstList = new Spread<MessageInstance>();

        [Import()]
        public ILogger FLogger;
        #endregion fields & pins

        public int fcr;

        [ImportingConstructor]
        public JoinMessageInstanceCheckNode()
        {
            fcr = 0;
        }

        //called when data for any output pin is requested
        public void Evaluate(int SpreadMax)
        {
            try
            {
                if (fcr == 0) InstList.SliceCount = 0;
                for (int i = 0; i < FnameIn.SliceCount; i++)
                {
                    bool found = false;
                    if (InstList.SliceCount != 0)
                    {
                        for (int j = 0; j < InstList.SliceCount; j++)
                        {
                            if (InstList[j].name == FnameIn[i])
                            {
                                found = true;
                                InstList[j].Age.Restart();
                                InstList[j].name = FnameIn[i];
                                InstList[j].message = FMessageIn[i];
                            }
                        }
                    }
                    if (!found)
                    {
                        MessageInstance temp = new MessageInstance();
                        temp.name = FnameIn[i];
                        temp.message = FMessageIn[i];
                        InstList.Add(temp);
                    }
                }
                if (InstList.SliceCount != 0)
                {
                    for (int i = 0; i < InstList.SliceCount; i++)
                    {
                        if (((double)InstList[i].Age.ElapsedMilliseconds / 1000) > FMaxAge[0])
                        {
                            InstList.RemoveAt(i);
                        }
                    }
                }
                fcr++;
            }
            catch { }
            FnameOut.SliceCount = InstList.SliceCount;
            FMessageOut.SliceCount = InstList.SliceCount;
            FAge.SliceCount = InstList.SliceCount;
            FFAge.SliceCount = InstList.SliceCount;
            for (int i = 0; i < InstList.SliceCount; i++)
            {
                double fage = InstList[i].FilterAge.Elapsed.TotalSeconds;
                double age = InstList[i].Age.Elapsed.TotalSeconds;
            	age = (age < 0.001) ? 0 : age;
                FnameOut[i] = InstList[i].name;
                FMessageOut[i] = InstList[i].message;
            	
                if(!FMessageOut[i].Attributes.Contains("InstanceAge"))
            		FMessageOut[i].Add("InstanceAge", fage);
            	FMessageOut[i]["InstanceAge"][0] = fage;
                if(!FMessageOut[i].Attributes.Contains("InstanceCountDown"))
                	FMessageOut[i].Add("InstanceCountDown", age);
            	FMessageOut[i]["InstanceCountDown"][0] = age;
                if(!FMessageOut[i].Attributes.Contains("InstanceName"))
                	FMessageOut[i].Add("InstanceName", FnameOut[i]);
            	FMessageOut[i]["InstanceName"][0] = FnameOut[i];
            	
                FFAge[i] = fage;
                FAge[i] = age;
            }
        }
    }
}
