using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtRt2LCRS
{
	/// <summary>
	/// https://www.codeproject.com/Articles/853355/Reading-Multi-channel-WAV-Files-with-NET-Csharp
	/// </summary>
	public static class MultiChannelWavReader
	{
		[Flags]
		public enum ChannelEnum : uint
		{
			FrontLeft = 0x1,
			FrontRight = 0x2,
			FrontCenter = 0x4,
			Lfe = 0x8,
			BackLeft = 0x10,
			BackRight = 0x20,
			FrontLeftOfCenter = 0x40,
			FrontRightOfCenter = 0x80,
			BackCenter = 0x100,
			SideLeft = 0x200,
			SideRight = 0x400,
			TopCenter = 0x800,
			TopFrontLeft = 0x1000,
			TopFrontCenter = 0x2000,
			TopFrontRight = 0x4000,
			TopBackLeft = 0x8000,
			TopBackCenter = 0x10000,
			TopBackRight = 0x20000
		}
		public static uint ReadSpeakerMask(byte[] bytes) =>
			bytes is not null && bytes.Length > 43 ?
				BitConverter.ToUInt32(new[] { bytes[40], bytes[41], bytes[42], bytes[43] }, 0)
				: throw new InvalidDataException("Invalid wFormatTag field!");
		public static uint MakeSpeakerMask(int channelCount) => (uint)Enum.GetValues<ChannelEnum>().Take(channelCount).Sum(e => (uint)e);
		//byte[] bytes = new byte[50];
		//using (FileStream stream = new FileStream(path, FileMode.Open))
		//{
		//	stream.Read(bytes, 0, 50);
		//}
		public static byte[] GetChannelBytes(byte[] fileAudioBytes, uint speakerMask, ChannelEnum channelToRead, int bitDepth, uint sampleStartIndex, uint sampleEndIndex)
		{
			ChannelEnum[] channels = FindExistingChannels(speakerMask).ToArray();
			int ch = Array.IndexOf(channels, channelToRead),
				byteDepth = bitDepth / 8,
				chOffset = ch * byteDepth,
				frameBytes = byteDepth * channels.Length,
				i = 0;
			long startByteIncIndex = sampleStartIndex * byteDepth * channels.Length,
				endByteIncIndex = sampleEndIndex * byteDepth * channels.Length,
				outputBytesCount = endByteIncIndex - startByteIncIndex;
			byte[] outputBytes = new byte[outputBytesCount / channels.Length];
			startByteIncIndex += chOffset;
			for (long j = startByteIncIndex; j < endByteIncIndex; j += frameBytes)
				for (long k = j; k < j + byteDepth; k++)
					outputBytes[i++] = fileAudioBytes[(k - startByteIncIndex) + chOffset];
			return outputBytes;
		}
		public static IEnumerable<ChannelEnum> FindExistingChannels(uint speakerMask) => Enum.GetValues<ChannelEnum>().Where(ch => (speakerMask & (uint)ch) == (uint)ch);
	}
}
