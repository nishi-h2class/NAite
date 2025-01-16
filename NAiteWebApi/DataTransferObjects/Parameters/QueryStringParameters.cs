namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public abstract class QueryStringParameters
    {
        const int maxPageSize = 1000;

        /// <summary>
        /// PageNumber
        /// </summary>
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 1000;

        /// <summary>
        /// PageSize
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        /// <summary>
        /// OrderBy指定
        /// </summary>
        public string OrderBy { get; set; } = string.Empty!;

        /// <summary>
        /// フィールド絞り込み
        /// </summary>
        //public string Fields { get; set; } = string.Empty;
    }
}
