using System;
using System.Windows;

namespace AlterApp.AttachedProperties
{
    internal static class SizeObservation
    {
        public static readonly DependencyProperty ObservedActualWidthProperty = DependencyProperty.RegisterAttached(
            "ObservedActualWidth",
            typeof(double),
            typeof(SizeObservation));

        public static double GetObservedActualWidth(FrameworkElement target)
        {
            ArgumentNullException.ThrowIfNull(target, nameof(target));
            return (double)target.GetValue(ObservedActualWidthProperty);
        }

        public static void SetObservedActualWidth(FrameworkElement target, double value)
        {
            ArgumentNullException.ThrowIfNull(target, nameof(target));
            target.SetValue(ObservedActualWidthProperty, value);
        }

        public static readonly DependencyProperty ObservedActualHeightProperty = DependencyProperty.RegisterAttached(
            "ObservedActualHeight",
            typeof(double),
            typeof(SizeObservation));

        public static double GetObservedActualHeight(FrameworkElement target)
        {
            ArgumentNullException.ThrowIfNull(target, nameof(target));
            return (double)target.GetValue(ObservedActualHeightProperty);
        }

        public static void SetObservedActualHeight(FrameworkElement target, double value)
        {
            ArgumentNullException.ThrowIfNull(target, nameof(target));
            target.SetValue(ObservedActualHeightProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(SizeObservation),
            new FrameworkPropertyMetadata(defaultValue: false, propertyChangedCallback: OnIsEnabledChanged));

        public static bool GetIsEnabled(FrameworkElement target)
        {
            ArgumentNullException.ThrowIfNull(target, nameof(target));
            return (bool)target.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(FrameworkElement target, bool value)
        {
            ArgumentNullException.ThrowIfNull(target, nameof(target));
            target.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var target = (FrameworkElement)dependencyObject;
            if ((bool)e.NewValue)
            {
                target.SizeChanged += OnFrameworkElementSizeChanged;
                UpdateObservedSizesForFrameworkElement(target);
            }
            else
            {
                target.SizeChanged -= OnFrameworkElementSizeChanged;
            }
        }

        private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateObservedSizesForFrameworkElement((FrameworkElement)sender);
        }

        private static void UpdateObservedSizesForFrameworkElement(FrameworkElement target)
        {
            target.SetCurrentValue(ObservedActualWidthProperty, target.ActualWidth);
            target.SetCurrentValue(ObservedActualHeightProperty, target.ActualHeight);
        }
    }
}
