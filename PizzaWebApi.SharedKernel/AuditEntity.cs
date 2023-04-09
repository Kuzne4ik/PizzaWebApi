namespace PizzaWebApi.SharedKernel
{
    public abstract class AuditEntity : BaseEntity
    {
        public DateTime Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? Updated { get; set; }

        public int? UpdatedBy { get; set; }

    }
}
