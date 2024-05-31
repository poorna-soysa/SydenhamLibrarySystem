namespace SydenhamLibrarySystem.Constants;

public class Enums
{
    public enum LibraryItemType
    {
        Book = 1,
        CD
    }

    public enum Roles
    {
        Guest = 1,
        Admin
    }

    public enum ReservationStatus
    {
        Pending = 1,
        Issued,
        Returned
    }
}