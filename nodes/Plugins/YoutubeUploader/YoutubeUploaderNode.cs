#region usings
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	public enum YoutubePrivacyStatus
	{
		Public,
		Unlisted,
		Private
	}
	#region PluginInfo
	[PluginInfo(Name = "Uploader", Category = "Youtube", Help = "Basic template with one string in/out", Tags = "")]
	#endregion PluginInfo
	public class YoutubeUploaderNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Video File", StringType = StringType.Filename)]
		public ISpread<string> FFile;
		[Input("Title")]
		public ISpread<string> FTitle;
		[Input("Description")]
		public ISpread<string> FDesc;
		[Input("Tags")]
		public ISpread<string> FTags;
		[Input("Category ID")]
		public ISpread<int> FCat;
		[Input("Privacy Status", DefaultEnumEntry = "Unlisted")]
		public ISpread<YoutubePrivacyStatus> FStatus;
		[Input("Secret JSON", StringType = StringType.Filename)]
		public ISpread<string> FSecret;
		[Input("Application Name")]
		public ISpread<string> FAppName;
		
		[Input("Upload", IsBang = true)]
		public ISpread<bool> FUp;

		[Output("Status")]
		public ISpread<string> FUpStatus;
		[Output("Uploaded Kb")]
		public ISpread<double> FSentKb;
		[Output("Video ID")]
		public ISpread<string> FVidID;
		
		public string VFile = "";
		public string VTitle = "";
		public string VDesc = "";
		public List<string> VTags = new List<string>();
		public string VCat = "0";
		public string VStatus = "unlisted";
		public string GSecret = "";
		public string GAppName = "";
		
		public string UStatus = "";
		public double USentKb = 0;
		public string UVID = "";
		
		Thread UThread;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		
		public async void UploadThread()
        {
            UserCredential credential;
            using (var stream = new FileStream(GSecret, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access.
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = GAppName
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = VTitle;
            video.Snippet.Description = VDesc;
            video.Snippet.Tags = VTags.ToArray();
            video.Snippet.CategoryId = VCat; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = VStatus; // or "private" or "public"

            using (var fileStream = new FileStream(VFile, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Starting:
                    UStatus = "Starting";
                    break;

                case UploadStatus.Uploading:
                    UStatus = "Uploading";
                    USentKb = (double)progress.BytesSent/1024;
                    break;

                case UploadStatus.Failed:
                    UStatus = progress.Exception.Message;
                    break;

                case UploadStatus.Completed:
                    UStatus = "Completed";
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            UVID = video.Id;
        	UThread.Abort();
        }
		
		public void Evaluate(int SpreadMax)
		{
			FUpStatus.SliceCount = 1;
			FSentKb.SliceCount = 1;
			FVidID.SliceCount = 1;
			
			if(FUp[0])
			{
				UStatus = "";
				USentKb = 0;
				UVID = "";
				
                try
                {
                	VTitle = FTitle[0];
                	VDesc = FDesc[0];
                	
                	VTags.Clear();
                	for(int i=0; i<FTags.SliceCount; i++)
                		VTags.Add(FTags[i]);
                	
                	VCat = FCat[0].ToString();
                    VStatus = Enum.GetName(typeof(YoutubePrivacyStatus),FStatus[0]).ToLower();
                	
                	GAppName = FAppName[0];
                	GSecret = FSecret[0];
                	
                	UThread = new Thread(new ThreadStart(this.UploadThread));
                	UThread.Start();
                	
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        UStatus += e.Message + "\n";
                    }
                }
			}
			
			FUpStatus[0] = UStatus;
			FSentKb[0] = USentKb;
			FVidID[0] = UVID;

			//FLogger.Log(LogType.Debug, "Logging to Renderer (TTY)");
		}
	}
}
