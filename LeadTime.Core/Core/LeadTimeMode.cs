namespace LeadTime.Core.Core
{
    public enum LeadTimeMode : ushort
    {
        ReportCommitAtShipDate,

        /// <summary>
        /// Note that only commits that HAVE SHIPPED will be reported, so newer data will be partial
        /// </summary>
        ReportCommitAtCommitDate
    }
}