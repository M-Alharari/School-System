using System;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsSchoolInfo
    {
        public int SchoolInfoID { get; set; }
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string LogoPath { get; set; }

        public bool IsNew => SchoolInfoID == 0;

        // Load the latest school info
        public static clsSchoolInfo GetSchoolInfo()
        {
            var dt = clsSchoolInfoData.GetSchoolInfo();
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new clsSchoolInfo
            {
                SchoolInfoID = Convert.ToInt32(row["SchoolInfoID"]),
                SchoolName = row["SchoolName"]?.ToString(),
                Address = row["Address"]?.ToString(),
                Phone = row["Phone"]?.ToString(),
                Email = row["Email"]?.ToString(),
                Website = row["Website"]?.ToString(),
                LogoPath = row["Logo"] != DBNull.Value ? row["Logo"].ToString() : null
            };
        }

        // Save: Add or Update automatically
        public bool Save(int currentUserID)
        {
            if (IsNew)
            {
                SchoolInfoID = clsSchoolInfoData.AddNewSchoolInfo(
                    SchoolName,
                    Address,
                    Phone,
                    Email,
                    Website,
                    LogoPath,
                    currentUserID,
                    DateTime.Now
                );
                return SchoolInfoID > 0;
            }
            else
            {
                return clsSchoolInfoData.UpdateSchoolInfo(
                    SchoolInfoID,
                    SchoolName,
                    Address,
                    Phone,
                    Email,
                    Website,
                    LogoPath,
                    currentUserID
                );
            }
        }
    }
}
