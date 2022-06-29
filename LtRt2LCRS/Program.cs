using AudioWorks.Api;
using System;

namespace LtRt2LCRS
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string path = args.Where(arg => File.Exists(arg)).FirstOrDefault() ?? throw new FileNotFoundException();
			AudioFile audioFile = new AudioFile(path);
		}
	}
}
