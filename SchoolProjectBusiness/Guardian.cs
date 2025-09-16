using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsGuardian
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int GuardianID { get; set; }
        public int PersonID { get; set; }
        public string Relationship { get; set; }
        public int CreatedBy { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public int ModifiedBy { get; private set; }
        public DateTime? ModifiedDate { get; private set; }

        public clsGuardian()
        {
            GuardianID = -1;
            PersonID = -1;
            Relationship = string.Empty;
            Mode = enMode.AddNew;
        }

        private clsGuardian(int guardianID, int personID, string relationship,
                            int createdBy, DateTime createdDate,
                            int modifiedBy, DateTime? modifiedDate)
        {
            GuardianID = guardianID;
            PersonID = personID;
            Relationship = relationship;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            ModifiedBy = modifiedBy;
            ModifiedDate = modifiedDate;
            Mode = enMode.Update;
        }

        private bool _AddNewGuardian(int currentUser)
        {
            int newID = clsGuardianData.AddNewGuardian(PersonID, Relationship, currentUser);
            if (newID != -1)
            {
                GuardianID = newID;
                CreatedBy = currentUser;
                CreatedDate = DateTime.Now;

                // Audit log
                string newValue = $"PersonID={PersonID}, Relationship={Relationship}";
                clsAuditLog.AddLog("Guardians", GuardianID, "INSERT", null, newValue, currentUser);

                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateGuardian(int currentUser)
        {
            // Get old values
            int oldPersonID = -1;
            string oldRelationship = string.Empty;
            clsGuardianData.GetGuardianInfoByID(GuardianID, ref oldPersonID, ref oldRelationship);
            string oldValue = $"PersonID={oldPersonID}, Relationship={oldRelationship}";
            string newValue = $"PersonID={PersonID}, Relationship={Relationship}";

            bool result = clsGuardianData.UpdateGuardian(GuardianID, PersonID, Relationship, currentUser);
            if (result)
            {
                ModifiedBy = currentUser;
                ModifiedDate = DateTime.Now;

                // Audit log
                clsAuditLog.AddLog("Guardians", GuardianID, "UPDATE", oldValue, newValue, currentUser);
            }
            return result;
        }

        public bool Save(int currentUser)
        {
            return Mode == enMode.AddNew
                ? _AddNewGuardian(currentUser)
                : _UpdateGuardian(currentUser);
        }
        // Map Guardian → Person
        public static int? GetPersonIDByGuardianID(int guardianID)
        {
            return clsGuardianData.GetPersonIDByGuardianID(guardianID);
        }
    
        public static clsGuardian FindGuardianByID(int guardianID)
        {
            int personID = -1;
            string relationship = string.Empty;
            int createdBy =-1 ;
            DateTime createdDate = DateTime.MinValue;
            int modifiedBy = -1;
            DateTime? modifiedDate = null;

            bool isFound = clsGuardianData.GetGuardianInfoByID(guardianID, ref personID, ref relationship, ref createdBy, ref createdDate, ref modifiedBy, ref modifiedDate);
            if (isFound)
                return new clsGuardian(guardianID, personID, relationship, createdBy, createdDate, modifiedBy, modifiedDate);

            return null;
        }

        public static DataTable GetAllGuardians()
        {
            return clsGuardianData.GetAllGuardians();
        }

        public static bool DeleteGuardian(int guardianID, int currentUser)
        {
            // Get old values for audit
            int oldPersonID = -1;
            string oldRelationship = string.Empty;
            clsGuardianData.GetGuardianInfoByID(guardianID, ref oldPersonID, ref oldRelationship);
            string oldValue = $"PersonID={oldPersonID}, Relationship={oldRelationship}";

            bool result = clsGuardianData.DeleteGuardian(guardianID);
            if (result)
            {
                clsAuditLog.AddLog("Guardians", guardianID, "DELETE", oldValue, null, currentUser);
            }
            return result;
        }
        /// <summary>
        /// Get the linked PersonID for a GuardianID
        /// </summary>
        public static int GetPersonID(int guardianID)
        {
            return clsGuardianData.GetPersonIDByGuardianID(guardianID);
        }

    }
}
