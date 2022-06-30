using System;
using WavDotNet.Core;

namespace LtRt2LCRS
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string path = args.Where(arg => arg.EndsWith(".wav", StringComparison.InvariantCultureIgnoreCase) && File.Exists(arg)).FirstOrDefault() ?? throw new FileNotFoundException();
			WavRead<float> wavRead = new(path);
			using (WavWrite<float> wavWrite = new WavWrite<float>("output.wav", wavRead.SampleRate))
			{
				foreach (ChannelPositions channelPositions in wavRead.AudioData.Keys)
					wavWrite.AudioData.Add(channelPositions, wavRead.AudioData[channelPositions]);
				wavWrite.Flush();
			}
		}
	}
}
