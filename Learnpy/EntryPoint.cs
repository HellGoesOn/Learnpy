using Learnpy.Content.Scenes;
using System;

namespace Learnpy
{
    static class EntryPoint
	{
		internal static LearnGame Instance;
		[STAThread]
		static void Main(string[] args)
		{
			using (Instance = new LearnGame())
			{
				Instance.Run();
			}
		}
	}
}
