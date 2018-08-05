using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Data;

namespace DTDMVVM.Utility
{
	/// <summary>
	/// A base class for view models which implements interfaces commonly used in view models.
	/// </summary>
	public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
	{
		private Dictionary<string, string> _Errors = null;
		private Dictionary<string, string> Errors
		{
			get
			{
				if (_Errors == null)
					_Errors = new Dictionary<string, string>();
				return _Errors;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event for the given property name.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void SetError(string column, string message)
		{
			if (column == null) throw new ArgumentNullException("column");

			if (message == null)
				if (Errors.ContainsKey(column))
					Errors.Remove(column);
				else
					return;
			else
				Errors[column] = message;

			OnPropertyChanged("Error");
			OnPropertyChanged(Binding.IndexerName);
		}

		/// <summary>
		/// Invalidates if validation action throws an exception, uses exception message for the error by default.
		/// </summary>
		/// <param name="key">Name of property being validated.</param>
		/// <param name="validationAction">The action that needs to execute without exception for the property to be valid.</param>
		/// <param name="errorMessage">The error message to associate with the property if it is invalid. If null the exception message will be used.</param>
		protected void Validate(string key, Action validationAction, string errorMessage = null)
		{
			var isValid = true;
			try
			{
				validationAction();
			}
			catch (Exception ex)
			{
				isValid = false;
				if (errorMessage == null)
					errorMessage = ex.Message;
			}
			Validate(key, isValid, errorMessage);
		}
		/// <summary>
		/// Invalidates if validation function returns false.
		/// </summary>
		/// <param name="key">Name of property being validated.</param>
		/// <param name="validationFunction">Function used to determine if property is valid.</param>
		/// <param name="errorMessage">The error message to associate with the property if it is invalid.</param>
		protected void Validate(string key, Func<bool> validationFunction, string errorMessage)
		{
			Validate(key, validationFunction(), errorMessage);
		}
		/// <summary>
		/// Invalidates according to boolean passed as argument.
		/// </summary>
		/// <param name="key">Name of property being validated.</param>
		/// <param name="validationFunction">A boolean indicating if property is valid.</param>
		/// <param name="errorMessage">The error message to associate with the property if it is invalid.</param>
		protected void Validate(string key, bool isValid, string errorMessage)
		{
			SetError(key, isValid ? null : errorMessage);
		}

		/// <summary>
		/// Returns a validation error for a property, null if there is none.
		/// <para>
		/// If the property has a <see cref="System.ComponentModel.DisplayNameAttribute"/> that name will appear in front of the error message.
		/// </para>
		/// </summary>
		/// <param name="propertyName">The property name.</param>
		/// <returns>Validation error message for this property, null if there is none.</returns>
		public string this[string propertyName]
		{
			get
			{
				if (Errors.ContainsKey(propertyName))
				{
					var error = Errors[propertyName];
					var displayName = this.GetType()
						.GetProperty(propertyName)
						.GetCustomAttributes(true)
						.OfType<DisplayNameAttribute>()
						.FirstOrDefault();
					if (displayName != null)
						return displayName.DisplayName + " - " + error;
					else
						return error;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// All validation errors of this object combined.
		/// </summary>
		public string Error
		{
			get
			{
				return String.Join(Environment.NewLine, Errors.Keys.Select(key => this[key]));
			}
		}
	}
}
