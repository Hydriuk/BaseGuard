namespace BaseGuard.Models
{
    public class Guard
    {
        public ushort AssetId { get; set; }
        public uint InstanceId { get; set; }
        public bool IsActive { get; set; }
        public float Range { get; set; }
        public float Shield { get; set; }

        public Guard(GuardAsset guardAsset, uint instanceId, bool isActive)
        {
            AssetId = guardAsset.Id;
            InstanceId = instanceId;
            IsActive = isActive;
            Range = guardAsset.Range;
            Shield = guardAsset.Shield;
        }
    }
}