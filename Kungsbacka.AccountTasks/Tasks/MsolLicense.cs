namespace Kungsbacka.AccountTasks
{
    public class MsolLicense
    {
        public string SkuId { get; }

        public string[] DisabledPlans { get; }

        public MsolLicense(string skuId = null, string[] disabledPlans = null)
        {
            SkuId = skuId;
            DisabledPlans = disabledPlans;
        }
    }
}

