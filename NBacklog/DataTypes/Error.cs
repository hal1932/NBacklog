namespace NBacklog.DataTypes
{
    public enum ErrorCode
    {
        InternalError = 1,
        LicenceError,
        LicenceExpiredError,
        AcceddDeniedError,
        UnauthorizedOperationError,
        NoResourceError,
        InvalidRequestError,
        SpaceOverCapacityError,
        ResourceOverflowError,
        TooLargeFileError,
        AuthenticationError,
        RequiredMFAError,
    }

    public class Error
    {
        public string Message { get; set; }
        public ErrorCode Code { get; set; }
        public string MoreInfo { get; set; }

        internal Error(_Error data)
        {
            Message = data.message;
            Code = (ErrorCode)data.code;
            MoreInfo = data.moreInfo;
        }
    }
}
