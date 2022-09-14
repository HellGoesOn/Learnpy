using Learnpy.Core;
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
