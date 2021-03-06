//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace HealthGateway.WebClient.Test.Services
{
    using Xunit;
    using Moq;
    using DeepEqual.Syntax;
    using HealthGateway.WebClient.Services;
    using HealthGateway.Database.Models;
    using HealthGateway.Database.Wrapper;
    using HealthGateway.Database.Delegates;
    using HealthGateway.Common.Services;
    using Microsoft.Extensions.Logging;
    using HealthGateway.Common.Models;
    using System;
    using HealthGateway.WebClient.Models;
    using HealthGateway.WebClient.Constant;
    using HealthGateway.Common.Delegates;
    using HealthGateway.Database.Constants;
    using HealthGateway.Common.Constants;
    using System.Collections.Generic;

    public class UserProfileServiceTest
    {
        readonly string hdid = "1234567890123456789012345678901234567890123456789012";

        [Fact]
        public void ShouldGetUserProfile()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
            };

            DBResult<UserProfile> userProfileDBResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = DBStatusCode.Read
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            LegalAgreement termsOfService = new LegalAgreement()
            {
                Id = Guid.NewGuid(),
                LegalText = "",
                EffectiveDate = DateTime.Now
            };

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.GetUserProfile(hdid)).Returns(userProfileDBResult);
            profileDelegateMock.Setup(s => s.Update(userProfile, true)).Returns(userProfileDBResult);

            UserPreference dbUserPreference = new UserPreference
            {
                HdId = hdid,
                Preference = "TutorialPopover",
                Value = true.ToString(),
            };
            List<UserPreference> userPreferences = new List<UserPreference>();
            userPreferences.Add(dbUserPreference);
            DBResult<IEnumerable<UserPreference>> readResult = new DBResult<IEnumerable<UserPreference>>
            {
                Payload = userPreferences,
                Status = DBStatusCode.Read
            };
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();
            preferenceDelegateMock.Setup(s => s.GetUserPreferences(hdid)).Returns(readResult);

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration());

            Mock<ILegalAgreementDelegate> legalAgreementDelegateMock = new Mock<ILegalAgreementDelegate>();
            legalAgreementDelegateMock
                .Setup(s => s.GetActiveByAgreementType(AgreementType.TermsofService))
                .Returns(new DBResult<LegalAgreement>() { Payload = termsOfService });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                legalAgreementDelegateMock.Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object);

            RequestResult<UserProfileModel> actualResult = service.GetUserProfile(hdid);

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.True(actualResult.ResourcePayload.IsDeepEqual(expected));
        }

        [Fact]
        public void ShouldInsertUserProfile()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true,
                Email = string.Empty
            };

            DBResult<UserProfile> insertResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = DBStatusCode.Created
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.InsertUserProfile(userProfile)).Returns(insertResult);
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration() { WebClient = new WebClientConfiguration() { RegistrationStatus = RegistrationStatus.Open } });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            cryptoDelegateMock.Setup(s => s.GenerateKey()).Returns("abc");

            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            notificationServiceMock.Setup(s => s.QueueNotificationSettings(It.IsAny<NotificationSettingsRequest>()));

            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();
            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                new Mock<ILegalAgreementDelegate>().Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object);

            RequestResult<UserProfileModel> actualResult = service.CreateUserProfile(new CreateUserRequest() { Profile = userProfile }, new Uri("http://localhost/"), "bearer_token");

            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.True(actualResult.ResourcePayload.IsDeepEqual(expected));
        }

        [Fact]
        public void ShouldQueueNotificationUpdate()
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = "1234567890123456789012345678901234567890123456789012",
                AcceptedTermsOfService = true,
                Email = string.Empty
            };

            DBResult<UserProfile> insertResult = new DBResult<UserProfile>
            {
                Payload = userProfile,
                Status = DBStatusCode.Created
            };

            UserProfileModel expected = UserProfileModel.CreateFromDbModel(userProfile);

            Mock<IEmailQueueService> emailer = new Mock<IEmailQueueService>();
            Mock<IUserProfileDelegate> profileDelegateMock = new Mock<IUserProfileDelegate>();
            profileDelegateMock.Setup(s => s.InsertUserProfile(userProfile)).Returns(insertResult);
            Mock<IUserPreferenceDelegate> preferenceDelegateMock = new Mock<IUserPreferenceDelegate>();

            Mock<IEmailDelegate> emailDelegateMock = new Mock<IEmailDelegate>();
            Mock<IMessagingVerificationDelegate> emailInviteDelegateMock = new Mock<IMessagingVerificationDelegate>();
            emailInviteDelegateMock.Setup(s => s.GetByInviteKey(It.IsAny<Guid>())).Returns(new MessagingVerification());

            Mock<IConfigurationService> configServiceMock = new Mock<IConfigurationService>();
            configServiceMock.Setup(s => s.GetConfiguration()).Returns(new ExternalConfiguration() { WebClient = new WebClientConfiguration() { RegistrationStatus = RegistrationStatus.Open } });

            Mock<ICryptoDelegate> cryptoDelegateMock = new Mock<ICryptoDelegate>();
            cryptoDelegateMock.Setup(s => s.GenerateKey()).Returns("abc");

            Mock<INotificationSettingsService> notificationServiceMock = new Mock<INotificationSettingsService>();
            notificationServiceMock.Setup(s => s.QueueNotificationSettings(It.IsAny<NotificationSettingsRequest>()));

            Mock<IMessagingVerificationDelegate> messageVerificationDelegateMock = new Mock<IMessagingVerificationDelegate>();

            IUserProfileService service = new UserProfileService(
                new Mock<ILogger<UserProfileService>>().Object,
                profileDelegateMock.Object,
                preferenceDelegateMock.Object,
                emailDelegateMock.Object,
                emailInviteDelegateMock.Object,
                configServiceMock.Object,
                emailer.Object,
                new Mock<ILegalAgreementDelegate>().Object,
                cryptoDelegateMock.Object,
                notificationServiceMock.Object,
                messageVerificationDelegateMock.Object);

            RequestResult<UserProfileModel> actualResult = service.CreateUserProfile(new CreateUserRequest() { Profile = userProfile }, new Uri("http://localhost/"), "bearer_token");
            notificationServiceMock.Verify(s => s.QueueNotificationSettings(It.IsAny<NotificationSettingsRequest>()), Times.Once());
            Assert.Equal(ResultType.Success, actualResult.ResultStatus);
            Assert.True(actualResult.ResourcePayload.IsDeepEqual(expected));
        }
    }
}
