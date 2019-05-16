namespace Common.Core.Enumerations
{
    public enum ErrorCodeEnum
    {
        LoginFailed = 1,
        IncorrectUser = 2,
        NotAuthorized = 3,
        IncorrectPassword = 4,
        PasswordWrongFormat = 5,
        PasswordExpired = 6,
        UserInactive = 7,
        YouHaveLogged = 8,
        LoginFailed3Time = 9,
        CannotDeleteUser = 10,
        CannotSendEmailToResetPassword = 11,
        DupplicationUser = 12,
        SameWithCurrentPass = 13,
        SessionExpired = 14,
        MultiplePasswordResetting = 15,
        UserDeletedOrNotExisted = 16,
        UserManagerExisted = 17,
        UserStaffExisted = 18,
        UserEmployeeExisted = 19,
        EmailManagerExisted = 20,
        EmailStaffExisted = 21,
        EmailEmployeeExisted = 22,
        UserNotFound = 23,
        ExitedSpecialInUsername = 24,
        CannotUpdateUsers = 25,
        CannotUserAssignment = 26,
        ManagerIsCurrentlyInactive = 27,
        StaffIsCurrentlyInactive = 28,
        CannotUnSelectGroup = 29,
        IncorrectEmail = 30
    }
}