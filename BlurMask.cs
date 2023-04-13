using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace BlurMaskExample;

public class BlurMask : Control
{
    public static readonly StyledProperty<bool> BlurEnabledProperty = AvaloniaProperty.Register<BlurMask, bool>("BlurEnabled", true);
    public static readonly StyledProperty<float> BlurRadiusProperty = AvaloniaProperty.Register<BlurMask, float>("BlurRadius", 3F);

    public bool BlurEnabled
    {
        get => GetValue(BlurEnabledProperty);
        set => SetValue(BlurEnabledProperty, value);
    }
    
    public float BlurRadius
    {
        get => GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    static BlurMask()
    {
        AffectsRender<BlurMask>(BlurEnabledProperty);
        AffectsRender<BlurMask>(BlurRadiusProperty);
    }
    
    class BlurBehindRenderOperation : ICustomDrawOperation
    {
        private readonly float _blurRadius;
        private readonly Rect _bounds;

        public BlurBehindRenderOperation(float blurRadius, Rect bounds)
        {
            _blurRadius = blurRadius;
            _bounds = bounds;
        }

        public void Dispose()
        {
        }

        public bool HitTest(Point p) => _bounds.Contains(p);

        public void Render(IDrawingContextImpl context)
        {
            var leaseFeature = context.GetFeature<ISkiaSharpApiLeaseFeature>()!;
            using var skia = leaseFeature.Lease();
            if (!skia.SkCanvas.TotalMatrix.TryInvert(out var currentInvertedTransform)) return;
            
            using var backgroundSnapshot = skia.SkSurface.Snapshot();
            using var backdropShader = SKShader.CreateImage(backgroundSnapshot, SKShaderTileMode.Clamp,
                SKShaderTileMode.Clamp, currentInvertedTransform);

            if (skia.GrContext == null) throw new NullReferenceException();
            
            using var filter = SKImageFilter.CreateBlur(_blurRadius, _blurRadius, SKShaderTileMode.Clamp);
            using var tmp = new SKPaint()
            {
                Shader = backdropShader,
                ImageFilter = filter
            };
            skia.SkCanvas.DrawRect(0, 0, (float)_bounds.Width, (float)_bounds.Height, tmp);
        }

        public Rect Bounds => _bounds.Inflate(4);

        public bool Equals(ICustomDrawOperation? other)
        {
            return other is BlurBehindRenderOperation op && op._bounds == _bounds;
        }
    }

    public override void Render(DrawingContext context)
    {
        if (BlurEnabled)
        {
            context.Custom(new BlurBehindRenderOperation(BlurRadius, new Rect(default, Bounds.Size)));
        }
    }
}