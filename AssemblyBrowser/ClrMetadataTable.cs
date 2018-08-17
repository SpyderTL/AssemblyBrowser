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
					for (var table = 0; table < rowCounts[0]; table++)
					{
						var generation = reader.ReadUInt16();
						var name = reader.ReadUInt16();
						var moduleID = reader.ReadUInt16();
						var encID = reader.ReadUInt16();
						var encBaseID = reader.ReadUInt16();

						yield return "Module: " + name;
					}

					//01 - TypeRef
					for (var table = 0; table < rowCounts[1]; table++)
					{
						var resolutionScopeTableIndex = reader.ReadUInt16();
						//var resolutionScopeIndex = reader.ReadUInt16();
						var typeName = reader.ReadUInt16();
						var typeNamespace = reader.ReadUInt16();

						yield return "TypeRef: " + typeNamespace + "." + typeName;
					}

					//02 - TypeDef
					for (var table = 0; table < rowCounts[2]; table++)
					{
						var typeDefFlags = reader.ReadUInt32();
						var typeName = reader.ReadUInt16();
						var typeNamespace = reader.ReadUInt16();
						//var extendsTable = reader.ReadUInt16();
						var extendsTableIndex = reader.ReadUInt16();
						var fieldListIndex = reader.ReadUInt16();
						var methodListIndex = reader.ReadUInt16();

						yield return "TypeDef: " + typeNamespace + "." + typeName;
					}

					//04 - Field
					for (var table = 0; table < rowCounts[4]; table++)
					{
						yield return "Field";
					}

					//06 - MethodDef
					for (var table = 0; table < rowCounts[6]; table++)
					{
						var address = reader.ReadUInt32();
						var implementationAttributes = reader.ReadUInt16();
						var methodAttributes = reader.ReadUInt16();
						var name = reader.ReadUInt16();
						var signature = reader.ReadUInt16();
						var parameterListIndex = reader.ReadUInt16();

						yield return new MethodDef { Address = address, Name = name, Path = Path, Position = address - (Address - Position) };
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
					for (var table = 0; table < rowCounts[8]; table++)
					{
						var parameterAttributes = reader.ReadUInt16();
						var sequence = reader.ReadUInt16();
						var name = reader.ReadUInt16();

						yield return "Param: " + name;
					}

					//09 - InterfaceImpl
					for (var table = 0; table < rowCounts[9]; table++)
					{
						yield return "InterfaceImpl";
					}

					//10 - MemberRef
					for (var table = 0; table < rowCounts[10]; table++)
					{
						var classTableIndex = reader.ReadUInt16();
						var name = reader.ReadUInt16();
						var signature = reader.ReadUInt16();

						yield return "MemberRef: " + name;
					}

					//11 - Constant
					for (var table = 0; table < rowCounts[11]; table++)
					{
						yield return "Constant";
					}

					//12 - CustomAttribute
					for (var table = 0; table < rowCounts[12]; table++)
					{
						var parent = reader.ReadUInt16();
						var type = reader.ReadUInt16();
						var value = reader.ReadUInt16();

						yield return "CustomAttribute";
					}

					//13 - FieldMarshal
					for (var table = 0; table < rowCounts[13]; table++)
					{
						yield return "FieldMarshal";
					}

					//14 - DeclSecurity
					for (var table = 0; table < rowCounts[14]; table++)
					{
						yield return "DeclSecurity";
					}

					//15 - ClassLayout
					for (var table = 0; table < rowCounts[15]; table++)
					{
						yield return "ClassLayout";
					}

					//16 - FieldLayout
					for (var table = 0; table < rowCounts[16]; table++)
					{
						yield return "FieldLayout";
					}

					//17 - StandAloneSig
					for (var table = 0; table < rowCounts[17]; table++)
					{
						yield return "StandAloneSig";
					}

					//18 - EventMap
					for (var table = 0; table < rowCounts[18]; table++)
					{
						yield return "EventMap";
					}

					//20 - Event
					for (var table = 0; table < rowCounts[20]; table++)
					{
						yield return "Event";
					}

					//21 - PropertyMap
					for (var table = 0; table < rowCounts[21]; table++)
					{
						yield return "PropertyMap";
					}

					//23 - Property
					for (var table = 0; table < rowCounts[23]; table++)
					{
						yield return "Property";
					}

					//24 - MethodSemantics
					for (var table = 0; table < rowCounts[24]; table++)
					{
						yield return "MethodSemantics";
					}

					//25 - MethodImpl
					for (var table = 0; table < rowCounts[25]; table++)
					{
						yield return "MethodImpl";
					}

					//26 - ModuleRef
					for (var table = 0; table < rowCounts[26]; table++)
					{
						yield return "ModuleRef";
					}

					//27 - TypeSpec
					for (var table = 0; table < rowCounts[27]; table++)
					{
						yield return "TypeSpec";
					}

					//28 - ImplMap
					for (var table = 0; table < rowCounts[28]; table++)
					{
						yield return "ImplMap";
					}

					//29 - FieldRVA
					for (var table = 0; table < rowCounts[29]; table++)
					{
						yield return "FieldRVA";
					}

					//32 - Assembly
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

						yield return "Assembly: " + name;
					}

					//33 - AssemblyProcessor
					for (var table = 0; table < rowCounts[33]; table++)
					{
						yield return "AssemblyProcessor";
					}

					//34 - AssemblyOS
					for (var table = 0; table < rowCounts[34]; table++)
					{
						yield return "AssemblyOS";
					}

					//35 - AssemblyRef
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

						yield return "AssemblyRef: " + name;
					}

					//36 - AssemblyRefProcessor
					for (var table = 0; table < rowCounts[36]; table++)
					{
						yield return "AssemblyRefProcessor";
					}

					//37 - AssemblyRefOS
					for (var table = 0; table < rowCounts[37]; table++)
					{
						yield return "AssemblyRefOS";
					}

					//38 - File
					for (var table = 0; table < rowCounts[38]; table++)
					{
						yield return "File";
					}

					//39 - ExportedType
					for (var table = 0; table < rowCounts[39]; table++)
					{
						yield return "ExportedType";
					}

					//40 - ManifestResource
					for (var table = 0; table < rowCounts[40]; table++)
					{
						yield return "ManifestResource";
					}

					//41 - NestedClass
					for (var table = 0; table < rowCounts[41]; table++)
					{
						yield return "NestedClass";
					}

					//42 - GenericParam
					for (var table = 0; table < rowCounts[42]; table++)
					{
						yield return "GenericParam";
					}

					//44 - GenericParamConstraint
					for (var table = 0; table < rowCounts[44]; table++)
					{
						yield return "GenericParamConstraint";
					}
				}
			}
		}

		public object Properties => new { Offset, Size };

		public override string ToString() => "Metadata Table";
	}
}