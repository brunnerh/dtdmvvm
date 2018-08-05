# DTDMVVM

A view-model binder for [WPF](https://en.wikipedia.org/wiki/Windows_Presentation_Foundation) which operates on data templates and uses a naming convention for locating views (*Data-Template-Driven [Model-View-ViewModel](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel)*).

Through WPF's implicit data template application, a *view-model first* approach to MVVM can easily be realized. Implicit data templates have their `DataType` set to the type of object they should be applied to. This micro-framework automates the process of creating such templates, hooking up view-models with views (commonly sub-classes of `UserControl`).

## Usage

The method `DataTemplateCreator.CreateViewModelDataTemplates(Assembly)` iterates the types of the given assembly, looking for a match to the naming convention `ViewModel` ⇒ `View`, i.e. the name of the view-model must contain `ViewModel` and all instances of the phrase will be replaced with `View` to locate the corresponding view class. (View model and view must be public.)

An example of matching namespace/class names:

    MyApp.ViewModels.Menu:FileMenuViewModel
    MyApp.Views.Menu:FileMenuView

(Replacing `ViewModel` in `ViewModels`, conveniently yields `Views`.)

The `DataTemplate` that would be created would look something like this:

```xml
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:vm="clr-namespace:MyApp.ViewModels.Menu;assembly=MyApp"
              DataType="{x:Type vm:FileMenuViewModel}">
    <v:FileMenuView xmlns:v="clr-namespace:MyApp.Views.Menu;assembly=MyApp"/>
</DataTemplate>
```

There are no explicit bindings set up, so the `DataContext` inside the view will be the view-model.

The `CreateViewModelDataTemplates` method returns a `ResourceDictionary`, which can, for example, be added to the `MergedDictionaries` of the `Application.Resources`:

```csharp
using System;
using System.Reflection;
using System.Windows;
using DTDMVVM;

public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    var assembly = Assembly.GetAssembly(typeof(App));
    var templates = DataTemplateCreator.CreateViewModelDataTemplates(assembly);
    Resources.MergedDictionaries.Add(templates);
  }
}
```

These templates are then automatically applied when e.g. a `ContentPresenter.Content` is bound to a view model instance, or an `ItemsControl.ItemsSource` is bound to a view model list.


## Special Cases

### Hierarchical Data Templates

For controls like the `TreeView`, a `HierarchicalDataTemplate` may be required. To make the template creator handle this, there is a `HierarchicalViewAttribute` which takes as argument the name of the property, the `HierarchicalDataTemplate.ItemsSource` should bind to.

### Generics

Sadly, generics are currently not supported. The implicit data template application mechanism does not seem to apply templates to generic type instances.

## Utilities

The `DTDMVVM.Utility` namespace provides two simple utility classes:

- `ViewModelBase`: Base class implementing `INotifyPropertyChanged` and `IDataErrorInfo`.
- `Command`: A delegate-based `ICommand` implementation.