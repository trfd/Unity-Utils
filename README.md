# Unity-Utils

Set useful classes for Unity.

## Serialization Wrappers

Unity-Utils includes a set of wrappers useful for serializing C# classes such as `System.Type` or `System.Reflection.FieldInfo` or Dictionaries etc…
The wrappers convert the informations to string to take benefit of Unity’s serialization.

| Utils Wrapper       | C# Class | 
| ------------------- |:------------------------------:| 
| `TypeWrapper `        | `System.Type`                    | 
| `FieldInfoWrapper`    | `System.Reflection.FieldInfo`    | 
| `PropertyInfoWrapper` | `System.Reflection.PropertyInfo` | 
| `MethodInfoWrapper`   | `System.Reflection.MethodInfo`   |
| `DataMemberWrapper`   | Either a `FieldInfo` or `PropertyInfo`   |
| `NestedDataMemberWrapper` | Either a `FieldInfo` or `PropertyInfo` nested in class architecture |
| `ComponentNestedDataMemberWrapper` | Either a `FieldInfo` or `PropertyInfo` nested in class architecture using a `UnityEngine.Component` subclass as base class |
| `DictionaryWrapper<T>` | `System.Collections.Generic.Dictionary<T>`


### Quick Usage

Every reflections related classes use a similar interface as TypeWrapper’s.

#### TypeWrapper

TypeWrapper as well as reflection related wrappers can be used for reflection-based tools or game architecture in Unity.
The following `MonoBehaviour` describes how a `System.Type` can be saved. If the method `Create` is called from an editor script the value is set to `TestClassB` then the type is serialized and de-serialized to and from string when the player is launched. 
When the `MonoBehaviour` starts the saved type is instantiated.

	using UnityEngine;
	using Utils;

	public TestClassA
	{}

	public TestClassB : TestClassA
	{}

	public class MyTest : MonoBehaviour
	{
		public TestClassA _myA;
		public TypeWrapper _myType;

		// Set type (Call from the inspector for testing)
		// (the attribute InspectorButton can be used for testing)
		//[InspectorButton(« Create »)
		void Create()
		{
			_myType = new TypeWrapper(typeof(TestClassB));
		}

		void Start()
		{
			_myA = (TestClassA) System.Activator.CreateInstance(_myType.Type);
				
			Debug.Log(« Type : » +_myA.GetType());
		}
	}

The output should be: 

	Type : TestClassB

#### DictionaryWrapper

The usage of `DictionaryWrapper<T>` is slightly different from the `TypeWrapper`. Unity’s serialization does not take into account the generic types. Thus `DictionaryWrapper<T>` can not be serialized instead an intermediary type should be used.

	using UnityEngine;
	using Utils;

	public class MyTest : MonoBehaviour
	{
		[System.Serializable]
		public class FillMap : DictionaryWrapper<int,string>
		{}

		public FillMap _myMap;

		// Fill the dictionary (Call from the inspector for testing)
		// (the attribute InspectorButton can be used for testing)
		//[InspectorButton(« FillMap »)
		void FillMap()
		{
			_myMap = new TestMap();

			_myMap.Dictionary.Add(0, « Hello »);
			_myMap.Dictionary.Add(42, « World! »);
		}

		void Start()
		{
			Debug.Log(« _myMap[0] = « + _myMap.Dictionary[0]);
			Debug.Log(« _myMap[42] = « + _myMap.Dictionary[42]);
		}
	}

This code should output:

	_myMap[0] = Hello
	_myMap[42] = World!

## Editor Utilities 

For rapid debugging Unity-Utils provides two useful attributes: 
`InspectorLabel` and `InspectorButton`.

#### InspectorLabel

In order to visualize the value of a private field, a property or a non-serializable member. The attribute `InspectorLabelAttribute` can be used as follow:

	using UnityEngine;
	using Utils;

	public class MyTestClass : MonoBehaviour
	{
		[InspectorLabel]
		private float m_WatchMember;

		void Update()
		{
			m_WatchMember = Time.timeSinceLevelLoad;
		}
	}

In the inspector of `MyTestClass`, a label should display the value of `m_WatchMember`

#### InspectorButton

The attribute `InspectorButtonAttribute` can be used to display button in the class’ inspector that calls a specific method. 
This can be used for live debugging or implementation testing.

	using UnityEngine;
	using Utils;

	public class MyTestClass : MonoBehaviour
	{
		[InspectorButton(«Test »)]
		void TestMethod()
		{
			Debug.Log(« Hello World! »);
		}
	}


## HTML Logger

Unity-Utils includes a basic implementation of HTML Logger based on [devmag.org.za’s logger](http://devmag.org.za/2011/01/25/make-your-logs-interactive-and-squash-bugs-faster/).
This logger is useful for debugging players, QA testing or advanced debugging.

For the moment the logger resides in the `StreamingAsset` folder. Simply paste the folder `Utils/Logger/log` into `StreamingAsset`

The implementation of the HTML logger should be improved with screenshots and graphs.

## Misc.

Unity-Utils also includes a variety of other  classes:

* `GameObjectRef` and `ComponentRef` to manage cross-scene weak references to GameObject and Component.

* Unique ID (`UID`)and `UIDManager` to identify uniquely objects.

* `RandomAnimationCurve` to animate randomly between to defined `AnimationCurves`



