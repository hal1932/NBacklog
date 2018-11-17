namespace NBacklog.DataTypes
{
    public class Error
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string MoreInfo { get; set; }

        internal Error(_Error data)
        {
            Message = data.message;
            Code = data.code;
            MoreInfo = data.moreInfo;
        }
    }
}
