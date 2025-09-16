using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsSubject
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int SubjectID { get; set; }
        public string SubjectName { get; set; }

        // Metadata
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }

        public clsSubject()
        {
            SubjectID = -1;
            SubjectName = "";
            Mode = enMode.AddNew;
        }

        private clsSubject(int SubjectID, string SubjectName, int createdBy, DateTime createdAt, int modifiedBy, DateTime modifiedAt)
        {
            this.SubjectID = SubjectID;
            this.SubjectName = SubjectName;

            this.CreatedBy = createdBy;
            this.CreatedAt = createdAt;
            this.ModifiedBy = modifiedBy;
            this.ModifiedAt = modifiedAt;

            Mode = enMode.Update;
        }

        private bool _AddNewSubject()
        {
            SubjectID = clsSubjectData.AddNewSubject(SubjectName, ModifiedBy); // Pass ModifiedBy for audit
            if (SubjectID != -1)
            {
                string newValue = $"SubjectName={SubjectName}";
                clsAuditLog.AddLog("Subjects", SubjectID, "INSERT", null, newValue, ModifiedBy);
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateSubject()
        {
            string oldValue = "";
            clsSubjectData.GetSubjectInfoByID(SubjectID, ref oldValue);

            string newValue = $"SubjectName={SubjectName}";
            ModifiedAt = DateTime.Now;

            bool updated = clsSubjectData.UpdateSubject(SubjectID, SubjectName, ModifiedBy); // Updated data layer
            if (updated)
            {
                clsAuditLog.AddLog("Subjects", SubjectID, "UPDATE", oldValue, newValue, ModifiedBy);
            }
            return updated;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    return _AddNewSubject();
                case enMode.Update:
                    return _UpdateSubject();
                default:
                    return false;
            }
        }

        public static clsSubject FindSubjectByID(int SubjectID)
        {
            string SubjectName = "";
            int createdBy = -1, modifiedBy = -1;
            DateTime createdAt = DateTime.MinValue, modifiedAt = DateTime.MinValue;

            bool isFound = clsSubjectData.GetSubjectInfoByID(SubjectID, ref SubjectName 
                );

            if (isFound)
                return new clsSubject(SubjectID, SubjectName, createdBy, createdAt, modifiedBy, modifiedAt);
            else
                return null;
        }

        public static clsSubject FindSubjectByName(string SubjectName)
        {
            int ID = -1, createdBy = -1, modifiedBy = -1;
            DateTime createdAt = DateTime.MinValue, modifiedAt = DateTime.MinValue;

            bool isFound = clsSubjectData.GetSubjectInfoByName(SubjectName, ref ID,
                ref createdBy, ref createdAt, ref modifiedBy, ref modifiedAt);

            if (isFound)
                return new clsSubject(ID, SubjectName, createdBy, createdAt, modifiedBy, modifiedAt);
            else
                return null;
        }

        public static DataTable GetAllSubjects()
        {
            return clsSubjectData.GetAllSubjects();
        }

        //public static bool DeleteSubject(int SubjectID, int modifiedBy)
        //{
        //    string oldValue = "";
        //    clsSubjectData.GetSubjectInfoByID(SubjectID, ref oldValue);

        //    bool deleted = clsSubjectData.DeleteSubject(SubjectID, modifiedBy);
        //    if (deleted)
        //    {
        //        clsAuditLog.AddLog("Subjects", SubjectID, "DELETE", oldValue, null, modifiedBy);
        //    }

        //    return deleted;
        //}
    }
}
