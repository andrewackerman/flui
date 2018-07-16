# FLUI - A UI file format for Flutter

FLUI is an entirely new data storage format created especially for use in Flutter apps. It is loosely based on the YAML syntax but designed to resemble the resulting Dart code in organization. Using this format (and the bundled builder) you will be able to write your UI code in a centralized place. Once converted, the build script converts the UI code into separate View and ViewModel classes.

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
		return SampleView(
			Center(
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
										title: 'Get Flui a bagel',
										onPressed: () => getBagel(),
									),
								],
							),
						),
					],
				),
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

	getBagel() {
		// TODO: Populate function getBagel
	}
}
```