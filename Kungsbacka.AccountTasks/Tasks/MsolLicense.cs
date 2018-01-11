using System;

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
        public static MsolLicense[] GetPackageFromName(string name)
        {
            switch (name.ToLower())
            {
                case "faculty":
                    return Faculty;
                case "student":
                    return Student;
                case "ems":
                    return EMS;
                case "e3":
                    return E3;
                case "e3+ems":
                    return E3_EMS;
                default:
                    throw new ArgumentException($"Unknown license package {name}");
            }
        }

        public static readonly MsolLicense[] E3_EMS = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "6fd2c87f-b296-42f0-b197-1e91e994b900", // Office 365 Enterprise E3
                DisabledPlans = new string[] {
                    "0feaeb32-d00e-4d66-bd5a-43b5b83db82c", // MCOSTANDARD
                    "efb87545-963c-4e0d-99df-69c6916d9eb0" // EXCHANGE_S_ENTERPRISE
                }
            },
            new MsolLicense()
            {
                SkuId = "efccb6f7-5641-4e0e-bd10-b4976e1bf68e" // Enterprise Mobility + Security E3
            }
        };

        public static readonly MsolLicense[] E3 = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "6fd2c87f-b296-42f0-b197-1e91e994b900", // Office 365 Enterprise E3
                DisabledPlans = new string[] {
                    "0feaeb32-d00e-4d66-bd5a-43b5b83db82c", // MCOSTANDARD
                    "efb87545-963c-4e0d-99df-69c6916d9eb0" // EXCHANGE_S_ENTERPRISE
                }
            }
        };

        public static readonly MsolLicense[] Faculty = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "e578b273-6db4-4691-bba0-8d691f4da603", // 	Office 365 A3 for faculty
                DisabledPlans = new string[] {
                    "efb87545-963c-4e0d-99df-69c6916d9eb0", // EXCHANGE_S_ENTERPRISE
                    "199a5c09-e0ca-4e37-8f7c-b05d533e1ea2", // MICROSOFTBOOKINGS
                    "0feaeb32-d00e-4d66-bd5a-43b5b83db82c"  // MCOSTANDARD
                }
            },
            new MsolLicense()
            {
                SkuId = "12b8c807-2e20-48fc-b453-542b6ee9d171" // Office 365 ProPlus for faculty
            }
        };

        public static readonly MsolLicense[] Student = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "98b6e773-24d4-4c0d-a968-6e787a1f8204", // Office 365 A3 for students
            },
            new MsolLicense()
            {
                SkuId = "c32f9321-a627-406d-a114-1f9c81aaafac" // Office 365 ProPlus for students
            }
        };

        public static readonly MsolLicense[] EMS = new MsolLicense[] {
            new MsolLicense()
            {
                SkuId = "efccb6f7-5641-4e0e-bd10-b4976e1bf68e" // Enterprise Mobility + Security E3
            }
        };
    }
}

