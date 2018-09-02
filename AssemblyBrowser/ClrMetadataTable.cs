using System.Collections;

namespace AssemblyBrowser
{
	internal class ClrMetadataTable : IFolder, IProperties
	{
		public uint Offset { get; set; }
		public uint Size { get; set; }
		public uint Position { get; set; }
		public string Path { get; set; }
		public uint Address { get; set; }

		public IEnumerable Items
		{
			get
			{
				using (var stream = System.IO.File.OpenRead(Path))
				using (var reader = new System.IO.BinaryReader(stream))
				{
					stream.Position = Position;

					var reserved = reader.ReadUInt32();
					var majorVersion = reader.ReadByte();
					var minorVersion = reader.ReadByte();
					var flags = reader.ReadByte();
					var reserved2 = reader.ReadByte();
					var tableFlags = reader.ReadUInt64();
					var sortedTableFlags = reader.ReadUInt64();

					var rowCounts = new uint[64];

					for (var table = 0; table < rowCounts.Length; table++)
						rowCounts[table] = (tableFlags & (1UL << table)) != 0UL ? reader.ReadUInt32() : 0U;

					//00 - Module
					var position = stream.Position;
					var index = 1;

					for (var table = 0; table < rowCounts[0]; table++)
					{
						var generation = reader.ReadUInt16();
						var name = reader.ReadUInt16();
						var moduleID = reader.ReadUInt16();
						var encID = reader.ReadUInt16();
						var encBaseID = reader.ReadUInt16();

						yield return "0." + index + ": Module: " + name;

						index++;
					}

					//01 - TypeRef
					position = stream.Position;
					index = 1;

					for (var table = 0; table < rowCounts[1]; table++)
					{
						var resolutionScopeToken = reader.ReadUInt16();
						var resolutionScopeTable = resolutionScopeToken & 0x03;
						var resolutionScopeIndex = resolutionScopeToken >> 2;

						var typeName = reader.ReadUInt16();
						var typeNamespace = reader.ReadUInt16();

						yield return "1." + index + ": TypeRef: " + resolutionScopeTable + "." + resolutionScopeIndex + "." + typeNamespace + "." + typeName;

						index++;
					}

					//02 - TypeDef
					position = stream.Position;
					index = 1;

					for (var table = 0; table < rowCounts[2]; table++)
					{
						var typeDefFlags = reader.ReadUInt32();
						var typeName = reader.ReadUInt16();
						var typeNamespace = reader.ReadUInt16();
						//var extendsTable = reader.ReadUInt16();
						var extendsTableIndex = reader.ReadUInt16();
						var fieldListIndex = reader.ReadUInt16();
						var methodListIndex = reader.ReadUInt16();

						yield return "2." + index + ": TypeDef: " + typeNamespace + "." + typeName;

						index++;
					}

					//04 - Field
					index = 1;

					for (var table = 0; table < rowCounts[4]; table++)
					{
						yield return "4." + index + ": Field";

						index++;
					}

					//06 - MethodDef
					position = stream.Position;
					index = 1;

					for (var table = 0; table < rowCounts[6]; table++)
					{
						var address = reader.ReadUInt32();
						var implementationAttributes = reader.ReadUInt16();
						var methodAttributes = reader.ReadUInt16();
						var name = reader.ReadUInt16();
						var signature = reader.ReadUInt16();
						var parameterListIndex = reader.ReadUInt16();

						yield return new MethodDef { Address = address, Name = name, Path = Path, Position = address - (Address - Position), Index = index };

						index++;

						//yield return "MethodDef: " + name;

						// address points to method header (1-byte or 12-byte), followed by method body.

						// tiny
						//		Flags		2 bits
						//		Size			6 bits

						// fat
						//		Flags						12 bits
						//		HeaderSize				4 bits
						//		MaxStack					16 bits
						//		CodeSize					32 bits
						//		LocalVariableToken	32 bits
					}

					//08 - Param
					index = 1;

					for (var table = 0; table < rowCounts[8]; table++)
					{
						var parameterAttributes = reader.ReadUInt16();
						var sequence = reader.ReadUInt16();
						var name = reader.ReadUInt16();

						yield return "8." + index + ": Param: " + name;

						index++;
					}

					//09 - InterfaceImpl
					index = 1;

					for (var table = 0; table < rowCounts[9]; table++)
					{
						yield return "9." + index + ": InterfaceImpl";

						index++;
					}

					//10 - MemberRef
					index = 1;

					for (var table = 0; table < rowCounts[10]; table++)
					{
						var memberRefParent = reader.ReadUInt16();
						var parentTable = memberRefParent & 0x07;
						var parentIndex = memberRefParent >> 3;
						var name = reader.ReadUInt16();
						var signature = reader.ReadUInt16();

						yield return "10." + index + ": MemberRef: " + parentTable + "." + parentIndex + "." + name;

						index++;
					}

					//11 - Constant
					index = 1;

					for (var table = 0; table < rowCounts[11]; table++)
					{
						yield return "11." + index + ": Constant";

						index++;
					}

					//12 - CustomAttribute
					index = 1;

					for (var table = 0; table < rowCounts[12]; table++)
					{
						var offset = stream.Position - position;

						var parent = reader.ReadUInt16();
						var type = reader.ReadUInt16();
						var value = reader.ReadUInt16();

						yield return "12." + index + ": CustomAttribute: " + parent + "." + type + "." + value;

						index++;
					}

					//13 - FieldMarshal
					index = 1;

					for (var table = 0; table < rowCounts[13]; table++)
					{
						yield return "13." + index + ": FieldMarshal";

						index++;
					}

					//14 - DeclSecurity
					index = 1;

					for (var table = 0; table < rowCounts[14]; table++)
					{
						yield return "14." + index + ": DeclSecurity";

						index++;
					}

					//15 - ClassLayout
					index = 1;

					for (var table = 0; table < rowCounts[15]; table++)
					{
						yield return "15." + index + ": ClassLayout";

						index++;
					}

					//16 - FieldLayout
					index = 1;

					for (var table = 0; table < rowCounts[16]; table++)
					{
						yield return "16." + index + ": FieldLayout";

						index++;
					}

					//17 - StandAloneSig
					index = 1;

					for (var table = 0; table < rowCounts[17]; table++)
					{
						yield return "17." + index + ": StandAloneSig";

						index++;
					}

					//18 - EventMap
					index = 1;

					for (var table = 0; table < rowCounts[18]; table++)
					{
						yield return "18." + index + ": EventMap";

						index++;
					}

					//20 - Event
					index = 1;

					for (var table = 0; table < rowCounts[20]; table++)
					{
						yield return "20." + index + ": Event";

						index++;
					}

					//21 - PropertyMap
					index = 1;

					for (var table = 0; table < rowCounts[21]; table++)
					{
						var offset = stream.Position - position;

						yield return "21." + index + ": PropertyMap";

						index++;
					}

					//23 - Property
					index = 1;

					for (var table = 0; table < rowCounts[23]; table++)
					{
						yield return "23." + index + ": Property";

						index++;
					}

					//24 - MethodSemantics
					index = 1;

					for (var table = 0; table < rowCounts[24]; table++)
					{
						yield return "24." + index + ": MethodSemantics";

						index++;
					}

					//25 - MethodImpl
					index = 1;

					for (var table = 0; table < rowCounts[25]; table++)
					{
						var offset = stream.Position - position;

						yield return "25." + index + ": MethodImpl";

						index++;
					}

					//26 - ModuleRef
					index = 1;

					for (var table = 0; table < rowCounts[26]; table++)
					{
						yield return "26." + index + ": ModuleRef";

						index++;
					}

					//27 - TypeSpec
					index = 1;

					for (var table = 0; table < rowCounts[27]; table++)
					{
						yield return "27." + index + ": TypeSpec";

						index++;
					}

					//28 - ImplMap
					index = 1;

					for (var table = 0; table < rowCounts[28]; table++)
					{
						yield return "28." + index + ": ImplMap";

						index++;
					}

					//29 - FieldRVA
					index = 1;

					for (var table = 0; table < rowCounts[29]; table++)
					{
						yield return "29." + index + ": FieldRVA";

						index++;
					}

					//32 - Assembly
					index = 1;

					for (var table = 0; table < rowCounts[32]; table++)
					{
						var assemblyID = reader.ReadUInt32();
						var assemblyMajorVersion = reader.ReadUInt16();
						var assemblyMinorVersion = reader.ReadUInt16();
						var assemblyBuildNumber = reader.ReadUInt16();
						var assemblyRevisionNumber = reader.ReadUInt16();
						var assemblyFlags = reader.ReadUInt32();
						var publicKey = reader.ReadUInt16();
						var name = reader.ReadUInt16();
						var culture = reader.ReadUInt16();

						yield return "32." + index + ": Assembly: " + name;

						index++;
					}

					//33 - AssemblyProcessor
					index = 1;

					for (var table = 0; table < rowCounts[33]; table++)
					{
						yield return "33." + index + ": AssemblyProcessor";

						index++;
					}

					//34 - AssemblyOS
					index = 1;

					for (var table = 0; table < rowCounts[34]; table++)
					{
						yield return "34." + index + ": AssemblyOS";

						index++;
					}

					//35 - AssemblyRef
					index = 1;

					for (var table = 0; table < rowCounts[35]; table++)
					{
						var assemblyMajorVersion = reader.ReadUInt16();
						var assemblyMinorVersion = reader.ReadUInt16();
						var assemblyBuildNumber = reader.ReadUInt16();
						var assemblyRevisionNumber = reader.ReadUInt16();
						var assemblyFlags = reader.ReadUInt32();
						var publicKeyOrToken = reader.ReadUInt16();
						var name = reader.ReadUInt16();
						var culture = reader.ReadUInt16();
						var hashValue = reader.ReadUInt16();

						yield return "35." + index + ": AssemblyRef: " + name;

						index++;
					}

					//36 - AssemblyRefProcessor
					index = 1;

					for (var table = 0; table < rowCounts[36]; table++)
					{
						yield return "36." + index + ": AssemblyRefProcessor";

						index++;
					}

					//37 - AssemblyRefOS
					index = 1;

					for (var table = 0; table < rowCounts[37]; table++)
					{
						yield return "37." + index + ": AssemblyRefOS";

						index++;
					}

					//38 - File
					index = 1;

					for (var table = 0; table < rowCounts[38]; table++)
					{
						yield return "38." + index + ": File";

						index++;
					}

					//39 - ExportedType
					index = 1;

					for (var table = 0; table < rowCounts[39]; table++)
					{
						yield return "39." + index + ": ExportedType";

						index++;
					}

					//40 - ManifestResource
					index = 1;

					for (var table = 0; table < rowCounts[40]; table++)
					{
						yield return "40." + index + ": ManifestResource";

						index++;
					}

					//41 - NestedClass
					index = 1;

					for (var table = 0; table < rowCounts[41]; table++)
					{
						yield return "41." + index + ": NestedClass";

						index++;
					}

					//42 - GenericParam
					index = 1;

					for (var table = 0; table < rowCounts[42]; table++)
					{
						yield return "42." + index + ": GenericParam";

						index++;
					}

					//44 - GenericParamConstraint
					index = 1;

					for (var table = 0; table < rowCounts[44]; table++)
					{
						yield return "44." + index + ": GenericParamConstraint";

						index++;
					}
				}
			}
		}

		public object Properties => new { Offset, Size };

		public override string ToString() => "Metadata Table";
	}
}