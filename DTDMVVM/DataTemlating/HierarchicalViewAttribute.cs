using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DTDMVVM
{
	/// <summary>
	/// Attribute that indicates to the <see cref="DTDMVVM.DataTemplateCreator"/> that a <see cref="System.Windows.HierarchicalDataTemplate"/> should be created for this view model.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	sealed public class HierarchicalViewModelAttribute : Attribute
	{
		private readonly string _ItemsSourcePropertyName = null;
		/// <summary>
		/// Gets the name of the property which should be bound to the <see cref="System.Windows.HierarchicalDataTemplate.ItemsSource"/>.
		/// </summary>
		public string ItemsSourcePropertyName { get { return _ItemsSourcePropertyName; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="DTDMVVM.HierarchicalViewModelAttribute"/> class.
		/// </summary>
		/// <param name="itemsSourcePropertyName">The name of the property which should be bound to the <see cref="System.Windows.HierarchicalDataTemplate.ItemsSource"/>.</param>
		public HierarchicalViewModelAttribute(string itemsSourcePropertyName)
		{
			if (itemsSourcePropertyName == null) throw new ArgumentNullException("itemsSourcePropertyName");

			_ItemsSourcePropertyName = itemsSourcePropertyName;
		}
	}
}
