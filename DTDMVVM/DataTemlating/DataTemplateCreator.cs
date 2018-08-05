﻿using System;
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
		// 0: assembly-name
		// 1: vm-type-namespace
		// 2: vm-type-name
		// 3: v-type-namespace
		// 4: v-type-name
		private const string DataTemplateFormatString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
				    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
					xmlns:vm=""clr-namespace:{1};assembly={0}""
					DataType=""{{x:Type vm:{2}}}"">
						<v:{4} xmlns:v=""clr-namespace:{3};assembly={0}""/>
					</DataTemplate>";
		// 5: itemssource-property-name
		private const string HierarchicalDataTemplateFormatString = @"<HierarchicalDataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
				    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
					xmlns:vm=""clr-namespace:{1};assembly={0}""
					DataType=""{{x:Type vm:{2}}}"" ItemsSource=""{{Binding {5}}}"">
						<v:{4} xmlns:v=""clr-namespace:{3};assembly={0}""/>
					</HierarchicalDataTemplate>";

		/// <summary>
		/// Creates implicit <see cref="System.Windows.DataTemplate"/>s for the view models in the calling assembly.
		/// <para>
		/// The naming convention expects the views to be in the view namespace having the respective name when 'ViewModel' is replaced by 'View'.
		/// </para>
		/// <para>
		/// Base.ViewModels.SubNamespace.MainViewModel -> Base.Views.SubNamespace.MainView
		/// </para>
		/// <para>
		/// To create a <see cref="System.Windows.HierarchicalDataTemplate"/> use the <see cref="DTDMVVM.HierarchicalViewModelAttribute"/> on the view model class.
		/// </para>
		/// </summary>
		/// <param name="assembly">The assembly containing the view-models and views for which <see cref="System.Windows.DataTemplate"/>s should be created.</param>
		/// <returns>
		/// A resource dictionary containing the implicit <see cref="System.Windows.DataTemplate"/>s,
		/// it should be added to the <see cref="System.Windows.Application.Resources"/>' <see cref="System.Windows.ResourceDictionary.MergedDictionaries"/>.
		/// </returns>
		public static ResourceDictionary CreateViewModelDataTemplates(Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException("assembly");

			var assemblyName = assembly.GetName().Name;
			var dataTemplateDictionary = new ResourceDictionary();

			var vmTypes = from type in assembly.GetTypes()
						  where !type.IsAbstract
							&& type.IsPublic
							&& !type.IsDefined(typeof(CompilerGeneratedAttribute), false)
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
					string templateString;
					if (attribute == null)
					{
						templateString = String.Format(DataTemplateFormatString, assemblyName, vmNamespace, vmTypeName, vNamespace, vName);
					}
					else
					{
						templateString = String.Format(HierarchicalDataTemplateFormatString, assemblyName, vmNamespace, vmTypeName, vNamespace, vName, attribute.ItemsSourcePropertyName);
					}

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