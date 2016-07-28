namespace RedPoint.ReefStatus.Common.UI.Controls
{
    using Microsoft.Windows.Controls.Ribbon;

    public class CustomRibbon : Ribbon
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            base.SetValue(IsHostedInRibbonWindowPropertyKey, true);
        }
    }
}
