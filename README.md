## MonoGame IME Helper for desktop and mobile platforms

### Targeted platforms at the moment

- WindowsDX(Desktop)
- SDL2(Desktop)
- Android
- iOS

## Getting started

### NuGet

MonoGame.IMEHelper is available on NuGet. It can be installed by issuing commands like the following command in the package manager console:

```
PM> Install-Package MonoGame.IMEHelper.WindowsDX
```

All available packages for specific platforms:

```
MonoGame.IMEHelper.Android
MonoGame.IMEHelper.DesktopGL
MonoGame.IMEHelper.iOS
MonoGame.IMEHelper.WindowsDX
```

### Initialize a IMEHandler instance in your game Initialize method

```c#
protected override void Initialize()
{
   imeHandler = IMEHandler.Create(this);
   imeHandler.TextInput += (s, e) => { ... };
}
```

*Remember to set `showDefaultIMEWindow` to true, `IMEHandler.Create(this. true)` e.g., since rendering Candidate Window in game is bugged and it's bugged in the Windows OS side*

### If your want to render Composition String

```#
   imeHandler = IMEHandler.Create(this, true);
   imeHandler.TextInput += (s, e) => { ... };
   imeHandler.TextComposition += (s, e) => { ... };
```

*Note that `TextComposition` event only works on WindowsDX platform due to limitation of the underlying library*

### Start Text Composition

`imeHandler.StartTextComposition();`

### Stop Text Composition

`imeHandler.StopTextComposition();`

### Get VirtualKeyboardHeight (only for mobile platforms)

`imeHandler.VirtualKeyboardHeight`

## Android Extra Setup

You have to change your Activity's base class to `AndroidGameActivityIME`, like the following:

```c#
public class Activity1 : AndroidGameActivityIME
{
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
        var g = new Game1();
        SetContentView((View)g.Services.GetService(typeof(View)));
        g.Run();
    }
}
```

## License

MonoGame.IMEHelper is released under the [The MIT License (MIT)](https://github.com/ryancheung/MonoGame.IMEHelper/blob/master/LICENSE).
