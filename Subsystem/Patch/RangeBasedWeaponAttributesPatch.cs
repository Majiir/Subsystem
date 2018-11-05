﻿namespace Subsystem.Patch
{
    public class RangeBasedWeaponAttributesPatch : IRemovable
    {
        public double? Accuracy { get; set; }
        public double? Distance { get; set; }
        public double? MinDistance { get; set; }
        public bool Remove { get; set; }
    }
}
