using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace DTDMVVM
{
	/// <summary>
	/// Contains a method which creates implicit DataTemplates.
	/// </summary>
	public static class DataTemplateCreator
	{
		private static string DataTemplateFormat(
			string assemblyName,
			string vmTypeNS,
			string vmTypeName,
			string vTypeNS,
			string vTypeName
		) =>
			$@"<DataTemplate
					xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
					xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
					xmlns:vm=""clr-namespace:{vmTypeNS};assembly={assemblyName}""
					DataType=""{{x:Type vm:{vmTypeName}}}"">
				<v:{vTypeName} xmlns:v=""clr-namespace:{vTypeNS};assembly={assemblyName}""/>
			</DataTemplate>";

		private static string HierarchicalDataTemplateFormat(
			string assemblyName,
			string vmTypeNS,
			string vmTypeName,
			string vTypeNS,
			string vTypeName,
			string itemsSourcePropertyName
		) =>
			$@"<HierarchicalDataTemplate
					xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
					xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
					xmlns:vm=""clr-namespace:{vmTypeNS};assembly={assemblyName}""
					DataType=""{{x:Type vm:{vmTypeName}}}""
					ItemsSource=""{{Binding {itemsSourcePropertyName}}}"">
				<v:{vTypeName} xmlns:v=""clr-namespace:{vTypeNS};assembly={assemblyName}""/>
			</HierarchicalDataTemplate>";

		/// <summary>
		/// Creates implicit <see cref="DataTemplate"/> instances for the view models in <paramref name="assembly"/>.
		/// <para>
		/// View models have to be non-abstract and public. The name needs to contain "ViewModel".
		/// </para>
		/// <para>
		/// If a view class is found by replacing 'ViewModel' with 'View' in the full view model type name,
		/// a DataTemplate is created. E.g.
		/// </para>
		/// <para>
		/// App.ViewModels.SubNamespace.MainViewModel -> App.Views.SubNamespace.MainView
		/// </para>
		/// <para>
		/// To create a <see cref="HierarchicalDataTemplate"/> use the <see cref="HierarchicalViewModelAttribute"/> on the view model class.
		/// </para>
		/// <para>Generic view models are currently not supported.</para>
		/// </summary>
		/// <param name="assembly">The assembly containing the view models and views for which <see cref="DataTemplate"/> instances should be created.</param>
		/// <returns>
		/// A resource dictionary containing the implicit <see cref="DataTemplate"/> instances.
		/// It should be added to the <see cref="Application.Resources"/>' <see cref="ResourceDictionary.MergedDictionaries"/>.
		/// </returns>
		public static ResourceDictionary CreateViewModelDataTemplates(Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException("assembly");

			var assemblyName = assembly.GetName().Name;
			var dataTemplateDictionary = new ResourceDictionary();

			var vmTypes = from type in assembly.GetTypes()
						  where
							type.IsAbstract == false
							&& type.IsPublic
							&& type.IsDefined(typeof(CompilerGeneratedAttribute), false) == false
							&& type.FullName.Contains("ViewModel")
						  select type;


			foreach (var vmType in vmTypes)
			{
				var vmTypeNames = new List<string>();

				if (vmType.IsGenericType)
				{
					continue; //Generic DataTemplates are a pain.

					//var attributes = vmType.GetCustomAttributes(true).OfType<GenericTypeParametersAttribute>();
					//foreach (var attribute in attributes)
					//{
					//	var typeName = String.Format("{0}[{1}]", vmType.Name, String.Join(",", attribute.TypeParameters.Select(t => t.Name)));
					//	vmTypeNames.Add(typeName);
					//}
				}
				else
				{
					vmTypeNames.Add(vmType.Name);
				}

				foreach (var vmTypeName in vmTypeNames)
				{
					var attribute = vmType.GetCustomAttributes(true)
						.OfType<HierarchicalViewModelAttribute>()
						.FirstOrDefault();

					var vFullName = vmType.FullName.Replace("ViewModel", "View").Split(new char[] { '`' }, 2)[0];
					var vType = Type.GetType(vFullName + "," + vmType.AssemblyQualifiedName.Split(new char[] { ',' }, 2)[1]);
					if (vType == null)
						continue;

					var vmNamespace = vmType.Namespace;
					var vNamespace = vType.Namespace;
					var vName = vType.Name;

					string templateString = attribute == null
						? DataTemplateFormat(assemblyName, vmNamespace, vmTypeName, vNamespace, vName)
						: HierarchicalDataTemplateFormat(assemblyName, vmNamespace, vmTypeName, vNamespace, vName, attribute.ItemsSourcePropertyName);

					using (var stringReader = new StringReader(templateString))
					using (var xmlReader = XmlReader.Create(stringReader))
					{
						var template = (DataTemplate)XamlReader.Load(xmlReader);
						dataTemplateDictionary.Add(new DataTemplateKey(vmType), template);
					}
				}
			}

			return dataTemplateDictionary;
		}
	}
}
