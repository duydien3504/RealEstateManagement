using RealEstateSystem.Domain.Common;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Domain.Entities
{
    public class OwnerProfileRequest : BaseEntity
    {
        public int RequestId { get; private set; }
        public Guid UserId { get; private set; }
        public Status Status { get; private set; }
        public string? RejectReason { get; private set; }
        public Guid? ApprovedByAdminId { get; private set; }



        protected OwnerProfileRequest() { }

        public OwnerProfileRequest(Guid _UserId)
        {
            UserId = _UserId;
            Status = Status.Pending;
        }

        public void Approve(Guid _ApprovedByAdminId)
        {
            if(Status != Status.Pending)
            {
                throw new InvalidOperationException("Yeu cau khong o trang thai cho duyet");
            }
            Status = Status.Approved;
            ApprovedByAdminId = _ApprovedByAdminId;
            RejectReason = null;
            MarkAsUpdated();
        }

        public void Reject(Guid _RejectedByAdminId, string Reason) 
        {
            if(Status != Status.Pending)
            {
                throw new InvalidOperationException("Yeu cau khong o trang thai cho duyet");
            }
            Status = Status.Rejected;
            ApprovedByAdminId = _RejectedByAdminId;
            RejectReason = Reason;
            MarkAsUpdated();
        }

        public virtual User? User { get; private set; }
        public virtual User? ApprovedByAdmin { get; private set;  }
    }
}
