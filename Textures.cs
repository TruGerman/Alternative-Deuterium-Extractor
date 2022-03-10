using UnityEngine;
using Verse;

namespace AltDeuteriumExtractor
{
    [StaticConstructorOnStartup]
    public static class Textures
    {
        public static readonly Texture2D GIZMO_POWER_LEVEL = ContentFinder<Texture2D>.Get("UI/Commands/ADE_GizmoSetPowerLevel");
    }
}