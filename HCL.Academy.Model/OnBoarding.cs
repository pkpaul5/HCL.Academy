namespace HCL.Academy.Model
{
    public class OnBoarding
    {
        public int boardingItemId { get; set; }

        public string boardingItemName { get; set; }

        public string boardingInternalName { get; set; }

        public string boardingItemDesc { get; set; }

        public string boardingItemLink { get; set; }

        public bool isWikiLink { get; set; }

        public int boardIngTrainingId { get; set; }

        public int boardIngAssessmentId { get; set; }
        public bool boardingIsMandatory { get; set; }

        public OnboardingStatus boardingStatus { get; set; }

        public OnboardingItemType boardingType { get; set; }

    }

    public class OnBoardingTrainingStatus
    {
        public int id { get; set; }
        public OnboardingItemType onboardingType { get; set; }
        public bool sendEmail { get; set; }
        public string boardingInternalName { get; set; }
        public bool status { get; set; }
    }

    public enum OnboardingStatus
    {
        NotStarted,
        OnGoing,
        Completed,
        Rejected,
        OverDue,
        Failed,

    }

    public enum OnboardingItemType
    {
        Default,
        Assessment,
        Training,
        RoleTraining
    }

    public enum bgColor
    {
        red,
        orange,
        green,
        blue
    }
}