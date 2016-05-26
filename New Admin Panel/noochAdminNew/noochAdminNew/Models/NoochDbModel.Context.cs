﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace noochAdminNew.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class NOOCHEntities : DbContext
    {
        public NOOCHEntities()
            : base("name=NOOCHEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ADD> ADDs { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<ALT> ALTs { get; set; }
        public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
        public DbSet<GeoLocation> GeoLocations { get; set; }
        public DbSet<InviteCode> InviteCodes { get; set; }
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<LanlordAppInterestedEmail> LanlordAppInterestedEmails { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<MemberNotification> MemberNotifications { get; set; }
        public DbSet<MemberPrivacySetting> MemberPrivacySettings { get; set; }
        public DbSet<MemberReportEntity> MemberReportEntities { get; set; }
        public DbSet<MemberReportResultEntity> MemberReportResultEntities { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MembersIPAddress> MembersIPAddresses { get; set; }
        public DbSet<MemberTargusReportEntity> MemberTargusReportEntities { get; set; }
        public DbSet<MostFrequentFriendsTemp> MostFrequentFriendsTemps { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyUnit> PropertyUnits { get; set; }
        public DbSet<ReferalCodeRequest> ReferalCodeRequests { get; set; }
        public DbSet<RentTransaction> RentTransactions { get; set; }
        public DbSet<SDN> SDNs { get; set; }
        public DbSet<SDNSearchResult> SDNSearchResults { get; set; }
        public DbSet<SocialMediaPost> SocialMediaPosts { get; set; }
        public DbSet<SpecificUserTransReportEntity> SpecificUserTransReportEntities { get; set; }
        public DbSet<SynapseBankLoginResult> SynapseBankLoginResults { get; set; }
        public DbSet<SynapseBanksOfMember> SynapseBanksOfMembers { get; set; }
        public DbSet<SynapseIdVerificationQuestion> SynapseIdVerificationQuestions { get; set; }
        public DbSet<SynapseSupportedBank> SynapseSupportedBanks { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantsIdDocument> TenantsIdDocuments { get; set; }
        public DbSet<TransactionCustomReportEntity> TransactionCustomReportEntities { get; set; }
        public DbSet<TransactionReportEntity> TransactionReportEntities { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UnitsOccupiedByTenant> UnitsOccupiedByTenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SynapseCreateUserResult> SynapseCreateUserResults { get; set; }
        public DbSet<SynapseAddTransactionResult> SynapseAddTransactionResults { get; set; }
    
        public virtual ObjectResult<string> GetReportsForMember(string memberId, string getWhat)
        {
            var memberIdParameter = memberId != null ?
                new ObjectParameter("MemberId", memberId) :
                new ObjectParameter("MemberId", typeof(string));
    
            var getWhatParameter = getWhat != null ?
                new ObjectParameter("GetWhat", getWhat) :
                new ObjectParameter("GetWhat", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("GetReportsForMember", memberIdParameter, getWhatParameter);
        }
    
        public virtual ObjectResult<GetTopReferringMembers_Result> GetTopReferringMembers(Nullable<int> vFilterType)
        {
            var vFilterTypeParameter = vFilterType.HasValue ?
                new ObjectParameter("vFilterType", vFilterType) :
                new ObjectParameter("vFilterType", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTopReferringMembers_Result>("GetTopReferringMembers", vFilterTypeParameter);
        }
    
        public virtual ObjectResult<GetLiveTransactionsForDashboard_Result1> GetLiveTransactionsForDashboard(Nullable<int> vType)
        {
            var vTypeParameter = vType.HasValue ?
                new ObjectParameter("vType", vType) :
                new ObjectParameter("vType", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetLiveTransactionsForDashboard_Result1>("GetLiveTransactionsForDashboard", vTypeParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> GetDashboardStats(string vStatsFor, string vStatsType)
        {
            var vStatsForParameter = vStatsFor != null ?
                new ObjectParameter("vStatsFor", vStatsFor) :
                new ObjectParameter("vStatsFor", typeof(string));
    
            var vStatsTypeParameter = vStatsType != null ?
                new ObjectParameter("vStatsType", vStatsType) :
                new ObjectParameter("vStatsType", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetDashboardStats", vStatsForParameter, vStatsTypeParameter);
        }
    
        public virtual ObjectResult<GetMembersInEachSynapseBank_Result> GetMembersInEachSynapseBank()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetMembersInEachSynapseBank_Result>("GetMembersInEachSynapseBank");
        }
    
        public virtual ObjectResult<SDNSearchResult> GetSDNListing(string searchtext)
        {
            var searchtextParameter = searchtext != null ?
                new ObjectParameter("searchtext", searchtext) :
                new ObjectParameter("searchtext", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SDNSearchResult>("GetSDNListing", searchtextParameter);
        }
    
        public virtual ObjectResult<SDNSearchResult> GetSDNListing(string searchtext, MergeOption mergeOption)
        {
            var searchtextParameter = searchtext != null ?
                new ObjectParameter("searchtext", searchtext) :
                new ObjectParameter("searchtext", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SDNSearchResult>("GetSDNListing", mergeOption, searchtextParameter);
        }
    }
}
