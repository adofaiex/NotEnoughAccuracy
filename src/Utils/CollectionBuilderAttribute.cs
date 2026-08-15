// ReSharper disable once CheckNamespace

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
internal sealed class CollectionBuilderAttribute(Type builderType, string methodName) : Attribute
{
	public Type BuilderType { get; } = builderType;
	public string MethodName { get; } = methodName;
}
