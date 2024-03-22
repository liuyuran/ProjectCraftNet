// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ProtoSrc/client.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from ProtoSrc/client.proto</summary>
public static partial class ClientReflection {

  #region Descriptor
  /// <summary>File descriptor for ProtoSrc/client.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static ClientReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChVQcm90b1NyYy9jbGllbnQucHJvdG8ijgEKClBsYXllck1vdmUSDwoHY2h1",
          "bmtfeBgBIAEoBRIPCgdjaHVua195GAIgASgFEg8KB2NodW5rX3oYAyABKAUS",
          "CQoBeBgEIAEoAhIJCgF5GAUgASgCEgkKAXoYBiABKAISCwoDeWF3GAcgASgC",
          "Eg0KBXBpdGNoGAggASgCEhAKCGhlYWRfeWF3GAkgASgCIogBChJQbGF5ZXJD",
          "b250cm9sQmxvY2sSDAoEdHlwZRgBIAEoBRIPCgdjaHVua194GAIgASgFEg8K",
          "B2NodW5rX3kYAyABKAUSDwoHY2h1bmtfehgEIAEoBRIPCgdibG9ja194GAUg",
          "ASgFEg8KB2Jsb2NrX3kYBiABKAUSDwoHYmxvY2tfehgHIAEoBSI2ChNQbGF5",
          "ZXJDb250cm9sRW50aXR5EgwKBHR5cGUYASABKAUSEQoJdGFyZ2V0X2lkGAIg",
          "ASgDQgP4AQFiBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::PlayerMove), global::PlayerMove.Parser, new[]{ "ChunkX", "ChunkY", "ChunkZ", "X", "Y", "Z", "Yaw", "Pitch", "HeadYaw" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::PlayerControlBlock), global::PlayerControlBlock.Parser, new[]{ "Type", "ChunkX", "ChunkY", "ChunkZ", "BlockX", "BlockY", "BlockZ" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::PlayerControlEntity), global::PlayerControlEntity.Parser, new[]{ "Type", "TargetId" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class PlayerMove : pb::IMessage<PlayerMove>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<PlayerMove> _parser = new pb::MessageParser<PlayerMove>(() => new PlayerMove());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<PlayerMove> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ClientReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerMove() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerMove(PlayerMove other) : this() {
    chunkX_ = other.chunkX_;
    chunkY_ = other.chunkY_;
    chunkZ_ = other.chunkZ_;
    x_ = other.x_;
    y_ = other.y_;
    z_ = other.z_;
    yaw_ = other.yaw_;
    pitch_ = other.pitch_;
    headYaw_ = other.headYaw_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerMove Clone() {
    return new PlayerMove(this);
  }

  /// <summary>Field number for the "chunk_x" field.</summary>
  public const int ChunkXFieldNumber = 1;
  private int chunkX_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int ChunkX {
    get { return chunkX_; }
    set {
      chunkX_ = value;
    }
  }

  /// <summary>Field number for the "chunk_y" field.</summary>
  public const int ChunkYFieldNumber = 2;
  private int chunkY_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int ChunkY {
    get { return chunkY_; }
    set {
      chunkY_ = value;
    }
  }

  /// <summary>Field number for the "chunk_z" field.</summary>
  public const int ChunkZFieldNumber = 3;
  private int chunkZ_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int ChunkZ {
    get { return chunkZ_; }
    set {
      chunkZ_ = value;
    }
  }

  /// <summary>Field number for the "x" field.</summary>
  public const int XFieldNumber = 4;
  private float x_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public float X {
    get { return x_; }
    set {
      x_ = value;
    }
  }

  /// <summary>Field number for the "y" field.</summary>
  public const int YFieldNumber = 5;
  private float y_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public float Y {
    get { return y_; }
    set {
      y_ = value;
    }
  }

  /// <summary>Field number for the "z" field.</summary>
  public const int ZFieldNumber = 6;
  private float z_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public float Z {
    get { return z_; }
    set {
      z_ = value;
    }
  }

  /// <summary>Field number for the "yaw" field.</summary>
  public const int YawFieldNumber = 7;
  private float yaw_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public float Yaw {
    get { return yaw_; }
    set {
      yaw_ = value;
    }
  }

  /// <summary>Field number for the "pitch" field.</summary>
  public const int PitchFieldNumber = 8;
  private float pitch_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public float Pitch {
    get { return pitch_; }
    set {
      pitch_ = value;
    }
  }

  /// <summary>Field number for the "head_yaw" field.</summary>
  public const int HeadYawFieldNumber = 9;
  private float headYaw_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public float HeadYaw {
    get { return headYaw_; }
    set {
      headYaw_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as PlayerMove);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(PlayerMove other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (ChunkX != other.ChunkX) return false;
    if (ChunkY != other.ChunkY) return false;
    if (ChunkZ != other.ChunkZ) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(X, other.X)) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Y, other.Y)) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Z, other.Z)) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Yaw, other.Yaw)) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Pitch, other.Pitch)) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(HeadYaw, other.HeadYaw)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (ChunkX != 0) hash ^= ChunkX.GetHashCode();
    if (ChunkY != 0) hash ^= ChunkY.GetHashCode();
    if (ChunkZ != 0) hash ^= ChunkZ.GetHashCode();
    if (X != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(X);
    if (Y != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Y);
    if (Z != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Z);
    if (Yaw != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Yaw);
    if (Pitch != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Pitch);
    if (HeadYaw != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(HeadYaw);
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (ChunkX != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(ChunkX);
    }
    if (ChunkY != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ChunkY);
    }
    if (ChunkZ != 0) {
      output.WriteRawTag(24);
      output.WriteInt32(ChunkZ);
    }
    if (X != 0F) {
      output.WriteRawTag(37);
      output.WriteFloat(X);
    }
    if (Y != 0F) {
      output.WriteRawTag(45);
      output.WriteFloat(Y);
    }
    if (Z != 0F) {
      output.WriteRawTag(53);
      output.WriteFloat(Z);
    }
    if (Yaw != 0F) {
      output.WriteRawTag(61);
      output.WriteFloat(Yaw);
    }
    if (Pitch != 0F) {
      output.WriteRawTag(69);
      output.WriteFloat(Pitch);
    }
    if (HeadYaw != 0F) {
      output.WriteRawTag(77);
      output.WriteFloat(HeadYaw);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (ChunkX != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(ChunkX);
    }
    if (ChunkY != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ChunkY);
    }
    if (ChunkZ != 0) {
      output.WriteRawTag(24);
      output.WriteInt32(ChunkZ);
    }
    if (X != 0F) {
      output.WriteRawTag(37);
      output.WriteFloat(X);
    }
    if (Y != 0F) {
      output.WriteRawTag(45);
      output.WriteFloat(Y);
    }
    if (Z != 0F) {
      output.WriteRawTag(53);
      output.WriteFloat(Z);
    }
    if (Yaw != 0F) {
      output.WriteRawTag(61);
      output.WriteFloat(Yaw);
    }
    if (Pitch != 0F) {
      output.WriteRawTag(69);
      output.WriteFloat(Pitch);
    }
    if (HeadYaw != 0F) {
      output.WriteRawTag(77);
      output.WriteFloat(HeadYaw);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (ChunkX != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChunkX);
    }
    if (ChunkY != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChunkY);
    }
    if (ChunkZ != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChunkZ);
    }
    if (X != 0F) {
      size += 1 + 4;
    }
    if (Y != 0F) {
      size += 1 + 4;
    }
    if (Z != 0F) {
      size += 1 + 4;
    }
    if (Yaw != 0F) {
      size += 1 + 4;
    }
    if (Pitch != 0F) {
      size += 1 + 4;
    }
    if (HeadYaw != 0F) {
      size += 1 + 4;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(PlayerMove other) {
    if (other == null) {
      return;
    }
    if (other.ChunkX != 0) {
      ChunkX = other.ChunkX;
    }
    if (other.ChunkY != 0) {
      ChunkY = other.ChunkY;
    }
    if (other.ChunkZ != 0) {
      ChunkZ = other.ChunkZ;
    }
    if (other.X != 0F) {
      X = other.X;
    }
    if (other.Y != 0F) {
      Y = other.Y;
    }
    if (other.Z != 0F) {
      Z = other.Z;
    }
    if (other.Yaw != 0F) {
      Yaw = other.Yaw;
    }
    if (other.Pitch != 0F) {
      Pitch = other.Pitch;
    }
    if (other.HeadYaw != 0F) {
      HeadYaw = other.HeadYaw;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          ChunkX = input.ReadInt32();
          break;
        }
        case 16: {
          ChunkY = input.ReadInt32();
          break;
        }
        case 24: {
          ChunkZ = input.ReadInt32();
          break;
        }
        case 37: {
          X = input.ReadFloat();
          break;
        }
        case 45: {
          Y = input.ReadFloat();
          break;
        }
        case 53: {
          Z = input.ReadFloat();
          break;
        }
        case 61: {
          Yaw = input.ReadFloat();
          break;
        }
        case 69: {
          Pitch = input.ReadFloat();
          break;
        }
        case 77: {
          HeadYaw = input.ReadFloat();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          ChunkX = input.ReadInt32();
          break;
        }
        case 16: {
          ChunkY = input.ReadInt32();
          break;
        }
        case 24: {
          ChunkZ = input.ReadInt32();
          break;
        }
        case 37: {
          X = input.ReadFloat();
          break;
        }
        case 45: {
          Y = input.ReadFloat();
          break;
        }
        case 53: {
          Z = input.ReadFloat();
          break;
        }
        case 61: {
          Yaw = input.ReadFloat();
          break;
        }
        case 69: {
          Pitch = input.ReadFloat();
          break;
        }
        case 77: {
          HeadYaw = input.ReadFloat();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class PlayerControlBlock : pb::IMessage<PlayerControlBlock>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<PlayerControlBlock> _parser = new pb::MessageParser<PlayerControlBlock>(() => new PlayerControlBlock());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<PlayerControlBlock> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ClientReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerControlBlock() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerControlBlock(PlayerControlBlock other) : this() {
    type_ = other.type_;
    chunkX_ = other.chunkX_;
    chunkY_ = other.chunkY_;
    chunkZ_ = other.chunkZ_;
    blockX_ = other.blockX_;
    blockY_ = other.blockY_;
    blockZ_ = other.blockZ_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerControlBlock Clone() {
    return new PlayerControlBlock(this);
  }

  /// <summary>Field number for the "type" field.</summary>
  public const int TypeFieldNumber = 1;
  private int type_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int Type {
    get { return type_; }
    set {
      type_ = value;
    }
  }

  /// <summary>Field number for the "chunk_x" field.</summary>
  public const int ChunkXFieldNumber = 2;
  private int chunkX_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int ChunkX {
    get { return chunkX_; }
    set {
      chunkX_ = value;
    }
  }

  /// <summary>Field number for the "chunk_y" field.</summary>
  public const int ChunkYFieldNumber = 3;
  private int chunkY_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int ChunkY {
    get { return chunkY_; }
    set {
      chunkY_ = value;
    }
  }

  /// <summary>Field number for the "chunk_z" field.</summary>
  public const int ChunkZFieldNumber = 4;
  private int chunkZ_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int ChunkZ {
    get { return chunkZ_; }
    set {
      chunkZ_ = value;
    }
  }

  /// <summary>Field number for the "block_x" field.</summary>
  public const int BlockXFieldNumber = 5;
  private int blockX_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int BlockX {
    get { return blockX_; }
    set {
      blockX_ = value;
    }
  }

  /// <summary>Field number for the "block_y" field.</summary>
  public const int BlockYFieldNumber = 6;
  private int blockY_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int BlockY {
    get { return blockY_; }
    set {
      blockY_ = value;
    }
  }

  /// <summary>Field number for the "block_z" field.</summary>
  public const int BlockZFieldNumber = 7;
  private int blockZ_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int BlockZ {
    get { return blockZ_; }
    set {
      blockZ_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as PlayerControlBlock);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(PlayerControlBlock other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Type != other.Type) return false;
    if (ChunkX != other.ChunkX) return false;
    if (ChunkY != other.ChunkY) return false;
    if (ChunkZ != other.ChunkZ) return false;
    if (BlockX != other.BlockX) return false;
    if (BlockY != other.BlockY) return false;
    if (BlockZ != other.BlockZ) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (Type != 0) hash ^= Type.GetHashCode();
    if (ChunkX != 0) hash ^= ChunkX.GetHashCode();
    if (ChunkY != 0) hash ^= ChunkY.GetHashCode();
    if (ChunkZ != 0) hash ^= ChunkZ.GetHashCode();
    if (BlockX != 0) hash ^= BlockX.GetHashCode();
    if (BlockY != 0) hash ^= BlockY.GetHashCode();
    if (BlockZ != 0) hash ^= BlockZ.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Type != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(Type);
    }
    if (ChunkX != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ChunkX);
    }
    if (ChunkY != 0) {
      output.WriteRawTag(24);
      output.WriteInt32(ChunkY);
    }
    if (ChunkZ != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(ChunkZ);
    }
    if (BlockX != 0) {
      output.WriteRawTag(40);
      output.WriteInt32(BlockX);
    }
    if (BlockY != 0) {
      output.WriteRawTag(48);
      output.WriteInt32(BlockY);
    }
    if (BlockZ != 0) {
      output.WriteRawTag(56);
      output.WriteInt32(BlockZ);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Type != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(Type);
    }
    if (ChunkX != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(ChunkX);
    }
    if (ChunkY != 0) {
      output.WriteRawTag(24);
      output.WriteInt32(ChunkY);
    }
    if (ChunkZ != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(ChunkZ);
    }
    if (BlockX != 0) {
      output.WriteRawTag(40);
      output.WriteInt32(BlockX);
    }
    if (BlockY != 0) {
      output.WriteRawTag(48);
      output.WriteInt32(BlockY);
    }
    if (BlockZ != 0) {
      output.WriteRawTag(56);
      output.WriteInt32(BlockZ);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (Type != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Type);
    }
    if (ChunkX != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChunkX);
    }
    if (ChunkY != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChunkY);
    }
    if (ChunkZ != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(ChunkZ);
    }
    if (BlockX != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(BlockX);
    }
    if (BlockY != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(BlockY);
    }
    if (BlockZ != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(BlockZ);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(PlayerControlBlock other) {
    if (other == null) {
      return;
    }
    if (other.Type != 0) {
      Type = other.Type;
    }
    if (other.ChunkX != 0) {
      ChunkX = other.ChunkX;
    }
    if (other.ChunkY != 0) {
      ChunkY = other.ChunkY;
    }
    if (other.ChunkZ != 0) {
      ChunkZ = other.ChunkZ;
    }
    if (other.BlockX != 0) {
      BlockX = other.BlockX;
    }
    if (other.BlockY != 0) {
      BlockY = other.BlockY;
    }
    if (other.BlockZ != 0) {
      BlockZ = other.BlockZ;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Type = input.ReadInt32();
          break;
        }
        case 16: {
          ChunkX = input.ReadInt32();
          break;
        }
        case 24: {
          ChunkY = input.ReadInt32();
          break;
        }
        case 32: {
          ChunkZ = input.ReadInt32();
          break;
        }
        case 40: {
          BlockX = input.ReadInt32();
          break;
        }
        case 48: {
          BlockY = input.ReadInt32();
          break;
        }
        case 56: {
          BlockZ = input.ReadInt32();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          Type = input.ReadInt32();
          break;
        }
        case 16: {
          ChunkX = input.ReadInt32();
          break;
        }
        case 24: {
          ChunkY = input.ReadInt32();
          break;
        }
        case 32: {
          ChunkZ = input.ReadInt32();
          break;
        }
        case 40: {
          BlockX = input.ReadInt32();
          break;
        }
        case 48: {
          BlockY = input.ReadInt32();
          break;
        }
        case 56: {
          BlockZ = input.ReadInt32();
          break;
        }
      }
    }
  }
  #endif

}

public sealed partial class PlayerControlEntity : pb::IMessage<PlayerControlEntity>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<PlayerControlEntity> _parser = new pb::MessageParser<PlayerControlEntity>(() => new PlayerControlEntity());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<PlayerControlEntity> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::ClientReflection.Descriptor.MessageTypes[2]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerControlEntity() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerControlEntity(PlayerControlEntity other) : this() {
    type_ = other.type_;
    targetId_ = other.targetId_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PlayerControlEntity Clone() {
    return new PlayerControlEntity(this);
  }

  /// <summary>Field number for the "type" field.</summary>
  public const int TypeFieldNumber = 1;
  private int type_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int Type {
    get { return type_; }
    set {
      type_ = value;
    }
  }

  /// <summary>Field number for the "target_id" field.</summary>
  public const int TargetIdFieldNumber = 2;
  private long targetId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public long TargetId {
    get { return targetId_; }
    set {
      targetId_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as PlayerControlEntity);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(PlayerControlEntity other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Type != other.Type) return false;
    if (TargetId != other.TargetId) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (Type != 0) hash ^= Type.GetHashCode();
    if (TargetId != 0L) hash ^= TargetId.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Type != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(Type);
    }
    if (TargetId != 0L) {
      output.WriteRawTag(16);
      output.WriteInt64(TargetId);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Type != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(Type);
    }
    if (TargetId != 0L) {
      output.WriteRawTag(16);
      output.WriteInt64(TargetId);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (Type != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Type);
    }
    if (TargetId != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(TargetId);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(PlayerControlEntity other) {
    if (other == null) {
      return;
    }
    if (other.Type != 0) {
      Type = other.Type;
    }
    if (other.TargetId != 0L) {
      TargetId = other.TargetId;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Type = input.ReadInt32();
          break;
        }
        case 16: {
          TargetId = input.ReadInt64();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 8: {
          Type = input.ReadInt32();
          break;
        }
        case 16: {
          TargetId = input.ReadInt64();
          break;
        }
      }
    }
  }
  #endif

}

#endregion


#endregion Designer generated code
