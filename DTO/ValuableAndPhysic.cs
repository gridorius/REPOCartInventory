namespace CartInventory.DTO;

public class ValuableAndPhysic(ValuableObject valuable, PhysGrabObject physic)
{
    public ValuableObject Valuable { get; } = valuable;
    public PhysGrabObject Physic { get; } = physic;
}