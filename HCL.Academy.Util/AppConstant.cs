namespace HCLAcademy.Util
{
    public class SessionConstant
    {
        #region Sessions
        public static string CurrentUser = "CurrentUser";
      //  public static string ClientContext = "ClientContext";
        #endregion
    }

    public class AppConstant
    {
        private AppConstant() { }

        #region ListName Constants
        public static string SPList_EmailTemplate = "Email Template"; 
        public static string SPList_EmailQueue = "EmailQueue";
        public static string SPList_RSSFeeds = "RssFeeds"; 
        public static string SPList_AcademyOnBoarding = "Academy OnBoarding";
        public static string SPList_UserAssessmentMapping = "User Assessments";
        public static string SPList_UserTrainingMapping = "User Trainings";
        public static string SPList_UserRoleTraining = "UserRoleTraining"; 
        public static string SPList_AcademyAssessment = "Assessment Questions";
        public static string SPList_SkillCompetencyLevelTrainings = "Skill Competency Level Trainings";
        public static string SPList_RoleTraining = "RoleTraining";
        public static string SPList_SkillCompetencyLevels = "Skill Competency Levels";
        public static string SPList_Assessments = "Assessments";
        public static string SPList_Skills = "Skills";
        public static string SPList_DefaultOnboardingTrainingAssessment = "Default Onboarding Assessment";
        public static string SPList_AcademyGEO = "AcademyGEO";
        public static string SPList_AcademyConfig = "AcademyConfig";
        public static string SPList_AcademyEvents = "Academy Events";
        public static string SPList_AcademyNews = "Academy News";
        public static string SPList_AcademyOnBoardingColumnSchema = "Academy Schema";
        public static string SPList_AcademyOnBoardingColumnSchemaInternalName = "AcademySchema";
        public static string SPList_FAQs = "FAQs";
        public static string SPList_AcademyVideos = "Academy Videos";
        public static string SPList_WikiDocuments = "Training Documents";
        public static string SPList_SiteMenu = "Site Menu";
        public static string SPList_Roles = "Roles";
        public static string SPList_TrainingPlan = "Training Content";
        public static string SPList_UserSkills = "User Skills";        
        public static string SPList_OnBoardingCheckList = "OnBoardingCheckList";
        public static string SPList_UserPoints = "UserPoints";
        public static string SPList_UserAssessmentHistory = "User Assessments History";
        public static string SPList_Projects = "Projects";
        public static string SPList_ProjectSkills = "ProjectSkills";
        public static string SPList_ProjectSkillResource = "ProjectSkillresource";
        public static string SPList_UserCheckList = "UserCheckList";
        #endregion

        #region RAG Status
        public static string Red = "Red";
        public static string Amber = "Orange";
        public static string Green = "Green";

        #endregion

        #region Item Status
        public static string Completed = "Completed";
        public static string InProgress  = "In Progress";
        public static string OverDue = "OverDue";
        #endregion

        public static int MaxQueForAssessment = 40;
        public static string OnboardingHelp = "Onboarding Help";
        public static string DefaultTrainingAssessmentData = "DefaultTrainingAssessmentData";
        public static string AllGEOData = "AllGEOData";
        public static string AllRoleData = "AllRoleData";
        public static string AllSkillData = "AllSkillData";
        public static string AllCompetencyData = "AllCompetencyData";
        public static string AllWikiPolicyData = "AllWikiPolicyData";
        public static string AllVideoData = "AllVideoData";
        public static string AllEventsData = "AllEventsData";
        public static string AllTrainingData = "AllTrainingData";
        public static string AllRoleTrainingData = "AllRoleTrainingData";
        public static string AllAssessmentData = "AllAssessmentData";
        public static string AllCheckListData = "AllCheckListData";
        public static string StorageTableLogs = "Logs";
        public static string ApplicationName = "AcademyPortal";
        public static string PartitionError = "Error";
        public static string PartitionInformation = "Information";
        public static string ErrorLogEntities = "ErrorLogEntities";
    }
    public class DataStore
    {
        public static string SharePoint = "SharePoint";        
        public static string SQLServer = "SqlSvr";

    }
    public static class ListConstant
    {
        #region Lists
        public static string Competencies = "Competencies";
        public static string TrainingMappingDetails = "TrainingMappingDetails";
        public static string SiteAssets = "Site Assets";
        public static string BannerFolder = "Banners";
        public static string LogoFolder = "logo";
        public static string LandingPageHeader = "LandingPageHeader";
        #endregion
    }

    public class FieldConstant
    {
        #region INGCompetencies List Fields
        public static string Competencies_SkillTypes = "SkillTypes";
        public static string Competencies_SkillHeader = "SkillHeader";
        public static string Competencies_SkillDescription = "SkillDescription";
        #endregion

        #region TrainingModule List Fields
        public static string TrainingModule_TrainingName = "Title";
        public static string TrainingModule_TrainingDescription = "Description1";
        public static string TrainingModule_IsWikiLink = "IsWikiLink";
        public static string TrainingModule_TrainingLink = "TrainingLink";
        public static string TrainingModule_SkillType = "SkillType";
        #endregion

        #region AcademyOnBoarding List Fields
        public static string AcademyOnBoarding_Skill = "Skill";
        #endregion

        public static string EncodedAbsUrl = "EncodedAbsUrl";
        public static string FileRef = "FileRef";
        public static string FileLeafRef = "FileLeafRef";
        
        public static string Banners_Ordering = "ordering";
    }

    public class FieldValueConstant
    {
        #region TrainingModule SkillType Field Value
        public static string TrainingModule_SkillType_SoftSkill = "Soft Skill";
        #endregion

        #region Competencies SkillType Field Value
        public static string Competencies_SkillTypes_SoftSkill = "Soft Skills";
        #endregion
    }

    public class MessageConstant
    {
        public static string AssesmentConfirmMsg = "Are you sure you want to start the assesment now? If yes please click OK to confirm else click Cancel. Otherwise you will loose an attempt.";
    }

    public class GroupConstant
    {
        public static string MarketRiskMembersGroup = "Market Risk Members";
    }
}