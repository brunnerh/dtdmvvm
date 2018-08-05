using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DTDMVVM.Utility
{
	/// <summary>
	/// A delegate-based <see cref="System.Windows.Input.ICommand"/> implementation.
	/// </summary>
	public class Command : ICommand
	{
		private readonly Func<object, bool> _canExecute = param => true;
		private readonly Action<object> _execute = param => { };

		/// <summary>
		/// Initializes a new instance of the <see cref="DTDMVVM.Command"/> class which does nothing and can always be executed.
		/// </summary>
		public Command() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="DTDMVVM.Command"/> class that can always be executed.
		/// </summary>
		/// <param name="execute">The action to execute, the input for the function is the <see cref="System.Windows.Input.ICommandSource.CommandParameter"/>.</param>
		public Command(Action<object> execute)
			: this()
		{
			_execute = execute;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DTDMVVM.Command"/> class that can only be executed if the can-execute-function returns true.
		/// </summary>
		/// <param name="canExecute">A function which determines if the command can be executed, the input for the function is the <see cref="System.Windows.Input.ICommandSource.CommandParameter"/>.</param>
		/// <param name="execute">The action to execute, the input for the function is the <see cref="System.Windows.Input.ICommandSource.CommandParameter"/>.</param>
		public Command(Func<object, bool> canExecute, Action<object> execute)
			: this(execute)
		{
			_canExecute = canExecute;
		}

		/// <summary>
		/// Evaluates whether the command can be executed.
		/// </summary>
		/// <param name="parameter">A parameter, usually passed by an <see cref="System.Windows.Input.ICommandSource"/>.</param>
		/// <returns>A boolean indicating whether the command can be executed.</returns>
		public bool CanExecute(object parameter)
		{
			return _canExecute(parameter);
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="parameter">A parameter, usually passed by an <see cref="System.Windows.Input.ICommandSource"/>.</param>
		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		/// <summary>
		/// Executes command only if <see cref="CanExecute"/> returns true.
		/// </summary>
		/// <param name="parameter">The parameter to be passed to <see cref="CanExecute"/> and <see cref="Execute"/>.</param>
		public void TryExecute(object parameter)
		{
			if (CanExecute(parameter))
				Execute(parameter);
		}


		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Raises the <see cref="CanExecuteChanged"/> event, this method should be called to make controls reevaluate the <see cref="CanExecute"/> method.
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			var handler = CanExecuteChanged;
			if (handler != null)
			{
				handler(this, null);
			}
		}
	}
}
