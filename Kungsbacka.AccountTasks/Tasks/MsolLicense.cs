﻿using System;

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

        public static MsolLicense[] E3_EMS => e3_ems;
        private static readonly MsolLicense[] e3_ems = new MsolLicense[] {
            new MsolLicense(
                "6fd2c87f-b296-42f0-b197-1e91e994b900", // Office 365 Enterprise E3
                new string[] {
                    "0feaeb32-d00e-4d66-bd5a-43b5b83db82c", // MCOSTANDARD
                    "efb87545-963c-4e0d-99df-69c6916d9eb0" // EXCHANGE_S_ENTERPRISE
                }
            ),
            new MsolLicense("efccb6f7-5641-4e0e-bd10-b4976e1bf68e") // Enterprise Mobility + Security E3
        };

        public static MsolLicense[] E3 => e3;
        private static readonly MsolLicense[] e3 = new MsolLicense[] {
            new MsolLicense(
                "6fd2c87f-b296-42f0-b197-1e91e994b900", // Office 365 Enterprise E3
                new string[] {
                    "0feaeb32-d00e-4d66-bd5a-43b5b83db82c", // MCOSTANDARD
                    "efb87545-963c-4e0d-99df-69c6916d9eb0"  // EXCHANGE_S_ENTERPRISE
                }
            )
        };

        public static MsolLicense[] Faculty => faculty;
        private static readonly MsolLicense[] faculty = new MsolLicense[] {
            new MsolLicense(
                "e578b273-6db4-4691-bba0-8d691f4da603", // 	Office 365 A3 for faculty
                new string[] {
                    "efb87545-963c-4e0d-99df-69c6916d9eb0", // EXCHANGE_S_ENTERPRISE
                    "199a5c09-e0ca-4e37-8f7c-b05d533e1ea2", // MICROSOFTBOOKINGS
                    "0feaeb32-d00e-4d66-bd5a-43b5b83db82c"  // MCOSTANDARD
                }
            ),
            new MsolLicense("12b8c807-2e20-48fc-b453-542b6ee9d171"), // Office 365 ProPlus for faculty
            new MsolLicense("98b6e773-24d4-4c0d-a968-6e787a1f8204")  // Minecraft Education Edition for faculty
        };

        public static MsolLicense[] Student => student;
        private static readonly MsolLicense[] student = new MsolLicense[] {
            new MsolLicense("98b6e773-24d4-4c0d-a968-6e787a1f8204"), // Office 365 A3 for students
            new MsolLicense("c32f9321-a627-406d-a114-1f9c81aaafac"), // Office 365 ProPlus for students
            new MsolLicense("533b8f26-f74b-4e9c-9c59-50fc4b393b63")  // Minecraft Education Edition for students
        };


        public static MsolLicense[] EMS => ems;
        private static readonly MsolLicense[] ems = new MsolLicense[] {
            new MsolLicense("efccb6f7-5641-4e0e-bd10-b4976e1bf68e") // Enterprise Mobility + Security E3
        };
    }
}

