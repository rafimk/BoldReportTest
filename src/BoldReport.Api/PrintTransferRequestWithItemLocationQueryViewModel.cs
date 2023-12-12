namespace BoldReport.Api;

public class PrintTransferRequestWithItemLocationQueryViewModel
{
    public int? TRNumber { get; set; }
    public string TRDate { get; set; }
    public string RequestedBy { get; set; }
    public string SupplierApplicability { get; set; }
    public List<TransferRequest> TransferRequestList { get; set; } = new List<TransferRequest>();
}


public class TransferRequest
{
    public int? SNo { get; set; }
    public string PartNumber { get; set; }
    public string MFR { get; set; }
    public string Description { get; set; }
    public string UnitofIssue { get; set; }
    public int? QTY1 { get; set; }
    public string Location { get; set; }
    public string SerialNo { get; set; }
    public string LotNo { get; set; }
    public int? QTY2 { get; set; }
}

