using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DTDMVVM
{
	//Commented out because it ain't gonna work

	/// <summary>
	/// Attribute that indicates for which types used as generic type parameters <see cref="DataTemplate"/>s should be created.
	/// <remarks>As of .NET 4 there are no implicit DataTemplates for generic type bases.</remarks>
	/// </summary>
	//[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	//public class GenericTypeParametersAttribute : Attribute
	//{
	//	private readonly IEnumerable<Type> _TypeParameters;
	//	internal IEnumerable<Type> TypeParameters { get { return _TypeParameters; } }

	//	/// <summary>
	//	/// Initializes a new instance of <see cref="GenericTypeParametersAttribute"/>.
	//	/// </summary>
	//	/// <param name="typeParameters">The type parameters for one <see cref="DataTemplate"/> instance.</param>
	//	public GenericTypeParametersAttribute(params Type[] typeParameters)
	//	{
	//		_TypeParameters = typeParameters;
	//	}
	//}
}
