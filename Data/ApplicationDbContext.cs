using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Milestone3WebApp.Models;

namespace Milestone3WebApp.Data
{
    
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // DbSets representing tables in the database
        public DbSet<Models.Learner> Learners { get; set; }
        public DbSet<Models.PersonalizationProfile> PersonalizationProfiles { get; set; }
        public DbSet<Models.Course> Courses { get; set; }
        public DbSet<Models.Module> Modules { get; set; }
        public DbSet<Models.LearningActivity> LearningActivities { get; set; }
        public DbSet<Models.CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Models.Badge> Badges { get; set; }
        public DbSet<Models.Achievement> Achievements { get; set; }
        public DbSet<Models.Leaderboard> Leaderboards { get; set; }
        public DbSet<Models.LearningPath> LearningPaths { get; set; }
        public DbSet<Models.InteractionLog> InteractionLogs { get; set; }
        public DbSet<Models.EmotionalFeedback> EmotionalFeedbacks { get; set; }
        public DbSet<Models.Quest> Quests { get; set; }
        public DbSet<Models.Reward> Rewards { get; set; }
        public DbSet<Models.Skill> Skills { get; set; }
        public DbSet<Models.SkillProgression> SkillProgressions { get; set; }
        public DbSet<Models.Assessment> Assessments { get; set; }
        public DbSet<Models.Instructor> Instructors { get; set; }
        public DbSet<Models.ContentLibrary> ContentLibraries { get; set; }
        public DbSet<Models.Notification> Notifications { get; set; }
        public DbSet<Models.DiscussionForum> DiscussionForums { get; set; }
        public DbSet<Models.LearningGoal> LearningGoals { get; set; }
        public DbSet<Models.Survey> Surveys { get; set; }
        public DbSet<Models.LearningPreference> LearningPreferences { get; set; }
        public DbSet<Models.HealthCondition> HealthConditions { get; set; }
        public DbSet<Models.TargetTrait> TargetTraits { get; set; }
        public DbSet<Models.ModuleContent> ModuleContents { get; set; }
        public DbSet<Models.PathReview> PathReviews { get; set; }
        public DbSet<Models.EmotionalFeedbackReview> EmotionalFeedbackReviews { get; set; }
        public DbSet<Models.Teaches> Teaches { get; set; }
        public DbSet<Models.Ranking> Rankings { get; set; }
        public DbSet<Models.LearnerGoal> LearnerGoals { get; set; }
        public DbSet<Models.SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<Models.FilledSurvey> FilledSurveys { get; set; }
        public DbSet<Models.ReceivedNotification> ReceivedNotifications { get; set; }
        public DbSet<Models.SkillMastery> SkillMasteries { get; set; }
        public DbSet<Models.Collaborative> Collaboratives { get; set; }
        public DbSet<Models.LearnerDiscussion> LearnerDiscussions { get; set; }
        public DbSet<Models.QuestReward> QuestRewards { get; set; }
        public DbSet<Models.LearnerAssessment> LearnerAssessments { get; set; }
        public DbSet<Models.CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserViewModel> UserViewModels { get; set; }
    }
}