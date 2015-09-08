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
        public DbSet<AdminLevelAccessRight> AdminLevelAccessRights { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<ALT> ALTs { get; set; }
        public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
        public DbSet<BankPicture> BankPictures { get; set; }
        public DbSet<GeoLocation> GeoLocations { get; set; }
        public DbSet<InviteCode> InviteCodes { get; set; }
        public DbSet<KnoxAccountDetail> KnoxAccountDetails { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<MemberNotification> MemberNotifications { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<SDN> SDNs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<SynapseBanksOfMember> SynapseBanksOfMembers { get; set; }
        public DbSet<SynapseSupportedBank> SynapseSupportedBanks { get; set; }
        public DbSet<MembersIPAddress> MembersIPAddresses { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<SynapseBankLoginResult> SynapseBankLoginResults { get; set; }
        public DbSet<SynapseCreateOrderResult> SynapseCreateOrderResults { get; set; }
        public DbSet<SynapseCreateUserResult> SynapseCreateUserResults { get; set; }
    
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
    }
}
