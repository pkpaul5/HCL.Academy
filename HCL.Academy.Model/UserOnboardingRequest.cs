namespace HCL.Academy.Model
{
    public class UserOnboardingRequest:RequestBase
    {
        public string EmailId { get; set; }
        public string Name { get; set; }
        public int SkillId { get; set; }
        public int CompetenceId { get; set; }
        public int GeoId { get; set; }
        public int RoleId { get; set; }
        public int EmployeeId { get; set; }
        public int RoleBasedSkillCount { get; set; }
    }
}
