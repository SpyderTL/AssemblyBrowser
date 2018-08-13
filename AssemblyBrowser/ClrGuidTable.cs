namespace AssemblyBrowser
{
	internal class ClrGuidTable
	{
		public uint Offset { get; set; }
		public uint Size { get; set; }
		public uint Position { get; set; }
		public string Path { get; set; }

		public object Properties => new { Offset, Size };

		public override string ToString() => "GUID Table";
	}
}