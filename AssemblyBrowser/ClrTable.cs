using System.Collections;

namespace AssemblyBrowser
{
	internal class ClrTable : IFolder, IProperties
	{
		public uint Address { get; set; }
		public uint Size { get; set; }
		public uint Position { get; set; }
		public string Path { get; set; }

		public IEnumerable Items
		{
			get
			{
				using (var stream = System.IO.File.OpenRead(Path))
				using (var reader = new System.IO.BinaryReader(stream))
				{
					stream.Position = Position;

					var headerSize = reader.ReadUInt32();
					var runtimeMajorVersion = reader.ReadUInt16();
					var runtimeMinorVersion = reader.ReadUInt16();
					var metadataAddress = reader.ReadUInt32();
					var metadataSize = reader.ReadUInt32();

					var flags = reader.ReadUInt32();
					var entryPointTable = reader.ReadUInt32();
					var entryPointIndex = reader.ReadUInt32();

					if (metadataSize != 0)
						yield return new ClrMetadata { Address = metadataAddress, Size = metadataSize, Position = metadataAddress - (Address - Position), Path = Path };

					var resourceAddress = reader.ReadUInt32();
					var resourceSize = reader.ReadUInt32();
					var strongNameAddress = reader.ReadUInt32();
					var strongNameSize = reader.ReadUInt32();
					var codeManagerAddress = reader.ReadUInt32();
					var codeManagerSize = reader.ReadUInt32();
					var vTableFixupAddress = reader.ReadUInt32();
					var vTableFixupSize = reader.ReadUInt32();
					var exportAddress = reader.ReadUInt32();
					var exportSize = reader.ReadUInt32();
					var managedNativeAddress = reader.ReadUInt32();
					var managedNativeSize = reader.ReadUInt32();

					yield break;
				}
			}
		}

		public object Properties => new { Address, Size };

		public override string ToString() => "CLR Table";
	}
}