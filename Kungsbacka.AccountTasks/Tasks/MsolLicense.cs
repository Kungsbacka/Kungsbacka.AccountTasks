namespace Kungsbacka.AccountTasks
{
    public class MsolLicense
    {
        public string SkuId { get; set; }

        public string[] DisabledPlans { get; set; }

        public MsolLicense() { }

        public MsolLicense(string[] disabledPlans = null, string skuId = null)
        {
            DisabledPlans = disabledPlans;
            SkuId = skuId;
        }
    }

    public static class MsolPredefinedLicensePackage
    {
        public static readonly MsolLicense[] Faculty = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "94763226-9b3c-4e75-a931-5c89701abe66", // STANDARDWOFFPACK_FACULTY
                DisabledPlans = new string[] {
                    "0feaeb32-d00e-4d66-bd5a-43b5b83db82c", // MCOSTANDARD
                    "9aaf7827-d63c-4b61-89c3-182f06f82e5c" // EXCHANGE_S_STANDARD
                }
            },
            new MsolLicense()
            {
                SkuId = "12b8c807-2e20-48fc-b453-542b6ee9d171" // OFFICESUBSCRIPTION_FACULTY
            }
        };

        public static readonly MsolLicense[] Student = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "314c4481-f395-4525-be8b-2ec4bb1e9d91", // STANDARDWOFFPACK_STUDENT
            },
            new MsolLicense()
            {
                SkuId = "c32f9321-a627-406d-a114-1f9c81aaafac" // OFFICESUBSCRIPTION_STUDENT
            }
        };

        public static readonly MsolLicense[] EMS = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "efccb6f7-5641-4e0e-bd10-b4976e1bf68e" // EMS
            }
        };
    }
}

