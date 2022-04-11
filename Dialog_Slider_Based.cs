using System;
using UnityEngine;
using Verse;

namespace AltDeuteriumExtractor
{
    public class Dialog_Slider_Based : Dialog_Slider
    {
        public Dialog_Slider_Based(Func<int, string> textGetter, int @from, int to, Action<int> confirmAction, int startingValue = -2147483648, float roundTo = 1) : base(textGetter, @from, to, confirmAction, startingValue, roundTo)
        {
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(new Rect(inRect.x, inRect.yMax - 30F, inRect.width / 2f, 30f), "CancelButton".Translate())) Close();
            if (Widgets.ButtonText(new Rect(inRect.x + inRect.width / 2f, inRect.yMax - 30F, inRect.width / 2f, 30f), "OK".Translate()))
            {
                Close();
                confirmAction(curValue);
            }
            curValue = (int)Widgets.HorizontalSlider(new Rect(inRect.x, inRect.y + 25F, inRect.width, 45F), curValue, @from, to, true, textGetter(curValue), roundTo: roundTo);
        }
    }
}
