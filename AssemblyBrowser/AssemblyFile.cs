using System.Collections;

namespace AssemblyBrowser
{
	internal class AssemblyFile : IFolder, IProperties
	{
		public string Path { get; set; }

		public IEnumerable Items
		{
			get
			{
				using (var stream = System.IO.File.OpenRead(Path))
				using (var reader = new System.IO.BinaryReader(stream))
				{
					var magic = reader.ReadChars(2);
					var lastPageSize = reader.ReadUInt16();
					var pageCount = reader.ReadUInt16();
					var relocationCount = reader.ReadUInt16();
					var headerSize = reader.ReadUInt16();
					var minimumParagraphCount = reader.ReadUInt16();
					var maximumParagraphCount = reader.ReadUInt16();
					var stackSegment = reader.ReadUInt16();
					var stackPointer = reader.ReadUInt16();
					var checksum = reader.ReadUInt16();
					var instructionPointer = reader.ReadUInt16();
					var codeSegment = reader.ReadUInt16();
					var relocationAddress = reader.ReadUInt16();
					var overlayNumber = reader.ReadUInt16();
					var reserved = reader.ReadBytes(8);
					var oemID = reader.ReadUInt16();
					var oemInformation = reader.ReadUInt16();
					var reserved2 = reader.ReadBytes(20);
					var nextHeaderAddress = reader.ReadUInt32();

					yield return "MS-DOS EXE Header";

					var stub = reader.ReadBytes((int)nextHeaderAddress - (int)stream.Position);

					yield return "MS-DOS Stub Program";

					var signature = reader.ReadChars(4);
					var machine = reader.ReadUInt16();
					var sectionCount = reader.ReadUInt16();
					var dateTimeStamp = reader.ReadUInt32();
					var symbolTablePointer = reader.ReadUInt32();
					var symbolTableNumber = reader.ReadUInt32();
					var optionalHeaderSize = reader.ReadUInt16();
					var characteristics = reader.ReadUInt16();

					yield return "PE Header";

					var magic2 = reader.ReadUInt16();

					var linkerMajorVersion = reader.ReadByte();
					var linkerMinorVersion = reader.ReadByte();
					var codeSize = reader.ReadUInt32();
					var initializedDataSize = reader.ReadUInt32();
					var uninitializedDataSize = reader.ReadUInt32();
					var entryPointAddress = reader.ReadUInt32();
					var codeBase = reader.ReadUInt32();

					if (magic2 != 0x010B)
					{
						yield return "PE32+ Optional Header";
						yield break;
					}

					var dataBase = reader.ReadUInt32();

					yield return "PE32 Optional Header";

					var imageBase = reader.ReadUInt32();
					var sectionAlignment = reader.ReadUInt32();
					var fileAlignment = reader.ReadUInt32();
					var operatingSystemMajorVersion = reader.ReadUInt16();
					var operatingSystemMinorVersion = reader.ReadUInt16();
					var imageMajorVersion = reader.ReadUInt16();
					var imageMinorVersion = reader.ReadUInt16();
					var subsystemMajorVersion = reader.ReadUInt16();
					var subsystemMinorVersion = reader.ReadUInt16();
					var windowsVersion = reader.ReadUInt32();
					var imageSize = reader.ReadUInt32();
					var headerSize2 = reader.ReadUInt32();
					var checksum2 = reader.ReadUInt32();
					var subsystem = reader.ReadUInt16();
					var dllCharacteristics = reader.ReadUInt16();
					var stackReserveSize = reader.ReadUInt32();
					var stackCommitSize = reader.ReadUInt32();
					var heapReserveSize = reader.ReadUInt32();
					var heapCommitSize = reader.ReadUInt32();
					var loaderFlags = reader.ReadUInt32();
					var rvaCountSize = reader.ReadUInt32();

					yield return "Windows Header";

					var exportTable = reader.ReadUInt32();
					var exportTableSize = reader.ReadUInt32();

					if (exportTableSize != 0)
						yield return "Export Table";

					var importTable = reader.ReadUInt32();
					var importTableSize = reader.ReadUInt32();

					if (importTableSize != 0)
						yield return "Import Table";

					var resourceTable = reader.ReadUInt32();
					var resourceTableSize = reader.ReadUInt32();

					if (resourceTableSize != 0)
						yield return "Resource Table";

					var exceptionTable = reader.ReadUInt32();
					var exceptionTableSize = reader.ReadUInt32();

					if (exceptionTableSize != 0)
						yield return "Exception Table";

					var certificateTable = reader.ReadUInt32();			// File Offset
					var certificateTableSize = reader.ReadUInt32();

					if (certificateTableSize != 0)
						yield return "Certificate Table";

					var relocationTable = reader.ReadUInt32();
					var relocationTableSize = reader.ReadUInt32();

					if (relocationTableSize != 0)
						yield return "Relocation Table";

					var debugTable = reader.ReadUInt32();
					var debugTableSize = reader.ReadUInt32();

					if (debugTableSize != 0)
						yield return "Debug Table";

					var architectureTable = reader.ReadUInt32();
					var architectureTableSize = reader.ReadUInt32();

					if (architectureTableSize != 0)
						yield return "Architecture Table";

					var globalTable = reader.ReadUInt32();
					var globalTableSize = reader.ReadUInt32();

					if (globalTableSize != 0)
						yield return "Global Table";

					var tlsTable = reader.ReadUInt32();
					var tlsTableSize = reader.ReadUInt32();

					if (tlsTableSize != 0)
						yield return "TLS Table";

					var loadConfigTable = reader.ReadUInt32();
					var loadConfigTableSize = reader.ReadUInt32();

					if (loadConfigTableSize != 0)
						yield return "Load Config Table";

					var boundImportTable = reader.ReadUInt32();
					var boundImportTableSize = reader.ReadUInt32();

					if (boundImportTableSize != 0)
						yield return "Bound Import Table";

					var importAddressTable = reader.ReadUInt32();
					var importAddressTableSize = reader.ReadUInt32();

					if (importAddressTableSize != 0)
						yield return "Import Address Table";

					var delayImportTable = reader.ReadUInt32();
					var delayImportTableSize = reader.ReadUInt32();

					if (delayImportTableSize != 0)
						yield return "Delay Import Table";

					var clrTable = reader.ReadUInt32();
					var clrTableSize = reader.ReadUInt32();

					ClrTable clrTableNode = null;

					if (clrTableSize != 0)
					{
						clrTableNode = new ClrTable
						{
							Address = clrTable,
							Size = clrTableSize,
							Path = Path
						};

						yield return clrTableNode;
					}

					var reserved3 = reader.ReadBytes(8);

					for (var section = 0; section < sectionCount; section++)
					{
						var sectionName = reader.ReadChars(8);
						var virtualSize = reader.ReadUInt32();
						var virtualAddress = reader.ReadUInt32();
						var rawDataSize = reader.ReadUInt32();
						var rawDataPointer = reader.ReadUInt32();
						var relocationPointer = reader.ReadUInt32();
						var lineNumberPointer = reader.ReadUInt32();
						var relocationCount2 = reader.ReadUInt16();
						var lineNumberCount = reader.ReadUInt16();
						var sectionCharacteristics = reader.ReadUInt32();

						yield return "Section: " + new string(sectionName);

						if (clrTableNode != null &&
							clrTableNode.Address >= virtualAddress &&
							clrTableNode.Address < virtualAddress + virtualSize)
							clrTableNode.Position = clrTableNode.Address - virtualAddress + rawDataPointer;
					}

					yield break;
				}
			}
		}

		public object Properties => new { Path };

		public override string ToString()
		{
			return System.IO.Path.GetFileName(Path);
		}
	}
}