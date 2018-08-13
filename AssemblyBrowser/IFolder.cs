using System.Collections;

namespace AssemblyBrowser
{
	internal interface IFolder
	{
		IEnumerable Items { get; }
	}
}