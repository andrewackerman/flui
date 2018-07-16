# FLUI - A UI file format for Flutter

FLUI is an entirely new data storage format created especially for use in Flutter apps. It is loosely based on the YAML syntax but designed to resemble the resulting Dart code in organization. Using this format (and the bundled builder) you will be able to write your UI code in a centralized place. Once converted, the build script converts the UI code into separate View and ViewModel classes.

Conceptually, the approach for View-ViewModel separation comes from [this article by Edrick Leong](https://blog.usejournal.com/easily-navigate-through-your-flutter-code-by-separating-view-and-view-model-240026191106). This is about the most abstracted way I could find to separate the build function from the other code in Widget classes. Every other method I can think of would require the use of the `dart:mirror` package (i.e. Reflection), which Flutter [doesn't currently support for optimization reasons](https://github.com/flutter/flutter/issues/1150).

## Examples

Here is an example `sample.flui` file:

```
Stateless .viewModel SampleViewModel, .view SampleView:
  Center .child:
    Column .children:
      ! Child 1
      Card .child:
        Column .children:
          Text 'Hello World'
          Text 'Flui says hello'
      ! Child 2
      Card .child:
        Column .children:
          Text 'Hello again, World', .style textStyle
          Text:
            'Flui wants a bagel' 
            .style $getStyle
          RaisedButton:
            .title 'Get Flui a bagel'
            .onPressed @getBagel
```

After being run through the build script, This code gets converted into the following:

`sample_view_model.dart`
```dart
import 'package:flutter/material.dart';

import './sample_view_model.dart';

class SampleView extends SampleViewModel {
  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        children: [
          Card(
            child: Column(
              children: [
                Text('Hello World'),
                Text('Flui says hello'),
              ],
            ),
          ),
          Card(
            child: Column(
              children: [
                Text (
                  'Hello again, World',
                  style: textStyle,
                ),
                Text (
                  'Flui wants a bagel',
                  style: getStyle(),
                ),
                RaisedButton (
                  title: 'Give Flui a bagel',
                  onPressed: () => giveBagel(),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
```

`sample_view.dart`
```dart
import 'package:flutter/material.dart';

class SampleViewModel extends StatelessWidget {
  var textStyle = null; // TODO: Populate field textStyle

  getStyle() {
    // TODO: Populate function getStyle
  }

  giveBagel() {
    // TODO: Populate function giveBagel
  }
}
```

The view file is entirely managed by the build script - any code written into the file would be overwritten the next time the buildscript is run.

## Syntax

As far as syntax goes, the file is incredibly simple.

```
Stateless .viewModel SampleViewModel, .view SampleView:
```

This is the file header and the root node for the entire document. It requires either `Stateless` or `Stateful` as the first value. It also requires the two attributes `.view` and `.viewModel`, the values of which are used to determine the class references and names of the generated `.dart` files and can be anything that is a valid Dart class name.

From there, the UI hierarchy is determined by indentation level. 

```
Center .child:
```

The first value is the name of an identifier for a UI element. (While it is not currently mandatory that the value be an _actual_ Flutter widget, the build script does differentiate between Flutter Widget names and other ordinary identifiers. This might become significant in the future for the building process, tooling for VS Code, etc.)

After the first identifier, you can list the attributes of the widget by using the period character `.` followed by the identifier of the attribute and then the value of the attribute.

For widgets that support a positional argument, you can specify a value directly without attaching it to an attribute, such as with `Text` widget:

```
Text 'Hello World'
```

### Single Line vs Multiline

Widgets and attributes can be supplied with values either on the same line as the identifier or on the next line with an increased indentation level. For example:

```
Text 'Hello World'
```

is functionally identical to:

```
Text:
  'Hello World'
```

For single line widget declarations, you separate multiple attributes using a comma:

```
Text 'Hello again, World', .style textStyle
```

As a general rule, you use a colon `:` to designate when a widget or attribute is listing its child(ren) on the next line. The colon is optional, however, so you could just as easily do:

```
Text
  'Hello World'
```

### ViewModel References

There are three different ways to directly reference an object that exists in the view model. First, you can reference a field object by typing the name of the object:

```
Text 'Hello again, World', .style textStyle
```

This ties directly to this line in the view model:

```
var textStyle = null; // TODO: Populate field textStyle
```

Next, you can call a function that returns a value by using the dollar sign `$` prefix:

```
Text:
  'Flui wants a bagel'
  .style $getStyle
```

This calls this function in the view model:

```
getStyle() {
  // TODO: Populate function getStyle
}
```

Finally, you can specify a callback for an action using the prefix `@`:

```
RaisedButton:
  .title 'Give Flui a bagel'
  .onPressed @giveBagel
```

In the view, this gets translated to the following:

```
RaisedButton (
  title: 'Give Flui a bagel',
  onPressed: () => giveBagel(),
),
```

Which, obviously, calls this function in the view model:

```
giveBagel() {
  // TODO: Populate function giveBagel
}
```

## TODO

This list in itself is a TODO, as I will probably think of new features and functions to add as the project comes along.

- [ ] Add Stateful widget support
- [ ] Intelligently add fields and functions to the view model so as to not overwrite existing changes
- [ ] Eliminate common attributes from having to be explicitly stated (e.g. `.child` for `Center`, `.children` for `Column`, etc.)
- [ ] VS Code plugin for syntax and colorization support
- [ ] VS Code integration for detecting changes in UI files to update the view and view model files (which would, in tirn, trigger hot reloading for Flutter itself)
- [ ] Hook up automated integration testing for future build changes