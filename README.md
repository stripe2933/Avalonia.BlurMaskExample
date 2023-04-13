# Avalonia.BlurMaskExample
Add Gaussian blur behind the mask using Skia in AvaloinaUI.

<img width="912" alt="Screenshot 2023-04-13 at 6 17 28 PM" src="https://user-images.githubusercontent.com/63503910/231713963-16158f32-6787-4f9f-a7ba-30b2b51443d5.png">

## Usage

Include `BlurMask.cs` class file into your application. Layout BlurMask control into a
portion that you want to make blur. Set `EnableBlur` (`bool`) and `BlurRadius` (`float`) properties to adjust the state.

```xml
<Panel Grid.Row="0">
    <Canvas>
        <Polyline Points="100,100 200,200 400,150 300,350 600,250 650,150"
                  Stroke="{DynamicResource ThemeForegroundBrush}"/>
    </Canvas>
    <local:BlurMask BlurEnabled="True"
                    BlurRadius="3"/>
</Panel>
```
