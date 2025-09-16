using SchoolProjectData;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsInstallmentPlan
    {
        public int PlanID { get; set; }
        public string Name { get; set; }
        public int Frequency { get; set; }

        public static int Add(string name, int frequency, int createdByUserID)
            => clsInstallmentPlanData.Add(name, frequency, createdByUserID);

        public static bool Update(int planID, string name, int frequency)
            => clsInstallmentPlanData.Update(planID, name, frequency);

        public static bool Delete(int planID)
            => clsInstallmentPlanData.Delete(planID);

        public static DataTable GetAll()
            => clsInstallmentPlanData.GetAll();
    }
}
