namespace LAMP.ViewModel
{
    /// <summary>
    /// Class ViewModelBase
    /// </summary>
    public class ViewModelBase
    {
        /// <summary>
        /// Gets or sets Error message
        /// </summary>
        public short Status { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    ///  Class PagingBase
    /// </summary>
    public class PagingBase
    {
        /// <summary>
        /// Current Page size
        /// </summary>
        public short PageSize { get; set; }
        /// <summary>
        /// Current Page number.
        /// </summary>
        public short CurrentPage { get; set; }
        /// <summary>
        /// Index of the first item of the current page.
        /// </summary>
        public int CurrentStartingRecordIndex { get; set; }
        /// <summary>
        /// Number of records to be skipped while fetching records.
        /// </summary>
        public long NumberOfRecordsToBeSkipped { get; set; }
        /// <summary>
        /// Total Number of items. This can different from number of items returned when we do paging
        /// and fetching items based on page number.
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Total pages
        /// </summary>
        public int TotalPages { get; set; }

    }
}
